using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class OTP : Form
    {
        private string userEmail;
        private string randomOTP;
        private System.Windows.Forms.Timer resendTimer;
        private int secondsRemaining;
        public OTP(string email)
        {
            InitializeComponent();
            this.userEmail = email;

            // Khởi tạo Timer (Cần được thực hiện tại đây)
            resendTimer = new System.Windows.Forms.Timer();
            resendTimer.Interval = 1000;
            resendTimer.Tick += ResendTimer_Tick;

            // Gửi OTP ngay khi Form mở
            SendOTP(this.userEmail);
        }

        private void SendOTP(string toEmail)
        {
            Random rand = new Random();


            randomOTP = rand.Next(10000, 99999).ToString();

            MailMessage message = new MailMessage();
            string from = "nguyenphatnvp26@gmail.com";
            string pass = " wekowpgedagktehq";

            message.Body = "Your OTP is: " + randomOTP;
            message.To.Add(toEmail);
            message.From = new MailAddress(from);
            message.Subject = "Password reseting OTP";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(from, pass);

            try
            {
                smtp.Send(message);
                MessageBox.Show("Mã OTP đã được gửi thành công đến " + toEmail, "Gửi thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                StartResendTimer(30);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể gửi mã OTP. Lỗi: " + ex.Message, "Lỗi Gửi Mail", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            llResendCode.Enabled = false;
            llResendCode.Text = "Sending new code...";

            SendOTP(this.userEmail);

            string userEmail = "email của bạn";
            MessageBox.Show("Mã xác nhận mới đã được gửi tới " + userEmail + ". Vui lòng kiểm tra hộp thư !");

        }

        private void btnXacNhanOTP_Click(object sender, EventArgs e) // Sự kiện click của Button "Xác nhận OTP"
        {
            string inputOTP = tb_otp.Text.Trim(); // Giả sử TextBox là txtOTP

            if (inputOTP == randomOTP)
            {
                MessageBox.Show("Xác nhận thành công! Mời bạn tạo mật khẩu mới.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Chuyển sang Form Tạo Mật khẩu mới và truyền Email
                MatKhauMoi newPassForm = new MatKhauMoi(this.userEmail);
                newPassForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Mã OTP không đúng. Vui lòng kiểm tra lại.", "Lỗi Xác Nhận", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResendTimer_Tick(object sender, EventArgs e)
        {
            if (secondsRemaining > 0)
            {
                secondsRemaining--;
                // Cập nhật Label hiển thị thời gian
                lblTimer.Text = $"Gửi lại sau: {secondsRemaining}s";
                llResendCode.Enabled = false; // Vẫn vô hiệu hóa nút gửi lại
            }
            else
            {
                resendTimer.Stop();
                lblTimer.Text = "";
                llResendCode.Text = "Gửi lại mã";
                llResendCode.Enabled = true; // Kích hoạt nút gửi lại
            }
        }

        private void StartResendTimer(int seconds)
        {
            secondsRemaining = seconds;
            llResendCode.Enabled = false;
            llResendCode.Text = "Đang đếm ngược...";

            // Đảm bảo Label Timer hiển thị thông báo ban đầu
            lblTimer.Text = $"Gửi lại sau: {secondsRemaining}s";

            resendTimer.Start();
        }
    }
}
