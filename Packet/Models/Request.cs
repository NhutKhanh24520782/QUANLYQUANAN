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
    public class AddTableRequest
    {
        public string Type => "AddTable"; // ✅ THÊM DÒNG NÀY
        public int MaBan { get; set; }
        public string TenBan { get; set; }
        public string TrangThai { get; set; }
    }
    public class UpdateTableRequest
    {
        public string Type => "UpdateTable"; // ✅ THÊM DÒNG NÀY
        public int MaBan { get; set; }
        public string TenBan { get; set; }
        public string TrangThai { get; set; }
    }
}

