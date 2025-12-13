using Models.Database;
using System;
using System.Collections.Generic;

namespace Models.Response
{

    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    // ==================== AUTHENTICATION RESPONSES ====================

    public class LoginResponse : BaseResponse
    {
        public LoginResponse() => Type = "LoginResponse";

        public string Role { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public int MaNguoiDung { get; set; }
        public string Email { get; set; } = string.Empty;
    }

    public class RegisterResponse : BaseResponse
    {
        public RegisterResponse() => Type = "RegisterResponse";
        public int MaNguoiDung { get; set; }
    }

    public class UpdatePasswordResponse : BaseResponse
    {
        public UpdatePasswordResponse() => Type = "UpdatePasswordResponse";
    }

    public class CheckEmailResponse : BaseResponse
    {
        public CheckEmailResponse() => Type = "CheckEmailResponse";
        public bool Exists { get; set; }
    }

    // ==================== EMPLOYEE RESPONSES ====================

    public class EmployeeData
    {
        public int MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public string SDT { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public bool TrangThai { get; set; } = true;
    }

    public class GetEmployeesResponse : BaseResponse
    {
        public GetEmployeesResponse() => Type = "GetEmployeesResponse";
        public List<EmployeeData> Employees { get; set; } = new List<EmployeeData>();
    }

    public class AddEmployeeResponse : BaseResponse
    {
        public AddEmployeeResponse() => Type = "AddEmployeeResponse";
        public int MaNguoiDung { get; set; }
    }

    public class UpdateEmployeeResponse : BaseResponse
    {
        public UpdateEmployeeResponse() => Type = "UpdateEmployeeResponse";
    }

    public class DeleteEmployeeResponse : BaseResponse
    {
        public DeleteEmployeeResponse() => Type = "DeleteEmployeeResponse";
    }


    public class ErrorResponse : BaseResponse
    {
        public ErrorResponse() => Type = "ErrorResponse";
        public string ErrorCode { get; set; } = string.Empty;
    }

    // ==================== DATABASE RESULT CLASSES ====================

    public class EmployeeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<EmployeeData> Employees { get; set; } = new List<EmployeeData>();
        public int MaNguoiDung { get; set; }
    }
    public class ThongKeDoanhThuResponse : BaseResponse
    {
        public ThongKeDoanhThuResponse() => Type = "ThongKeDoanhThuResponse";
        public TongDoanhThu TongDoanhThu { get; set; } = new TongDoanhThu();
        public List<DoanhThuTheoBan> DoanhThuTheoBan { get; set; } = new List<DoanhThuTheoBan>();
    }

    /// <summary>
    /// Response xuất báo cáo
    /// </summary>
    public class XuatBaoCaoResponse : BaseResponse
    {
        public XuatBaoCaoResponse() => Type = "XuatBaoCaoResponse";
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
    public class MenuItemData
    {
        public int MaMon { get; set; }
        public string TenMon { get; set; } = "";
        public decimal Gia { get; set; }
        public string MoTa { get; set; } = "";
        public int? MaLoaiMon { get; set; }
        public string TrangThai { get; set; } = "ConMon";
    }
    public class UpdateMenuStatusResponse : BaseResponse
    {
        public UpdateMenuStatusResponse() => Type = "UpdateMenuStatusResponse";
    }
    public class GetMenuResponse : BaseResponse
    {
        public GetMenuResponse() => Type = "GetMenuResponse";
        public List<MenuItemData> Items { get; set; } = new();
    }

    public class AddMenuResponse : BaseResponse
    {
        public AddMenuResponse() => Type = "AddMenuResponse";
        public int MaMon { get; set; }
    }

    public class UpdateMenuResponse : BaseResponse
    {
        public UpdateMenuResponse() => Type = "UpdateMenuResponse";
    }

    public class DeleteMenuResponse : BaseResponse
    {
        public DeleteMenuResponse() => Type = "DeleteMenuResponse";
    }


    // ==================== ENUMS ====================

    public enum UserRole
    {
        Unknown = 0,
        Admin = 1,
        PhucVu = 2,
        Bep = 3
    }

    //public enum OrderStatus
    //{
    //    ChoXacNhan = 0,
    //    DangCheBien = 1,
    //    HoanThanh = 2,
    //    Huy = 3
    //}

    public enum TableStatus
    {
        Trong = 0,
        DangSuDung = 1,
        DaDat = 2
    }
    // ==================== ENUMS CHO BẾP ====================

    /// <summary>
    /// Trạng thái món ăn (cho bếp)
    /// </summary>
    public enum DishStatus
    {
        ChoXacNhan = 0,     // Chờ xác nhận
        DangCheBien = 1,    // Đang chế biến
        HoanThanh = 2,      // Hoàn thành
        CoVanDe = 3,        // Có vấn đề
        Huy = 4             // Hủy
    }

    /// <summary>
    /// Trạng thái đơn hàng
    /// </summary>
    public enum OrderStatus
    {
        ChoXacNhan = 0,
        DangCheBien = 1,
        HoanThanh = 2,
        CoVanDe = 3,
        Huy = 4
    }

