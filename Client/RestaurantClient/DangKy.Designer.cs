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
            lbl_title.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl_title.ForeColor = Color.FromArgb(64, 64, 64);
            lbl_title.Location = new Point(59, 14);
            lbl_title.Name = "lbl_title";
            lbl_title.Size = new Size(341, 28);
            lbl_title.TabIndex = 0;
            lbl_title.Text = "ĐĂNG KÝ TÀI KHOẢN SMARTDINE";
            lbl_title.Click += lbl_title_Click;
            // 
            // lbl_hoten
            // 
            lbl_hoten.AutoSize = true;
            lbl_hoten.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_hoten.Location = new Point(42, 64);
            lbl_hoten.Name = "lbl_hoten";
            lbl_hoten.Size = new Size(88, 23);
            lbl_hoten.TabIndex = 1;
            lbl_hoten.Text = "Họ và tên:";
            // 
            // lbl_passwd
            // 
            lbl_passwd.AutoSize = true;
            lbl_passwd.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_passwd.Location = new Point(42, 307);
            lbl_passwd.Name = "lbl_passwd";
            lbl_passwd.Size = new Size(84, 23);
            lbl_passwd.TabIndex = 2;
            lbl_passwd.Text = "Password:";
            // 
            // lbl_loainv
            // 
            lbl_loainv.AutoSize = true;
            lbl_loainv.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_loainv.Location = new Point(41, 266);
            lbl_loainv.Name = "lbl_loainv";
            lbl_loainv.Size = new Size(125, 23);
            lbl_loainv.TabIndex = 3;
            lbl_loainv.Text = "Loại nhân viên:";
            // 
            // lbl_username
            // 
            lbl_username.AutoSize = true;
            lbl_username.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_username.Location = new Point(42, 120);
            lbl_username.Name = "lbl_username";
            lbl_username.Size = new Size(91, 23);
            lbl_username.TabIndex = 4;
            lbl_username.Text = "Username:";
            // 
            // lbl_checkpasswd
            // 
            lbl_checkpasswd.AutoSize = true;
            lbl_checkpasswd.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_checkpasswd.Location = new Point(41, 376);
            lbl_checkpasswd.Name = "lbl_checkpasswd";
            lbl_checkpasswd.Size = new Size(161, 23);
            lbl_checkpasswd.TabIndex = 5;
            lbl_checkpasswd.Text = "Xác nhận password:";
            // 
            // tb_hoten
            // 
            tb_hoten.BackColor = Color.White;
            tb_hoten.Location = new Point(42, 90);
            tb_hoten.Name = "tb_hoten";
            tb_hoten.Size = new Size(378, 27);
            tb_hoten.TabIndex = 6;
            tb_hoten.TextChanged += tb_hoten_TextChanged;
            // 
            // tb_checkpasswd
            // 
            tb_checkpasswd.BackColor = Color.White;
            tb_checkpasswd.Location = new Point(41, 402);
            tb_checkpasswd.Name = "tb_checkpasswd";
            tb_checkpasswd.Size = new Size(379, 27);
            tb_checkpasswd.TabIndex = 8;
            // 
            // tb_username
            // 
            tb_username.BackColor = Color.White;
            tb_username.Location = new Point(42, 146);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(378, 27);
            tb_username.TabIndex = 10;
            // 
            // radioButton_phucvu
            // 
            radioButton_phucvu.AutoSize = true;
            radioButton_phucvu.Location = new Point(188, 265);
            radioButton_phucvu.Name = "radioButton_phucvu";
            radioButton_phucvu.Size = new Size(80, 24);
            radioButton_phucvu.TabIndex = 11;
            radioButton_phucvu.TabStop = true;
            radioButton_phucvu.Text = "Phục vụ";
            radioButton_phucvu.UseVisualStyleBackColor = true;
            radioButton_phucvu.CheckedChanged += radioButton_phucvu_CheckedChanged;
            // 
            // radioButton_bep
            // 
            radioButton_bep.AutoSize = true;
            radioButton_bep.Location = new Point(300, 265);
            radioButton_bep.Name = "radioButton_bep";
            radioButton_bep.Size = new Size(56, 24);
            radioButton_bep.TabIndex = 12;
            radioButton_bep.TabStop = true;
            radioButton_bep.Text = "Bếp";
            radioButton_bep.UseVisualStyleBackColor = true;
            // 
            // btn_dangky
            // 
            btn_dangky.AutoSize = true;
            btn_dangky.BackColor = Color.White;
            btn_dangky.BackgroundImage = (Image)resources.GetObject("btn_dangky.BackgroundImage");
            btn_dangky.BackgroundImageLayout = ImageLayout.Stretch;
            btn_dangky.FlatAppearance.BorderColor = Color.White;
            btn_dangky.FlatAppearance.BorderSize = 0;
            btn_dangky.FlatStyle = FlatStyle.Flat;
            btn_dangky.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_dangky.ForeColor = Color.Black;
            btn_dangky.Location = new Point(42, 457);
            btn_dangky.Name = "btn_dangky";
            btn_dangky.Size = new Size(358, 36);
            btn_dangky.TabIndex = 13;
            btn_dangky.UseVisualStyleBackColor = false;
            btn_dangky.Click += btn_dangky_Click;
            // 
            // linkLabel_quaylai
            // 
            linkLabel_quaylai.AutoSize = true;
            linkLabel_quaylai.BackColor = Color.Transparent;
            linkLabel_quaylai.LinkColor = Color.Gray;
            linkLabel_quaylai.Location = new Point(256, 514);
            linkLabel_quaylai.Name = "linkLabel_quaylai";
            linkLabel_quaylai.Size = new Size(63, 20);
            linkLabel_quaylai.TabIndex = 14;
            linkLabel_quaylai.TabStop = true;
            linkLabel_quaylai.Text = "Quay lại";
            linkLabel_quaylai.LinkClicked += linkLabel_quaylai_LinkClicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(128, 64, 64);
            label1.Location = new Point(97, 514);
            label1.Name = "label1";
            label1.Size = new Size(153, 20);
            label1.TabIndex = 15;
            label1.Text = "Bạn đã có tài khoản?";
            // 
            // tb_email
            // 
            tb_email.BackColor = Color.White;
            tb_email.Location = new Point(41, 213);
            tb_email.Name = "tb_email";
            tb_email.Size = new Size(378, 27);
            tb_email.TabIndex = 17;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(41, 187);
            label2.Name = "label2";
            label2.Size = new Size(55, 23);
            label2.TabIndex = 16;
            label2.Text = "Email:";
            label2.Click += label2_Click;
            // 
            // tb_passwd
            // 
            tb_passwd.BackColor = Color.White;
            tb_passwd.Location = new Point(42, 333);
            tb_passwd.Name = "tb_passwd";
            tb_passwd.Size = new Size(378, 27);
            tb_passwd.TabIndex = 7;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.BorderStyle = BorderStyle.FixedSingle;
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
            panel1.Location = new Point(277, 24);
            panel1.Name = "panel1";
            panel1.Size = new Size(455, 548);
            panel1.TabIndex = 19;
            // 
            // DangKy
            // 
            AcceptButton = btn_dangky;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1004, 605);
            Controls.Add(panel1);
            Name = "DangKy";
            Text = "Smartdine";
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