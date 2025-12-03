using BCrypt.Net;
using Models;
using Models.Database;
using Models.Request;
using Models.Response;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

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
                    string q = "SELECT * FROM MENUITEMS MN JOIN LOAIMON LM ON MN.MaLoaiMon = LM.MaLoaiMon ORDER BY MN.MaMon DESC";
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
                    string q = @"SELECT * FROM MENUITEMS MN JOIN LOAIMON LM ON MN.MaLoaiMon = LM.MaLoaiMon WHERE @kw='' OR MN.TenMon LIKE '%' + @kw + '%' ORDER BY MN.MaMon DESC";
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

                    // 1. KIỂM TRA MA LOẠI MÓN CÓ TỒN TẠI KHÔNG (nếu có giá trị)
                    if (maLoaiMon.HasValue && maLoaiMon.Value > 0)
                    {
                        string checkQuery = "SELECT COUNT(*) FROM LOAIMON WHERE MaLoaiMon = @MaLoaiMon AND TrangThai = 1";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@MaLoaiMon", maLoaiMon.Value);
                            int exists = (int)checkCmd.ExecuteScalar();

                            if (exists == 0)
                            {
                                return new MenuResult
                                {
                                    Success = false,
                                    Message = $"Lỗi: Mã loại món '{maLoaiMon.Value}' không tồn tại hoặc đã bị khóa"
                                };
                            }
                        }
                    }

                    // 2. THÊM MÓN MỚI
                    string insertQuery = @"INSERT INTO MENUITEMS (TenMon, Gia, MoTa, MaLoaiMon, TrangThai)
                                   OUTPUT INSERTED.MaMon
                                   VALUES (@TenMon, @Gia, @MoTa, @MaLoaiMon, @TrangThai)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenMon", tenMon);
                        cmd.Parameters.AddWithValue("@Gia", gia);
                        cmd.Parameters.AddWithValue("@MoTa", moTa ?? string.Empty);

                        // Xử lý giá trị NULL cho ngoại khóa
                        if (maLoaiMon.HasValue && maLoaiMon.Value > 0)
                        {
                            cmd.Parameters.AddWithValue("@MaLoaiMon", maLoaiMon.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@MaLoaiMon", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                        int newId = (int)cmd.ExecuteScalar();

                        return new MenuResult
                        {
                            Success = true,
                            Message = "Thêm món thành công",
                            MaMon = newId
                        };
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

                    // 1. KIỂM TRA MA LOẠI MÓN CÓ TỒN TẠI KHÔNG (nếu có giá trị)
                    if (maLoaiMon.HasValue && maLoaiMon.Value > 0)
                    {
                        string checkQuery = "SELECT COUNT(*) FROM LOAIMON WHERE MaLoaiMon = @MaLoaiMon";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@MaLoaiMon", maLoaiMon.Value);
                            int exists = (int)checkCmd.ExecuteScalar();

                            if (exists == 0)
                            {
                                return new MenuResult
                                {
                                    Success = false,
                                    Message = $"Mã loại món '{maLoaiMon.Value}' không tồn tại trong hệ thống"
                                };
                            }
                        }
                    }

                    // 2. CẬP NHẬT THÔNG TIN MÓN
                    string updateQuery = @"UPDATE MENUITEMS 
                                   SET TenMon = @TenMon, 
                                       Gia = @Gia, 
                                       MoTa = @MoTa, 
                                       MaLoaiMon = @MaLoaiMon, 
                                       TrangThai = @TrangThai
                                   WHERE MaMon = @MaMon";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaMon", maMon);
                        cmd.Parameters.AddWithValue("@TenMon", tenMon);
                        cmd.Parameters.AddWithValue("@Gia", gia);
                        cmd.Parameters.AddWithValue("@MoTa", moTa ?? "");

                        // Xử lý đúng giá trị NULL cho ngoại khóa
                        if (maLoaiMon.HasValue && maLoaiMon.Value > 0)
                        {
                            cmd.Parameters.AddWithValue("@MaLoaiMon", maLoaiMon.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@MaLoaiMon", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return new MenuResult
                        {
                            Success = rowsAffected > 0,
                            Message = rowsAffected > 0 ? "Cập nhật món thành công" : "Món không tồn tại"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new MenuResult
                {
                    Success = false,
                    Message = $"Lỗi cập nhật: {ex.Message}"
                };
            }
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
        // ====================== BAN AN ======================

        public static BanAnResult GetTables()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // ✅ CHỈ LẤY BÀN KHÔNG ẨN
                    string query = @"SELECT MaBanAn, TenBan, SoChoNgoi, TrangThai, MaNhanVien 
                           FROM BAN 
                           WHERE TrangThai != N'An'
                           ORDER BY MaBanAn";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        var tables = new List<BanAnData>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tables.Add(new BanAnData
                                {
                                    MaBanAn = (int)reader["MaBanAn"],
                                    TenBan = reader["TenBan"].ToString(),
                                    SoChoNgoi = reader["SoChoNgoi"] as int?,
                                    TrangThai = reader["TrangThai"].ToString(),
                                    MaNhanVien = reader["MaNhanVien"] as int?
                                });
                            }
                        }
                        return new BanAnResult { Success = true, Tables = tables, Message = $"Lấy danh sách {tables.Count} bàn thành công" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BanAnResult { Success = false, Message = $"Lỗi lấy danh sách bàn: {ex.Message}" };
            }
        }
        public static BanAnResult SearchTables(string keyword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT MaBanAn, TenBan, SoChoNgoi, TrangThai, MaNhanVien 
                           FROM BAN 
                           WHERE @Keyword = '' OR TenBan LIKE '%' + @Keyword + '%'
                           ORDER BY MaBanAn";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Keyword", keyword);
                        var tables = new List<BanAnData>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tables.Add(new BanAnData
                                {
                                    MaBanAn = (int)reader["MaBanAn"],
                                    TenBan = reader["TenBan"].ToString(),
                                    SoChoNgoi = reader["SoChoNgoi"] as int?,
                                    TrangThai = reader["TrangThai"].ToString(),
                                    MaNhanVien = reader["MaNhanVien"] as int?
                                });
                            }
                        }
                        return new BanAnResult { Success = true, Tables = tables, Message = $"Tìm thấy {tables.Count} bàn" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BanAnResult { Success = false, Message = $"Lỗi tìm kiếm bàn: {ex.Message}" };
            }
        }

        /// <summary>
        /// Thêm bàn ăn mới
        /// </summary>
        public static BanAnResult AddTable(string tenBan, int? soChoNgoi, string trangThai, int? maNhanVien)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra tên bàn đã tồn tại chưa
                    string checkQuery = "SELECT COUNT(*) FROM BAN WHERE TenBan = @TenBan";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@TenBan", tenBan);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                            return new BanAnResult { Success = false, Message = "Tên bàn đã tồn tại" };
                    }

                    // Thêm bàn mới
                    string insertQuery = @"INSERT INTO BAN (TenBan, SoChoNgoi, TrangThai, MaNhanVien) 
                                 OUTPUT INSERTED.MaBanAn
                                 VALUES (@TenBan, @SoChoNgoi, @TrangThai, @MaNhanVien)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenBan", tenBan);
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                        // THÊM KIỂM TRA NÀY VÀO CẢ 2 PHƯƠNG THỨC:
                        if (string.IsNullOrWhiteSpace(tenBan))
                            return new BanAnResult { Success = false, Message = "Tên bàn không được để trống" };

                        if (soChoNgoi.HasValue && soChoNgoi.Value <= 0)
                            return new BanAnResult { Success = false, Message = "Số chỗ ngồi phải lớn hơn 0" };
                        // Xử lý parameter nullable
                        if (soChoNgoi.HasValue)
                            cmd.Parameters.AddWithValue("@SoChoNgoi", soChoNgoi.Value);
                        else
                            cmd.Parameters.AddWithValue("@SoChoNgoi", DBNull.Value);

                        if (maNhanVien.HasValue)
                            cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien.Value);
                        else
                            cmd.Parameters.AddWithValue("@MaNhanVien", DBNull.Value);

                        int newId = (int)cmd.ExecuteScalar();
                        return new BanAnResult
                        {
                            Success = true,
                            Message = "Thêm bàn thành công",
                            MaBanAn = newId
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BanAnResult { Success = false, Message = $"Lỗi thêm bàn: {ex.Message}" };
            }
        }

        /// <summary>
        /// Cập nhật thông tin bàn ăn
        /// </summary>
        public static BanAnResult UpdateTable(int maBanAn, string tenBan, int? soChoNgoi, string trangThai, int? maNhanVien)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra tên bàn đã tồn tại chưa (trừ bàn hiện tại)
                    string checkQuery = "SELECT COUNT(*) FROM BAN WHERE TenBan = @TenBan AND MaBanAn != @MaBanAn";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@TenBan", tenBan);
                        checkCmd.Parameters.AddWithValue("@MaBanAn", maBanAn);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                            return new BanAnResult { Success = false, Message = "Tên bàn đã tồn tại" };
                    }

                    // Cập nhật bàn
                    string updateQuery = @"UPDATE BAN 
                                 SET TenBan = @TenBan, 
                                     SoChoNgoi = @SoChoNgoi, 
                                     TrangThai = @TrangThai,
                                     MaNhanVien = @MaNhanVien
                                 WHERE MaBanAn = @MaBanAn";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaBanAn", maBanAn);
                        cmd.Parameters.AddWithValue("@TenBan", tenBan);
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                        // Xử lý parameter nullable
                        // THÊM KIỂM TRA NÀY VÀO CẢ 2 PHƯƠNG THỨC:
                        if (string.IsNullOrWhiteSpace(tenBan))
                            return new BanAnResult { Success = false, Message = "Tên bàn không được để trống" };

                        if (soChoNgoi.HasValue && soChoNgoi.Value <= 0)
                            return new BanAnResult { Success = false, Message = "Số chỗ ngồi phải lớn hơn 0" };

                        if (soChoNgoi.HasValue)
                            cmd.Parameters.AddWithValue("@SoChoNgoi", soChoNgoi.Value);
                        else
                            cmd.Parameters.AddWithValue("@SoChoNgoi", DBNull.Value);

                        if (maNhanVien.HasValue)
                            cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien.Value);
                        else
                            cmd.Parameters.AddWithValue("@MaNhanVien", DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            return new BanAnResult { Success = true, Message = "Cập nhật bàn thành công" };
                        else
                            return new BanAnResult { Success = false, Message = "Không tìm thấy bàn để cập nhật" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BanAnResult { Success = false, Message = $"Lỗi cập nhật bàn: {ex.Message}" };
            }
        }

        /// <summary>
        /// Xóa bàn ăn (soft delete - cập nhật trạng thái)
        public static BanAnResult DeleteTable(int maBanAn)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ THAY VÌ XÓA -> ẨN BÀN
                    string hideQuery = "UPDATE BAN SET TrangThai = N'An' WHERE MaBanAn = @MaBanAn";
                    using (SqlCommand cmd = new SqlCommand(hideQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaBanAn", maBanAn);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                            return new BanAnResult { Success = true, Message = "Đã ẩn bàn thành công" };
                        else
                            return new BanAnResult { Success = false, Message = "Không tìm thấy bàn để ẩn" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new BanAnResult { Success = false, Message = $"Lỗi ẩn bàn: {ex.Message}" };
            }
        }
      
        //===================== DOANH THU ======================
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
        // ==================== LẤY DANH SÁCH CHỜ THANH TOÁN ====================
        public static PendingPaymentResult GetPendingPayments(int maNhanVien)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                    SELECT 
                        hd.MaHD,
                        hd.MaBanAn,
                        b.TenBan,
                        hd.MaNV AS MaNhanVien,
                        nd.HoTen AS TenNhanVien,
                        hd.Ngay AS NgayTao,
                        hd.TongTien,
                        hd.TrangThai,
                        (SELECT COUNT(*) FROM CTHD WHERE MaHD = hd.MaHD) AS SoMon
                    FROM HOADON hd
                    INNER JOIN BAN b ON hd.MaBanAn = b.MaBanAn
                    INNER JOIN NGUOIDUNG nd ON hd.MaNV = nd.MaNguoiDung
                    WHERE hd.TrangThai = N'ChuaThanhToan'
                        AND (@MaNhanVien = 0 OR hd.MaNV = @MaNhanVien)
                    ORDER BY hd.Ngay DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);

                        var payments = new List<PendingPaymentData>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                payments.Add(new PendingPaymentData
                                {
                                    MaHD = (int)reader["MaHD"],
                                    MaBanAn = (int)reader["MaBanAn"],
                                    TenBan = reader["TenBan"].ToString(),
                                    MaNhanVien = (int)reader["MaNhanVien"],
                                    TenNhanVien = reader["TenNhanVien"].ToString(),
                                    NgayTao = (DateTime)reader["NgayTao"],
                                    TongTien = Convert.ToDecimal(reader["TongTien"]),
                                    TrangThai = reader["TrangThai"].ToString(),
                                    SoMon = (int)reader["SoMon"]
                                });
                            }
                        }

                        return new PendingPaymentResult
                        {
                            Success = true,
                            Message = $"Lấy được {payments.Count} hóa đơn chờ thanh toán",
                            PendingPayments = payments
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new PendingPaymentResult
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    PendingPayments = new List<PendingPaymentData>()
                };
            }
        }

        // ==================== XỬ LÝ THANH TOÁN TIỀN MẶT ====================
        public static CashPaymentResult ProcessCashPayment(int maHD, decimal soTienNhan, int maNV)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Kiểm tra hóa đơn tồn tại và chưa thanh toán
                        string checkSql = @"
                        SELECT TongTien, TrangThai 
                        FROM HOADON 
                        WHERE MaHD = @MaHD";

                        decimal tongTien = 0;
                        string trangThai = "";

                        using (SqlCommand checkCmd = new SqlCommand(checkSql, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@MaHD", maHD);
                            using (SqlDataReader reader = checkCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    tongTien = Convert.ToDecimal(reader["TongTien"]);
                                    trangThai = reader["TrangThai"].ToString();
                                }
                            }
                        }

                        if (tongTien == 0)
                            throw new Exception("Hóa đơn không tồn tại");

                        if (trangThai != "ChuaThanhToan")
                            throw new Exception("Hóa đơn đã được thanh toán");

                        // 2. Kiểm tra số tiền nhận
                        if (soTienNhan < tongTien)
                            throw new Exception($"Số tiền nhận không đủ. Cần: {tongTien:N0} VNĐ");

                        decimal soTienThua = soTienNhan - tongTien;

                        // 3. Thêm giao dịch thanh toán vào bảng THANHTOAN
                        string insertPaymentSql = @"
                        INSERT INTO THANHTOAN (
                            MaHD, MaNhanVien, PhuongThucThanhToan, 
                            SoTienThanhToan, SoTienNhan, SoTienThua,
                            TrangThai, ThoiGianThanhToan, GhiChu
                        )
                        VALUES (
                            @MaHD, @MaNhanVien, N'TienMat',
                            @SoTienThanhToan, @SoTienNhan, @SoTienThua,
                            N'ThanhCong', GETDATE(), N'Thanh toán tiền mặt'
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        int maGiaoDich;
                        using (SqlCommand insertCmd = new SqlCommand(insertPaymentSql, conn, transaction))
                        {
                            insertCmd.Parameters.AddWithValue("@MaHD", maHD);
                            insertCmd.Parameters.AddWithValue("@MaNhanVien", maNV);
                            insertCmd.Parameters.AddWithValue("@SoTienThanhToan", tongTien);
                            insertCmd.Parameters.AddWithValue("@SoTienNhan", soTienNhan);
                            insertCmd.Parameters.AddWithValue("@SoTienThua", soTienThua);

                            maGiaoDich = Convert.ToInt32(insertCmd.ExecuteScalar());
                        }

                        // 4. Cập nhật trạng thái hóa đơn thành "Đã thanh toán"
                        string updateHoaDonSql = @"
                        UPDATE HOADON 
                        SET TrangThai = N'DaThanhToan',
                            PhuongThucThanhToan = N'TienMat'
                        WHERE MaHD = @MaHD";

                        using (SqlCommand updateCmd = new SqlCommand(updateHoaDonSql, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@MaHD", maHD);
                            int rowsAffected = updateCmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                                throw new Exception("Không thể cập nhật hóa đơn");
                        }

                        transaction.Commit();

                        return new CashPaymentResult
                        {
                            Success = true,
                            Message = "Thanh toán tiền mặt thành công",
                            SoTienThua = soTienThua,
                            NgayThanhToan = DateTime.Now,
                            MaGiaoDich = maGiaoDich.ToString(),
                            MaGiaoDichId = maGiaoDich
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new CashPaymentResult
                        {
                            Success = false,
                            Message = $"Lỗi thanh toán: {ex.Message}"
                        };
                    }
                }
            }
        }

        // ==================== XỬ LÝ THANH TOÁN CHUYỂN KHOẢN ====================
        public static TransferPaymentResult ProcessTransferPayment(int maHD, int maNV)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Kiểm tra hóa đơn
                        string checkSql = @"
                        SELECT TongTien, TrangThai 
                        FROM HOADON 
                        WHERE MaHD = @MaHD";

                        decimal tongTien = 0;
                        string trangThai = "";

                        using (SqlCommand checkCmd = new SqlCommand(checkSql, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@MaHD", maHD);
                            using (SqlDataReader reader = checkCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    tongTien = Convert.ToDecimal(reader["TongTien"]);
                                    trangThai = reader["TrangThai"].ToString();
                                }
                            }
                        }

                        if (tongTien == 0)
                            throw new Exception("Hóa đơn không tồn tại");

                        if (trangThai != "ChuaThanhToan")
                            throw new Exception("Hóa đơn đã được thanh toán");

                        // 2. Tạo mã giao dịch ngân hàng
                        string transactionNo = "TRF" + DateTime.Now.ToString("yyyyMMddHHmmss") + maHD;

                        // 3. Tạo QR code data (giả lập)
                        string qrCodeData = $"bank://transfer?amount={tongTien}&account=NH_QUANAN&note=HD{maHD}";

                        // 4. Thêm giao dịch thanh toán
                        string insertPaymentSql = @"
                        INSERT INTO THANHTOAN (
                            MaHD, MaNhanVien, PhuongThucThanhToan, 
                            SoTienThanhToan, TrangThai, 
                            MaGiaoDichNganHang, QRCodeData, GhiChu
                        )
                        VALUES (
                            @MaHD, @MaNhanVien, N'ChuyenKhoan',
                            @SoTienThanhToan, N'ThanhCong',
                            @MaGiaoDichNganHang, @QRCodeData, N'Thanh toán chuyển khoản'
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        int maGiaoDich;
                        using (SqlCommand insertCmd = new SqlCommand(insertPaymentSql, conn, transaction))
                        {
                            insertCmd.Parameters.AddWithValue("@MaHD", maHD);
                            insertCmd.Parameters.AddWithValue("@MaNhanVien", maNV);
                            insertCmd.Parameters.AddWithValue("@SoTienThanhToan", tongTien);
                            insertCmd.Parameters.AddWithValue("@MaGiaoDichNganHang", transactionNo);
                            insertCmd.Parameters.AddWithValue("@QRCodeData", qrCodeData);

                            maGiaoDich = Convert.ToInt32(insertCmd.ExecuteScalar());
                        }

                        // 5. Cập nhật trạng thái hóa đơn
                        string updateHoaDonSql = @"
                        UPDATE HOADON 
                        SET TrangThai = N'DaThanhToan',
                            PhuongThucThanhToan = N'ChuyenKhoan'
                        WHERE MaHD = @MaHD";

                        using (SqlCommand updateCmd = new SqlCommand(updateHoaDonSql, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@MaHD", maHD);
                            int rowsAffected = updateCmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                                throw new Exception("Không thể cập nhật hóa đơn");
                        }

                        transaction.Commit();

                        return new TransferPaymentResult
                        {
                            Success = true,
                            Message = "Thanh toán chuyển khoản thành công",
                            TransactionNo = transactionNo,
                            QRCodeData = qrCodeData,
                            NgayThanhToan = DateTime.Now,
                            MaGiaoDichId = maGiaoDich
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new TransferPaymentResult
                        {
                            Success = false,
                            Message = $"Lỗi thanh toán: {ex.Message}"
                        };
                    }
                }
            }
        }

        public static OrderMonResult GetMon()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT MaMon, TenMon, Gia, MoTa, TrangThai FROM MENUITEMS ";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        var mons = new List<OrderMonData>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mons.Add(new OrderMonData
                                {
                                    MaMon = (int)reader["MaMon"],
                                    TenMon = reader["TenMon"].ToString(),
                                    Gia = (int)reader["Gia"],
                                    MoTa = reader["Gia"].ToString(),
                                    TrangThai = reader["TrangThai"]?.ToString() ?? ""
                                });
                            }
                        }
                        return new OrderMonResult { Success = true, Message = $"Tìm thấy {mons.Count} món", OrderMons = mons };
                    }
                }
            }
            catch (Exception ex) { return new OrderMonResult { Success = false, Message = $"Lỗi truy xuất món: {ex.Message}", OrderMons = new List<OrderMonData>() }; }
        }
        // ==================== chọn theo danh mục món ====================

        public static CategoryResult GetCategories()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT MaLoaiMon, TenLoai 
                             FROM LoaiMon 
                             WHERE TrangThai = 1 
                             ORDER BY MaLoaiMon";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        var categories = new List<CategoryData>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new CategoryData
                                {
                                    MaLoaiMon = (int)reader["MaLoaiMon"],
                                    TenLoai = reader["TenLoai"].ToString()
                                });
                            }
                        }
                        return new CategoryResult
                        {
                            Success = true,
                            Categories = categories,
                            Message = $"Lấy được {categories.Count} loại món"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new CategoryResult
                {
                    Success = false,
                    Message = $"Lỗi lấy danh sách loại món: {ex.Message}",
                    Categories = new List<CategoryData>()
                };
            }
        }

        // 🔥 THÊM: Lấy món ăn theo loại
        public static MenuResult GetMenuByCategory(int maLoaiMon)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;
                    SqlCommand cmd;

                    if (maLoaiMon == 0) // Tất cả món
                    {
                        query = @"SELECT * FROM MENUITEMS 
                          WHERE TrangThai != 'HetMon' 
                          ORDER BY MaMon DESC";
                        cmd = new SqlCommand(query, conn);
                    }
                    else // Món theo loại
                    {
                        query = @"SELECT * FROM MENUITEMS 
                          WHERE MaLoaiMon = @MaLoaiMon 
                          AND TrangThai != 'HetMon' 
                          ORDER BY MaMon DESC";
                        cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaLoaiMon", maLoaiMon);
                    }

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

                    return new MenuResult
                    {
                        Success = true,
                        Items = list,
                        Message = $"Tìm thấy {list.Count} món ăn"
                    };
                }
            }
            catch (Exception ex)
            {
                return new MenuResult
                {
                    Success = false,
                    Message = $"Lỗi lấy món theo loại: {ex.Message}",
                    Items = new List<MenuItemData>()
                };
            }
        }
        public static CreateOrderResult CreateOrder(int maBan, int maNhanVien, decimal tongTien, List<ChiTietOrder> chiTiet)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Thêm hóa đơn
                        string insertHoaDon = @"
                    INSERT INTO HOADON (MaBanAn, MaNV, Ngay, TongTien, TrangThai)
                    OUTPUT INSERTED.MaHD
                    VALUES (@MaBanAn, @MaNV, GETDATE(), @TongTien, N'ChuaThanhToan')";

                        int maHoaDon;
                        using (SqlCommand cmd = new SqlCommand(insertHoaDon, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@MaBanAn", maBan);
                            cmd.Parameters.AddWithValue("@MaNV", maNhanVien);
                            cmd.Parameters.AddWithValue("@TongTien", tongTien);
                            maHoaDon = (int)cmd.ExecuteScalar();
                        }

                        // 2. Thêm chi tiết hóa đơn
                        string insertChiTiet = @"
                    INSERT INTO CTHD (MaHD, MaMon, SoLuong, DonGia)
                    VALUES (@MaHD, @MaMon, @SoLuong, @DonGia)";

                        foreach (var item in chiTiet)
                        {
                            using (SqlCommand cmd = new SqlCommand(insertChiTiet, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@MaHD", maHoaDon);
                                cmd.Parameters.AddWithValue("@MaMon", item.MaMon);
                                cmd.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                                cmd.Parameters.AddWithValue("@DonGia", item.DonGia);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3. Cập nhật trạng thái bàn
                        string updateBan = "UPDATE BAN SET TrangThai = 'DangSuDung' WHERE MaBanAn = @MaBanAn";
                        using (SqlCommand cmd = new SqlCommand(updateBan, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@MaBanAn", maBan);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        return new CreateOrderResult
                        {
                            Success = true,
                            Message = "Tạo order thành công",
                            MaHoaDon = maHoaDon
                        };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new CreateOrderResult
                        {
                            Success = false,
                            Message = $"Lỗi tạo order: {ex.Message}"
                        };
                    }
                }
            }
        }
        // 1. Thêm tham số string trangThai vào hàm
        public static GetTableDetailResponse GetTableDetails(int maBanAn, string trangThai)
        {
            var result = new GetTableDetailResponse();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    // 🔥 SỬA QUERY: Thêm hd.TrangThai vào danh sách lấy
                    string query = @"
                SELECT 
                    hd.MaBanAn, 
                    hd.TrangThai, -- 🔥 Lấy thêm cột này
                    m.TenMon, 
                    ct.SoLuong, 
                    ct.DonGia, 
                    hd.Ngay
                FROM HOADON hd
                JOIN CTHD ct ON hd.MaHD = ct.MaHD
                JOIN MENUITEMS m ON ct.MaMon = m.MaMon
                WHERE (@MaBanAn = 0 OR hd.MaBanAn = @MaBanAn) 
                AND (@TrangThai = '' OR hd.TrangThai = @TrangThai)
                  AND (@TrangThai = '' OR hd.TrangThai = @TrangThai)
                ORDER BY hd.Ngay DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaBanAn", maBanAn);
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai ?? "");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Orders.Add(new TableOrderDetailData
                                {
                                    MaBanAn = Convert.ToInt32(reader["MaBanAn"]),

                                    // 🔥 Đọc trạng thái từ SQL bỏ vào hộp
                                    TrangThai = reader["TrangThai"].ToString(),

                                    TenMon = reader["TenMon"].ToString(),
                                    SoLuong = Convert.ToInt32(reader["SoLuong"]),
                                    DonGia = Convert.ToDecimal(reader["DonGia"]),
                                    ThoiGianGoi = Convert.ToDateTime(reader["Ngay"])
                                });
                            }
                        }
                    }
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Lỗi: " + ex.Message;
            }
            return result;
        }

        // ==================== KITCHEN ORDER FUNCTIONS ====================

        public static KitchenOrdersResult GetKitchenOrders(string trangThai = "", string timKiemBan = "",
     string sapXep = "ThoiGian", int? maNhanVienBep = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query tổng hợp TẤT CẢ trong 1 lần
                    string query = @"
                SELECT TOP 50
                    dh.MaDonHang,
                    dh.MaBanAn,
                    b.TenBan,
                    dh.NgayOrder,
                    dh.TrangThai as TrangThaiDon,
                    nv.HoTen as TenNhanVienOrder,
                    
                    -- Thống kê món
                    (SELECT COUNT(*) FROM CHITIET_DONHANG ctdh WHERE ctdh.MaDonHang = dh.MaDonHang) as TongSoMon,
                    
                    -- Số món theo trạng thái (SUBQUERY hiệu quả hơn)
                    (SELECT COUNT(*) FROM CHITIET_DONHANG ctdh2 
                     WHERE ctdh2.MaDonHang = dh.MaDonHang AND ctdh2.TrangThai = 'ChoXacNhan') as SoMonChoXacNhan,
                    (SELECT COUNT(*) FROM CHITIET_DONHANG ctdh2 
                     WHERE ctdh2.MaDonHang = dh.MaDonHang AND ctdh2.TrangThai = 'DangCheBien') as SoMonDangCheBien,
                    (SELECT COUNT(*) FROM CHITIET_DONHANG ctdh2 
                     WHERE ctdh2.MaDonHang = dh.MaDonHang AND ctdh2.TrangThai = 'HoanThanh') as SoMonHoanThanh,
                    (SELECT COUNT(*) FROM CHITIET_DONHANG ctdh2 
                     WHERE ctdh2.MaDonHang = dh.MaDonHang AND ctdh2.TrangThai = 'CoVanDe') as SoMonCoVanDe,
                    (SELECT COUNT(*) FROM CHITIET_DONHANG ctdh2 
                     WHERE ctdh2.MaDonHang = dh.MaDonHang AND ctdh2.TrangThai = 'Huy') as SoMonHuy,
                    
                    -- Ưu tiên cao nhất
                    (SELECT MAX(UuTien) FROM CHITIET_DONHANG ctdh3 
                     WHERE ctdh3.MaDonHang = dh.MaDonHang) as UuTienCaoNhat,
                    
                    -- Tổng tiền
                    ISNULL((SELECT SUM(ctdh.SoLuong * ctdh.DonGia) 
                           FROM CHITIET_DONHANG ctdh 
                           WHERE ctdh.MaDonHang = dh.MaDonHang), 0) as TongTien
                    
                FROM DONHANG dh
                LEFT JOIN BAN b ON dh.MaBanAn = b.MaBanAn
                LEFT JOIN NGUOIDUNG nv ON dh.MaNVOrder = nv.MaNguoiDung
                WHERE 1=1";

                    // Thêm điều kiện WHERE
                    if (!string.IsNullOrEmpty(timKiemBan))
                    {
                        query += " AND b.TenBan LIKE '%' + @TimKiemBan + '%'";
                    }

                    if (!string.IsNullOrEmpty(trangThai) && trangThai != "TatCa")
                    {
                        query += " AND dh.TrangThai = @TrangThai";
                    }

                    // Sắp xếp
                    query += " ORDER BY ";
                    switch (sapXep)
                    {
                        case "UuTien":
                            query += "UuTienCaoNhat DESC, dh.NgayOrder DESC";
                            break;
                        case "Ban":
                            query += "b.TenBan ASC, dh.NgayOrder DESC";
                            break;
                        case "ThoiGianCho":
                            query += "dh.NgayOrder ASC";
                            break;
                        default: // "ThoiGian"
                            query += "dh.NgayOrder DESC";
                            break;
                    }

                    SqlCommand cmd = new SqlCommand(query, conn);

                    if (!string.IsNullOrEmpty(timKiemBan))
                        cmd.Parameters.AddWithValue("@TimKiemBan", timKiemBan);

                    if (!string.IsNullOrEmpty(trangThai) && trangThai != "TatCa")
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    List<KitchenOrderData> orders = new List<KitchenOrderData>();

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var order = new KitchenOrderData
                            {
                                MaDonHang = (int)r["MaDonHang"],
                                MaBanAn = r["MaBanAn"] != DBNull.Value ? Convert.ToInt32(r["MaBanAn"]) : 0,
                                TenBan = r["TenBan"]?.ToString() ?? "Không xác định",
                                NgayOrder = (DateTime)r["NgayOrder"],
                                TenNhanVienOrder = r["TenNhanVienOrder"]?.ToString() ?? "Không xác định",
                                TongSoMon = r["TongSoMon"] != DBNull.Value ? Convert.ToInt32(r["TongSoMon"]) : 0,
                                TongTien = r["TongTien"] != DBNull.Value ? Convert.ToDecimal(r["TongTien"]) : 0,

                                // Thống kê món
                                SoMonChoXacNhan = r["SoMonChoXacNhan"] != DBNull.Value ? Convert.ToInt32(r["SoMonChoXacNhan"]) : 0,
                                SoMonDangCheBien = r["SoMonDangCheBien"] != DBNull.Value ? Convert.ToInt32(r["SoMonDangCheBien"]) : 0,
                                SoMonHoanThanh = r["SoMonHoanThanh"] != DBNull.Value ? Convert.ToInt32(r["SoMonHoanThanh"]) : 0,
                                SoMonCoVanDe = r["SoMonCoVanDe"] != DBNull.Value ? Convert.ToInt32(r["SoMonCoVanDe"]) : 0,
                                SoMonHuy = r["SoMonHuy"] != DBNull.Value ? Convert.ToInt32(r["SoMonHuy"]) : 0,
                                UuTienCaoNhat = r["UuTienCaoNhat"] != DBNull.Value ? Convert.ToInt32(r["UuTienCaoNhat"]) : 1
                            };

                            orders.Add(order);
                        }
                    }

                    // Tính thống kê (không cần query thêm)
                    var thongKe = new ThongKeBep
                    {
                        TongSoDon = orders.Count,
                        TongSoMon = orders.Sum(o => o.TongSoMon),
                        DonChoXacNhan = orders.Count(o => o.SoMonChoXacNhan > 0),
                        DonDangCheBien = orders.Count(o => o.SoMonDangCheBien > 0),
                        DonHoanThanh = orders.Count(o => o.SoMonHoanThanh == o.TongSoMon && o.TongSoMon > 0),
                        DonCoVanDe = orders.Count(o => o.SoMonCoVanDe > 0),
                        DonHuy = orders.Count(o => o.SoMonHuy == o.TongSoMon && o.TongSoMon > 0)
                    };

                    return new KitchenOrdersResult
                    {
                        Success = true,
                        DonHang = orders,
                        ThongKe = thongKe,
                        Message = $"Tìm thấy {orders.Count} đơn hàng"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ LỖI GetKitchenOrders: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return new KitchenOrdersResult
                {
                    Success = false,
                    Message = $"Lỗi lấy đơn hàng bếp: {ex.Message}"
                };
            }
        }
        /// <summary>
        /// Lấy chi tiết đơn hàng cho bếp
        /// </summary>
        public static OrderDetailResult GetOrderDetail(int maDonHang)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ KIỂM TRA: Query có chạy được không?
                    string orderQuery = @"
                SELECT 
                    dh.MaDonHang,
                    dh.MaBanAn,
                    b.TenBan,
                    dh.NgayOrder,
                    nv.HoTen as TenNhanVienOrder,
                    dh.TrangThai as TrangThaiDon,
                    ISNULL((SELECT SUM(ctdh.SoLuong * ctdh.DonGia) 
                            FROM CHITIET_DONHANG ctdh 
                            WHERE ctdh.MaDonHang = dh.MaDonHang), 0) as TongTien,
                    (SELECT MAX(ThoiGianDuKien) FROM CHITIET_DONHANG ctdh 
                     WHERE ctdh.MaDonHang = dh.MaDonHang 
                     AND ctdh.ThoiGianDuKien IS NOT NULL) as ThoiGianDuKienHoanThanh
                FROM DONHANG dh
                INNER JOIN BAN b ON dh.MaBanAn = b.MaBanAn
                INNER JOIN NGUOIDUNG nv ON dh.MaNVOrder = nv.MaNguoiDung
                WHERE dh.MaDonHang = @MaDonHang";

                    SqlCommand cmd = new SqlCommand(orderQuery, conn);
                    cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                    KitchenOrderDetailData orderDetail = null;

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            orderDetail = new KitchenOrderDetailData
                            {
                                MaDonHang = (int)r["MaDonHang"],
                                MaBanAn = (int)r["MaBanAn"],
                                TenBan = r["TenBan"].ToString(),
                                NgayOrder = (DateTime)r["NgayOrder"],
                                TenNhanVienOrder = r["TenNhanVienOrder"].ToString(),
                                TrangThaiDon = r["TrangThaiDon"].ToString(),
                                TongTien = Convert.ToDecimal(r["TongTien"]),
                                ThoiGianDuKienHoanThanh = r["ThoiGianDuKienHoanThanh"] != DBNull.Value ?
                                    (DateTime?)Convert.ToDateTime(r["ThoiGianDuKienHoanThanh"]) : null,

                                // ✅ QUAN TRỌNG: PHẢI KHỞI TẠO
                                DanhSachMon = new List<KitchenDishData>(),
                                TinNhan = new List<KitchenMessageData>()
                            };
                        }
                    }

                    // ✅ KIỂM TRA: Có tìm thấy đơn hàng không?
                    if (orderDetail == null)
                    {
                        Console.WriteLine($"❌ Không tìm thấy đơn hàng MaDonHang={maDonHang}");
                        return new OrderDetailResult
                        {
                            Success = false,
                            Message = "Không tìm thấy đơn hàng"
                        };
                    }

                    // Query chi tiết món ăn
                    string dishesQuery = @"
                SELECT 
                    ctdh.MaChiTiet,
                    ctdh.MaMon,
                    mi.TenMon,
                    ctdh.SoLuong,
                    ctdh.DonGia,
                    ctdh.GhiChuKhach,
                    ctdh.TrangThai,
                    ctdh.GhiChuBep,
                    ctdh.MaNhanVienCheBien,
                    nv.HoTen as TenNhanVienCheBien,
                    ctdh.UuTien,
                    ctdh.ThoiGianBatDau,
                    ctdh.ThoiGianHoanThanh,
                    ctdh.ThoiGianDuKien
                FROM CHITIET_DONHANG ctdh
                INNER JOIN MENUITEMS mi ON ctdh.MaMon = mi.MaMon
                LEFT JOIN NGUOIDUNG nv ON ctdh.MaNhanVienCheBien = nv.MaNguoiDung
                WHERE ctdh.MaDonHang = @MaDonHang
                ORDER BY ctdh.UuTien DESC, ctdh.MaChiTiet";

                    cmd = new SqlCommand(dishesQuery, conn);
                    cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var dish = new KitchenDishData
                            {
                                MaChiTiet = (int)r["MaChiTiet"],
                                MaMon = (int)r["MaMon"],
                                TenMon = r["TenMon"].ToString(),
                                SoLuong = (int)r["SoLuong"],
                                DonGia = Convert.ToDecimal(r["DonGia"]),
                                GhiChuKhach = r["GhiChuKhach"] != DBNull.Value ? r["GhiChuKhach"].ToString() : "",
                                TrangThai = r["TrangThai"].ToString(),
                                GhiChuBep = r["GhiChuBep"] != DBNull.Value ? r["GhiChuBep"].ToString() : "",
                                MaNhanVienCheBien = r["MaNhanVienCheBien"] != DBNull.Value ?
                                    (int?)Convert.ToInt32(r["MaNhanVienCheBien"]) : null,
                                TenNhanVienCheBien = r["TenNhanVienCheBien"] != DBNull.Value ?
                                    r["TenNhanVienCheBien"].ToString() : "",
                                UuTien = (int)r["UuTien"],
                                ThoiGianBatDau = r["ThoiGianBatDau"] != DBNull.Value ?
                                    (DateTime?)Convert.ToDateTime(r["ThoiGianBatDau"]) : null,
                                ThoiGianHoanThanh = r["ThoiGianHoanThanh"] != DBNull.Value ?
                                    (DateTime?)Convert.ToDateTime(r["ThoiGianHoanThanh"]) : null,
                                ThoiGianDuKien = r["ThoiGianDuKien"] != DBNull.Value ?
                                    (DateTime?)Convert.ToDateTime(r["ThoiGianDuKien"]) : null
                            };

                            orderDetail.DanhSachMon.Add(dish);
                        }
                    }

                    // ✅ THÊM LOG ĐỂ DEBUG
                    Console.WriteLine($"✅ GetOrderDetail: MaDonHang={maDonHang}, SoMon={orderDetail.DanhSachMon.Count}");

                    // Query tin nhắn (có thể bỏ qua nếu không cần)
                    // ...

                    return new OrderDetailResult
                    {
                        Success = true,
                        ChiTietDonHang = orderDetail,
                        Message = "Lấy chi tiết đơn hàng thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                // ✅ LOG CHI TIẾT
                Console.WriteLine($"❌ LỖI GetOrderDetail: {ex.Message}");
                Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");

                return new OrderDetailResult
                {
                    Success = false,
                    Message = $"Lỗi lấy chi tiết đơn: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Cập nhật trạng thái món ăn
        /// </summary>
        public static UpdateDishStatusResult UpdateDishStatus(int maDonHang, int maChiTiet, string trangThaiMoi,
            int? maNhanVienBep, string ghiChuBep, DateTime? thoiGianDuKienHoanThanh, int uuTien, bool guiThongBao)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Lấy thông tin món cũ trước khi update
                    string getOldInfo = @"
                        SELECT mi.TenMon, ctdh.TrangThai as TrangThaiCu
                        FROM CHITIET_DONHANG ctdh
                        INNER JOIN MENUITEMS mi ON ctdh.MaMon = mi.MaMon
                        WHERE ctdh.MaChiTiet = @MaChiTiet AND ctdh.MaDonHang = @MaDonHang";

                    SqlCommand cmd = new SqlCommand(getOldInfo, conn);
                    cmd.Parameters.AddWithValue("@MaChiTiet", maChiTiet);
                    cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                    string tenMon = "";
                    string trangThaiCu = "";

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            tenMon = r["TenMon"].ToString();
                            trangThaiCu = r["TrangThaiCu"].ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(tenMon))
                    {
                        return new UpdateDishStatusResult
                        {
                            Success = false,
                            Message = "Không tìm thấy món cần cập nhật"
                        };
                    }

                    // Update trạng thái món
                    string updateQuery = @"
                        UPDATE CHITIET_DONHANG SET 
                            TrangThai = @TrangThaiMoi,
                            GhiChuBep = @GhiChuBep,
                            MaNhanVienCheBien = @MaNhanVienBep,
                            UuTien = @UuTien,
                            ThoiGianDuKien = @ThoiGianDuKien
                        WHERE MaChiTiet = @MaChiTiet AND MaDonHang = @MaDonHang";

                    // Cập nhật thời gian bắt đầu nếu chuyển sang "DangCheBien"
                    if (trangThaiMoi == "DangCheBien" && trangThaiCu != "DangCheBien")
                    {
                        updateQuery = @"
                            UPDATE CHITIET_DONHANG SET 
                                TrangThai = @TrangThaiMoi,
                                GhiChuBep = @GhiChuBep,
                                MaNhanVienCheBien = @MaNhanVienBep,
                                UuTien = @UuTien,
                                ThoiGianDuKien = @ThoiGianDuKien,
                                ThoiGianBatDau = GETDATE()
                            WHERE MaChiTiet = @MaChiTiet AND MaDonHang = @MaDonHang";
                    }

                    // Cập nhật thời gian hoàn thành nếu chuyển sang "HoanThanh"
                    if (trangThaiMoi == "HoanThanh" && trangThaiCu != "HoanThanh")
                    {
                        updateQuery = @"
                            UPDATE CHITIET_DONHANG SET 
                                TrangThai = @TrangThaiMoi,
                                GhiChuBep = @GhiChuBep,
                                MaNhanVienCheBien = @MaNhanVienBep,
                                UuTien = @UuTien,
                                ThoiGianDuKien = @ThoiGianDuKien,
                                ThoiGianHoanThanh = GETDATE()
                            WHERE MaChiTiet = @MaChiTiet AND MaDonHang = @MaDonHang";
                    }

                    cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@MaChiTiet", maChiTiet);
                    cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                    cmd.Parameters.AddWithValue("@TrangThaiMoi", trangThaiMoi);
                    cmd.Parameters.AddWithValue("@GhiChuBep", ghiChuBep ?? "");
                    cmd.Parameters.AddWithValue("@MaNhanVienBep", maNhanVienBep ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UuTien", uuTien);
                    cmd.Parameters.AddWithValue("@ThoiGianDuKien", thoiGianDuKienHoanThanh ?? (object)DBNull.Value);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Gửi thông báo cho phục vụ nếu được yêu cầu
                        if (guiThongBao)
                        {
                            SendKitchenNotification(conn, maDonHang, tenMon, trangThaiCu, trangThaiMoi, ghiChuBep);
                        }

                        return new UpdateDishStatusResult
                        {
                            Success = true,
                            TenMon = tenMon,
                            TrangThaiCu = trangThaiCu,
                            TrangThaiMoi = trangThaiMoi,
                            Message = $"Đã cập nhật trạng thái '{tenMon}' thành công"
                        };
                    }
                    else
                    {
                        return new UpdateDishStatusResult
                        {
                            Success = false,
                            Message = "Không thể cập nhật trạng thái món"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new UpdateDishStatusResult
                {
                    Success = false,
                    Message = $"Lỗi cập nhật trạng thái: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Gửi thông báo từ bếp đến phục vụ
        /// </summary>
        private static void SendKitchenNotification(SqlConnection conn, int maDonHang, string tenMon,
            string trangThaiCu, string trangThaiMoi, string ghiChuBep)
        {
            try
            {
                // Lấy thông tin bàn và nhân viên phục vụ
                string getInfoQuery = @"
                    SELECT dh.MaBanAn, b.TenBan, nv.MaNguoiDung, nv.HoTen
                    FROM DONHANG dh
                    INNER JOIN BAN b ON dh.MaBanAn = b.MaBanAn
                    INNER JOIN NGUOIDUNG nv ON b.MaNhanVien = nv.MaNguoiDung
                    WHERE dh.MaDonHang = @MaDonHang AND nv.VaiTro = 'PhucVu'";

                SqlCommand cmd = new SqlCommand(getInfoQuery, conn);
                cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        int maBanAn = (int)r["MaBanAn"];
                        string tenBan = r["TenBan"].ToString();
                        int maNhanVienPhucVu = (int)r["MaNguoiDung"];
                        string tenNhanVienPhucVu = r["HoTen"].ToString();

                        // Tạo nội dung thông báo
                        string noiDung = $"Bếp: Trạng thái '{tenMon}' đã thay đổi từ '{trangThaiCu}' sang '{trangThaiMoi}'";
                        if (!string.IsNullOrEmpty(ghiChuBep))
                        {
                            noiDung += $". Ghi chú: {ghiChuBep}";
                        }

                        // Lấy mã đầu bếp hiện tại (giả sử đang xử lý)
                        int maDauBep = GetCurrentChefId(conn);

                        // Gửi tin nhắn
                        SendMessage(conn, maDauBep, maNhanVienPhucVu, noiDung);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi thông báo: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy ID đầu bếp hiện tại (giả sử)
        /// </summary>
        private static int GetCurrentChefId(SqlConnection conn)
        {
            try
            {
                string query = "SELECT TOP 1 MaNguoiDung FROM NGUOIDUNG WHERE VaiTro = 'Bep' AND TrangThai = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 1; // Default to 1 if not found
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Gửi tin nhắn
        /// </summary>
        private static void SendMessage(SqlConnection conn, int maNguoiGui, int maNguoiNhan, string noiDung)
        {
            try
            {
                string insertQuery = @"
                    INSERT INTO TINNHAN (MaNguoiGui, MaNguoiNhan, NoiDung, ThoiGian, DaDoc)
                    VALUES (@MaNguoiGui, @MaNguoiNhan, @NoiDung, GETDATE(), 0)";

                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@MaNguoiGui", maNguoiGui);
                cmd.Parameters.AddWithValue("@MaNguoiNhan", maNguoiNhan);
                cmd.Parameters.AddWithValue("@NoiDung", noiDung);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gửi tin nhắn: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thống kê công việc bếp
        /// </summary>
        public static KitchenStatisticsResult GetKitchenStatistics(DateTime tuNgay, DateTime denNgay, int? maNhanVienBep = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    KitchenStatisticsData statistics = new KitchenStatisticsData
                    {
                        TuNgay = tuNgay,
                        DenNgay = denNgay,
                        TopMonAn = new List<TopMonData>(),
                        HieuSuatDauBep = new List<HieuSuatDauBep>(),
                        PhanBoTheoGio = new Dictionary<int, int>()
                    };

                    // Tổng số đơn và món
                    string totalQuery = @"
                        SELECT 
                            COUNT(DISTINCT dh.MaDonHang) as TongSoDon,
                            COUNT(ctdh.MaChiTiet) as TongSoMon,
                            AVG(DATEDIFF(MINUTE, ctdh.ThoiGianBatDau, ctdh.ThoiGianHoanThanh)) as ThoiGianTrungBinh,
                            SUM(CASE WHEN ctdh.TrangThai = 'ChoXacNhan' THEN 1 ELSE 0 END) as SoMonChoXacNhan,
                            SUM(CASE WHEN ctdh.TrangThai = 'DangCheBien' THEN 1 ELSE 0 END) as SoMonDangCheBien,
                            SUM(CASE WHEN ctdh.TrangThai = 'HoanThanh' THEN 1 ELSE 0 END) as SoMonHoanThanh,
                            SUM(CASE WHEN ctdh.TrangThai = 'CoVanDe' THEN 1 ELSE 0 END) as SoMonCoVanDe,
                            SUM(CASE WHEN ctdh.TrangThai = 'Huy' THEN 1 ELSE 0 END) as SoMonHuy
                        FROM DONHANG dh
                        INNER JOIN CHITIET_DONHANG ctdh ON dh.MaDonHang = ctdh.MaDonHang
                        WHERE dh.NgayOrder BETWEEN @TuNgay AND @DenNgay
                        AND (@MaNhanVienBep IS NULL OR ctdh.MaNhanVienCheBien = @MaNhanVienBep)";

                    SqlCommand cmd = new SqlCommand(totalQuery, conn);
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    if (maNhanVienBep.HasValue)
                        cmd.Parameters.AddWithValue("@MaNhanVienBep", maNhanVienBep.Value);
                    else
                        cmd.Parameters.AddWithValue("@MaNhanVienBep", DBNull.Value);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            statistics.TongSoDon = r["TongSoDon"] != DBNull.Value ? Convert.ToInt32(r["TongSoDon"]) : 0;
                            statistics.TongSoMon = r["TongSoMon"] != DBNull.Value ? Convert.ToInt32(r["TongSoMon"]) : 0;
                            statistics.ThoiGianTrungBinh = r["ThoiGianTrungBinh"] != DBNull.Value ?
                                TimeSpan.FromMinutes(Convert.ToDouble(r["ThoiGianTrungBinh"])) : TimeSpan.Zero;
                            statistics.SoMonChoXacNhan = r["SoMonChoXacNhan"] != DBNull.Value ? Convert.ToInt32(r["SoMonChoXacNhan"]) : 0;
                            statistics.SoMonDangCheBien = r["SoMonDangCheBien"] != DBNull.Value ? Convert.ToInt32(r["SoMonDangCheBien"]) : 0;
                            statistics.SoMonHoanThanh = r["SoMonHoanThanh"] != DBNull.Value ? Convert.ToInt32(r["SoMonHoanThanh"]) : 0;
                            statistics.SoMonCoVanDe = r["SoMonCoVanDe"] != DBNull.Value ? Convert.ToInt32(r["SoMonCoVanDe"]) : 0;
                            statistics.SoMonHuy = r["SoMonHuy"] != DBNull.Value ? Convert.ToInt32(r["SoMonHuy"]) : 0;
                        }
                    }

                    // Top 5 món được order nhiều nhất
                    string topMonQuery = @"
                        SELECT TOP 5 
                            ctdh.MaMon,
                            mi.TenMon,
                            COUNT(*) as SoLanOrder,
                            SUM(ctdh.SoLuong) as TongSoPhan
                        FROM CHITIET_DONHANG ctdh
                        INNER JOIN MENUITEMS mi ON ctdh.MaMon = mi.MaMon
                        INNER JOIN DONHANG dh ON ctdh.MaDonHang = dh.MaDonHang
                        WHERE dh.NgayOrder BETWEEN @TuNgay AND @DenNgay
                        GROUP BY ctdh.MaMon, mi.TenMon
                        ORDER BY SoLanOrder DESC, TongSoPhan DESC";

                    cmd = new SqlCommand(topMonQuery, conn);
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            statistics.TopMonAn.Add(new TopMonData
                            {
                                MaMon = (int)r["MaMon"],
                                TenMon = r["TenMon"].ToString(),
                                SoLanOrder = (int)r["SoLanOrder"],
                                TongSoPhan = (int)r["TongSoPhan"]
                            });
                        }
                    }

                    // Hiệu suất đầu bếp
                    string hieuSuatQuery = @"
                        SELECT 
                            nv.MaNguoiDung,
                            nv.HoTen as TenNhanVien,
                            COUNT(ctdh.MaChiTiet) as TongSoMon,
                            AVG(DATEDIFF(MINUTE, ctdh.ThoiGianBatDau, ctdh.ThoiGianHoanThanh)) as ThoiGianTrungBinh
                        FROM NGUOIDUNG nv
                        INNER JOIN CHITIET_DONHANG ctdh ON nv.MaNguoiDung = ctdh.MaNhanVienCheBien
                        INNER JOIN DONHANG dh ON ctdh.MaDonHang = dh.MaDonHang
                        WHERE nv.VaiTro = 'Bep' 
                        AND nv.TrangThai = 1
                        AND dh.NgayOrder BETWEEN @TuNgay AND @DenNgay
                        AND (@MaNhanVienBep IS NULL OR nv.MaNguoiDung = @MaNhanVienBep)
                        GROUP BY nv.MaNguoiDung, nv.HoTen
                        HAVING COUNT(ctdh.MaChiTiet) > 0
                        ORDER BY TongSoMon DESC";

                    cmd = new SqlCommand(hieuSuatQuery, conn);
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                    if (maNhanVienBep.HasValue)
                        cmd.Parameters.AddWithValue("@MaNhanVienBep", maNhanVienBep.Value);
                    else
                        cmd.Parameters.AddWithValue("@MaNhanVienBep", DBNull.Value);

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var hieuSuat = new HieuSuatDauBep
                            {
                                MaNhanVien = (int)r["MaNguoiDung"],
                                TenNhanVien = r["TenNhanVien"].ToString(),
                                TongSoMon = (int)r["TongSoMon"],
                                ThoiGianTrungBinh = r["ThoiGianTrungBinh"] != DBNull.Value ?
                                    TimeSpan.FromMinutes(Convert.ToDouble(r["ThoiGianTrungBinh"])) : TimeSpan.Zero
                            };

                            // Tính hiệu suất phần trăm (giả sử)
                            hieuSuat.HieuSuatPhanTram = CalculateEfficiency(hieuSuat);

                            statistics.HieuSuatDauBep.Add(hieuSuat);
                        }
                    }

                    // Phân bố theo giờ
                    for (int gio = 7; gio <= 22; gio++) // Từ 7h đến 22h
                    {
                        string hourQuery = @"
                            SELECT COUNT(DISTINCT dh.MaDonHang) as SoDon
                            FROM DONHANG dh
                            WHERE dh.NgayOrder BETWEEN @TuNgay AND @DenNgay
                            AND DATEPART(HOUR, dh.NgayOrder) = @Gio";

                        cmd = new SqlCommand(hourQuery, conn);
                        cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                        cmd.Parameters.AddWithValue("@DenNgay", denNgay);
                        cmd.Parameters.AddWithValue("@Gio", gio);

                        int soDon = Convert.ToInt32(cmd.ExecuteScalar());
                        statistics.PhanBoTheoGio[gio] = soDon;
                    }

                    return new KitchenStatisticsResult
                    {
                        Success = true,
                        ThongKe = statistics,
                        Message = "Lấy thống kê thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                return new KitchenStatisticsResult
                {
                    Success = false,
                    Message = $"Lỗi lấy thống kê: {ex.Message}"
                };
            }
        }

        private static int CalculateEfficiency(HieuSuatDauBep hieuSuat)
        {
            // Logic tính hiệu suất (giả sử)
            // Nếu thời gian trung bình < 10 phút => hiệu suất cao
            // 10-15 phút => trung bình
            // >15 phút => thấp

            if (hieuSuat.ThoiGianTrungBinh.TotalMinutes <= 10)
                return 90 + hieuSuat.TongSoMon; // Tăng theo số món đã làm
            else if (hieuSuat.ThoiGianTrungBinh.TotalMinutes <= 15)
                return 70 + hieuSuat.TongSoMon;
            else
                return 50 + hieuSuat.TongSoMon;
        }

        /// <summary>
        /// Cập nhật nhiều món cùng lúc
        /// </summary>
        public static UpdateDishStatusResult UpdateMultipleDishes(int maDonHang, List<DishUpdateItem> danhSachMon, bool guiThongBao)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    int soMonDaCapNhat = 0;
                    List<string> tenCacMon = new List<string>();

                    foreach (var mon in danhSachMon)
                    {
                        var result = UpdateDishStatus(
                            maDonHang,
                            mon.MaChiTiet,
                            mon.TrangThaiMoi,
                            mon.MaNhanVienBep,
                            mon.GhiChuBep,
                            mon.ThoiGianDuKien,
                            mon.UuTien,
                            guiThongBao && soMonDaCapNhat == 0 // Chỉ gửi thông báo cho món đầu tiên
                        );

                        if (result.Success)          
                        {
                            soMonDaCapNhat++;
                            tenCacMon.Add(result.TenMon);
                        }
                    }

                    return new UpdateDishStatusResult
                    {
                        Success = soMonDaCapNhat > 0,
                        TenMon = string.Join(", ", tenCacMon),
                        Message = $"Đã cập nhật {soMonDaCapNhat} món thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                return new UpdateDishStatusResult
                {
                    Success = false,
                    Message = $"Lỗi cập nhật nhiều món: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Gửi tin nhắn từ bếp
        /// </summary>
        public static SendMessageResult SendKitchenMessage(int maDonHang, int maNhanVienGui, int? maNhanVienNhan,
            string noiDung, string loaiTinNhan, bool hienPopup, bool phatAmThanh)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Nếu không chỉ định người nhận, gửi cho tất cả phục vụ phụ trách bàn đó
                    if (!maNhanVienNhan.HasValue || maNhanVienNhan.Value == 0)
                    {
                        // Lấy tất cả phục vụ phụ trách bàn
                        string getWaitersQuery = @"
                            SELECT DISTINCT nv.MaNguoiDung
                            FROM DONHANG dh
                            INNER JOIN BAN b ON dh.MaBanAn = b.MaBanAn
                            INNER JOIN NGUOIDUNG nv ON b.MaNhanVien = nv.MaNguoiDung
                            WHERE dh.MaDonHang = @MaDonHang AND nv.VaiTro = 'PhucVu'";

                        SqlCommand cmd = new SqlCommand(getWaitersQuery, conn);
                        cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                        List<int> maCacNhanVien = new List<int>();
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                maCacNhanVien.Add((int)r["MaNguoiDung"]);
                            }
                        }

                        // Gửi tin nhắn cho từng phục vụ
                        foreach (int maNV in maCacNhanVien)
                        {
                            SendMessage(conn, maNhanVienGui, maNV, noiDung);
                        }

                        return new SendMessageResult
                        {
                            Success = true,
                            MaTinNhan = maCacNhanVien.Count > 0 ? 1 : 0,
                            Message = $"Đã gửi tin nhắn cho {maCacNhanVien.Count} phục vụ"
                        };
                    }
                    else
                    {
                        // Gửi cho một phục vụ cụ thể
                        SendMessage(conn, maNhanVienGui, maNhanVienNhan.Value, noiDung);

                        return new SendMessageResult
                        {
                            Success = true,
                            MaTinNhan = 1,
                            Message = "Đã gửi tin nhắn thành công"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new SendMessageResult
                {
                    Success = false,
                    Message = $"Lỗi gửi tin nhắn: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Lấy lịch sử tin nhắn
        /// </summary>
        public static KitchenMessagesResult GetKitchenMessages(int maDonHang, int? maNhanVienBep)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Lấy thông tin bàn từ đơn hàng
                    string getTableQuery = "SELECT MaBanAn FROM DONHANG WHERE MaDonHang = @MaDonHang";
                    SqlCommand cmd = new SqlCommand(getTableQuery, conn);
                    cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        return new KitchenMessagesResult
                        {
                            Success = false,
                            Message = "Không tìm thấy đơn hàng"
                        };
                    }

                    int maBanAn = (int)result;

                    // Lấy tin nhắn liên quan đến bàn này
                    string messagesQuery = @"
                        SELECT 
                            tn.MaTin,
                            tn.MaNguoiGui,
                            gui.HoTen as TenNguoiGui,
                            gui.VaiTro as VaiTroNguoiGui,
                            tn.MaNguoiNhan,
                            nhan.HoTen as TenNguoiNhan,
                            tn.NoiDung,
                            tn.ThoiGian,
                            tn.DaDoc
                        FROM TINNHAN tn
                        INNER JOIN NGUOIDUNG gui ON tn.MaNguoiGui = gui.MaNguoiDung
                        INNER JOIN NGUOIDUNG nhan ON tn.MaNguoiNhan = nhan.MaNguoiDung
                        WHERE (tn.MaNguoiGui = @MaNhanVienBep OR tn.MaNguoiNhan = @MaNhanVienBep)
                        AND EXISTS (
                            SELECT 1 FROM DONHANG dh 
                            WHERE dh.MaBanAn = @MaBanAn
                            AND (dh.MaDonHang = @MaDonHang OR tn.NoiDung LIKE '%' + CAST(@MaDonHang AS NVARCHAR) + '%')
                        )
                        ORDER BY tn.ThoiGian DESC";

                    cmd = new SqlCommand(messagesQuery, conn);
                    cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                    cmd.Parameters.AddWithValue("@MaBanAn", maBanAn);
                    cmd.Parameters.AddWithValue("@MaNhanVienBep", maNhanVienBep ?? (object)DBNull.Value);

                    List<KitchenMessageData> tinNhan = new List<KitchenMessageData>();

                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var message = new KitchenMessageData
                            {
                                MaTin = (int)r["MaTin"],
                                MaNguoiGui = (int)r["MaNguoiGui"],
                                TenNguoiGui = r["TenNguoiGui"].ToString(),
                                VaiTroNguoiGui = r["VaiTroNguoiGui"].ToString(),
                                MaNguoiNhan = (int)r["MaNguoiNhan"],
                                TenNguoiNhan = r["TenNguoiNhan"].ToString(),
                                NoiDung = r["NoiDung"].ToString(),
                                ThoiGian = (DateTime)r["ThoiGian"],
                                DaDoc = (bool)r["DaDoc"]
                            };

                            tinNhan.Add(message);
                        }
                    }

                    // Đánh dấu đã đọc những tin nhắn gửi cho mình
                    if (maNhanVienBep.HasValue)
                    {
                        string markReadQuery = @"
                            UPDATE TINNHAN SET DaDoc = 1 
                            WHERE MaNguoiNhan = @MaNhanVienBep 
                            AND DaDoc = 0
                            AND EXISTS (
                                SELECT 1 FROM DONHANG dh 
                                WHERE dh.MaBanAn = @MaBanAn
                                AND dh.MaDonHang = @MaDonHang
                            )";

                        cmd = new SqlCommand(markReadQuery, conn);
                        cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                        cmd.Parameters.AddWithValue("@MaBanAn", maBanAn);
                        cmd.Parameters.AddWithValue("@MaNhanVienBep", maNhanVienBep.Value);
                        cmd.ExecuteNonQuery();
                    }

                    return new KitchenMessagesResult
                    {
                        Success = true,
                        TinNhan = tinNhan,
                        Message = $"Lấy được {tinNhan.Count} tin nhắn"
                    };
                }
            }
            catch (Exception ex)
            {
                return new KitchenMessagesResult
                {
                    Success = false,
                    Message = $"Lỗi lấy tin nhắn: {ex.Message}"
                };
            }
        }
    }
}