    /// <summary>
    /// Độ ưu tiên
    /// </summary>
    public enum PriorityLevel
    {
        BinhThuong = 1,     // ⭐
        Thap = 2,           // 🔥
        TrungBinh = 3,      // 🔥🔥
        Cao = 4,            // 🔥🔥🔥
        KhanCap = 5         // 🔥🔥🔥🔥
    }

    /// <summary>
    /// Loại tin nhắn
    /// </summary>
    public enum MessageType
    {
        ThongBao = 0,
        YeuCau = 1,
        CanhBao = 2
    }


    // ==================== BILL RESPONSES ====================

    public class GetBillResponse : BaseResponse
    {
        public GetBillResponse() => Type = "GetBillResponse";
        public List<BillData> Bills { get; set; } = new List<BillData>();
    }
    //=================================
    // ==================== BÀN ĂN (TABLE) RESPONSES ====================
    // ==================== BÀN ĂN (TABLE) RESPONSES ====================

    // 1. Response cho Thêm Bàn (Cần trả về MaBan để hiển thị lên ô ID)
    public class AddTableResponse : BaseResponse
    {
        public AddTableResponse() => Type = "AddTableResponse";
        public int MaBan { get; set; } // ID vừa được SQL tạo
    }

    // 2. Response cho Sửa Bàn (Chỉ cần báo thành công/thất bại, BaseResponse đã đủ nhưng tạo riêng cho rõ ràng)
    public class UpdateTableResponse : BaseResponse
    {
        public UpdateTableResponse() => Type = "UpdateTableResponse";
    }

    // 3. Response cho Xóa Bàn
    public class DeleteTableResponse : BaseResponse
    {
        public DeleteTableResponse() => Type = "DeleteTableResponse";
    }

    // 4. Response cho Lấy danh sách bàn
    public class GetTablesResponse : BaseResponse
    {
        public GetTablesResponse() => Type = "GetTablesResponse";

        // Chứa danh sách bàn lấy từ SQL
        // Lưu ý: Đảm bảo Models.Database.Database.BanAn tồn tại
        public List<BanAnData> ListBan { get; set; } = new List<BanAnData>();
    }
    public class ProcessPaymentResponse : BaseResponse
    {
        public ProcessPaymentResponse() => Type = "ProcessPaymentResponse";
        public int MaGiaoDich { get; set; }
        public string MaHoaDon { get; set; } = string.Empty;
        public DateTime NgayThanhToan { get; set; }
        public string PhuongThucThanhToan { get; set; } = string.Empty;
        public decimal SoTienThanhToan { get; set; }
        public decimal SoTienThua { get; set; }
    }

    public class CashPaymentResponse : BaseResponse
    {
        public CashPaymentResponse() => Type = "CashPaymentResponse";
        public decimal SoTienThua { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public string MaGiaoDich { get; set; } = string.Empty;
    }

    public class TransferPaymentResponse : BaseResponse
    {
        public TransferPaymentResponse() => Type = "TransferPaymentResponse";
        public string QRCodeData { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public DateTime NgayThanhToan { get; set; }
    }

    public class PaymentStatusResponse : BaseResponse
    {
        public PaymentStatusResponse() => Type = "PaymentStatusResponse";
        public string PaymentStatus { get; set; } = string.Empty; // "ThanhCong", "ThatBai", "DangXuLy"
        public DateTime? PaymentTime { get; set; }
        public string TransactionNo { get; set; } = string.Empty;
    }

    public class PaymentDetailsResponse : BaseResponse
    {
        public PaymentDetailsResponse() => Type = "PaymentDetailsResponse";
        public PaymentDetailData PaymentDetails { get; set; } = new PaymentDetailData();
    }
    public class GetPendingPaymentsResponse : BaseResponse
    {
        public GetPendingPaymentsResponse() => Type = "GetPendingPaymentsResponse";
        public List<PendingPaymentData> PendingPayments { get; set; } = new List<PendingPaymentData>();
    }
    // ==================== PAYMENT DATA MODELS ====================

    public class PendingPaymentData
    {
        public int MaHD { get; set; }
        public int MaBanAn { get; set; }
        public int MaNhanVien { get; set; }
        public string TenBan { get; set; } = string.Empty;
        public string TenNhanVien { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public int SoMon { get; set; }
        public string TrangThai { get; set; } = "ChuaThanhToan";
        public List<PaymentItemData> ChiTiet { get; set; } = new List<PaymentItemData>();
    }

