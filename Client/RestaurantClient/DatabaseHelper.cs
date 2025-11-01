using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

public class DatabaseHelper
{
    // ================================================================
    // 1. CHUỖI KẾT NỐI
    // ================================================================
    private string connectionString =
        "Server=localhost;Database=QLQuanAn;Integrated Security=True;TrustServerCertificate=True;";

    // ================================================================
    // 2. HÀM BĂM MẬT KHẨU (SHA-256)
    // ================================================================
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    // ================================================================
    // 3. ĐĂNG KÝ NGƯỜI DÙNG
    // ================================================================
    public bool RegisterUser(string username, string password, string fullName, string email = null, string vaiTro = "PhucVu")
    {
        string hashedPassword = HashPassword(password);

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"
                INSERT INTO NGUOIDUNG (TenDangNhap, MatKhau, HoTen, Email, VaiTro)
                VALUES (@Username, @PasswordHash, @FullName, @Email, @VaiTro)
            ";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                command.Parameters.AddWithValue("@FullName", fullName);

                // Nếu email rỗng, dùng DBNull.Value
                if (string.IsNullOrWhiteSpace(email))
                    command.Parameters.AddWithValue("@Email", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@Email", email);

                command.Parameters.AddWithValue("@VaiTro", vaiTro);

                try
                {
                    connection.Open();
                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        MessageBox.Show("Tên đăng nhập đã tồn tại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Lỗi SQL: " + ex.Message);
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                    return false;
                }
            }
        }
    }

    // ================================================================
    // 4. ĐĂNG NHẬP NGƯỜI DÙNG
    // ================================================================
    public bool LoginUser(string username, string password)
    {
        string storedHash = "";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = @"
                SELECT MatKhau 
                FROM NGUOIDUNG 
                WHERE TenDangNhap = @Username AND TrangThai = 1
            ";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result == null)
                        return false;

                    storedHash = result.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đăng nhập: " + ex.Message);
                    return false;
                }
            }
        }

        // So sánh mật khẩu
        string inputHash = HashPassword(password);
        return storedHash == inputHash;
    }
}
