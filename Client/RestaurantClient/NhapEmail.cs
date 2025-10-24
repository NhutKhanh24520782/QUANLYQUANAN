using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
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
                MessageBox.Show("Vui lòng nhập Email ", "Thiếu thông tin");
                return;
            }

            if (IsValidGmail.GmailCheck(tb_email.Text) == false)
            {
                MessageBox.Show("Vui lòng nhập đúng định dạng Email ", "Sai định dạng");
                return;
            }



            

                MessageBox.Show($"Mã xác nhận đã được gửi tới địa chỉ: {currentEmail}", "Kiểm tra Email", MessageBoxButtons.OK, MessageBoxIcon.Information);



                OTP verifyForm = new OTP(currentEmail);
                verifyForm.Show();
                this.Hide();
            

        }
    }
}