    public class PaymentItemData
    {
        public string TenMon { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class PaymentDetailData
    {
        public int MaHD { get; set; }
        public string TenBan { get; set; } = string.Empty;
        public string TenNhanVien { get; set; } = string.Empty;
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public List<PaymentItemData> ChiTiet { get; set; } = new List<PaymentItemData>();
    }
    //==============ORDER RESPONSE===============
    public class GetMonResponse : BaseResponse
    {
        public GetMonResponse() => Type = "GetMonResponse";
        public int MaMon { get; set; }
        public string TenMon { get; set; } = "";
        public int Gia { get; set; }
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
        public List<OrderMonData> OrderMons { get; set; } = new List<OrderMonData>();
    }
    public class AddMonResponse : BaseResponse
    {
        public AddMonResponse() => Type = "AddMonResponse";
        public List<OrderMonData> OrderMons { get; set; } = new List<OrderMonData>();
    }
    public class DeleteMonResponse : BaseResponse
    {
        public DeleteMonResponse() => Type = "DeleteMonResponse";
        public List<OrderMonData> OrderMons { get; set; } = new List<OrderMonData>();
    }
    public class SendOrderResponse : BaseResponse
    {
        public SendOrderResponse() => Type = "SendOrderMonResponse";
        public List<OrderMonData> OrderMons { get; set; } = new List<OrderMonData>();
    }
    public class GetCategoriesResponse : BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<CategoryData> Categories { get; set; } = new List<CategoryData>();
    }
    public class TableOrderDetailData
    {
        public int MaBanAn { get; set; } 
        public string TenMon { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
        public DateTime ThoiGianGoi { get; set; }
        public string TrangThai { get; set; } = "";
    }

    public class GetTableDetailResponse : BaseResponse
    {
        public GetTableDetailResponse() => Type = "GetTableDetailResponse";
        public List<TableOrderDetailData> Orders { get; set; } = new List<TableOrderDetailData>();
    }
    // ==================== KITCHEN ORDER RESPONSES ====================

    /// <summary>
    /// Response danh sách đơn hàng cho bếp (Form chính)
    /// </summary>
    // Thêm method tính toán trong GetKitchenOrdersResponse

    public class GetKitchenOrdersResponse : BaseResponse
    {
        public GetKitchenOrdersResponse() => Type = "GetKitchenOrdersResponse";
        public List<KitchenOrderData> DonHang { get; set; } = new List<KitchenOrderData>();
        public ThongKeBep ThongKe { get; set; } = new ThongKeBep();

        // =========== THÊM: Tính toán thống kê chính xác ===========
        public void TinhToanThongKe()
        {
            if (DonHang == null || DonHang.Count == 0)
            {
                ThongKe = new ThongKeBep();
                return;
            }

            // Tạo thống kê mới từ danh sách đơn hàng
            ThongKe = new ThongKeBep(DonHang);
        }

        // =========== THÊM: Tính thống kê chi tiết ===========
        public ThongKeBepChiTiet GetThongKeChiTiet()
        {
            var thongKeChiTiet = new ThongKeBepChiTiet
            {
                Success = true,
                Message = "Thống kê chi tiết"
            };

            thongKeChiTiet.TinhToanTuDonHang(DonHang);
            return thongKeChiTiet;
        }
    }

    /// <summary>
    /// Response chi tiết đơn hàng (Form chi tiết)
    /// </summary>
    public class GetOrderDetailResponse : BaseResponse
    {
        public GetOrderDetailResponse() => Type = "GetOrderDetailResponse";
        public KitchenOrderDetailData ChiTietDonHang { get; set; } = new KitchenOrderDetailData();
    }

    /// <summary>
    /// Response cập nhật trạng thái món
    /// </summary>
    public class UpdateDishStatusResponse : BaseResponse
    {
        public UpdateDishStatusResponse() => Type = "UpdateDishStatusResponse";
        public string TenMon { get; set; } = "";
        public string TrangThaiCu { get; set; } = "";
        public string TrangThaiMoi { get; set; } = "";
        public DateTime ThoiGianCapNhat { get; set; }
        public DateTime? ThoiGianHoanThanh { get; set; } // THÊM DÒNG NÀY
        public int MaChiTiet { get; set; }
    }

    /// <summary>
    /// Response cập nhật nhiều món
    /// </summary>
    public class UpdateMultipleDishesResponse : BaseResponse
    {
        public UpdateMultipleDishesResponse() => Type = "UpdateMultipleDishesResponse";
        public int SoMonDaCapNhat { get; set; }
        public List<string> TenCacMon { get; set; } = new List<string>();
    }

    /// <summary>
    /// Response gửi tin nhắn
    /// </summary>
    public class SendKitchenMessageResponse : BaseResponse
    {
        public SendKitchenMessageResponse() => Type = "SendKitchenMessageResponse";
        public int MaTinNhan { get; set; }
        public DateTime ThoiGianGui { get; set; }
    }

    /// <summary>
    /// Response lấy lịch sử tin nhắn
    /// </summary>
    public class GetKitchenMessagesResponse : BaseResponse
    {
        public GetKitchenMessagesResponse() => Type = "GetKitchenMessagesResponse";
        public List<KitchenMessageData> TinNhan { get; set; } = new List<KitchenMessageData>();
    }

    /// <summary>
    /// Response thống kê bếp
    /// </summary>
    public class GetKitchenStatisticsResponse : BaseResponse
    {
        public GetKitchenStatisticsResponse() => Type = "GetKitchenStatisticsResponse";
        public KitchenStatisticsData ThongKe { get; set; } = new KitchenStatisticsData();
    }

    // ==================== KITCHEN DATA MODELS ====================

