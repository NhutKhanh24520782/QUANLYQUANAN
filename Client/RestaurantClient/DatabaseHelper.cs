// Thêm các thư viện cần thiết
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

public class DatabaseHelper
{
	// =================================================================
	// PHẦN 1: THAY ĐỔI THEO CSDL CỦA BẠN
	// =================================================================

	// 1. Chuỗi kết nối (Connection String)
	private string connectionString = "Server=.\\SQLEXPRESS;Database=QLQuanAn;Integrated Security=True;";
	// (Hãy đảm bảo "Server=.\\SQLEXPRESS" là đúng với máy của bạn)

	// =================================================================
	// PHẦN 2: HÀM BẢO MẬT (Giữ nguyên)
	// =================================================================

	/// <summary>
	/// Băm mật khẩu bằng SHA-256.
	/// Kết quả là 64 ký tự Hex, vừa vặn với cột MatKhau (NVARCHAR(100)) của bạn.
	/// </summary>
	private string HashPassword(string password)
	{
		using (SHA256 sha256 = SHA256.Create())
		{
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
			byte[] hashBytes = sha256.ComputeHash(passwordBytes);
			StringBuilder sb = new StringBuilder();
			foreach (byte b in hashBytes)
			{
				sb.Append(b.ToString("x2"));
			}
			return sb.ToString();
		}
	}

	// =================================================================
	// PHẦN 3: LOGIC ĐĂNG KÝ (Đã cập nhật)
	// =================================================================

	/// <summary>
	/// Đăng ký người dùng mới vào bảng NGUOIDUNG.
	/// </summary>
	public bool RegisterUser(string username, string password, string fullName)
	{
		// 1. Băm mật khẩu
		string hashedPassword = HashPassword(password);

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			// 2. **** CẬP NHẬT CÂU LỆNH SQL ****
			// Đã đổi "INSERT INTO TaiKhoan..." thành "INSERT INTO NGUOIDUNG..."
			// Đã đổi các cột cho khớp: (TenDangNhap, MatKhau, HoTen)
			// Cột MatKhau của bạn sẽ lưu mật khẩu đã băm (hashedPassword)
			string query = "INSERT INTO NGUOIDUNG (TenDangNhap, MatKhau, HoTen) VALUES (@Username, @PasswordHash, @FullName)";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				// 3. Gán giá trị cho tham số
				command.Parameters.AddWithValue("@Username", username);
				command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
				command.Parameters.AddWithValue("@FullName", fullName);
				// Các cột khác (VaiTro, SDT, Email) của bạn cho phép NULL
				// nên chúng ta không cần chèn vào lúc đăng ký.
				// Các cột (TrangThai, NgayTao) đã có DEFAULT.

				try
				{
					connection.Open();
					int rowsAffected = command.ExecuteNonQuery();
					return rowsAffected > 0;
				}
				catch (SqlException ex)
				{
					// Lỗi 2627/2601 là lỗi UNIQUE (trùng TenDangNhap)
					if (ex.Number == 2627 || ex.Number == 2601)
					{
						MessageBox.Show("Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.", "Lỗi Đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
					{
						MessageBox.Show("Lỗi CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					return false;
				}
				catch (Exception ex)
				{
					MessageBox.Show("Lỗi không xác định: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}
		}
	}

	// =================================================================
	// PHẦN 4: LOGIC ĐĂNG NHẬP (Đã cập nhật)
	// =================================================================

	/// <summary>
	/// Xác thực (Login) người dùng từ bảng NGUOIDUNG.
	/// </summary>
	public bool LoginUser(string username, string password)
	{
		string storedHash = ""; // Lưu mật khẩu băm lấy từ CSDL

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			// 1. **** CẬP NHẬT CÂU LỆNH SQL ****
			// Đã đổi "SELECT MatKhauHash FROM TaiKhoan..." thành:
			// "SELECT MatKhau FROM NGUOIDUNG..."
			string query = "SELECT MatKhau FROM NGUOIDUNG WHERE TenDangNhap = @Username";

			using (SqlCommand command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Username", username);

				try
				{
					connection.Open();
					object result = command.ExecuteScalar();

					if (result != null && result != DBNull.Value)
					{
						storedHash = (string)result; // Lấy được mật khẩu băm
					}
					else
					{
						return false; // Không tìm thấy user
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Lỗi khi đăng nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
			}
		}

		// 2. Băm mật khẩu người dùng nhập
		string inputHash = HashPassword(password);

		// 3. So sánh
		return string.Equals(storedHash, inputHash, StringComparison.Ordinal);
	}
}