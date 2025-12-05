using Models.Database;
using Models.Response;
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
        public int MaBanAn { get; set; }
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
        public string? TenNhanVien { get; set; }
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
    public class MenuResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int MaMon { get; set; }
        public List<MenuItemData> Items { get; set; } = new();
    }
    public class BanAnResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<BanAnData> Tables { get; set; } = new List<BanAnData>();
        public int MaBanAn { get; set; }
    }
    public class BanAnData
    {
        public int MaBanAn { get; set; }
        public string TenBan { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "Trong";
        public int? SoChoNgoi { get; set; } // Thêm theo database
        public int? MaNhanVien { get; set; } // Thêm theo database
    }

    public class OrderMon
    {
        public string TenMon { get; set; } = "";
        public int Gia { get; set; }
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
        public int MaMon { get; set; }
    }
    public class OrderMonData
    {
        public string TenMon { get; set; } = "";
        public int Gia { get; set; }
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
        public int MaMon { get; set; }
    }
    public class OrderMonResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int MaMon { get; set; }
        public List<OrderMonData> OrderMons { get; set; } = new List<OrderMonData>();
    }
    public class ThanhToanData
    {
        public int MaGiaoDich { get; set; }
        public int MaHD { get; set; }
        public int MaNhanVien { get; set; }
        public string PhuongThucThanhToan { get; set; } = "TienMat"; // "TienMat", "ChuyenKhoan"
        public decimal SoTienThanhToan { get; set; }
        public decimal? SoTienNhan { get; set; }
        public decimal? SoTienThua { get; set; }
        public string TrangThai { get; set; } = "ThanhCong"; // "ThanhCong", "ThatBai", "Huy"
        public string? MaGiaoDichNganHang { get; set; }
        public string? QRCodeData { get; set; }
        public DateTime ThoiGianTao { get; set; }
        public DateTime ThoiGianThanhToan { get; set; }
        public string? GhiChu { get; set; }
    }

    /// <summary>
    /// Kết quả lấy danh sách chờ thanh toán
    /// </summary>
    public class PendingPaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<PendingPaymentData> PendingPayments { get; set; } = new List<PendingPaymentData>();
    }

    /// <summary>
    /// Kết quả thanh toán tiền mặt
    /// </summary>
    public class CashPaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal SoTienThua { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public string MaGiaoDich { get; set; } = string.Empty;
        public int MaGiaoDichId { get; set; }
    }

    /// <summary>
    /// Kết quả thanh toán chuyển khoản
    /// </summary>
    public class TransferPaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public string QRCodeData { get; set; } = string.Empty;
        public DateTime NgayThanhToan { get; set; }
        public int MaGiaoDichId { get; set; }
    }

    /// <summary>
    /// Kết quả xác nhận thanh toán
    /// </summary>
    public class ConfirmPaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kết quả kiểm tra trạng thái thanh toán
    /// </summary>
    public class PaymentStatusResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty; // "ThanhCong", "ThatBai", "DangXuLy"
        public DateTime? PaymentTime { get; set; }
        public string TransactionNo { get; set; } = string.Empty;
    }

    /// <summary>
    /// Kết quả lấy chi tiết thanh toán
    /// </summary>
    public class PaymentDetailsResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaymentDetailData PaymentDetails { get; set; } = new PaymentDetailData();
    }

    /// <summary>
    /// Kết quả tổng quát cho thanh toán
    /// </summary>
    public class ThanhToanResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int MaGiaoDich { get; set; }
        public decimal SoTienThua { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public string TransactionNo { get; set; } = string.Empty;
    }
    public class CategoryData
    {
        public int MaLoaiMon { get; set; }
        public string TenLoai { get; set; }
    }

    public class CategoryResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<CategoryData> Categories { get; set; } = new List<CategoryData>();
    }
    public class CartItem
    {
        public int MaMon { get; set; }
        public string TenMon { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
    }
    public class CreateOrderRequest
    {
        public string Type => "CreateOrder";
        public int MaBanAn { get; set; }
        public int MaNhanVien { get; set; }
        public decimal TongTien { get; set; }
        public List<ChiTietOrder> ChiTietOrder { get; set; }
        public string GhiChu { get; set; } = string.Empty;
    }

    public class ChiTietOrder
    {
        public int MaMon { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public string GhiChu { get; set; } = "";
    }

    public class CreateOrderResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int MaHoaDon { get; set; }
        public int MaDonHang { get; set; } // ✅ THÊM

    }
    public class CreateOrderResult
    {
        public bool Success { get; set; }
        public int MaDonHang { get; set; } // ✅ THÊM
        public string Message { get; set; }
        public int MaHoaDon { get; set; }
    }

    // ==================== KITCHEN DATABASE RESULTS ====================

    /// <summary>
    /// Kết quả lấy danh sách đơn hàng cho bếp
    /// </summary>
    public class KitchenOrdersResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<KitchenOrderData> DonHang { get; set; } = new List<KitchenOrderData>();
        public ThongKeBep ThongKe { get; set; } = new ThongKeBep();
    }

    /// <summary>
    /// Kết quả lấy chi tiết đơn hàng
    /// </summary>
    public class OrderDetailResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public KitchenOrderDetailData ChiTietDonHang { get; set; } = new KitchenOrderDetailData();
    }

    /// <summary>
    /// Kết quả cập nhật trạng thái món
    /// </summary>
    public class UpdateDishStatusResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TenMon { get; set; } = "";
        public string TrangThaiCu { get; set; } = "";
        public string TrangThaiMoi { get; set; } = "";
        public int SoMonDaCapNhat { get; set; }
        public object TenCacMon { get; set; }
    }

    /// <summary>
    /// Kết quả thống kê bếp
    /// </summary>
    public class KitchenStatisticsResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public KitchenStatisticsData ThongKe { get; set; } = new KitchenStatisticsData();
    }

    /// <summary>
    /// Kết quả lấy tin nhắn
    /// </summary>
    public class KitchenMessagesResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<KitchenMessageData> TinNhan { get; set; } = new List<KitchenMessageData>();
    }

    /// <summary>
    /// Kết quả gửi tin nhắn
    /// </summary>
    public class SendMessageResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int MaTinNhan { get; set; }
    }
    /// <summary>
    /// ĐƠN HÀNG - DONHANG table
    /// </summary>
    public class DonHang
    {
        public int MaDonHang { get; set; }
        public int MaBanAn { get; set; }
        public int MaNVOrder { get; set; }
        public DateTime NgayOrder { get; set; } = DateTime.Now;
        public string TrangThai { get; set; } = "ChoXacNhan"; // "ChoXacNhan", "DangCheBien", "HoanThanh", "Huy"
    }

    /// <summary>
    /// CHI TIẾT ĐƠN HÀNG - CHITIET_DONHANG table (Quan trọng nhất cho bếp)
    /// </summary>
    public class ChiTietDonHang
    {
        public int MaChiTiet { get; set; }
        public int MaDonHang { get; set; }
        public int MaMon { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public string GhiChuKhach { get; set; } = string.Empty;
        public string TrangThai { get; set; } = "ChoXacNhan"; // "ChoXacNhan", "DangCheBien", "HoanThanh", "CoVanDe", "Huy"
        public string GhiChuBep { get; set; } = string.Empty;
        public int? MaNhanVienCheBien { get; set; }
        public int UuTien { get; set; } = 1; // 1-5
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianHoanThanh { get; set; }
        public DateTime? ThoiGianDuKien { get; set; }
    }

    /// <summary>
    /// TIN NHẮN - TINNHAN table (cho chat bếp - phục vụ)
    /// </summary>
    public class TinNhan
    {
        public int MaTin { get; set; }
        public int MaNguoiGui { get; set; }
        public int MaNguoiNhan { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public DateTime ThoiGian { get; set; } = DateTime.Now;
        public bool DaDoc { get; set; } = false;
    }

    /// <summary>
    /// CHI TIẾT HÓA ĐƠN - CTHD table
    /// </summary>
    public class CTHD
    {
        public int MaHD { get; set; }
        public int MaMon { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal Gia => SoLuong * DonGia;
        public string GhiChu { get; set; } = string.Empty;
    }

    public class ThongKeDauBepSPResult
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public int TongDon { get; set; }
        public int DonHoanThanh { get; set; }
        public decimal TyLeHoanThanh => TongDon > 0 ? (decimal)DonHoanThanh / TongDon * 100 : 0;
        public int TongMon { get; set; }
        public int MonHoanThanh { get; set; }
        public decimal TyLeMonHoanThanh => TongMon > 0 ? (decimal)MonHoanThanh / TongMon * 100 : 0;
        public decimal? ThoiGianTrungBinh { get; set; }
        public string DanhGiaHieuSuat
        {
            get
            {
                if (ThoiGianTrungBinh == null || TyLeHoanThanh == 0)
                    return "⭐☆☆☆☆";

                double diem = ((double)TyLeHoanThanh / 100.0 * 0.7) +
                              ((30.0 - Math.Min((double)ThoiGianTrungBinh.Value, 30.0)) / 30.0 * 0.3);
                if (diem >= 0.8) return "⭐⭐⭐⭐⭐";
                else if (diem >= 0.6) return "⭐⭐⭐⭐☆";
                else if (diem >= 0.4) return "⭐⭐⭐☆☆";
                else if (diem >= 0.2) return "⭐⭐☆☆☆";
                else return "⭐☆☆☆☆";
            }
        }
    }

    /// <summary>
    /// Kết quả top món phổ biến
    /// </summary>
    public class TopMonPhobienResult
    {
        public int MaMon { get; set; }
        public string TenMon { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public int SoDon { get; set; }
        public string TenLoai { get; set; } = string.Empty;
        public decimal TyLe { get; set; }
    }

    /// <summary>
    /// Kết quả thống kê tổng quan
    /// </summary>
    public class ThongKeTongQuanResult
    {
        public int TongDon { get; set; }
        public int DonHoanThanh { get; set; }
        public decimal TyLeHoanThanh => TongDon > 0 ? (decimal)DonHoanThanh / TongDon * 100 : 0;
        public int TongMon { get; set; }
        public decimal? ThoiGianTrungBinh { get; set; }
    }

    /// <summary>
    /// Kết quả lấy danh sách đầu bếp
    /// </summary>
    public class GetDanhSachDauBepResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<NguoiDung> DanhSachDauBep { get; set; } = new List<NguoiDung>();
    }

    /// <summary>
    /// Kết quả lấy thống kê chi tiết đầu bếp
    /// </summary>
    public class GetThongKeDauBepChiTietResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ThongKeDauBepSPResult ThongKe { get; set; } = new ThongKeDauBepSPResult();
        public List<ChiTietDonHang> DanhSachMonDaCheBien { get; set; } = new List<ChiTietDonHang>();
    }


    public class ThongKeBepTongQuan
    {
        public int TongDon { get; set; }
        public decimal TyLeHoanThanh { get; set; }
        public int TongMon { get; set; }
        public decimal? ThoiGianTrungBinh { get; set; }
        public int DonHoanThanh { get; set; }
    }
    public class ThongKeDauBep
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public int TongDon { get; set; }
        public int DonHoanThanh { get; set; }
        public int TongMon { get; set; }
        public int MonHoanThanh { get; set; }
        public decimal? ThoiGianTrungBinh { get; set; }

        // Có setter để có thể gán từ bên ngoài
        public decimal TyLeHoanThanh { get; set; }

        // Có setter để có thể gán từ bên ngoài
        public string DanhGiaHieuSuat { get; set; } = string.Empty;

        public decimal TyLeMonHoanThanh => TongMon > 0 ? (decimal)MonHoanThanh / TongMon * 100 : 0;
    }
    public class TopMonAnThongKe
    {
        public int MaMon { get; set; }
        public string TenMon { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public int SoDon { get; set; }
        public string TenLoai { get; set; } = string.Empty;
        public decimal TyLe { get; set; }
    }
    public class ThongKeBepDayDuResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ThongKeBepTongQuan TongQuan { get; set; } = new ThongKeBepTongQuan();
        public List<ThongKeDauBep> DanhSachDauBep { get; set; } = new List<ThongKeDauBep>(); // Đổi thành ThongKeDauBep
        public List<TopMonAnThongKe> TopMonAn { get; set; } = new List<TopMonAnThongKe>();
    }


}

