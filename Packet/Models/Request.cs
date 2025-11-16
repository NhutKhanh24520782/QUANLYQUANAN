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
    public class AddTableRequest 
    {
        public int MaBan { get; set; }
        public string TenBan { get; set; }
        public string TrangThai { get; set; }
    }
}

