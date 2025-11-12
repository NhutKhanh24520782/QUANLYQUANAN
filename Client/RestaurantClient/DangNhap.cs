using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
            tb_passwd.PasswordChar = '●';
        }

        private void btn_dangnhap_Click(object sender, EventArgs e)
        {
            string username = tb_username.Text.Trim();
            string password = tb_passwd.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!");
                return;
            }

            var request = new
            {
                Type = "Login",
                Username = username,
                Password = password
            };

            string response = SendRequest(request);

            if (response.Contains("\"Success\":true"))
            {
                MessageBox.Show("Đăng nhập thành công!");
                this.Hide();
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!");
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
        private void linkLabel_dangky_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void linkLabel_dangky_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Ẩn tạm Form đăng nhập
            this.Hide();

            // Tạo instance Form đăng ký
            DangKy frmDangKy = new DangKy();

            // Khi Form đăng ký đóng, hiện lại Form đăng nhập
            frmDangKy.FormClosed += (s, args) => this.Show();

            // Mở Form đăng ký
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