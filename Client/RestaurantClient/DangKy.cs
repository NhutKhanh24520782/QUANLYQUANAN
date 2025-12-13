using Models.Request;
using Models.Response;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class DangKy : Form
    {
        public DangKy()
        {
            InitializeComponent();
        }

        // ✅ SỬA: Thêm async
        private async void btn_dangky_Click(object sender, EventArgs e)
        {
            string username = tb_username.Text.Trim();
            string password = tb_passwd.Text.Trim();
            string confirm = tb_checkpasswd.Text.Trim();
            string fullname = tb_hoten.Text.Trim();
            string email = tb_email.Text.Trim();
            string role = radioButton_phucvu.Checked ? "PhucVu" :
                          radioButton_bep.Checked ? "Bep" : "";

            // Validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Mật khẩu không khớp!");
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự!");
                return;
            }

            // ✅ Disable button khi đang xử lý
            btn_dangky.Enabled = false;
            btn_dangky.Text = "Đang đăng ký...";

            try
            {
                var request = new RegisterRequest
                {
                    Username = username,
                    Password = password,
                    HoTen = fullname,
                    Email = email,
                    Role = role
                };

                // ✅ SỬA: Gọi async method
                string response = await SendRequestAsync(request);
                var registerResponse = JsonConvert.DeserializeObject<RegisterResponse>(response);

                if (registerResponse?.Success == true)
                {
                    MessageBox.Show("Đăng ký thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(registerResponse?.Message ?? "Đăng ký thất bại!",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btn_dangky.Enabled = true;
                btn_dangky.Text = "Đăng ký";
            }
        }

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
                    return response;
                }
            }
        }

        private void linkLabel_quaylai_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
        }

        private void tb_hoten_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton_phucvu_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void lbl_title_Click(object sender, EventArgs e)
        {

        }
    }
}