    public class KitchenOrderData
    {
        public int MaDonHang { get; set; }
        public string MaDonHangDisplay => $"#{MaDonHang:D3}";
        public int MaBanAn { get; set; }
        public string TenBan { get; set; } = "";
        public DateTime NgayOrder { get; set; }
        public string ThoiGianDisplay => NgayOrder.ToString("HH:mm");
        public string NgayOrderDisplay => NgayOrder.ToString("dd/MM/yyyy HH:mm");

        // Tổng số món và số món theo trạng thái
        public int TongSoMon { get; set; }
        public int SoMonChoXacNhan { get; set; }
        public int SoMonDangCheBien { get; set; }
        public int SoMonHoanThanh { get; set; }
        public int SoMonCoVanDe { get; set; }
        public int SoMonHuy { get; set; }

        // =========== SỬA QUAN TRỌNG ===========
        // Thêm thuộc tính kiểm tra đơn hoàn thành
        public bool LaDonHoanThanh => TongSoMon > 0 && SoMonHoanThanh == TongSoMon;
        public bool LaDonHuy => TongSoMon > 0 && SoMonHuy == TongSoMon;

        // Trạng thái tổng của đơn hàng (tự động tính)
        // public string TrangThaiDon { get; set; }
        // Thêm thuộc tính mới để lưu trạng thái từ SQL
        public string? TrangThaiThucTe { get; set; } = "";

        // Sửa TrangThaiDon để ưu tiên trạng thái thực tế nếu có
        public string TrangThaiDon
        {
            get
            {
                // Ưu tiên trạng thái từ SQL nếu có giá trị
                if (!string.IsNullOrEmpty(TrangThaiThucTe))
                    return TrangThaiThucTe;

                // Ngược lại tính toán tự động
                if (LaDonHuy) return "Huy";
                if (LaDonHoanThanh) return "HoanThanh";
                if (SoMonCoVanDe > 0) return "CoVanDe";
                if (SoMonDangCheBien > 0) return "DangCheBien";
                if (SoMonChoXacNhan > 0) return "ChoXacNhan";
                return "ChoXacNhan";
            }
        }
        public string TrangThaiDisplay
        {
            get
            {
                return TrangThaiDon switch
                {
                    "ChoXacNhan" => "⏳ Chờ xác nhận",
                    "DangCheBien" => "👨‍🍳 Đang chế biến",
                    "HoanThanh" => "✅ Hoàn thành",
                    "CoVanDe" => "⚠️ Có vấn đề",
                    "Huy" => "❌ Hủy",
                    _ => TrangThaiDon
                };
            }
        }

        public string ThongKeMonDisplay => $"{SoMonChoXacNhan}⏳ {SoMonDangCheBien}👨‍🍳 {SoMonHoanThanh}✅";

        // Độ ưu tiên cao nhất trong các món
        public int UuTienCaoNhat { get; set; } = 1;
        public string UuTienDisplay
        {
            get
            {
                return UuTienCaoNhat switch
                {
                    1 => "⭐",
                    2 => "🔥",
                    3 => "🔥🔥",
                    4 => "🔥🔥🔥",
                    5 => "🔥🔥🔥🔥",
                    _ => "⭐"
                };
            }
        }

        // Thời gian chờ
        public TimeSpan ThoiGianCho => DateTime.Now - NgayOrder;
        public string ThoiGianChoDisplay
        {
            get
            {
                var waitingTime = ThoiGianCho;
                if (waitingTime.TotalMinutes < 1)
                    return "Vừa xong";
                else if (waitingTime.TotalHours < 1)
                    return $"{(int)waitingTime.TotalMinutes} phút";
                else
                    return $"{(int)waitingTime.TotalHours} giờ {(int)waitingTime.TotalMinutes % 60} phút";
            }
        }

        public string TenNhanVienOrder { get; set; } = "";
        public decimal TongTien { get; set; }
        public DateTime? ThoiGianDuKienHoanThanh { get; set; }

        // Cho màu sắc trong DataGridView
        public System.Drawing.Color MauTrangThai
        {
            get
            {
                return TrangThaiDon switch
                {
                    "ChoXacNhan" => System.Drawing.Color.Orange,
                    "DangCheBien" => System.Drawing.Color.DodgerBlue,
                    "HoanThanh" => System.Drawing.Color.Green,
                    "CoVanDe" => System.Drawing.Color.Red,
                    "Huy" => System.Drawing.Color.Gray,
                    _ => System.Drawing.Color.Gray
                };
            }
        }
    }

    /// <summary>
    /// Dữ liệu chi tiết đơn hàng (FORM CHI TIẾT)
    /// </summary>
    public class KitchenOrderDetailData
    {
        public int MaChiTiet { get; set; }
        public int MaDonHang { get; set; }
        public string MaDonHangDisplay => $"#{MaDonHang:D3}";
        public int MaBanAn { get; set; }
        public string TenBan { get; set; } = "";
        public DateTime NgayOrder { get; set; }
        public string ThoiGianDisplay => NgayOrder.ToString("HH:mm");
        public string TenNhanVienOrder { get; set; } = "";
        public string TrangThaiDon { get; set; } = "";
        public decimal TongTien { get; set; }
        public string GhiChuDacBiet { get; set; } = "";
        public DateTime? ThoiGianDuKienHoanThanh { get; set; }

