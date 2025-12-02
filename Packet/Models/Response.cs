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

    public enum OrderStatus
    {
        ChoXacNhan = 0,
        DangCheBien = 1,
        HoanThanh = 2,
        Huy = 3
    }

    public enum TableStatus
    {
        Trong = 0,
        DangSuDung = 1,
        DaDat = 2
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
        public string TenMon { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
        public DateTime ThoiGianGoi { get; set; }
    }
    public class GetTableDetailResponse : BaseResponse
    {
        public GetTableDetailResponse() => Type = "GetTableDetailResponse";
        public List<TableOrderDetailData> Orders { get; set; } = new List<TableOrderDetailData>();
    }
}