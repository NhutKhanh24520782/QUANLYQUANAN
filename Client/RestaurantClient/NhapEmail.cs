using Models.Request;
using Models.Response;
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
        // Sử dụng method SendRequestAsync đã có trong DangNhap.cs
        private async Task<bool> CheckEmailInDatabaseAsync(string email)
        {
            try
            {
                var request = new CheckEmailRequest { Email = email };
                string response = await SendRequestAsync(request);
                var checkResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckEmailResponse>(response);

                if (checkResponse == null)
                {
                    MessageBox.Show("Lỗi parse response từ server", "Lỗi");
                    return false;
                }

                // ✅ DEBUG
                Console.WriteLine($"📧 Client Response - Success: {checkResponse.Success}, Exists: {checkResponse.Exists}, Message: {checkResponse.Message}");

                if (!checkResponse.Success)
                {
                    MessageBox.Show(checkResponse.Message, "Lỗi kiểm tra email");
                    return false;
                }

                return checkResponse.Exists;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi");
                return false;
            }
        }

        // ✅ Thêm method SendRequestAsync vào NhapEmail.cs
        private async Task<string> SendRequestAsync<T>(T data)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data) + "\n";

            using (TcpClient client = new TcpClient())
            {
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                await client.ConnectAsync("127.0.0.1", 5000);

                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(json.TrimEnd('\n'));
                    string response = await reader.ReadLineAsync();
                    return response ?? "";
                }
            }
        }

    }
}