        public List<KitchenDishData> DanhSachMon { get; set; } = new List<KitchenDishData>();
        public List<KitchenMessageData> TinNhan { get; set; } = new List<KitchenMessageData>();

        // Tính toán
        public int TongSoMon => DanhSachMon?.Count ?? 0;
        public int SoMonChoXacNhan => DanhSachMon?.Count(m => m.TrangThai == "ChoXacNhan") ?? 0;
        public int SoMonDangCheBien => DanhSachMon?.Count(m => m.TrangThai == "DangCheBien") ?? 0;
        public int SoMonHoanThanh => DanhSachMon?.Count(m => m.TrangThai == "HoanThanh") ?? 0;

        public string ThongKeDisplay => $"{SoMonChoXacNhan}⏳ {SoMonDangCheBien}👨‍🍳 {SoMonHoanThanh}✅";

        public string ThoiGianConLaiDisplay
        {
            get
            {
                if (!ThoiGianDuKienHoanThanh.HasValue)
                    return "Chưa có ước tính";

                var timeLeft = ThoiGianDuKienHoanThanh.Value - DateTime.Now;
                if (timeLeft.TotalMinutes <= 0)
                    return "Đã quá hạn";
                else if (timeLeft.TotalMinutes < 1)
                    return "Sắp xong";
                else
                    return $"Còn {(int)timeLeft.TotalMinutes} phút";
            }
        }
    }

    /// <summary>
    /// Dữ liệu món ăn trong đơn hàng
    /// </summary>
    public class KitchenDishData
    {
        public int MaChiTiet { get; set; }
        public int MaMon { get; set; }
        public string TenMon { get; set; } = "";
        public string IconMon { get; set; } = "🍽️";
        public int SoLuong { get; set; }
        public string SoLuongDisplay => $"×{SoLuong}";
        public decimal DonGia { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
        public string GhiChuKhach { get; set; } = "";
        public string TrangThai { get; set; } = "ChoXacNhan";

        public string TrangThaiDisplay
        {
            get
            {
                return TrangThai switch
                {
                    "ChoXacNhan" => "⏳ Chờ xác nhận",
                    "DangCheBien" => "👨‍🍳 Đang chế biến",
                    "HoanThanh" => "✅ Hoàn thành",
                    "CoVanDe" => "⚠️ Có vấn đề",
                    "Huy" => "❌ Hủy",
                    _ => TrangThai
                };
            }
        }

        // Thời gian
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianHoanThanh { get; set; }
        public DateTime? ThoiGianDuKien { get; set; }

        public string ThoiGianDisplay
        {
            get
            {
                if (ThoiGianHoanThanh.HasValue)
                    return ThoiGianHoanThanh.Value.ToString("HH:mm");
                else if (ThoiGianBatDau.HasValue)
                {
                    var elapsed = DateTime.Now - ThoiGianBatDau.Value;
                    return $"{ThoiGianBatDau.Value:HH:mm} (+{(int)elapsed.TotalMinutes})";
                }
                return "--";
            }
        }

        // Ưu tiên (1-5)
        public int UuTien { get; set; } = 1;
        public string UuTienDisplay
        {
            get
            {
                return UuTien switch
                {
                    1 => "⭐",
                    2 => "🔥",
                    3 => "🔥🔥",
                    4 => "🔥🔥🔥",
                    5 => "🔥🔥🔥🔥",
                    _ => "⭐"
                };
            }
        }

        public string GhiChuBep { get; set; } = "";
        public string TenNhanVienCheBien { get; set; } = "";
        public int? MaNhanVienCheBien { get; set; }

        // Cho màu sắc
        public System.Drawing.Color MauTrangThai
        {
            get
            {
                return TrangThai switch
                {
                    "ChoXacNhan" => System.Drawing.Color.Orange,
                    "DangCheBien" => System.Drawing.Color.DodgerBlue,
                    "HoanThanh" => System.Drawing.Color.Green,
                    "CoVanDe" => System.Drawing.Color.Red,
                    "Huy" => System.Drawing.Color.Gray,
                    _ => System.Drawing.Color.Gray
                };
            }
        }

        // Cho việc chọn món
        public bool IsSelected { get; set; } = false;
    }

    /// <summary>
    /// Dữ liệu tin nhắn bếp - phục vụ
    /// </summary>
    public class KitchenMessageData
    {
        public int MaTin { get; set; }
        public int MaDonHang { get; set; }
        public int MaNguoiGui { get; set; }
        public string TenNguoiGui { get; set; } = "";
        public string VaiTroNguoiGui { get; set; } = ""; // "Bep", "PhucVu", "Admin"
        public int MaNguoiNhan { get; set; }
        public string TenNguoiNhan { get; set; } = "";
        public string NoiDung { get; set; } = "";
        public string LoaiTinNhan { get; set; } = "ThongBao";
        public DateTime ThoiGian { get; set; }
        public string ThoiGianDisplay => ThoiGian.ToString("HH:mm");
        public bool DaDoc { get; set; }

