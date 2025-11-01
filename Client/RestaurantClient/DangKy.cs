using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class DangKy : Form
    {
        // 1. Khai báo "nhân viên" helper ở đây
        private DatabaseHelper dbHelper;

        public DangKy()
        {
            InitializeComponent();
            dbHelper = new DatabaseHelper(); // Khởi tạo "nhân viên"
        }

        private void DangKy_Load(object sender, EventArgs e)
        {

        }

        private void lbl_checkpasswd_Click(object sender, EventArgs e)
        {

        }
        private void btn_dangky_Click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu từ các control trên Form
            string username = tb_username.Text.Trim();
            string password = tb_passwod.Text;
            string checkPassword = tb_checkpasswd.Text;
            string fullName = tb_hoten.Text.Trim();
            string email = tb_email.Text.Trim();
            string vaiTro = ""; // Biến để lưu vai trò

            // 2. Kiểm tra xem RadioButton nào được chọn
            if (radioButton_phucvu.Checked)
            {
                // Phải khớp với CSDL (N'PhucVu')
                vaiTro = "PhucVu";
            }
            else if (radioButton_bep.Checked)
            {
                // Phải khớp với CSDL (N'Bep')
                vaiTro = "Bep";
            }

            // 3. Kiểm tra dữ liệu đầu vào (Validation)
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên đăng nhập, Mật khẩu và Họ tên.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Dừng thực thi
            }

            if (password != checkPassword)
            {
                MessageBox.Show("Mật khẩu và mật khẩu xác nhận không khớp.", "Lỗi Mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Dừng thực thi
            }

            if (string.IsNullOrWhiteSpace(vaiTro))
            {
                MessageBox.Show("Vui lòng chọn vai trò (Phục vụ hoặc Bếp).", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Dừng thực thi
            }

            // 4. Gọi hàm đăng ký (đã cập nhật trong DatabaseHelper)
            try
            {
                bool success = dbHelper.RegisterUser(username, password, fullName, email, vaiTro);

                if (success)
                {
                    MessageBox.Show("Đăng ký tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // Đóng form đăng ký lại
                }
                // (Nếu thất bại, dbHelper sẽ tự hiển thị lỗi)
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi ngoài ý muốn: " + ex.Message, "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. ĐIỀN LOGIC CHO NÚT QUAY LẠI
        private void linkLabel_quaylai_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Đơn giản là đóng Form này lại
            this.Close();
        }
    }
}