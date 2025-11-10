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
                DatabaseHelper dbHelper = new DatabaseHelper();
                bool updated = dbHelper.UpdatePassword(userEmail, newPass); // Hàm mới bạn sẽ thêm

                if (updated)
                {
                    MessageBox.Show("Đã đổi mật khẩu thành công", "Chúc mừng!", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Đổi mật khẩu thất bại. Kiểm tra email hoặc kết nối.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MatKhauMoi_Load(object sender, EventArgs e)
        {

        }
    }
}