        // Cho hiển thị
        public string DisplayText => $"{ThoiGianDisplay} • {TenNguoiGui} ({VaiTroNguoiGui}): {NoiDung}";
    }


    public class ThongKeBep
    {
        // Properties
        public int TongSoDon { get; private set; }
        public int TongSoMon { get; private set; }
        public int DonChoXacNhan { get; private set; }
        public int DonDangCheBien { get; private set; }
        public int DonHoanThanh { get; private set; }
        public int DonCoVanDe { get; private set; }
        public int DonHuy { get; private set; }
        public decimal TongDoanhThu { get; private set; }
        public int TongMonHoanThanh { get; private set; }

        // Constructor duy nhất
        public ThongKeBep(List<KitchenOrderData> donHang = null)
        {
            if (donHang != null && donHang.Count > 0)
            {
                CalculateStatistics(donHang);
            }
        }

        // Phương thức tính toán thống kê chính
        private void CalculateStatistics(List<KitchenOrderData> donHang)
        {
            if (donHang == null || donHang.Count == 0)
                return;

            // Reset các giá trị
            TongSoDon = donHang.Count;
            TongSoMon = 0;
            TongMonHoanThanh = 0;
            DonChoXacNhan = 0;
            DonDangCheBien = 0;
            DonHoanThanh = 0;
            DonCoVanDe = 0;
            DonHuy = 0;
            TongDoanhThu = 0;

            foreach (var order in donHang)
            {
                // Tính tổng số món
                TongSoMon += order.TongSoMon;

                // Tính tổng món hoàn thành
                TongMonHoanThanh += order.SoMonHoanThanh;

                // Tính doanh thu từ đơn hoàn thành (tất cả món đều hoàn thành)
                if (order.LaDonHoanThanh)
                {
                    TongDoanhThu += order.TongTien;
                }

                // ⚠️ QUAN TRỌNG: Tính trạng thái đơn dựa trên LOGIC THỐNG NHẤT
                // Ưu tiên: Hủy > Hoàn thành > Có vấn đề > Đang chế biến > Chờ xác nhận

                // 1. Kiểm tra đơn hủy (tất cả món đều hủy)
                if (order.LaDonHuy)
                {
                    DonHuy++;
                }
                // 2. Kiểm tra đơn hoàn thành (tất cả món đều hoàn thành)
                else if (order.LaDonHoanThanh)
                {
                    DonHoanThanh++;
                }
                // 3. Kiểm tra có món có vấn đề
                else if (order.SoMonCoVanDe > 0)
                {
                    DonCoVanDe++;
                }
                // 4. Kiểm tra có món đang chế biến
                else if (order.SoMonDangCheBien > 0)
                {
                    DonDangCheBien++;
                }
                // 5. Mặc định là chờ xác nhận
                else
                {
                    DonChoXacNhan++;
                }
            }

            // Debug output
            Console.WriteLine($"📊 Thống kê từ {TongSoDon} đơn:");
            Console.WriteLine($"   - Tổng món: {TongSoMon} ({TongMonHoanThanh}✅)");
            Console.WriteLine($"   - Chờ xác nhận: {DonChoXacNhan}");
            Console.WriteLine($"   - Đang chế biến: {DonDangCheBien}");
            Console.WriteLine($"   - Hoàn thành: {DonHoanThanh}");
            Console.WriteLine($"   - Có vấn đề: {DonCoVanDe}");
            Console.WriteLine($"   - Hủy: {DonHuy}");
            Console.WriteLine($"   - Doanh thu đơn hoàn thành: {TongDoanhThu:N0} VNĐ");
        }

        // Tỷ lệ hoàn thành (đơn)
        public double TyLeHoanThanhDon => TongSoDon > 0
            ? Math.Round((double)DonHoanThanh / TongSoDon * 100, 1)
            : 0;

        // Tỷ lệ hoàn thành (món)
        public double TyLeHoanThanhMon => TongSoMon > 0
            ? Math.Round((double)TongMonHoanThanh / TongSoMon * 100, 1)
            : 0;

        // Hiển thị text thống kê
        public string ThongKeDisplay =>
            $"Tổng: {TongSoDon} đơn | " +
            $"⏳{DonChoXacNhan} 👨‍🍳{DonDangCheBien} ✅{DonHoanThanh} ⚠️{DonCoVanDe} ❌{DonHuy}";

        public string DisplayText =>
            $"Tổng: {TongSoDon} đơn • ⏳ {DonChoXacNhan} chờ • 👨‍🍳 {DonDangCheBien} đang • ✅ {DonHoanThanh} xong • ⚠️ {DonCoVanDe} vấn đề • ❌ {DonHuy} hủy";

        // Phương thức cập nhật thống kê từ danh sách đơn mới
        public void UpdateFromOrders(List<KitchenOrderData> donHang)
        {
            CalculateStatistics(donHang);
        }

     
    }

    /// <summary>
    /// Thống kê bếp chi tiết với logic đúng đơn hoàn thành
    /// </summary>
    public class ThongKeBepChiTiet : BaseResponse
    {
        public ThongKeBepChiTiet() => Type = "ThongKeBepChiTiet";

