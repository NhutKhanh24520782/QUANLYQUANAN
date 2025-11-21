using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Database
{

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Role { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int MaNguoiDung { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kết quả đăng ký từ Database
    /// </summary>
    public class RegisterResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int MaNguoiDung { get; set; }
    }

    /// <summary>
    /// Kết quả kiểm tra email
    /// </summary>
    public class EmailCheckResult
    {
        public bool Success { get; set; }
        public bool Exists { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kết quả thao tác chung (update, delete, insert)
    /// </summary>
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RowsAffected { get; set; }
    }

    // ==================== ENTITIES (MAP VỚI DATABASE TABLES) ====================

    /// <summary>
    /// Entity người dùng - map với table NGUOIDUNG
    /// </summary>
    public class NguoiDung
    {
        public int MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
    }
    public class BanAn
    {
        public int MaBan { get; set; }
        public string TenBan { get; set; } // nvarchar
        public string TrangThai { get; set; } // "Trống", "Có người",...
    }
}

