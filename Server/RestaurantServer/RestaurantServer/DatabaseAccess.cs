using BCrypt.Net;
using Models;
using Models.Database;
using Models.Response;
using System;
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Drawing;

namespace RestaurantServer
{
    public static class DatabaseAccess
    {
        private static string connectionString =
               "Server=tcp:quanlyquanan.database.windows.net,1433;" +
               "Initial Catalog=restaurant;" +
               "User ID=lamnhutkhanh;" +
               "Password=Khanh251106;" +
               "Encrypt=True;" +
               "TrustServerCertificate=False;" +
               "Connection Timeout=30;";

        public static LoginResult LoginUser(string username, string password)
        {
            Console.WriteLine($"🔐 Login attempt: {username}"); // DEBUG

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // ✅ ĐÚNG: Thêm TrangThai vào SELECT
                string query = @"
            SELECT MaNguoiDung, TenDangNhap, MatKhau, VaiTro, HoTen, Email, TrangThai
            FROM NGUOIDUNG 
            WHERE TenDangNhap = @user AND TrangThai = 1";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user", username);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string hashedPassword = reader["MatKhau"].ToString();
                        bool trangThai = (bool)reader["TrangThai"];

                        Console.WriteLine($"📊 User found - Status: {trangThai}, Password hash: {hashedPassword.Substring(0, 20)}...");

                        if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                        {
                            Console.WriteLine($"✅ Password correct!");
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
                        else
                        {
                            Console.WriteLine($"❌ Password incorrect!");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"❌ User not found or inactive: {username}");
                    }
                }

