using Models.Database;

namespace Models.Request
{

    // ==================== AUTHENTICATION REQUESTS ====================

    public class LoginRequest
    {
        public string Type => "Login";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public (bool isValid, string error) Validate()
        {
            if (string.IsNullOrWhiteSpace(Username))
                return (false, "Username không được để trống");
            if (string.IsNullOrWhiteSpace(Password))
                return (false, "Password không được để trống");
            return (true, string.Empty);
        }
    }

    public class RegisterRequest
    {
        public string Type => "Register";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public (bool isValid, string error) Validate()
        {
            if (string.IsNullOrWhiteSpace(Username))
                return (false, "Username không được để trống");
            if (string.IsNullOrWhiteSpace(Password))
                return (false, "Password không được để trống");
            if (string.IsNullOrWhiteSpace(HoTen))
                return (false, "Họ tên không được để trống");
            if (string.IsNullOrWhiteSpace(Email))
                return (false, "Email không được để trống");
            if (string.IsNullOrWhiteSpace(Role))
                return (false, "Vai trò không được để trống");
            return (true, string.Empty);
        }
    }

    public class UpdatePasswordRequest
    {
        public string Type => "UpdatePassword";
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        public (bool isValid, string error) Validate()
        {
            if (string.IsNullOrWhiteSpace(Email))
                return (false, "Email không được để trống");
            if (string.IsNullOrWhiteSpace(NewPassword))
                return (false, "Mật khẩu mới không được để trống");
            return (true, string.Empty);
        }
    }

    public class CheckEmailRequest
    {
        public string Type => "CheckEmail";
        public string Email { get; set; } = string.Empty;

        public (bool isValid, string error) Validate()
        {
            if (string.IsNullOrWhiteSpace(Email))
                return (false, "Email không được để trống");
            return (true, string.Empty);
        }
    }
    public class GetEmployeesRequest
    {
        public string Type => "GetEmployees";
        public string Keyword { get; set; } = "";
        public string VaiTro { get; set; } = "";
    }

    public class AddEmployeeRequest
    {
        public string Type => "AddEmployee";
        public string TenDangNhap { get; set; } = "";
        public string MatKhau { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string VaiTro { get; set; } = "";
        public string SDT { get; set; } = "";
        public DateTime NgayVaoLam { get; set; } = DateTime.Now;
    }

    public class UpdateEmployeeRequest
    {
        public string Type => "UpdateEmployee";
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string VaiTro { get; set; } = "";
        public string SDT { get; set; } = "";
        public bool TrangThai { get; set; } = true;
    }

    public class DeleteEmployeeRequest
    {
        public string Type => "DeleteEmployee";
        public int MaNguoiDung { get; set; }
    }
    public class ThongKeDoanhThuRequest
    {
        public string Type => "ThongKeDoanhThu";
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        public (bool isValid, string error) Validate()
        {
            if (TuNgay > DenNgay)
                return (false, "Từ ngày không được lớn hơn đến ngày");

            return (true, string.Empty);
        }
    }

    /// <summary>
    /// Request xuất báo cáo
    /// </summary>
    public class XuatBaoCaoRequest
    {
        public string Type => "XuatBaoCao";
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public List<DoanhThuTheoBan> Data { get; set; } = new List<DoanhThuTheoBan>();
        public decimal TongDoanhThu { get; set; }
    }


    public class GetBillRequest
    {
        public string Type => "GetBills";
        public int MaHoaDon { get; set; }
        public int MaBanAn { get; set; }
        public int MaNhanVien { get; set; }
        public DateTime NgayXuatHoaDon { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
    }
    // ===== MENU REQUESTS =====
    public class GetMenuRequest
    {
        public string Type => "GetMenu";

    }

    public class SearchMenuRequest
    {
        public string Type => "SearchMenu";
        public string Keyword { get; set; } = "";
    }

    public class AddMenuRequest
    {
        public string Type => "AddMenu";
        public string TenMon { get; set; } = "";
        public decimal Gia { get; set; } = 0m;
        public string MoTa { get; set; } = "";
        public int? MaLoaiMon { get; set; } = null;
        public string TrangThai { get; set; } = "ConMon"; // ✅ THÊM

    }

    public class UpdateMenuRequest
    {
        public string Type => "UpdateMenu";
        public int MaMon { get; set; }
        public string TenMon { get; set; } = "";
        public decimal Gia { get; set; } = 0m;
        public string MoTa { get; set; } = "";
        public int? MaLoaiMon { get; set; } = null;
        public string TrangThai { get; set; } = "ConMon"; // ✅ THÊM

    }

