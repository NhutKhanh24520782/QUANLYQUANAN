using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using BCrypt.Net;

namespace RestaurantServer
{
    public static class DatabaseAccess
    {
        private static string connectionString =
            "Data Source=localhost;Initial Catalog=QLQuanAn;Integrated Security=True";

        public static bool LoginUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT MatKhau FROM NGUOIDUNG WHERE TenDangNhap = @user";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user", username);

                object result = cmd.ExecuteScalar();
                if (result == null) return false;

                string hashedPassword = result.ToString();
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
        }

        public static bool RegisterUser(string username, string password, string fullName, string email, string role)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string hashed = BCrypt.Net.BCrypt.HashPassword(password);

                    string query = @"INSERT INTO NGUOIDUNG (TenDangNhap, MatKhau, HoTen, Email, VaiTro)
                                     VALUES (@u, @p, @n, @e, @r)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", hashed);
                    cmd.Parameters.AddWithValue("@n", fullName);
                    cmd.Parameters.AddWithValue("@e", email);
                    cmd.Parameters.AddWithValue("@r", role);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Lỗi đăng ký: " + ex.Message);
                return false;
            }
        }
        public static bool UpdatePassword(string email, string newPassword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string hashed = BCrypt.Net.BCrypt.HashPassword(newPassword);

                    string query = "UPDATE NGUOIDUNG SET MatKhau = @p WHERE Email = @e";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@p", hashed);
                    cmd.Parameters.AddWithValue("@e", email);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Lỗi đổi mật khẩu: " + ex.Message);
                return false;
            }
        }
        // Kiểm tra email có tồn tại trong database hay không
        public static bool CheckEmailExists(string email)
        {
            bool exists = false;
            string query = "SELECT COUNT(*) FROM NGUOIDUNG WHERE Email = @Email"; 

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                exists = count > 0;
            }

            return exists;
        }
    }
}

