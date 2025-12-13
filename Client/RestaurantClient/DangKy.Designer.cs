namespace RestaurantClient
{
    partial class DangKy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DangKy));
            lbl_title = new Label();
            lbl_hoten = new Label();
            lbl_passwd = new Label();
            lbl_loainv = new Label();
            lbl_username = new Label();
            lbl_checkpasswd = new Label();
            tb_hoten = new TextBox();
            tb_checkpasswd = new TextBox();
            tb_username = new TextBox();
            radioButton_phucvu = new RadioButton();
            radioButton_bep = new RadioButton();
            btn_dangky = new Button();
            linkLabel_quaylai = new LinkLabel();
            label1 = new Label();
            tb_email = new TextBox();
            label2 = new Label();
            tb_passwd = new TextBox();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lbl_title
            // 
            lbl_title.AutoSize = true;
            lbl_title.BackColor = Color.Transparent;
            lbl_title.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl_title.ForeColor = Color.FromArgb(101, 67, 33);
            lbl_title.Location = new Point(50, 30);
            lbl_title.Name = "lbl_title";
            lbl_title.Size = new Size(292, 37);
            lbl_title.TabIndex = 0;
            lbl_title.Text = "ĐĂNG KÝ TÀI KHOẢN";
            lbl_title.Click += lbl_title_Click;
            // 
            // lbl_hoten
            // 
            lbl_hoten.AutoSize = true;
            lbl_hoten.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_hoten.ForeColor = Color.FromArgb(101, 67, 33);
            lbl_hoten.Location = new Point(50, 90);
            lbl_hoten.Name = "lbl_hoten";
            lbl_hoten.Size = new Size(84, 23);
            lbl_hoten.TabIndex = 1;
            lbl_hoten.Text = "Họ và tên";
            // 
            // lbl_passwd
            // 
            lbl_passwd.AutoSize = true;
            lbl_passwd.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_passwd.ForeColor = Color.FromArgb(101, 67, 33);
            lbl_passwd.Location = new Point(50, 350);
            lbl_passwd.Name = "lbl_passwd";
            lbl_passwd.Size = new Size(82, 23);
            lbl_passwd.TabIndex = 2;
            lbl_passwd.Text = "Mật khẩu";
            // 
            // lbl_loainv
            // 
            lbl_loainv.AutoSize = true;
            lbl_loainv.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_loainv.ForeColor = Color.FromArgb(101, 67, 33);
            lbl_loainv.Location = new Point(50, 285);
            lbl_loainv.Name = "lbl_loainv";
            lbl_loainv.Size = new Size(121, 23);
            lbl_loainv.TabIndex = 3;
            lbl_loainv.Text = "Loại nhân viên";
            // 
            // lbl_username
            // 
            lbl_username.AutoSize = true;
            lbl_username.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_username.ForeColor = Color.FromArgb(101, 67, 33);
            lbl_username.Location = new Point(50, 155);
            lbl_username.Name = "lbl_username";
            lbl_username.Size = new Size(112, 23);
            lbl_username.TabIndex = 4;
            lbl_username.Text = "Tên tài khoản";
            // 
            // lbl_checkpasswd
            // 
            lbl_checkpasswd.AutoSize = true;
            lbl_checkpasswd.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_checkpasswd.ForeColor = Color.FromArgb(101, 67, 33);
            lbl_checkpasswd.Location = new Point(50, 415);
            lbl_checkpasswd.Name = "lbl_checkpasswd";
            lbl_checkpasswd.Size = new Size(158, 23);
            lbl_checkpasswd.TabIndex = 5;
            lbl_checkpasswd.Text = "Xác nhận mật khẩu";
            // 
            // tb_hoten
            // 
            tb_hoten.BackColor = Color.FromArgb(255, 248, 240);
            tb_hoten.BorderStyle = BorderStyle.FixedSingle;
            tb_hoten.Font = new Font("Segoe UI", 10F);
            tb_hoten.ForeColor = Color.FromArgb(101, 67, 33);
            tb_hoten.Location = new Point(50, 116);
            tb_hoten.Name = "tb_hoten";
            tb_hoten.Size = new Size(400, 30);
            tb_hoten.TabIndex = 6;
            tb_hoten.TextChanged += tb_hoten_TextChanged;
            // 
            // tb_checkpasswd
            // 
            tb_checkpasswd.BackColor = Color.FromArgb(255, 248, 240);
            tb_checkpasswd.BorderStyle = BorderStyle.FixedSingle;
            tb_checkpasswd.Font = new Font("Segoe UI", 10F);
            tb_checkpasswd.ForeColor = Color.FromArgb(101, 67, 33);
            tb_checkpasswd.Location = new Point(50, 441);
            tb_checkpasswd.Name = "tb_checkpasswd";
            tb_checkpasswd.PasswordChar = '●';
            tb_checkpasswd.Size = new Size(400, 30);
            tb_checkpasswd.TabIndex = 8;
            // 
            // tb_username
            // 
            tb_username.BackColor = Color.FromArgb(255, 248, 240);
            tb_username.BorderStyle = BorderStyle.FixedSingle;
            tb_username.Font = new Font("Segoe UI", 10F);
            tb_username.ForeColor = Color.FromArgb(101, 67, 33);
            tb_username.Location = new Point(50, 181);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(400, 30);
            tb_username.TabIndex = 10;
            // 
            // radioButton_phucvu
            // 
            radioButton_phucvu.AutoSize = true;
            radioButton_phucvu.Font = new Font("Segoe UI", 10F);
            radioButton_phucvu.ForeColor = Color.FromArgb(101, 67, 33);
            radioButton_phucvu.Location = new Point(200, 285);
            radioButton_phucvu.Name = "radioButton_phucvu";
            radioButton_phucvu.Size = new Size(92, 27);
            radioButton_phucvu.TabIndex = 11;
            radioButton_phucvu.TabStop = true;
            radioButton_phucvu.Text = "Phục vụ";
            radioButton_phucvu.UseVisualStyleBackColor = true;
            radioButton_phucvu.CheckedChanged += radioButton_phucvu_CheckedChanged;
            // 
            // radioButton_bep
            // 
            radioButton_bep.AutoSize = true;
            radioButton_bep.Font = new Font("Segoe UI", 10F);
            radioButton_bep.ForeColor = Color.FromArgb(101, 67, 33);
            radioButton_bep.Location = new Point(320, 285);
            radioButton_bep.Name = "radioButton_bep";
            radioButton_bep.Size = new Size(60, 27);
            radioButton_bep.TabIndex = 12;
            radioButton_bep.TabStop = true;
            radioButton_bep.Text = "Bếp";
            radioButton_bep.UseVisualStyleBackColor = true;
            // 
            // btn_dangky
            // 
            btn_dangky.BackColor = Color.FromArgb(139, 90, 43);
            btn_dangky.Cursor = Cursors.Hand;
            btn_dangky.FlatAppearance.BorderSize = 0;
            btn_dangky.FlatStyle = FlatStyle.Flat;
            btn_dangky.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_dangky.ForeColor = Color.White;
            btn_dangky.Location = new Point(50, 495);
            btn_dangky.Name = "btn_dangky";
            btn_dangky.Size = new Size(400, 45);
            btn_dangky.TabIndex = 13;
            btn_dangky.Text = "ĐĂNG KÝ";
            btn_dangky.UseVisualStyleBackColor = false;
            btn_dangky.Click += btn_dangky_Click;
            // 
            // linkLabel_quaylai
            // 
            linkLabel_quaylai.AutoSize = true;
            linkLabel_quaylai.BackColor = Color.Transparent;
            linkLabel_quaylai.Font = new Font("Segoe UI", 9.5F);
            linkLabel_quaylai.LinkColor = Color.FromArgb(139, 90, 43);
            linkLabel_quaylai.Location = new Point(280, 565);
            linkLabel_quaylai.Name = "linkLabel_quaylai";
            linkLabel_quaylai.Size = new Size(67, 21);
            linkLabel_quaylai.TabIndex = 14;
            linkLabel_quaylai.TabStop = true;
            linkLabel_quaylai.Text = "Quay lại";
            linkLabel_quaylai.LinkClicked += linkLabel_quaylai_LinkClicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(101, 67, 33);
            label1.Location = new Point(135, 565);
            label1.Name = "label1";
            label1.Size = new Size(152, 21);
            label1.TabIndex = 15;
            label1.Text = "Bạn đã có tài khoản?";
            // 
            // tb_email
            // 
            tb_email.BackColor = Color.FromArgb(255, 248, 240);
            tb_email.BorderStyle = BorderStyle.FixedSingle;
            tb_email.Font = new Font("Segoe UI", 10F);
            tb_email.ForeColor = Color.FromArgb(101, 67, 33);
            tb_email.Location = new Point(50, 246);
            tb_email.Name = "tb_email";
            tb_email.Size = new Size(400, 30);
            tb_email.TabIndex = 17;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(101, 67, 33);
            label2.Location = new Point(50, 220);
            label2.Name = "label2";
            label2.Size = new Size(51, 23);
            label2.TabIndex = 16;
            label2.Text = "Email";
            label2.Click += label2_Click;
            // 
            // tb_passwd
            // 
            tb_passwd.BackColor = Color.FromArgb(255, 248, 240);
            tb_passwd.BorderStyle = BorderStyle.FixedSingle;
            tb_passwd.Font = new Font("Segoe UI", 10F);
            tb_passwd.ForeColor = Color.FromArgb(101, 67, 33);
            tb_passwd.Location = new Point(50, 376);
            tb_passwd.Name = "tb_passwd";
            tb_passwd.PasswordChar = '●';
            tb_passwd.Size = new Size(400, 30);
            tb_passwd.TabIndex = 7;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(250, 240, 230);
            panel1.Controls.Add(tb_hoten);
            panel1.Controls.Add(tb_checkpasswd);
            panel1.Controls.Add(lbl_title);
            panel1.Controls.Add(linkLabel_quaylai);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(tb_passwd);
            panel1.Controls.Add(lbl_checkpasswd);
            panel1.Controls.Add(btn_dangky);
            panel1.Controls.Add(lbl_loainv);
            panel1.Controls.Add(lbl_passwd);
            panel1.Controls.Add(radioButton_phucvu);
            panel1.Controls.Add(radioButton_bep);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(tb_email);
            panel1.Controls.Add(tb_username);
            panel1.Controls.Add(lbl_hoten);
            panel1.Controls.Add(lbl_username);
            panel1.Location = new Point(252, 10);
            panel1.Name = "panel1";
            panel1.Size = new Size(500, 610);
            panel1.TabIndex = 19;
            // 
            // DangKy
            // 
            AcceptButton = btn_dangky;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.BurlyWood;
            ClientSize = new Size(1004, 630);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DangKy";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Smartdine - Đăng ký";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lbl_title;
        private Label lbl_hoten;
        private Label lbl_passwd;
        private Label lbl_loainv;
        private Label lbl_username;
        private Label lbl_checkpasswd;
        private TextBox tb_hoten;
        private TextBox tb_checkpasswd;
        private TextBox tb_username;
        private RadioButton radioButton_phucvu;
        private RadioButton radioButton_bep;
        private Button btn_dangky;
        private LinkLabel linkLabel_quaylai;
        private Label label1;
        private TextBox tb_email;
        private Label label2;
        private TextBox tb_passwd;
        private Panel panel1;
    }
}