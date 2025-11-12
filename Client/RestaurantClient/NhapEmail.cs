using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class NhapEmail : Form
    {
        public NhapEmail()
        {
            InitializeComponent();
        }

        private async void btn_send_Click(object sender, EventArgs e)
        {
            string currentEmail = tb_email.Text.Trim();

            if (string.IsNullOrEmpty(currentEmail))
            {
                MessageBox.Show("Vui lòng nhập Email", "Thiếu thông tin");
                return;
            }

            if (!IsValidGmail.GmailCheck(currentEmail))
            {
                MessageBox.Show("Vui lòng nhập đúng định dạng Email", "Sai định dạng");
                return;
            }

            // 🔹 Kiểm tra email có tồn tại trong database
            bool emailExists = await CheckEmailInDatabaseAsync(currentEmail);
            if (!emailExists)
            {
                MessageBox.Show("Email này chưa đăng ký tài khoản.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 🔹 Nếu có trong database, gửi OTP
            MessageBox.Show($"Mã xác nhận đã được gửi tới địa chỉ: {currentEmail}", "Kiểm tra Email", MessageBoxButtons.OK, MessageBoxIcon.Information);

            OTP verifyForm = new OTP(currentEmail);
            verifyForm.Show();
            this.Hide();
        }

        // 🔹 Gọi server kiểm tra email (giả sử bạn có TCP client)
        private Task<bool> CheckEmailInDatabaseAsync(string email)
        {
            return Task.Run(() =>
            {
                try
                {
                    // Ví dụ dùng TCP gửi request server
                    using (TcpClient client = new TcpClient("127.0.0.1", 5000))
                    using (NetworkStream stream = client.GetStream())
                    {
                        // Gửi request dạng JSON
                        string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(new
                        {
                            Type = "CheckEmail",
                            Email = email
                        });
                        byte[] data = Encoding.UTF8.GetBytes(requestJson);
                        stream.Write(data, 0, data.Length);

                        // Nhận phản hồi
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        dynamic response = Newtonsoft.Json.JsonConvert.DeserializeObject(responseJson);
                        return (bool)response.Exists;
                    }
                }
                catch
                {
                    return false;
                }
            });
        }

    }
}
