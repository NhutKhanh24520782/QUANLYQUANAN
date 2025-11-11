using System;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class DangNhap : Form
    {
        DatabaseHelper db = new DatabaseHelper();

        public DangNhap()
        {
            InitializeComponent();
            tb_passwd.PasswordChar = '●';   // Ẩn mật khẩu
        }

        private void btn_dangnhap_Click(object sender, EventArgs e)
        {
            string username = tb_username.Text.Trim();
            string password = tb_passwd.Text.Trim();

            // Kiểm tra nhập đầy đủ
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!",
                    "Thiếu thông tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Gọi DatabaseHelper để kiểm tra login
            bool loginSuccess = db.LoginUser(username, password);

            if (loginSuccess)
            {
                MessageBox.Show(
                    "Đăng nhập thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // ✅ Ẩn form đăng nhập → CHẠY NỀN, KHÔNG THOÁT APP
                this.Hide();
            }
            else
            {
                MessageBox.Show(
                    "Sai tên đăng nhập hoặc mật khẩu!",
                    "Đăng nhập thất bại",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void linkLabel_dangky_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();

            // 2. Tạo và hiển thị form đăng ký dưới dạng modal
            using (DangKy dk = new DangKy())
            {
                dk.ShowDialog();  // Khi đóng form đăng ký, dòng này mới kết thúc
            }

            // 3. Hiện lại form đăng nhập
            this.Show();
        }

        private void linkLabel_forgetpasswd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();

            using (NhapEmail frm = new NhapEmail())
            {
                frm.ShowDialog();
            }

            this.Show();
        }

        private void DangNhap_Load(object sender, EventArgs e)
        {

        }
    }
}