        // Tổng quan
        public int TongDon { get; set; }
        public int TongMon { get; set; }

        // =========== QUAN TRỌNG: Đơn hoàn thành được tính chính xác ===========
        public int DonHoanThanh { get; set; }
        public int DonChoXacNhan { get; set; }
        public int DonDangCheBien { get; set; }
        public int DonCoVanDe { get; set; }
        public int DonHuy { get; set; }

        // Tỷ lệ
        public double TyLeHoanThanh => TongDon > 0
            ? Math.Round((double)DonHoanThanh / TongDon * 100, 1)
            : 0;

        // Thời gian
        public double? ThoiGianTrungBinh { get; set; } // phút
        public string ThoiGianTrungBinhDisplay => ThoiGianTrungBinh.HasValue
            ? $"{Math.Round(ThoiGianTrungBinh.Value, 1)} phút"
            : "N/A";

        // Chi tiết từng đơn
        public List<DonHangThongKe> ChiTietDonHang { get; set; } = new List<DonHangThongKe>();

        // Tính toán từ danh sách đơn hàng
        public void TinhToanTuDonHang(List<KitchenOrderData> donHang)
        {
            if (donHang == null || donHang.Count == 0)
                return;

            TongDon = donHang.Count;
            TongMon = donHang.Sum(d => d.TongSoMon);

            // QUAN TRỌNG: Đơn hoàn thành khi TẤT CẢ món đều hoàn thành
            DonHoanThanh = donHang.Count(d => d.LaDonHoanThanh);

            DonChoXacNhan = donHang.Count(d => d.TrangThaiDon == "ChoXacNhan");
            DonDangCheBien = donHang.Count(d => d.TrangThaiDon == "DangCheBien");
            DonCoVanDe = donHang.Count(d => d.TrangThaiDon == "CoVanDe");
            DonHuy = donHang.Count(d => d.TrangThaiDon == "Huy");

            // Tạo chi tiết
            ChiTietDonHang = donHang.Select(d => new DonHangThongKe
            {
                MaDonHang = d.MaDonHang,
                TenBan = d.TenBan,
                TongSoMon = d.TongSoMon,
                SoMonHoanThanh = d.SoMonHoanThanh,
                LaDonHoanThanh = d.LaDonHoanThanh,
                TrangThai = d.TrangThaiDon,
                NgayOrder = d.NgayOrder
            }).ToList();
        }
    }

    /// <summary>
    /// Chi tiết từng đơn hàng cho thống kê
    /// </summary>
    public class DonHangThongKe
    {
        public int MaDonHang { get; set; }
        public string TenBan { get; set; } = "";
        public int TongSoMon { get; set; }
        public int SoMonHoanThanh { get; set; }

        // =========== QUAN TRỌNG: Kiểm tra đơn hoàn thành ===========
        public bool LaDonHoanThanh { get; set; }

        public string TrangThai { get; set; } = "";
        public DateTime NgayOrder { get; set; }

        public string TrangThaiDisplay => LaDonHoanThanh ? "✅ ĐÃ HOÀN THÀNH" :
            TrangThai switch
            {
                "ChoXacNhan" => "⏳ Chờ xác nhận",
                "DangCheBien" => "👨‍🍳 Đang chế biến",
                "HoanThanh" => "✅ Hoàn thành",
                "CoVanDe" => "⚠️ Có vấn đề",
                "Huy" => "❌ Hủy",
                _ => TrangThai
            };

        public double TyLeHoanThanh => TongSoMon > 0
            ? Math.Round((double)SoMonHoanThanh / TongSoMon * 100, 1)
            : 0;
    }
    /// <summary>
    /// Dữ liệu top món ăn
    /// </summary>
    public class TopMonData
    {
        public int MaMon { get; set; }
        public string TenMon { get; set; } = "";
        public int SoLanOrder { get; set; }
        public int TongSoPhan { get; set; }
    }

    /// <summary>
    /// Hiệu suất đầu bếp
    /// </summary>
    public class HieuSuatDauBep
    {
        public int MaNhanVien { get; set; }
        public string TenNhanVien { get; set; } = "";
        public int TongSoMon { get; set; }
        public TimeSpan ThoiGianTrungBinh { get; set; }
        public string ThoiGianTrungBinhDisplay => $"{(int)ThoiGianTrungBinh.TotalMinutes} phút/món";
        public int HieuSuatPhanTram { get; set; } // 0-100
    }


    public class GetThongKeBepResponse : BaseResponse
    {
        public GetThongKeBepResponse() => Type = "GetThongKeBepResponse";

        public ThongKeBepTongQuan TongQuan { get; set; } = new ThongKeBepTongQuan();
        public List<ThongKeDauBep> DanhSachDauBep { get; set; } = new List<ThongKeDauBep>(); // Đổi thành ThongKeDauBep
        public List<TopMonAnThongKe> TopMonAn { get; set; } = new List<TopMonAnThongKe>(); // Đổi thành TopMonAnThongKe
    }
    /// Response danh sách đầu bếp
    /// </summary>
    public class GetDanhSachDauBepResponse : BaseResponse
    {
        public GetDanhSachDauBepResponse() => Type = "GetDanhSachDauBepResponse";
        public List<NguoiDung> DanhSachDauBep { get; set; } = new List<NguoiDung>();
    }
  
