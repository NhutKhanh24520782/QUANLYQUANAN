using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Forms;
using Models.Request;
using Models.Response;

namespace RestaurantClient
{

    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
            tb_passwd.PasswordChar = '●';
        }

        private async void btn_dangnhap_Click(object sender, EventArgs e)
        {
            var request = new LoginRequest
            {
                Username = tb_username.Text.Trim(),
                Password = tb_passwd.Text.Trim()
            };

            var validation = request.Validate();
            if (!validation.isValid)
            {
                MessageBox.Show(validation.error, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string response = await SendRequestAsync(request);
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response);

                if (loginResponse?.Success == true)
                {
                    MessageBox.Show($"Đăng nhập thành công! Chào {loginResponse.HoTen}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Hide();

                    // Điều hướng theo role
                    switch (loginResponse.Role)
                    {
                        case "Admin":
                            new Admin().Show();
                            break;
                        case "PhucVu":
                            new NVPhucVu(loginResponse.MaNguoiDung, loginResponse.HoTen).Show();
                            break;
                        case "Bep":
                            new NVBep().Show();
                            break;
                        default:
                            MessageBox.Show($"Vai trò không xác định: {loginResponse.Role}");
                            this.Show();
                            break;
                    }
                }
                else
                {
                    MessageBox.Show(loginResponse?.Message ?? "Đăng nhập thất bại",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                NetworkStream stream = client.GetStream();

                byte[] sendData = Encoding.UTF8.GetBytes(json);
                await stream.WriteAsync(sendData, 0, sendData.Length);

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return await reader.ReadLineAsync();
                }
            }
        }
        private void linkLabel_dangky_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void linkLabel_dangky_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            DangKy frmDangKy = new DangKy();
            frmDangKy.FormClosed += (s, args) => this.Show();
            frmDangKy.Show();
        }

        private void linkLabel_forgetpasswd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            NhapEmail nhapEmail = new NhapEmail();
            nhapEmail.FormClosed += (s, args) => this.Show();
            nhapEmail.Show();
        }
    }
}