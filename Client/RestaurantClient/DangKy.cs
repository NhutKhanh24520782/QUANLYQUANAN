using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class DangKy : Form
    {
        public DangKy()
        {
            InitializeComponent();
        }

        private void btn_dangky_Click(object sender, EventArgs e)
        {
            string username = tb_username.Text.Trim();
            string password = tb_passwd.Text.Trim();
            string confirm = tb_checkpasswd.Text.Trim();
            string fullname = tb_hoten.Text.Trim();
            string email = tb_email.Text.Trim();
            string role = radioButton_phucvu.Checked ? "PhucVu" :
                          radioButton_bep.Checked ? "Bep" : "";

            if (password != confirm)
            {
                MessageBox.Show("Mật khẩu không khớp!");
                return;
            }

            var request = new
            {
                Type = "Register",
                Username = username,
                Password = password,
                FullName = fullname,
                Email = email,
                Role = role
            };

            string response = SendRequest(request);

            if (response.Contains("\"Success\":true"))
            {
                MessageBox.Show("Đăng ký thành công!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Đăng ký thất bại! Tài khoản hoặc email đã tồn tại");
            }
        }

        private string SendRequest(object data)
        {
            string json = JsonConvert.SerializeObject(data);
            using (TcpClient client = new TcpClient("127.0.0.1", 5000))
            {
                NetworkStream stream = client.GetStream();
                byte[] sendData = Encoding.UTF8.GetBytes(json);
                stream.Write(sendData, 0, sendData.Length);

                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                stream.Close();
                client.Close();
                return response;
            }
        }

        private void linkLabel_quaylai_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();

            // Tạo instance Form đăng ký
            DangNhap frmDangNhap = new DangNhap();

            // Khi Form đăng ký đóng, hiện lại Form đăng nhập
            frmDangNhap.FormClosed += (s, args) => this.Show();

            // Mở Form đăng ký
            frmDangNhap.Show();
        }
    }
}