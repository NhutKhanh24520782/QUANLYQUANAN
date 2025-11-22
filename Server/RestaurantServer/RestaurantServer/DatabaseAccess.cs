using BCrypt.Net;
using Models.Database;
using Models;
using Models.Response;
using System;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Data.SqlClient;

namespace RestaurantServer
{
    public static class DatabaseAccess
    {
        // ✅ SỬA 1: Xóa đoạn string thừa, chỉ giữ 1 biến connectionString
        private static string connectionString =
            "Server=tcp:quanlyquanan.database.windows.net,1433;" +
            "Initial Catalog=restaurant;" +
            "User ID=lamnhutkhanh;" +
            "Password=Khanh251106;" +
            "Encrypt=True;" +
            "TrustServerCertificate=False;" +
            "Connection Timeout=30;";

        // ========================= LOGIN =========================
        public static LoginResult LoginUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT MaNguoiDung, TenDangNhap, MatKhau, VaiTro, HoTen, Email, TrangThai
                                 FROM NGUOIDUNG WHERE TenDangNhap = @user AND TrangThai = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user", username);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hashed = reader["MatKhau"].ToString();
                        if (BCrypt.Net.BCrypt.Verify(password, hashed))
                        {
                            return new LoginResult
                            {
                                Success = true,
                                MaNguoiDung = (int)reader["MaNguoiDung"],
                                Role = reader["VaiTro"].ToString(),
                                HoTen = reader["HoTen"].ToString(),
                                Email = reader["Email"].ToString(),
                                Message = "Đăng nhập thành công"
                            };
                        }
                    }
                }
                return new LoginResult { Success = false, Message = "Sai tên đăng nhập hoặc mật khẩu" };
            }
        }

        // ====================== REGISTER ======================
        public static RegisterResult RegisterUser(string username, string password, string fullName, string email, string role)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string check = "SELECT COUNT(*) FROM NGUOIDUNG WHERE TenDangNhap=@u OR Email=@e";
                    using (SqlCommand c = new SqlCommand(check, conn))
                    {
                        c.Parameters.AddWithValue("@u", username);
                        c.Parameters.AddWithValue("@e", email);
                        if ((int)c.ExecuteScalar() > 0)
                            return new RegisterResult { Success = false, Message = "Tài khoản hoặc email đã tồn tại" };
                    }

                    string hashed = BCrypt.Net.BCrypt.HashPassword(password);
                    string insert = @"INSERT INTO NGUOIDUNG(TenDangNhap, MatKhau, HoTen, Email, VaiTro, TrangThai, NgayTao)
                                      VALUES(@u,@p,@n,@e,@r,1,GETDATE());
                                      SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(insert, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", hashed);
                        cmd.Parameters.AddWithValue("@n", fullName);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.Parameters.AddWithValue("@r", role);
                        int newId = Convert.ToInt32(cmd.ExecuteScalar());
                        return new RegisterResult { Success = true, Message = "Đăng ký thành công", MaNguoiDung = newId };
                    }
                }
            }
            catch (Exception ex) { return new RegisterResult { Success = false, Message = ex.Message }; }
        }

        // ====================== CHECK EMAIL ======================
        public static EmailCheckResult CheckEmailExists(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM NGUOIDUNG WHERE Email=@e AND TrangThai=1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@e", email);
                    int count = (int)cmd.ExecuteScalar();
                    return new EmailCheckResult { Success = true, Exists = count > 0, Message = count > 0 ? "Email tồn tại" : "Email chưa đăng ký" };
                }
            }
            catch (Exception ex) { return new EmailCheckResult { Success = false, Exists = false, Message = ex.Message }; }
        }

        // ====================== GET EMPLOYEES ======================
        public static EmployeeResult GetEmployees(string keyword = "", string vaiTro = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT MaNguoiDung, TenDangNhap, HoTen, Email, VaiTro, SDT, NgayTao, TrangThai
                                     FROM NGUOIDUNG
                                     WHERE VaiTro IN ('Admin','PhucVu','Bep')
                                     AND (@Keyword='' OR HoTen LIKE '%' + @Keyword + '%' OR Email LIKE '%' + @Keyword + '%')
                                     AND (@VaiTro='' OR VaiTro=@VaiTro)
                                     ORDER BY MaNguoiDung DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Keyword", keyword);
                    cmd.Parameters.AddWithValue("@VaiTro", vaiTro);

                    List<EmployeeData> list = new List<EmployeeData>();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new EmployeeData
                            {
                                MaNguoiDung = (int)r["MaNguoiDung"],
                                TenDangNhap = r["TenDangNhap"].ToString(),
                                HoTen = r["HoTen"].ToString(),
                                Email = r["Email"].ToString(),
                                VaiTro = r["VaiTro"].ToString(),
                                SDT = r["SDT"].ToString(),
                                NgayTao = (DateTime)r["NgayTao"],
                                TrangThai = (bool)r["TrangThai"]
                            });
                        }
                    }
                    return new EmployeeResult { Success = true, Employees = list };
                }
            }
            catch (Exception ex) { return new EmployeeResult { Success = false, Message = ex.Message }; }
        }

        // ====================== ADD EMPLOYEE ======================
        public static EmployeeResult AddEmployee(string tenDangNhap, string matKhau, string hoTen, string email, string vaiTro, string sdt, DateTime ngayVaoLam)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string check = "SELECT COUNT(*) FROM NGUOIDUNG WHERE TenDangNhap=@u OR Email=@e";
                    SqlCommand chk = new SqlCommand(check, conn);
                    chk.Parameters.AddWithValue("@u", tenDangNhap);
                    chk.Parameters.AddWithValue("@e", email);
                    if ((int)chk.ExecuteScalar() > 0) return new EmployeeResult { Success = false, Message = "Trùng tài khoản hoặc email" };

                    string hashed = BCrypt.Net.BCrypt.HashPassword(matKhau);
                    string insert = @"INSERT INTO NGUOIDUNG(TenDangNhap,MatKhau,HoTen,Email,VaiTro,SDT,NgayTao,TrangThai)
                                      OUTPUT INSERTED.MaNguoiDung
                                      VALUES(@u,@p,@n,@e,@r,@sdt,@ngay,1)";

                    SqlCommand cmd = new SqlCommand(insert, conn);
                    cmd.Parameters.AddWithValue("@u", tenDangNhap);
                    cmd.Parameters.AddWithValue("@p", hashed);
                    cmd.Parameters.AddWithValue("@n", hoTen);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@r", vaiTro);
                    cmd.Parameters.AddWithValue("@sdt", sdt ?? "");
                    cmd.Parameters.AddWithValue("@ngay", ngayVaoLam);

                    int newId = (int)cmd.ExecuteScalar();
                    return new EmployeeResult { Success = true, MaNguoiDung = newId };
                }
            }
            catch (Exception ex) { return new EmployeeResult { Success = false, Message = ex.Message }; }
        }

        // ====================== UPDATE EMPLOYEE ======================
        public static EmployeeResult UpdateEmployee(int maNguoiDung, string hoTen, string email, string vaiTro, string sdt, bool trangThai)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string update = @"UPDATE NGUOIDUNG SET HoTen=@n, Email=@e, VaiTro=@r, SDT=@sdt, TrangThai=@st WHERE MaNguoiDung=@id";
                    SqlCommand cmd = new SqlCommand(update, conn);
                    cmd.Parameters.AddWithValue("@id", maNguoiDung);
                    cmd.Parameters.AddWithValue("@n", hoTen);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@r", vaiTro);
                    cmd.Parameters.AddWithValue("@sdt", sdt ?? "");
                    cmd.Parameters.AddWithValue("@st", trangThai);
                    int row = cmd.ExecuteNonQuery();
                    return new EmployeeResult { Success = row > 0 };
                }
            }
            catch (Exception ex) { return new EmployeeResult { Success = false, Message = ex.Message }; }
        }

        // ====================== DELETE EMPLOYEE ======================
        public static EmployeeResult DeleteEmployee(int ma)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string soft = "UPDATE NGUOIDUNG SET TrangThai=0 WHERE MaNguoiDung=@id";
                    SqlCommand cmd = new SqlCommand(soft, conn);
                    cmd.Parameters.AddWithValue("@id", ma);
                    int rows = cmd.ExecuteNonQuery();
                    return new EmployeeResult { Success = rows > 0 };
                }
            }
            catch (Exception ex) { return new EmployeeResult { Success = false, Message = ex.Message }; }
        }

        // ====================== MENU & BILLS ======================
        public static MenuResult GetMenu()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string q = "SELECT * FROM MENUITEMS ORDER BY MaMon DESC";
                    SqlCommand cmd = new SqlCommand(q, conn);
                    var list = new List<MenuItemData>();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new MenuItemData
                            {
                                MaMon = (int)r["MaMon"],
                                TenMon = r["TenMon"].ToString(),
                                Gia = Convert.ToDecimal(r["Gia"]),
                                MoTa = r["MoTa"].ToString(),
                                TrangThai = r["TrangThai"].ToString(),
                                MaLoaiMon = r["MaLoaiMon"] as int?
                            });
                        }
                    }
                    return new MenuResult { Success = true, Items = list };
                }
            }
            catch (Exception ex) { return new MenuResult { Success = false, Message = ex.Message }; }
        }

        public static MenuResult SearchMenu(string keyword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string q = @"SELECT * FROM MENUITEMS WHERE @kw='' OR TenMon LIKE '%' + @kw + '%' ORDER BY MaMon DESC";
                    SqlCommand cmd = new SqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@kw", keyword);
                    var list = new List<MenuItemData>();
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new MenuItemData
                            {
                                MaMon = (int)r["MaMon"],
                                TenMon = r["TenMon"].ToString(),
                                Gia = Convert.ToDecimal(r["Gia"]),
                                MoTa = r["MoTa"].ToString(),
                                TrangThai = r["TrangThai"].ToString(),
                                MaLoaiMon = r["MaLoaiMon"] as int?
                            });
                        }
                    }
                    return new MenuResult { Success = true, Items = list };
                }
            }
            catch (Exception ex) { return new MenuResult { Success = false, Message = ex.Message }; }
        }

        public static MenuResult AddMenu(string tenMon, decimal gia, string moTa, int? maLoaiMon, string trangThai)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO MENUITEMS (TenMon, Gia, MoTa, MaLoaiMon, TrangThai)
                                     OUTPUT INSERTED.MaMon
                                     VALUES (@TenMon, @Gia, @MoTa, @MaLoaiMon, @TrangThai)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenMon", tenMon);
                        cmd.Parameters.AddWithValue("@Gia", gia);
                        cmd.Parameters.AddWithValue("@MoTa", moTa ?? "");
                        cmd.Parameters.AddWithValue("@MaLoaiMon", maLoaiMon ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                        int newId = (int)cmd.ExecuteScalar();
                        return new MenuResult { Success = true, Message = "Thêm món thành công", MaMon = newId };
                    }
                }
            }
            catch (Exception ex) { return new MenuResult { Success = false, Message = $"Lỗi thêm món: {ex.Message}" }; }
        }

        public static MenuResult UpdateMenu(int maMon, string tenMon, decimal gia, string moTa, int? maLoaiMon, string trangThai)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE MENUITEMS SET TenMon = @TenMon, Gia = @Gia, MoTa = @MoTa, MaLoaiMon = @MaLoaiMon, TrangThai = @TrangThai
                                     WHERE MaMon = @MaMon";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaMon", maMon);
                        cmd.Parameters.AddWithValue("@TenMon", tenMon);
                        cmd.Parameters.AddWithValue("@Gia", gia);
                        cmd.Parameters.AddWithValue("@MoTa", moTa ?? "");
                        cmd.Parameters.AddWithValue("@MaLoaiMon", maLoaiMon ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return new MenuResult { Success = rowsAffected > 0, Message = rowsAffected > 0 ? "Cập nhật món thành công" : "Món không tồn tại" };
                    }
                }
            }
            catch (Exception ex) { return new MenuResult { Success = false, Message = $"Lỗi cập nhật: {ex.Message}" }; }
        }

        public static MenuResult DeleteMenu(int maMon)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string q = "UPDATE MENUITEMS SET TrangThai='HetMon' WHERE MaMon=@id";
                    SqlCommand cmd = new SqlCommand(q, conn);
                    cmd.Parameters.AddWithValue("@id", maMon);
                    int rows = cmd.ExecuteNonQuery();
                    return new MenuResult { Success = rows > 0 };
                }
            }
            catch (Exception ex) { return new MenuResult { Success = false, Message = ex.Message }; }
        }

        public static MenuResult UpdateMenuStatus(int maMon, string trangThai)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE MENUITEMS SET TrangThai = @TrangThai WHERE MaMon = @MaMon";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaMon", maMon);
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return new MenuResult
                        {
                            Success = rowsAffected > 0,
                            Message = rowsAffected > 0 ? $"Đã cập nhật trạng thái thành '{trangThai}'" : "Món không tồn tại"
                        };
                    }
                }
            }
            catch (Exception ex) { return new MenuResult { Success = false, Message = $"Lỗi cập nhật: {ex.Message}" }; }
        }

        public static RegisterResult UpdatePassword(string email, string newPassword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string check = "SELECT COUNT(*) FROM NGUOIDUNG WHERE Email = @e AND TrangThai = 1";
                    using (SqlCommand cmdCheck = new SqlCommand(check, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@e", email);
                        if (Convert.ToInt32(cmdCheck.ExecuteScalar()) == 0)
                            return new RegisterResult { Success = false, Message = "Email chưa đăng ký hoặc tài khoản đã bị khóa" };
                    }

                    string hashed = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    string update = "UPDATE NGUOIDUNG SET MatKhau = @p WHERE Email = @e AND TrangThai = 1";
                    using (SqlCommand cmdUpdate = new SqlCommand(update, conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@p", hashed);
                        cmdUpdate.Parameters.AddWithValue("@e", email);
                        int rows = cmdUpdate.ExecuteNonQuery();
                        return new RegisterResult { Success = rows > 0, Message = rows > 0 ? "Đổi mật khẩu thành công" : "Đổi mật khẩu thất bại" };
                    }
                }
            }
            catch (Exception ex) { return new RegisterResult { Success = false, Message = $"Lỗi hệ thống: {ex.Message}" }; }
        }

        public static BillResult GetBills()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT MaHD, MaBanAn, MaNV, Ngay, TongTien, TrangThai FROM HOADON ORDER BY Ngay DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        var bills = new List<BillData>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bills.Add(new BillData
                                {
                                    MaHoaDon = (int)reader["MaHD"],
                                    MaBanAn = (int)reader["MaBanAn"],
                                    MaNhanVien = (int)reader["MaNV"],
                                    NgayXuatHoaDon = (DateTime)reader["Ngay"],
                                    TongTien = reader["TongTien"] != DBNull.Value ? Convert.ToDecimal(reader["TongTien"]) : 0,
                                    TrangThai = reader["TrangThai"]?.ToString() ?? ""
                                });
                            }
                        }
                        return new BillResult { Success = true, Message = $"Tìm thấy {bills.Count} hóa đơn", Bills = bills };
                    }
                }
            }
            catch (Exception ex) { return new BillResult { Success = false, Message = $"Lỗi truy xuất hóa đơn: {ex.Message}", Bills = new List<BillData>() }; }
        }

        // ✅ SỬA 2: Tách riêng hàm GetDoanhThuTheoBan, viết lại logic đúng
        public static List<DoanhThuTheoBan> GetDoanhThuTheoBan(DateTime tuNgay, DateTime denNgay)
        {
            var result = new List<DoanhThuTheoBan>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT b.TenBan, b.MaBanAn,
                                     COUNT(hd.MaHD) as SoLuongHoaDon,
                                     ISNULL(SUM(hd.TongTien), 0) as DoanhThu,
                                     ISNULL(MAX(hd.TongTien), 0) as HoaDonLonNhat,
                                     ISNULL(MIN(CASE WHEN hd.TongTien > 0 THEN hd.TongTien END), 0) as HoaDonNhoNhat,
                                     ISNULL(AVG(CASE WHEN hd.TongTien > 0 THEN hd.TongTien END), 0) as DoanhThuTB
                                     FROM BAN b
                                     LEFT JOIN HOADON hd ON b.MaBanAn = hd.MaBanAn
                                     AND hd.Ngay BETWEEN @TuNgay AND @DenNgay
                                     AND hd.TrangThai = N'DaThanhToan'
                                     GROUP BY b.MaBanAn, b.TenBan
                                     ORDER BY DoanhThu DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new DoanhThuTheoBan
                                {
                                    TenBan = reader["TenBan"].ToString(),
                                    MaBanAn = Convert.ToInt32(reader["MaBanAn"]),
                                    SoLuongHoaDon = Convert.ToInt32(reader["SoLuongHoaDon"]),
                                    DoanhThu = Convert.ToDecimal(reader["DoanhThu"]),
                                    HoaDonLonNhat = Convert.ToDecimal(reader["HoaDonLonNhat"]),
                                    HoaDonNhoNhat = Convert.ToDecimal(reader["HoaDonNhoNhat"]),
                                    DoanhThuTB = Convert.ToDecimal(reader["DoanhThuTB"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            return result;
        }

        // ✅ SỬA 3: Thêm lại hàm AddBanToSQL đúng chuẩn
        public static bool AddBanToSQL(Models.Database.BanAn banMoi)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // Giả sử bảng của bạn là BAN, cột là MaBanAn (hoặc MaBan), TenBan, TrangThai
                    // Bạn cần chỉnh lại tên bảng/cột cho khớp DB của bạn nếu khác
                    string insertQuery = @"INSERT INTO BAN (MaBanAn, TenBan, TrangThai) VALUES (@id, @ten, @trangthai)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", banMoi.MaBan);
                        cmd.Parameters.AddWithValue("@ten", banMoi.TenBan);
                        cmd.Parameters.AddWithValue("@trangthai", banMoi.TrangThai);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                return false;
            }
        }

        public static decimal GetTongDoanhThu(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT ISNULL(SUM(TongTien), 0) as TongDoanhThu
                                     FROM HOADON
                                     WHERE Ngay BETWEEN @TuNgay AND @DenNgay
                                     AND TrangThai = N'DaThanhToan'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                        object result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi GetTongDoanhThu: {ex.Message}");
                return 0;
            }
        }

        public static DoanhThuResult GetDoanhThuFull(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                var tongDoanhThu = GetTongDoanhThu(tuNgay, denNgay);
                var doanhThuTheoBan = GetDoanhThuTheoBan(tuNgay, denNgay);
                return new DoanhThuResult
                {
                    Success = true,
                    Message = "Thống kê thành công",
                    TongDoanhThu = new TongDoanhThu
                    {
                        tongDoanhThu = tongDoanhThu,
                        TongSoHoaDon = doanhThuTheoBan.Sum(x => x.SoLuongHoaDon),
                        TongSoBan = doanhThuTheoBan.Count(x => x.SoLuongHoaDon > 0),
                        TuNgay = tuNgay,
                        DenNgay = denNgay
                    },
                    DoanhThuTheoBan = doanhThuTheoBan
                };
            }
            catch (Exception ex)
            {
                return new DoanhThuResult { Success = false, Message = $"Lỗi thống kê: {ex.Message}" };
            }
        }

        // ✅ SỬA 4: Xóa code rác ở cuối hàm
        public static (bool success, string filePath, string message) XuatBaoCaoExcel(
            DateTime tuNgay, DateTime denNgay, List<DoanhThuTheoBan> data, decimal tongDoanhThu)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string fileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("DoanhThu");

                    // Tiêu đề
                    worksheet.Cells["A1:F1"].Merge = true;
                    worksheet.Cells["A1"].Value = "BÁO CÁO DOANH THU NHÀ HÀNG";
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.Font.Size = 16;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A2"].Value = $"Từ ngày: {tuNgay:dd/MM/yyyy}";
                    worksheet.Cells["D2"].Value = $"Đến ngày: {denNgay:dd/MM/yyyy}";

                    worksheet.Cells["A3"].Value = "TỔNG DOANH THU:";
                    worksheet.Cells["B3"].Value = tongDoanhThu;
                    worksheet.Cells["B3"].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells["C3"].Value = "VNĐ";
                    worksheet.Cells["A3"].Style.Font.Bold = true;
                    worksheet.Cells["B3"].Style.Font.Bold = true;
                    worksheet.Cells["B3"].Style.Font.Color.SetColor(Color.Red);

                    worksheet.Cells["A4"].Value = $"Tổng số bàn: {data.Count}";
                    worksheet.Cells["C4"].Value = $"Số bàn có doanh thu: {data.Count(x => x.DoanhThu > 0)}";
                    worksheet.Cells["E4"].Value = $"Tổng hóa đơn: {data.Sum(x => x.SoLuongHoaDon)}";

                    // Header bảng
                    string[] headers = { "Tên Bàn", "Số Hóa Đơn", "Doanh Thu (VNĐ)", "Hóa Đơn Lớn Nhất (VNĐ)", "Hóa Đơn Nhỏ Nhất (VNĐ)", "Doanh Thu TB (VNĐ)" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cells[6, i + 1];
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Data
                    int row = 7;
                    foreach (var item in data)
                    {
                        worksheet.Cells[row, 1].Value = item.TenBan;
                        worksheet.Cells[row, 2].Value = item.SoLuongHoaDon;
                        worksheet.Cells[row, 3].Value = item.DoanhThu;
                        worksheet.Cells[row, 4].Value = item.HoaDonLonNhat;
                        worksheet.Cells[row, 5].Value = item.HoaDonNhoNhat;
                        worksheet.Cells[row, 6].Value = item.DoanhThuTB;

                        for (int col = 3; col <= 6; col++) worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";

                        if (item.DoanhThu > 0)
                        {
                            for (int col = 1; col <= 6; col++)
                            {
                                worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                            }
                        }
                        row++;
                    }

                    // Tổng kết cuối bảng
                    worksheet.Cells[row + 1, 1].Value = "TỔNG CỘNG:";
                    worksheet.Cells[row + 1, 1].Style.Font.Bold = true;
                    worksheet.Cells[row + 1, 2].Formula = $"SUM(B7:B{row})";
                    worksheet.Cells[row + 1, 2].Style.Font.Bold = true;
                    worksheet.Cells[row + 1, 3].Formula = $"SUM(C7:C{row})";
                    worksheet.Cells[row + 1, 3].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row + 1, 3].Style.Font.Bold = true;
                    worksheet.Cells[row + 1, 3].Style.Font.Color.SetColor(Color.Red);

                    // Format border & Autofit
                    var dataRange = worksheet.Cells[6, 1, row + 1, 6];
                    dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[1, 1, row + 1, 6].AutoFitColumns();

                    // Footer
                    worksheet.Cells[row + 3, 1].Value = $"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[row + 4, 1].Value = "Hệ thống Quản lý Nhà hàng";

                    package.SaveAs(new FileInfo(filePath));
                }
                return (true, filePath, "Xuất báo cáo Excel thành công");
            }
            catch (Exception ex)
            {
                return (false, "", $"Lỗi xuất báo cáo Excel: {ex.Message}");
            }
        }
    }
}