namespace RestaurantClient
{
    partial class DangNhap
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox_dangnhap = new GroupBox();
            tb_passwd = new TextBox();
            tb_username = new TextBox();
            lbl_passwd = new Label();
            lbl_username = new Label();
            label1 = new Label();
            label2 = new Label();
            linkLabel_dangky = new LinkLabel();
            btn_dangnhap = new Button();
            linkLabel_forgetpasswd = new LinkLabel();
            groupBox_dangnhap.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox_dangnhap
            // 
            groupBox_dangnhap.Controls.Add(tb_passwd);
            groupBox_dangnhap.Controls.Add(tb_username);
            groupBox_dangnhap.Controls.Add(lbl_passwd);
            groupBox_dangnhap.Controls.Add(lbl_username);
            groupBox_dangnhap.Location = new Point(12, 161);
            groupBox_dangnhap.Name = "groupBox_dangnhap";
            groupBox_dangnhap.Size = new Size(668, 172);
            groupBox_dangnhap.TabIndex = 0;
            groupBox_dangnhap.TabStop = false;
            groupBox_dangnhap.Text = "Thông tin đăng nhập";
            // 
            // tb_passwd
            // 
            tb_passwd.Location = new Point(183, 88);
            tb_passwd.Name = "tb_passwd";
            tb_passwd.Size = new Size(281, 27);
            tb_passwd.TabIndex = 3;
            // 
            // tb_username
            // 
            tb_username.Location = new Point(183, 39);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(281, 27);
            tb_username.TabIndex = 2;
            // 
            // lbl_passwd
            // 
            lbl_passwd.AutoSize = true;
            lbl_passwd.Location = new Point(43, 95);
            lbl_passwd.Name = "lbl_passwd";
            lbl_passwd.Size = new Size(70, 20);
            lbl_passwd.TabIndex = 1;
            lbl_passwd.Text = "Password";
            // 
            // lbl_username
            // 
            lbl_username.AutoSize = true;
            lbl_username.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_username.Location = new Point(43, 39);
            lbl_username.Name = "lbl_username";
            lbl_username.Size = new Size(91, 23);
            lbl_username.TabIndex = 0;
            lbl_username.Text = "Username:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(259, 30);
            label1.Name = "label1";
            label1.Size = new Size(153, 31);
            label1.TabIndex = 1;
            label1.Text = "ĐĂNG NHẬP";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(195, 415);
            label2.Name = "label2";
            label2.Size = new Size(267, 20);
            label2.TabIndex = 2;
            label2.Text = "Tạo tài khoản mới, hãy chọn Đăng ký";
            // 
            // linkLabel_dangky
            // 
            linkLabel_dangky.AutoSize = true;
            linkLabel_dangky.Location = new Point(287, 448);
            linkLabel_dangky.Name = "linkLabel_dangky";
            linkLabel_dangky.Size = new Size(63, 20);
            linkLabel_dangky.TabIndex = 3;
            linkLabel_dangky.TabStop = true;
            linkLabel_dangky.Text = "Đăng ký";
            // 
            // btn_dangnhap
            // 
            btn_dangnhap.AutoSize = true;
            btn_dangnhap.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_dangnhap.Location = new Point(259, 364);
            btn_dangnhap.Name = "btn_dangnhap";
            btn_dangnhap.Size = new Size(105, 33);
            btn_dangnhap.TabIndex = 4;
            btn_dangnhap.Text = "Đăng nhập";
            btn_dangnhap.UseVisualStyleBackColor = true;
            btn_dangnhap.Click += btn_dangnhap_Click;
            // 
            // linkLabel_forgetpasswd
            // 
            linkLabel_forgetpasswd.AutoSize = true;
            linkLabel_forgetpasswd.Location = new Point(100, 371);
            linkLabel_forgetpasswd.Name = "linkLabel_forgetpasswd";
            linkLabel_forgetpasswd.Size = new Size(116, 20);
            linkLabel_forgetpasswd.TabIndex = 5;
            linkLabel_forgetpasswd.TabStop = true;
            linkLabel_forgetpasswd.Text = "Quên mật khẩu?";
            // 
            // DangNhap
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Cornsilk;
            ClientSize = new Size(701, 506);
            Controls.Add(linkLabel_forgetpasswd);
            Controls.Add(btn_dangnhap);
            Controls.Add(linkLabel_dangky);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(groupBox_dangnhap);
            ForeColor = Color.DarkBlue;
            Name = "DangNhap";
            Text = "DangNhap";
         
            groupBox_dangnhap.ResumeLayout(false);
            groupBox_dangnhap.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox_dangnhap;
        private Label lbl_passwd;
        private Label lbl_username;
        private TextBox tb_passwd;
        private TextBox tb_username;
        private Label label1;
        private Label label2;
        private LinkLabel linkLabel_dangky;
        private Button btn_dangnhap;
        private LinkLabel linkLabel_forgetpasswd;
    }
}