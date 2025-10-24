using BCrypt.Net;
namespace RestaurantClient
{
    public static class PasswordHashing
    {
        /// <summary>
        /// Hàm băm mật khẩu sử dụng thuật toán BCrypt an toàn.
        /// </summary>
        /// <param name="password">Mật khẩu thô (plain text) cần băm.</param>
        /// <returns>Chuỗi mật khẩu đã được băm (hash) kèm theo salt.</returns>
        public static string HashPassword(string password)
        {
            // BCrypt.HashPassword tự động tạo salt (chuỗi ngẫu nhiên) và gắn nó vào chuỗi hash.
            // Work Factor (mặc định là 10) là độ phức tạp tính toán.
            // PasswordStrength.Medium là mức độ bảo mật tiêu chuẩn.
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Hàm kiểm tra mật khẩu.
        /// </summary>
        /// <param name="password">Mật khẩu thô do người dùng nhập.</param>
        /// <param name="hashedPassword">Chuỗi mật khẩu đã băm (lấy từ database).</param>
        /// <returns>True nếu mật khẩu khớp, False nếu không.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // BCrypt sẽ tự động trích xuất salt từ chuỗi hash và so sánh.
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
