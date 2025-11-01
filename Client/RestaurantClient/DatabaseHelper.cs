// Thêm các thư viện cần thiết
using System;
using System.Data;
using System.Data.SqlClient; // Thư viện chính để làm việc với SQL Server
using System.Security.Cryptography; // Thư viện để băm mật khẩu
using System.Text; // Thư viện để chuyển đổi chuỗi
using System.Windows.Forms; // Để dùng MessageBox

public class DatabaseHelper
{
	// =================================================================
	// PHẦN 1: CHUỖI KẾT NỐI
	// =================================================================

	// Tên CSDL đã được cập nhật thành "QLQuanAn"
	private string connectionString = "Server=.\\SQLEXPRESS;Database=QLQuanAn;Integrated Security=True;";
	// (Hãy đảm bảo "Server=.\\SQLEXPRESS" là đúng với máy của bạn)

	// =================================================================
	// PHẦN 2: HÀM BẢO MẬT (Hashing)
	// =================================================================

	/// <summary>
	/// Băm một chuỗi mật khẩu gốc (plain text) bằng thuật toán SHA-256.
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
				sb.Append(b.ToString("x2")); // "x2" là định dạng Hex
			}
			return sb.ToString();
		}
	}

	// =================================================================
	// PHẦN 3: LOGIC ĐĂNG KÝ (Đã cập nhật)
	// =================================================================

	/// <summary>
	/// Đăng ký người dùng mới vào CSDL (Phiên bản cập nhật cho Form mới)
	/// </summary>
	public bool RegisterUser(string username, string password, string fullName, string email, string vaiTro)
	{
		// 1. Băm mật khẩu
		string hashedPassword = HashPassword(password);

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			// 2. Cập nhật câu lệnh SQL INSERT
			// Thêm các cột Email và VaiTro cho khớp với CSDL và Form
			string query = "INSERT INTO NGUOIDUNG (TenDangNhap, MatKhau, HoTen, Email, VaiTro) 

							VALUES(@Username, @PasswordHash, @FullName, @Email, @VaiTro)";


			using (SqlCommand command = new SqlCommand(query, connection))
			{
				// 3. Gán giá trị cho các tham số (thêm @Email và @VaiTro)
				command.Parameters.AddWithValue("@Username", username);
				command.Parameters.AddWithValue("@PasswordHash", hashedPassword);
				command.Parameters.AddWithValue("@FullName", fullName);

				// Nếu email rỗng, truyền DBNull.Value để CSDL hiểu là NULL
				if (string.IsNullOrWhiteSpace(email))
				{
					command.Parameters.AddWithValue("@Email", DBNull.Value);
				}
				else
				{
					command.Parameters.AddWithValue("@Email", email);
				}

				command.Parameters.AddWithValue("@VaiTro", vaiTro);

				// 4. Phần thực thi (Giữ nguyên)
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
	// PHẦN 4: LOGIC ĐĂNG NHẬP (Không đổi)
	// =================================================================

	/// <summary>
	/// Xác thực (Login) người dùng từ bảng NGUOIDUNG.
	/// </summary>
	public bool LoginUser(string username, string password)
	{
		string storedHash = ""; // Lưu mật khẩu băm lấy từ CSDL

		using (SqlConnection connection = new SqlConnection(connectionString))
		{
			// Câu lệnh SQL khớp với CSDL của bạn
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