    public class DeleteMenuRequest
    {
        public string Type => "DeleteMenu";
        public int MaMon { get; set; }
    }

    public class UpdateMenuStatusRequest
    {
        public string Type => "UpdateMenuStatus";
        public int MaMon { get; set; }
        public string TrangThai { get; set; } = "ConMon"; // "ConMon" hoặc "HetMon"
    }
    // ==================== BÀN ĂN (TABLE) REQUESTS ====================
    // Trong Models/Request.cs
    public class AddTableRequest
    {
        public string Type => "AddTable";
        public string TenBan { get; set; } = string.Empty;
        public int? SoChoNgoi { get; set; }
        public string TrangThai { get; set; } = "Trong";    
        public int? MaNhanVien { get; set; } = null;
    }

    public class UpdateTableRequest
    {
        public string Type => "UpdateTable";
        public int MaBanAn { get; set; } // Đổi từ MaBan -> MaBanAn
        public string TenBan { get; set; } = string.Empty;
        public int? SoChoNgoi { get; set; }
        public string TrangThai { get; set; } = "Trong";
        public int? MaNhanVien { get; set; } = null;
    }

    public class DeleteTableRequest
    {
        public string Type => "DeleteTable";
        public int MaBanAn { get; set; } // Đổi từ MaBan -> MaBanAn
    }
    public class GetTablesRequest
    {
        public string Type => "GetTables";
        // Không cần tham số gì thêm vì lấy toàn bộ
    }
    public class SearchTablesRequest
    {
        public string Type => "SearchTables";
        public string Keyword { get; set; } = "";
    }
    // Models/Request/PaymentRequest.cs
    public class GetPendingPaymentsRequest
    {
        public string Type => "GetPendingPayments";
        public int MaNhanVien { get; set; }

        public (bool isValid, string error) Validate()
        {
            if (MaNhanVien <= 0) return (false, "Mã nhân viên không hợp lệ");
            return (true, string.Empty);
        }
    }

    public class ProcessPaymentRequest
    {
        public string Type => "ProcessPayment";
        public int MaHD { get; set; }
        public int MaNhanVien { get; set; }
        public string PhuongThucThanhToan { get; set; } = "TienMat"; // "TienMat", "ChuyenKhoan"
        public decimal SoTienThanhToan { get; set; }
        public decimal SoTienNhan { get; set; }

        public (bool isValid, string error) Validate()
        {
            if (MaHD <= 0) return (false, "Mã hóa đơn không hợp lệ");
            if (MaNhanVien <= 0) return (false, "Mã nhân viên không hợp lệ");
            if (SoTienThanhToan <= 0) return (false, "Số tiền thanh toán phải lớn hơn 0");
            if (PhuongThucThanhToan != "TienMat" && PhuongThucThanhToan != "ChuyenKhoan")
                return (false, "Phương thức thanh toán không hợp lệ");
            if (PhuongThucThanhToan == "TienMat" && SoTienNhan < SoTienThanhToan)
                return (false, "Số tiền nhận không đủ để thanh toán");
            return (true, string.Empty);
        }
    }

    public class ProcessCashPaymentRequest
    {
        public string Type => "ProcessCashPayment";
        public int MaHD { get; set; }
        public decimal SoTienNhan { get; set; }
        public string GhiChu { get; set; } = string.Empty;
        public int MaNV { get; set; }

        public (bool isValid, string error) Validate()
        {
            if (MaHD <= 0) return (false, "Mã hóa đơn không hợp lệ");
            if (SoTienNhan <= 0) return (false, "Số tiền nhận phải lớn hơn 0");
            if (MaNV <= 0) return (false, "Mã nhân viên không hợp lệ");
            return (true, string.Empty);
        }
    }

    public class ProcessTransferPaymentRequest
    {
        public string Type => "ProcessTransferPayment";
        public int MaHD { get; set; }
        public string OrderInfo { get; set; } = string.Empty;
        public int MaNV { get; set; }

        public (bool isValid, string error) Validate()
        {
            if (MaHD <= 0) return (false, "Mã hóa đơn không hợp lệ");
            if (string.IsNullOrWhiteSpace(OrderInfo)) return (false, "Thông tin đơn hàng không được trống");
            if (MaNV <= 0) return (false, "Mã nhân viên không hợp lệ");
            return (true, string.Empty);
        }
    }

    public class CheckPaymentStatusRequest
    {
        public string Type => "CheckPaymentStatus";
        public int MaHoaDon { get; set; }
        public string TransactionNo { get; set; } = string.Empty;