                return new LoginResult
                {
                    Success = false,
                    Message = "Sai tên đăng nhập hoặc mật khẩu"
                };
            }
        }
        public static RegisterResult RegisterUser(string username, string password, string fullName, string email, string role)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM NGUOIDUNG WHERE TenDangNhap=@u OR Email=@e";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@u", username);
                        checkCmd.Parameters.AddWithValue("@e", email);
                        int count = (int)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            return new RegisterResult
                            {
                                Success = false,
                                Message = "Tài khoản hoặc email đã tồn tại"
                            };
                        }
                    }

                    string hashed = BCrypt.Net.BCrypt.HashPassword(password);

                    // ✅ ĐÚNG: Thêm TrangThai và NgayTao
                    string insertQuery = @"INSERT INTO NGUOIDUNG (TenDangNhap, MatKhau, HoTen, Email, VaiTro, TrangThai, NgayTao)
                           VALUES (@u, @p, @n, @e, @r, 1, GETDATE());
                           SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", hashed);
                        cmd.Parameters.AddWithValue("@n", fullName);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.Parameters.AddWithValue("@r", role);

                        int newId = Convert.ToInt32(cmd.ExecuteScalar());
                        return new RegisterResult
                        {
                            Success = true,
                            Message = "Đăng ký thành công",
                            MaNguoiDung = newId
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Register error: {ex.Message}");
                return new RegisterResult
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }
        public static RegisterResult UpdatePassword(string email, string newPassword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ THÊM: Kiểm tra cả TrangThai = 1
                    string checkQuery = "SELECT COUNT(*) FROM NGUOIDUNG WHERE Email = @e AND TrangThai = 1";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@e", email);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        return new RegisterResult
                        {
                            Success = false,
                            Message = "Email chưa đăng ký tài khoản hoặc tài khoản đã bị khóa"
                        };
                    }

                    string hashed = BCrypt.Net.BCrypt.HashPassword(newPassword);

                    string query = "UPDATE NGUOIDUNG SET MatKhau = @p WHERE Email = @e AND TrangThai = 1";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@p", hashed);
                    cmd.Parameters.AddWithValue("@e", email);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    Console.WriteLine($"🔑 Update password: {email}, Rows affected: {rowsAffected}"); // DEBUG

                    return new RegisterResult
                    {
                        Success = rowsAffected > 0,
                        Message = rowsAffected > 0 ? "Đổi mật khẩu thành công" : "Đổi mật khẩu thất bại"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ UpdatePassword error: {ex.Message}"); // DEBUG
                return new RegisterResult
                {
                    Success = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }
        public static EmailCheckResult CheckEmailExists(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // ✅ THÊM: Kiểm tra cả TrangThai = 1 (tài khoản hoạt động)
                    string query = "SELECT COUNT(*) FROM NGUOIDUNG WHERE Email = @Email AND TrangThai = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        int count = (int)cmd.ExecuteScalar();
                        bool exists = count > 0;

                        Console.WriteLine($"🔍 Check email: {email}, Exists: {exists}, Count: {count}"); // DEBUG

                        return new EmailCheckResult
                        {
                            Success = true,
                            Exists = exists,
                            Message = exists ? "Email tồn tại" : "Email chưa đăng ký tài khoản"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ CheckEmail error: {ex.Message}"); // DEBUG
                return new EmailCheckResult
                {
                    Success = false,
                    Exists = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }
        public static EmployeeResult GetEmployees(string keyword = "", string vaiTro = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT MaNguoiDung, TenDangNhap, HoTen, Email, VaiTro, SDT, NgayTao, TrangThai
                        FROM NGUOIDUNG 
                        WHERE VaiTro IN ('Admin', 'PhucVu', 'Bep') 
                            AND (@Keyword = '' OR HoTen LIKE '%' + @Keyword + '%' OR Email LIKE '%' + @Keyword + '%')
                            AND (@VaiTro = '' OR VaiTro = @VaiTro)
                        ORDER BY MaNguoiDung DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Keyword", keyword ?? "");
                        cmd.Parameters.AddWithValue("@VaiTro", vaiTro ?? "");

                        var employees = new List<EmployeeData>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new EmployeeData
                                {
                                    MaNguoiDung = (int)reader["MaNguoiDung"],
                                    TenDangNhap = reader["TenDangNhap"].ToString(),
                                    HoTen = reader["HoTen"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    VaiTro = reader["VaiTro"].ToString(),
                                    SDT = reader["SDT"].ToString(),
                                    NgayTao = (DateTime)reader["NgayTao"],
                                    TrangThai = (bool)reader["TrangThai"]
                                });
                            }
                        }

                        return new EmployeeResult
                        {
                            Success = true,
                            Message = $"Tìm thấy {employees.Count} nhân viên",
                            Employees = employees
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new EmployeeResult
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}"
                };
            }
        }

        public static EmployeeResult AddEmployee(string tenDangNhap, string matKhau, string hoTen,
            string email, string vaiTro, string sdt, DateTime ngayVaoLam)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra trùng
                    string checkQuery = "SELECT COUNT(*) FROM NGUOIDUNG WHERE TenDangNhap=@user OR Email=@email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@user", tenDangNhap);
                        checkCmd.Parameters.AddWithValue("@email", email);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                            return new EmployeeResult { Success = false, Message = "Tên đăng nhập hoặc email đã tồn tại" };
                    }

                    // Thêm mới nhân viên
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(matKhau);

                    string insertQuery = @"
                        INSERT INTO NGUOIDUNG (TenDangNhap, MatKhau, HoTen, Email, VaiTro, SDT, NgayTao, TrangThai)
                        OUTPUT INSERTED.MaNguoiDung
                        VALUES (@user, @pass, @name, @email, @role, @sdt, @ngayTao, 1)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", tenDangNhap);
                        cmd.Parameters.AddWithValue("@pass", hashedPassword);
                        cmd.Parameters.AddWithValue("@name", hoTen);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@role", vaiTro);
                        cmd.Parameters.AddWithValue("@sdt", sdt ?? "");
                        cmd.Parameters.AddWithValue("@ngayTao", ngayVaoLam);

                        int newId = (int)cmd.ExecuteScalar();

                        return new EmployeeResult
                        {
                            Success = true,
                            Message = "Thêm nhân viên thành công",
                            MaNguoiDung = newId
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new EmployeeResult
                {
                    Success = false,
                    Message = $"Lỗi thêm nhân viên: {ex.Message}"
                };
            }
        }

        public static EmployeeResult UpdateEmployee(int maNguoiDung, string hoTen, string email,
            string vaiTro, string sdt, bool trangThai)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra tồn tại
                    string checkQuery = "SELECT COUNT(*) FROM NGUOIDUNG WHERE MaNguoiDung = @id";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", maNguoiDung);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count == 0)
                            return new EmployeeResult { Success = false, Message = "Nhân viên không tồn tại" };
                    }

                    // Cập nhật thông tin
                    string updateQuery = @"
                        UPDATE NGUOIDUNG 
                        SET HoTen = @name, Email = @email, VaiTro = @role, 
                            SDT = @sdt, TrangThai = @status
                        WHERE MaNguoiDung = @id";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", maNguoiDung);
                        cmd.Parameters.AddWithValue("@name", hoTen);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@role", vaiTro);
                        cmd.Parameters.AddWithValue("@sdt", sdt ?? "");
                        cmd.Parameters.AddWithValue("@status", trangThai);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        return new EmployeeResult
                        {
                            Success = rowsAffected > 0,
                            Message = rowsAffected > 0 ? "Cập nhật thành công" : "Cập nhật thất bại"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new EmployeeResult
                {
                    Success = false,
                    Message = $"Lỗi cập nhật: {ex.Message}"
                };
            }
        }

        public static EmployeeResult DeleteEmployee(int maNguoiDung)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string checkAdminQuery = @"
                        SELECT COUNT(*) FROM NGUOIDUNG 
                        WHERE VaiTro = 'Admin' AND TrangThai = 1 AND MaNguoiDung != @id";

                    using (SqlCommand checkCmd = new SqlCommand(checkAdminQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", maNguoiDung);
                        int adminCount = (int)checkCmd.ExecuteScalar();

                        if (adminCount == 0)
                            return new EmployeeResult { Success = false, Message = "Không thể xóa admin cuối cùng" };
                    }
                    string deleteQuery = @"
                        UPDATE NGUOIDUNG 
                        SET TrangThai = 0 
                        WHERE MaNguoiDung = @id";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", maNguoiDung);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        return new EmployeeResult
                        {
                            Success = rowsAffected > 0,
                            Message = rowsAffected > 0 ? "Xóa nhân viên thành công" : "Nhân viên không tồn tại"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new EmployeeResult
                {
                    Success = false,
                    Message = $"Lỗi xóa nhân viên: {ex.Message}"
                };
            }
        }
        public static decimal GetTongDoanhThu(DateTime tuNgay, DateTime denNgay)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                SELECT ISNULL(SUM(TongTien), 0) 
                FROM HOADON 
                WHERE Ngay BETWEEN @TuNgay AND @DenNgay 
                AND TrangThai = N'DaThanhToan'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TuNgay", tuNgay);
                    cmd.Parameters.AddWithValue("@DenNgay", denNgay);

                    var result = cmd.ExecuteScalar();
                    return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
                }
            }
        }

        /// <summary>
        /// Lấy doanh thu theo bàn
        /// </summary>
        public static List<DoanhThuTheoBan> GetDoanhThuTheoBan(DateTime tuNgay, DateTime denNgay)
        {
            var result = new List<DoanhThuTheoBan>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                SELECT 
                    b.TenBan,
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

            return result;
        }

        /// <summary>
        /// Lấy toàn bộ thông tin doanh thu
        /// </summary>
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
                return new DoanhThuResult
                {
                    Success = false,
                    Message = $"Lỗi thống kê: {ex.Message}"
                };
            }
        }

        /// Xuất báo cáo doanh thu ra file Excel (EPPlus 5+)
        /// </summary>
        public static (bool success, string filePath, string message) XuatBaoCaoExcel(
            DateTime tuNgay, DateTime denNgay, List<DoanhThuTheoBan> data, decimal tongDoanhThu)
        {
            try
            {
                // Set license context (BẮT BUỘC cho EPPlus 5+)
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                string fileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("DoanhThu");

                    // ==================== TIÊU ĐỀ BÁO CÁO ====================
                    // Tiêu đề chính
                    worksheet.Cells["A1:F1"].Merge = true;
                    worksheet.Cells["A1"].Value = "BÁO CÁO DOANH THU NHÀ HÀNG";
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.Font.Size = 16;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // Thông tin thời gian
                    worksheet.Cells["A2"].Value = $"Từ ngày: {tuNgay:dd/MM/yyyy}";
                    worksheet.Cells["D2"].Value = $"Đến ngày: {denNgay:dd/MM/yyyy}";

                    // Tổng doanh thu
                    worksheet.Cells["A3"].Value = "TỔNG DOANH THU:";
                    worksheet.Cells["B3"].Value = tongDoanhThu;
                    worksheet.Cells["B3"].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells["C3"].Value = "VNĐ";
                    worksheet.Cells["A3"].Style.Font.Bold = true;
                    worksheet.Cells["B3"].Style.Font.Bold = true;
                    worksheet.Cells["B3"].Style.Font.Color.SetColor(Color.Red);

                    // Thống kê
                    worksheet.Cells["A4"].Value = $"Tổng số bàn: {data.Count}";
                    worksheet.Cells["C4"].Value = $"Số bàn có doanh thu: {data.Count(x => x.DoanhThu > 0)}";
                    worksheet.Cells["E4"].Value = $"Tổng hóa đơn: {data.Sum(x => x.SoLuongHoaDon)}";

                    // ==================== HEADER TABLE ====================
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

                    // ==================== DỮ LIỆU ====================
                    int row = 7;
                    foreach (var item in data)
                    {
                        worksheet.Cells[row, 1].Value = item.TenBan;
                        worksheet.Cells[row, 2].Value = item.SoLuongHoaDon;
                        worksheet.Cells[row, 3].Value = item.DoanhThu;
                        worksheet.Cells[row, 4].Value = item.HoaDonLonNhat;
                        worksheet.Cells[row, 5].Value = item.HoaDonNhoNhat;
                        worksheet.Cells[row, 6].Value = item.DoanhThuTB;

                        // Định dạng số cho các cột tiền
                        for (int col = 3; col <= 6; col++)
                        {
                            worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
                        }

                        // Tô màu cho dòng có doanh thu
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

                    // ==================== TỔNG KẾT ====================
                    worksheet.Cells[row + 1, 1].Value = "TỔNG CỘNG:";
                    worksheet.Cells[row + 1, 1].Style.Font.Bold = true;

                    // Tổng số hóa đơn
                    worksheet.Cells[row + 1, 2].Formula = $"SUM(B7:B{row})";
                    worksheet.Cells[row + 1, 2].Style.Font.Bold = true;

                    // Tổng doanh thu
                    worksheet.Cells[row + 1, 3].Formula = $"SUM(C7:C{row})";
                    worksheet.Cells[row + 1, 3].Style.Numberformat.Format = "#,##0";
                    worksheet.Cells[row + 1, 3].Style.Font.Bold = true;
                    worksheet.Cells[row + 1, 3].Style.Font.Color.SetColor(Color.Red);

                    // ==================== ĐỊNH DẠNG BORDER ====================
                    var dataRange = worksheet.Cells[6, 1, row + 1, 6];
                    dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    // ==================== AUTO FIT COLUMNS ====================
                    worksheet.Cells[1, 1, row + 1, 6].AutoFitColumns();

                    // ==================== FOOTER ====================
                    worksheet.Cells[row + 3, 1].Value = $"Ngày xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                    worksheet.Cells[row + 4, 1].Value = "Hệ thống Quản lý Nhà hàng - Restaurant Management System";

                    // Lưu file
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

        //------------------------------
        //------------------------------
        // 4. Gửi yêu cầu "INSERT" cho SQL (ĐÃ SỬA)
        public static bool AddBanToSQL(BanAn banMoi)
        {
            System.Diagnostics.Debug.WriteLine("4. DatabaseAccess: Gửi lệnh 'INSERT' cho SQL...");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Giả sử tên bảng của bạn là BAN
                    // Nếu tên bảng khác (ví dụ: BANAN), hãy sửa lại dòng dưới
                    string insertQuery = @"
                        INSERT INTO BAN (MaBan, TenBan, TrangThai) 
                        VALUES (@id, @ten, @trangthai)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", banMoi.MaBan);
                        cmd.Parameters.AddWithValue("@ten", banMoi.TenBan);
                        cmd.Parameters.AddWithValue("@trangthai", banMoi.TrangThai);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("5. SQL: Thêm thành công. Trả về true.");
                            return true; // Thêm thành công
                        }

                        // Trường hợp này ít xảy ra nếu không có lỗi
                        System.Diagnostics.Debug.WriteLine("5. SQL: Thêm thất bại (không có dòng nào bị ảnh hưởng).");
                        return false;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Bắt lỗi vi phạm Primary Key (trùng ID)
                // 2627 và 2601 là mã lỗi cho Unique Constraint / Primary Key violation
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    System.Diagnostics.Debug.WriteLine("5. SQL: Lỗi! ID bàn đã tồn tại. Trả về false.");
                    return false; // Trả về false nếu trùng ID
                }

                // Ghi log các lỗi SQL khác
                System.Diagnostics.Debug.WriteLine($"💥 AddBanToSQL SQL Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Ghi log các lỗi chung khác
                System.Diagnostics.Debug.WriteLine($"💥 AddBanToSQL General Error: {ex.Message}");
                return false;
            }
        }
    } // Đóng '}' của class DatabaseAccess
} // Đóng '}' của namespace
    
