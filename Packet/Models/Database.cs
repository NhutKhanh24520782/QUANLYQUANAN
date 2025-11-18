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
    public class DoanhThuTheoBan
    {
        public string TenBan { get; set; } = string.Empty;
        public int SoLuongHoaDon { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal HoaDonLonNhat { get; set; }
        public decimal HoaDonNhoNhat { get; set; }
        public decimal DoanhThuTB { get; set; }
    }

    /// <summary>
    /// Tổng doanh thu - hiển thị trên Label
    /// </summary>
    public class TongDoanhThu
    {
        public decimal tongDoanhThu { get; set; }
        public int TongSoHoaDon { get; set; }
        public int TongSoBan { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }

    /// <summary>
    /// Kết quả thống kê doanh thu
    /// </summary>
    public class DoanhThuResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public TongDoanhThu TongDoanhThu { get; set; } = new TongDoanhThu();
        public List<DoanhThuTheoBan> DoanhThuTheoBan { get; set; } = new List<DoanhThuTheoBan>();
    }

        public class HoaDon
        {
            public int MaHoaDon { get; set; }
            public int MaBanAn { get; set; } 
            public int MaNhanVien { get; set; }
            public DateTime NgayXuatHoaDon { get; set; }
            public string TrangThai { get; set; } = string.Empty;
            public string PhuongThucThanhToan { get; set; } = string.Empty;
            public int TongTien { get; set; }
            public string GhiChu { get; set; } = string.Empty;
        }
    public class BillData
    {
        public int MaHoaDon { get; set; }
        public int MaBanAn { get; set; }
        public int MaNhanVien { get; set; }
        public DateTime NgayXuatHoaDon { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }

    public class BillResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<BillData> Bills { get; set; } = new List<BillData>();
    }


}

