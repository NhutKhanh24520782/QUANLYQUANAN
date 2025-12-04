using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Models.Request;
using Models.Response;

namespace RestaurantClient
{
    public partial class MatKhauMoi : Form
    {
        private string userEmail;

        public MatKhauMoi(string email)
        {
            InitializeComponent();
            this.userEmail = email;
            SetupUI();
        }

        private void SetupUI()
        {
            // Setup password fields
            tb_newPass.PasswordChar = '●';
            tb_confirmPass.PasswordChar = '●';

            // Hiển thị email đang được đổi mật khẩu
            this.Text = $"Đặt lại mật khẩu - {userEmail}";
        }

        private async void btn_hoanTat_Click(object sender, EventArgs e)
        {
            string newPass = tb_newPass.Text.Trim();
            string confirmPass = tb_confirmPass.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(newPass))
            {
                MessageBox.Show("Mật khẩu không được để trống.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                tb_newPass.Focus();
                return;
            }

            if (newPass.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                tb_newPass.Focus();
                return;
            }

            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                tb_confirmPass.Focus();
                return;
            }

            // Disable button during processing
            btn_hoanTat.Enabled = false;
            btn_hoanTat.Text = "Đang xử lý...";

            try
            {
                var request = new UpdatePasswordRequest
                {
                    Email = userEmail,
                    NewPassword = newPass
                };

                string response = await SendRequestAsync(request);
                var updateResponse = JsonConvert.DeserializeObject<UpdatePasswordResponse>(response);

                if (updateResponse?.Success == true)
                {
                    MessageBox.Show("Đã đổi mật khẩu thành công! Vui lòng đăng nhập lại.", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                    DangNhap loginForm = new DangNhap();
                    loginForm.Show();
                }
                else
                {
                    MessageBox.Show(updateResponse?.Message ?? "Đổi mật khẩu thất bại", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable button
                btn_hoanTat.Enabled = true;
                btn_hoanTat.Text = "Hoàn tất";
            }
        }

        // ✅ Async method với proper error handling
        private async Task<string> SendRequestAsync<T>(T data)
        {
            string json = JsonConvert.SerializeObject(data) + "\n";

            using (TcpClient client = new TcpClient())
            {
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                await client.ConnectAsync("127.0.0.1", 5000);

                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // ✅ Gửi request
                    await writer.WriteLineAsync(json.TrimEnd('\n'));

                    // ✅ Nhận response
                    string response = await reader.ReadLineAsync();
                    return response ?? "";
                }
            }
        }

        private void MatKhauMoi_Load(object sender, EventArgs e)
        {
            // Focus vào ô nhập mật khẩu mới
            tb_newPass.Focus();
        }

        // ✅ Thêm event handlers cho Enter key
        private void tb_newPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                tb_confirmPass.Focus();
                e.Handled = true;
            }
        }

        private void tb_confirmPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btn_hoanTat.PerformClick();
                e.Handled = true;
            }
        }

        // ✅ Thêm nút Hủy (nếu muốn)
        private void btn_huy_Click(object sender, EventArgs e)
        {
            this.Close();
            new DangNhap().Show();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}