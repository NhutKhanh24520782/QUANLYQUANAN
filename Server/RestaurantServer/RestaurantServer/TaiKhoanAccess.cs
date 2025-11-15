// File: TaiKhoanAccess.cs
// Nằm trong dự án: RestaurantServer

// 1. Thêm các thư viện cần thiết
using Microsoft.Data.SqlClient; // Để kết nối SQL Server
using System.Configuration;     // Để đọc file App.config
using System.Threading.Tasks; // Để chạy hàm bất đồng bộ (async/await)
using System;

// Đây là lớp Data Access Layer (DAL) - Lớp Truy cập Dữ liệu
// Nhiệm vụ duy nhất của nó là nói chuyện với CSDL về Bảng 'Users'
public class TaiKhoanAccess
{
    // Biến này sẽ chứa chuỗi kết nối CSDL, 'readonly' nghĩa là chỉ gán 1 lần
    private readonly string connectionString;

    // Hàm khởi tạo (Constructor)
    // Sẽ được gọi ngay khi một đối tượng TaiKhoanAccess được tạo ra
    public TaiKhoanAccess()
    {
        // Tự động đọc file App.config và lấy chuỗi kết nối có 'name' là "MySecureConnection"
        connectionString = ConfigurationManager.ConnectionStrings["MySecureConnection"].ConnectionString;
    }

    // =====================================================================
    // PHƯƠNG THỨC #1: ĐĂNG KÝ (Register) - (Thao tác bằng TaiKhoan)
    // =====================================================================
    /// <summary>
    /// Đăng ký một người dùng mới với mật khẩu đã băm (Mã hóa mật khẩu).
    /// </summary>
    /// <param name="username">Tên đăng nhập</param>
    /// <param name="plainPassword">Mật khẩu THUẦN (chưa băm)</param>
    /// <returns>True nếu thành công</returns>
    /// <exception cref="InvalidOperationException">Ném lỗi nếu username đã tồn tại</exception>
    /// <exception cref="Exception">Ném các lỗi CSDL khác</exception>
    public async Task<bool> RegisterAsync(string username, string plainPassword)
    {
        try
        {
            // ---- PHẦN MÃ HÓA MẬT KHẨU ----
            // Sử dụng BCrypt để "băm" mật khẩu.
            // BCrypt tự động thêm "muối" (salt) để tăng bảo mật.
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // ---- PHẦN KẾT NỐI SQL SERVER ----
            // 'await using' đảm bảo kết nối (conn) và lệnh (cmd) được đóng tự động
            // kể cả khi có lỗi xảy ra. Đây là cách làm hiện đại.
            await using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Mở kết nối đến CSDL
                await conn.OpenAsync();

                // ---- PHẦN THAO TÁC (CHỐNG SQL INJECTION) ----
                // Chúng ta dùng @username và @passwordHash
                // Đây gọi là "Parameterized Query" -> Cách duy nhất CHỐNG TẤN CÔNG SQL INJECTION.
                // TUYỆT ĐỐI KHÔNG dùng: "INSERT INTO... VALUES ('" + username + "', ...)"
                string query = "INSERT INTO Users (Username, PasswordHash) VALUES (@username, @passwordHash)";

                await using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Cung cấp giá trị cho các tham số
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@passwordHash", hashedPassword);

                    // Thực thi câu lệnh (NonQuery = không trả về dữ liệu, chỉ thực thi)
                    await cmd.ExecuteNonQueryAsync();

                    return true; // Đăng ký thành công
                }
            }
        }
        catch (SqlException sqlEx)
        {
            // Xử lý lỗi nếu username đã tồn tại (do ta đặt 'UNIQUE' trong CSDL)
            // Mã 2601 và 2627 là lỗi "Vi phạm ràng buộc UNIQUE"
            if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
            {
                // Ném một lỗi rõ ràng để Server (và sau này là Client) biết
                throw new InvalidOperationException("Tên đăng nhập này đã tồn tại.");
            }

            // Ném các lỗi CSDL khác (mất mạng, sai tên bảng...)
            throw new Exception("Lỗi CSDL: " + sqlEx.Message);
        }
    }

    // =====================================================================
    // PHƯƠNG THỨC #2: ĐĂNG NHẬP (Login) - (Thao tác bằng TaiKhoan)
    // =====================================================================
    /// <summary>
    /// Kiểm tra thông tin đăng nhập.
    /// </summary>
    /// <param name="username">Tên đăng nhập</param>
    /// <param name="plainPassword">Mật khẩu THUẦN (chưa băm)</param>
    /// <returns>True nếu đăng nhập đúng, False nếu sai</returns>
    public async Task<bool> LoginAsync(string username, string plainPassword)
    {
        try
        {
            string storedHash = null; // Biến để lưu chuỗi hash lấy từ CSDL

            // --- 1. Lấy chuỗi hash đã lưu ---
            await using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                // Cũng dùng Parameterized Query để chống SQL Injection
                string query = "SELECT PasswordHash FROM Users WHERE Username = @username";

                await using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    // Dùng ExecuteScalarAsync: Lấy về 1 giá trị duy nhất (ô đầu tiên, hàng đầu tiên)
                    object result = await cmd.ExecuteScalarAsync();

                    if (result != null)
                    {
                        storedHash = result.ToString();
                    }
                }
            } // Kết nối tự động đóng

            // --- 2. So sánh mật khẩu ---

            // Nếu user không tồn tại, 'storedHash' sẽ là null
            if (storedHash == null)
            {
                return false; // User không tồn tại
            }

            // ---- PHẦN MÃ HÓA MẬT KHẨU (Xác thực) ----
            // Dùng BCrypt.Verify để so sánh mật khẩu thuần (plainPassword)
            // với chuỗi hash đã lưu (storedHash).
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);

            return isPasswordCorrect;
        }
        catch (Exception ex)
        {
            // Trong môi trường Server, ta nên ghi log lỗi ra
            Console.WriteLine($"[LỖI NGHIÊM TRỌNG] Không thể xác thực: {ex.Message}");
            return false; // Trả về False nếu có bất kỳ lỗi nào (mất kết nối CSDL...)
        }
    }
}