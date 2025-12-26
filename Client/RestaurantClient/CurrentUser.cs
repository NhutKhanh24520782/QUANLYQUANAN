using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantClient
{
    /// <summary>
    /// Lưu thông tin user đang đăng nhập (Singleton pattern)
    /// </summary>
    public static class CurrentUser
    {
        // ==================== THÔNG TIN CƠ BẢN ====================
        public static int Id { get; set; }
        public static string Username { get; set; } = "";
        public static string Email { get; set; } = "";
        public static string FullName { get; set; } = "";
        public static string Role { get; set; } = "";

        // ==================== TOKEN AUTHENTICATION ====================
        public static string Token { get; set; } = "";
        public static DateTime TokenExpiry { get; set; }

        /// <summary>
        /// Kiểm tra token còn hạn không
        /// </summary>
        public static bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(Token) && TokenExpiry > DateTime.Now;
        }

        /// <summary>
        /// Kiểm tra token sắp hết hạn (còn dưới 1 giờ)
        /// </summary>
        public static bool IsTokenExpiringSoon()
        {
            if (!IsTokenValid()) return true;
            return (TokenExpiry - DateTime.Now).TotalHours < 1;
        }

        /// <summary>
        /// Kiểm tra đã đăng nhập chưa
        /// </summary>
        public static bool IsLoggedIn()
        {
            return Id > 0 && IsTokenValid();
        }

        /// <summary>
        /// Reset toàn bộ thông tin user (dùng khi logout)
        /// </summary>
        public static void Clear()
        {
            Id = 0;
            Username = "";
            Email = "";
            FullName = "";
            Role = "";
            Token = "";
            TokenExpiry = DateTime.MinValue;

            Console.WriteLine("🗑️ CurrentUser đã được xóa sạch");
        }

        /// <summary>
        /// In thông tin debug
        /// </summary>
        public static void DebugInfo()
        {
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("        CURRENT USER INFO");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine($"  ID:        {Id}");
            Console.WriteLine($"  Username:  {Username}");
            Console.WriteLine($"  FullName:  {FullName}");
            Console.WriteLine($"  Email:     {Email}");
            Console.WriteLine($"  Role:      {Role}");
            Console.WriteLine($"  Token:     {(string.IsNullOrEmpty(Token) ? "(empty)" : Token.Substring(0, Math.Min(8, Token.Length)) + "...")}");
            Console.WriteLine($"  Expiry:    {TokenExpiry:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine($"  IsValid:   {IsTokenValid()}");
            Console.WriteLine($"  LoggedIn:  {IsLoggedIn()}");
            Console.WriteLine("═══════════════════════════════════════");
        }
    }
}