        public (bool isValid, string error) Validate()
        {
            if (MaHoaDon <= 0) return (false, "Mã hóa đơn không hợp lệ");
            if (string.IsNullOrWhiteSpace(TransactionNo)) return (false, "Mã giao dịch không được trống");
            return (true, string.Empty);
        }
    }

    public class GetPaymentDetailsRequest
    {
        public string Type => "GetPaymentDetails";
        public int MaHoaDon { get; set; }

        public (bool isValid, string error) Validate()
        {
            if (MaHoaDon <= 0) return (false, "Mã hóa đơn không hợp lệ");
            return (true, string.Empty);
        }
    }
    // ==================== ORDER REQUESTS ====================
    public class GetMonRequest
    {
        public string Type => "GetMon";
        public int MaMon { get; set; }
        public string TenMon { get; set; } = "";
        public int Gia { get; set; }
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class AddMonRequest
    {
        public string Type => "AddMon";
        public int MaLoaiMon { get; set; }
        public string TenMon { get; set; } = "";
        public int Gia { get; set; }
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class DeleteMonRequest
    {
        public string Type => "DeleteMon";
        public string TenMon { get; set; } = "";
    }
    public class SendOrderRequest
    {
        public string Type => "SendOrder";
        public string TenMon { get; set; } = "";
        public int Gia { get; set; }
        public string MoTa { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }
    public class GetMenuByCategoryRequest
    {
        public string Type => "GetMenuByCategory";
        public int MaLoaiMon { get; set; }
    }
    public class GetCategoriesRequest
    {
        public string Type => "GetCategories";
    }
    public class GetTableDetailRequest
    {
        public string Type => "GetTableDetail";
        public int MaBanAn { get; set; }        // Lọc theo bàn (0 = tất cả)
        public string TrangThai { get; set; } = ""; // Lọc theo trạng thái ("" = tất cả)
    }
    // ==================== KITCHEN ORDER REQUESTS ====================

    /// <summary>
    /// Request lấy danh sách đơn hàng cho bếp
    /// </summary>
    public class GetKitchenOrdersRequest
    {
        public string Type => "GetKitchenOrders";
        public string TrangThai { get; set; } = ""; // "TatCa", "ChoXacNhan", "DangCheBien", "HoanThanh", "Huy"
        public string TimKiemBan { get; set; } = "";
        public string SapXep { get; set; } = "ThoiGian"; // "ThoiGian", "UuTien", "Ban"
        public int? MaNhanVienBep { get; set; } // Đầu bếp hiện tại (để lọc món đang xử lý)
    }

    /// <summary>
    /// Request lấy chi tiết đơn hàng
    /// </summary>
    public class GetOrderDetailRequest
    {
        public string Type => "GetOrderDetail";
        public int MaDonHang { get; set; }
    }

    /// <summary>
    /// Request cập nhật trạng thái món ăn
    /// </summary>
    public class UpdateDishStatusRequest
    {
        public string Type => "UpdateDishStatus";
        public int MaDonHang { get; set; }
        public int MaChiTiet { get; set; }
        public string TrangThaiMoi { get; set; } // "ChoXacNhan", "DangCheBien", "HoanThanh", "CoVanDe", "Huy"
        public int? MaNhanVienBep { get; set; } // ID đầu bếp xử lý
        public string GhiChuBep { get; set; } = "";
        public DateTime? ThoiGianDuKienHoanThanh { get; set; }
        public int UuTien { get; set; } = 1;
        public bool GuiThongBao { get; set; } = true;

        public (bool isValid, string error) Validate()
        {
            if (MaDonHang <= 0)
                return (false, "Mã đơn hàng không hợp lệ");
            if (MaChiTiet <= 0)
                return (false, "Mã chi tiết không hợp lệ");
            if (string.IsNullOrEmpty(TrangThaiMoi))
                return (false, "Trạng thái mới không được để trống");

            var validStatuses = new[] { "ChoXacNhan", "DangCheBien", "HoanThanh", "CoVanDe", "Huy" };
            if (!validStatuses.Contains(TrangThaiMoi))
                return (false, "Trạng thái không hợp lệ");

            return (true, string.Empty);
        }
    }

    /// <summary>
    /// Request cập nhật nhiều món cùng lúc
    /// </summary>
    public class UpdateMultipleDishesRequest
    {
        public string Type => "UpdateMultipleDishes";
        public int MaDonHang { get; set; }
        public List<DishUpdateItem> DanhSachMon { get; set; } = new List<DishUpdateItem>();
        public bool GuiThongBao { get; set; } = true;

        public (bool isValid, string error) Validate()
        {
            if (MaDonHang <= 0)
                return (false, "Mã đơn hàng không hợp lệ");
            if (DanhSachMon == null || DanhSachMon.Count == 0)
                return (false, "Danh sách món không được để trống");

            foreach (var mon in DanhSachMon)
            {
                var validStatuses = new[] { "ChoXacNhan", "DangCheBien", "HoanThanh", "CoVanDe", "Huy" };
                if (!validStatuses.Contains(mon.TrangThaiMoi))
                    return (false, $"Trạng thái không hợp lệ cho món {mon.MaChiTiet}");
            }

            return (true, string.Empty);
        }
    }

    /// <summary>
    /// Item trong danh sách cập nhật nhiều món
    /// </summary>
    public class DishUpdateItem
    {
        public int MaChiTiet { get; set; }
        public string TrangThaiMoi { get; set; } = "";
        public string GhiChuBep { get; set; } = "";
        public int? MaNhanVienBep { get; set; }
        public DateTime? ThoiGianDuKien { get; set; }
        public int UuTien { get; set; } = 1;
    }

    /// <summary>
    /// Request gửi tin nhắn từ bếp đến phục vụ
    /// </summary>
    public class SendKitchenMessageRequest
    {
        public string Type => "SendKitchenMessage";
        public int MaDonHang { get; set; }
        public int MaNhanVienGui { get; set; } // ID đầu bếp
        public int? MaNhanVienNhan { get; set; } // ID phục vụ (null = gửi cho tất cả phục vụ)
        public string NoiDung { get; set; } = "";
        public string LoaiTinNhan { get; set; } = "ThongBao"; // "ThongBao", "YeuCau", "CanhBao"
        public bool HienPopup { get; set; } = true;
        public bool PhatAmThanh { get; set; } = true;

        public (bool isValid, string error) Validate()
        {
            if (MaDonHang <= 0)
                return (false, "Mã đơn hàng không hợp lệ");
            if (MaNhanVienGui <= 0)
                return (false, "Mã người gửi không hợp lệ");
            if (string.IsNullOrWhiteSpace(NoiDung))
                return (false, "Nội dung không được để trống");

            return (true, string.Empty);
        }
    }

    /// <summary>
    /// Request lấy lịch sử tin nhắn
    /// </summary>
    public class GetKitchenMessagesRequest
    {
        public string Type => "GetKitchenMessages";
        public int MaDonHang { get; set; }
        public int? MaNhanVienBep { get; set; }
    }

    /// <summary>
    /// Request thống kê công việc bếp
    /// </summary>
    public class GetKitchenStatisticsRequest
    {
        public string Type => "GetKitchenStatistics";
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public int? MaNhanVienBep { get; set; } // Thống kê theo đầu bếp

        public (bool isValid, string error) Validate()
        {
            if (TuNgay > DenNgay)
                return (false, "Từ ngày không được lớn hơn đến ngày");

            return (true, string.Empty);
        }
    }
    public class GetThongKeBepRequest
    {
        public string Type => "GetThongKeBep";
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public int? MaNhanVienBep { get; set; }

        public (bool isValid, string error) Validate()
        {
            if (TuNgay > DenNgay)
                return (false, "Từ ngày không được lớn hơn đến ngày");

            return (true, string.Empty);
        }
    }

    /// <summary>
    /// Request lấy danh sách đầu bếp
    /// </summary>
    public class GetDanhSachDauBepRequest
    {
        public string Type => "GetDanhSachDauBep";
        // Không cần tham số vì lấy tất cả đầu bếp
    }

    /// <summary>
    /// Request lấy thống kê chi tiết đầu bếp
    /// </summary>
    public class GetThongKeDauBepChiTietRequest
    {
        public string Type => "GetThongKeDauBepChiTiet";
        public int MaNhanVien { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        public (bool isValid, string error) Validate()
        {
            if (MaNhanVien <= 0)
                return (false, "Mã nhân viên không hợp lệ");
            if (TuNgay > DenNgay)
                return (false, "Từ ngày không được lớn hơn đến ngày");

            return (true, string.Empty);
        }
    }

    /// <summary>
    /// Request xuất báo cáo thống kê bếp
    /// </summary>
    public class XuatBaoCaoThongKeBepRequest
    {
        public string Type => "XuatBaoCaoThongKeBep";
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public int? MaNhanVienBep { get; set; }
        public string LoaiBaoCao { get; set; } = "TongHop"; // "TongHop", "ChiTiet", "TheoDauBep"
        public string DinhDang { get; set; } = "PDF"; // "PDF", "Excel", "Word"

        public (bool isValid, string error) Validate()
        {
            if (TuNgay > DenNgay)
                return (false, "Từ ngày không được lớn hơn đến ngày");

            return (true, string.Empty);
        }
    }
}

