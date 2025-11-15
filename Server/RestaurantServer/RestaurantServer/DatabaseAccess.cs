using BCrypt.Net;
using Models.Database;
using Models.Response;
using System;
using System.Collections.Generic;  

using System.Data.SqlClient;

namespace RestaurantServer
{
    public static class DatabaseAccess
    {
        private static string connectionString =
            "Data Source=localhost;Initial Catalog=QLQuanAn;Integrated Security=True";

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
    }
}