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
    }
}
