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

    // ==================== ERROR RESPONSE ====================

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
}