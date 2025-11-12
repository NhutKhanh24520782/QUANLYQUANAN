using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RestaurantClient
{
    public partial class MatKhauMoi : Form
    {
        private string userEmail;
        public MatKhauMoi(string email)
        {
            InitializeComponent();
            this.userEmail = email;
        }
        private string SendRequest(object data)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            using (var client = new System.Net.Sockets.TcpClient("127.0.0.1", 5000)) // đổi IP nếu server ở xa
            {
                var stream = client.GetStream();
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

        private void btn_hoanTat_Click(object sender, EventArgs e)
        {
            string newPass = tb_newPass.Text;
            string confirmPass = tb_confirmPass.Text;

            if (newPass != confirmPass)
            {
                MessageBox.Show("Mật khẩu mới và xác nhận mật khẩu không khớp.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(newPass))
            {
                MessageBox.Show("Mật khẩu không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var request = new
                {
                    Type = "UpdatePassword",
                    Email = userEmail,
                    NewPassword = newPass
                };

                string response = SendRequest(request);
                dynamic res = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

                if (res.Success == true)
                {
                    MessageBox.Show("Đã đổi mật khẩu thành công", "Chúc mừng!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    DangNhap loginForm = new DangNhap();
                    loginForm.Show();
                }
                else
                {
                    MessageBox.Show("Đổi mật khẩu thất bại. Kiểm tra email hoặc kết nối.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi mạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MatKhauMoi_Load(object sender, EventArgs e)
        {

        }
    }
}