    public class CheckTransferStatusResponse : BaseResponse
    {
        public CheckTransferStatusResponse() => Type = "CheckTransferStatusResponse";
        public bool IsPaid { get; set; } // True = Đã thanh toán, False = Chưa
    }
    // ==================== CHAT RESPONSES ====================

    /// <summary>
    /// Response danh sách user để chat
    /// </summary>
    public class GetChatUsersResponse : BaseResponse
    {
        public GetChatUsersResponse() => Type = "LoGetChatUsersResponse";
        public List<ChatUserData> Users { get; set; } = new List<ChatUserData>();
        public int TongSoUser { get; set; }
    }

    /// <summary>
    /// Response gửi tin nhắn
    /// </summary>
    public class SendChatMessageResponse : BaseResponse
    {
        public SendChatMessageResponse() => Type = "SendChatMessageResponse";
        public int MaTinNhan { get; set; }
        public DateTime ThoiGianGui { get; set; }
        public int SoNguoiNhan { get; set; }        // Số người nhận (nếu broadcast)
    }

    /// <summary>
    /// Response danh sách tin nhắn
    /// </summary>
    public class GetChatMessagesResponse : BaseResponse
    {
        public GetChatMessagesResponse() => Type = "GetChatMessagesResponse";
        public List<ChatMessageData> Messages { get; set; } = new List<ChatMessageData>();
        public int TongSoTinNhan { get; set; }
    }

    /// <summary>
    /// Response đánh dấu đã đọc
    /// </summary>
    public class MarkMessagesReadResponse : BaseResponse
    {
        public MarkMessagesReadResponse() => Type = "MarkMessagesReadResponse";
        public int SoTinDaDoc { get; set; }
    }

    /// <summary>
    /// Response số tin nhắn chưa đọc
    /// </summary>
    public class GetUnreadCountResponse : BaseResponse
    {
        public GetUnreadCountResponse() => Type = "GetUnreadCountResponse";
        public int TongChuaDoc { get; set; }
        public List<UnreadCountData> ChiTietChuaDoc { get; set; } = new List<UnreadCountData>();
    }

    /// <summary>
    /// Response kiểm tra tin nhắn mới
    /// </summary>
    public class CheckNewMessagesResponse : BaseResponse
    {
        public CheckNewMessagesResponse() => Type = "CheckNewMessagesResponse";
        public bool CoTinMoi { get; set; }
        public int SoTinMoi { get; set; }
        public List<ChatMessageData> TinNhanMoi { get; set; } = new List<ChatMessageData>();
    }

    // ==================== CHAT DATA MODELS ====================

    /// <summary>
    /// Thông tin user để hiển thị trong danh sách chat
    /// </summary>
    public class ChatUserData
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; } = "";
        public string VaiTro { get; set; } = "";
        public string VaiTroDisplay
        {
            get
            {
                return VaiTro switch
                {
                    "Admin" => "👑 Quản lý",
                    "PhucVu" => "🍽️ Phục vụ",
                    "Bep" => "👨‍🍳 Bếp",
                    _ => VaiTro
                };
            }
        }
        public bool DangOnline { get; set; }
        public DateTime? LanCuoiOnline { get; set; }
        public int SoTinChuaDoc { get; set; }

        // Hiển thị
        public string TrangThaiDisplay => DangOnline ? "●" : "○";
        public System.Drawing.Color MauTrangThai => DangOnline ?
            System.Drawing.Color.Green : System.Drawing.Color.Gray;
    }

    /// <summary>
    /// Thông tin tin nhắn chat
    /// </summary>
    public class ChatMessageData
    {
        public int MaTinNhan { get; set; }
        public int MaNguoiGui { get; set; }
        public string TenNguoiGui { get; set; } = "";
        public string VaiTroNguoiGui { get; set; } = "";
        public int MaNguoiNhan { get; set; }
        public string TenNguoiNhan { get; set; } = "";
        public string NoiDung { get; set; } = "";
        public DateTime ThoiGian { get; set; }
        public bool DaDoc { get; set; }
        public bool LaTinBroadcast { get; set; }    // Tin gửi cho tất cả

        // Hiển thị
        public string ThoiGianDisplay => ThoiGian.ToString("HH:mm");
        public string ThoiGianDayDu => ThoiGian.ToString("HH:mm dd/MM/yyyy");

        // Kiểm tra tin của mình hay người khác
        public bool LaTinCuaToi(int maNguoiDungHienTai) => MaNguoiGui == maNguoiDungHienTai;
    }

    /// <summary>
    /// Số tin chưa đọc theo từng người
    /// </summary>
    public class UnreadCountData
    {
        public int MaNguoiGui { get; set; }
        public string TenNguoiGui { get; set; } = "";
        public int SoChuaDoc { get; set; }
    }
}