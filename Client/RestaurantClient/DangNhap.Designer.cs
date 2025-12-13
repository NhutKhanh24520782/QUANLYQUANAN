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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DangNhap));
            tb_passwd = new TextBox();
            label1 = new Label();
            label2 = new Label();
            linkLabel_dangky = new LinkLabel();
            btn_dangnhap = new Button();
            linkLabel_forgetpasswd = new LinkLabel();
            panel1 = new Panel();
            panel4 = new Panel();
            pictureBox3 = new PictureBox();
            panel3 = new Panel();
            pictureBox2 = new PictureBox();
            tb_username = new TextBox();
            pictureBox1 = new PictureBox();
            panel2 = new Panel();
            panel1.SuspendLayout();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tb_passwd
            // 
            tb_passwd.BorderStyle = BorderStyle.None;
            tb_passwd.Location = new Point(41, 6);
            tb_passwd.Name = "tb_passwd";
            tb_passwd.Size = new Size(392, 20);
            tb_passwd.TabIndex = 1;
            tb_passwd.TextChanged += tb_passwd_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Gray;
            label1.Location = new Point(40, 155);
            label1.Name = "label1";
            label1.Size = new Size(134, 28);
            label1.TabIndex = 1;
            label1.Text = "ĐĂNG NHẬP";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(128, 64, 64);
            label2.Location = new Point(40, 403);
            label2.Name = "label2";
            label2.Size = new Size(267, 20);
            label2.TabIndex = 2;
            label2.Text = "Tạo tài khoản mới, hãy chọn Đăng ký";
            // 
            // linkLabel_dangky
            // 
            linkLabel_dangky.AutoSize = true;
            linkLabel_dangky.BackColor = Color.Transparent;
            linkLabel_dangky.LinkColor = Color.FromArgb(64, 64, 64);
            linkLabel_dangky.Location = new Point(40, 433);
            linkLabel_dangky.Name = "linkLabel_dangky";
            linkLabel_dangky.Size = new Size(63, 20);
            linkLabel_dangky.TabIndex = 3;
            linkLabel_dangky.TabStop = true;
            linkLabel_dangky.Text = "Đăng ký";
            linkLabel_dangky.LinkClicked += linkLabel_dangky_LinkClicked_1;
            // 
            // btn_dangnhap
            // 
            btn_dangnhap.AutoSize = true;
            btn_dangnhap.BackColor = Color.FromArgb(128, 64, 64);
            btn_dangnhap.FlatStyle = FlatStyle.Flat;
            btn_dangnhap.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_dangnhap.ForeColor = Color.White;
            btn_dangnhap.Location = new Point(40, 315);
            btn_dangnhap.Name = "btn_dangnhap";
            btn_dangnhap.Size = new Size(445, 42);
            btn_dangnhap.TabIndex = 2;
            btn_dangnhap.Text = "Đăng nhập";
            btn_dangnhap.UseVisualStyleBackColor = false;
            btn_dangnhap.Click += btn_dangnhap_Click;
            // 
            // linkLabel_forgetpasswd
            // 
            linkLabel_forgetpasswd.AutoSize = true;
            linkLabel_forgetpasswd.BackColor = Color.Transparent;
            linkLabel_forgetpasswd.LinkColor = Color.FromArgb(64, 64, 64);
            linkLabel_forgetpasswd.Location = new Point(40, 371);
            linkLabel_forgetpasswd.Name = "linkLabel_forgetpasswd";
            linkLabel_forgetpasswd.Size = new Size(116, 20);
            linkLabel_forgetpasswd.TabIndex = 5;
            linkLabel_forgetpasswd.TabStop = true;
            linkLabel_forgetpasswd.Text = "Quên mật khẩu?";
            linkLabel_forgetpasswd.LinkClicked += linkLabel_forgetpasswd_LinkClicked;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(panel4);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(linkLabel_dangky);
            panel1.Controls.Add(linkLabel_forgetpasswd);
            panel1.Controls.Add(btn_dangnhap);
            panel1.Location = new Point(-3, -2);
            panel1.Name = "panel1";
            panel1.Size = new Size(567, 503);
            panel1.TabIndex = 6;
            // 
            // panel4
            // 
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(pictureBox3);
            panel4.Controls.Add(tb_passwd);
            panel4.Location = new Point(47, 257);
            panel4.Name = "panel4";
            panel4.Size = new Size(438, 38);
            panel4.TabIndex = 11;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(-1, -2);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(39, 39);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 10;
            pictureBox3.TabStop = false;
            // 
            // panel3
            // 
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(pictureBox2);
            panel3.Controls.Add(tb_username);
            panel3.Location = new Point(47, 196);
            panel3.Name = "panel3";
            panel3.Size = new Size(438, 38);
            panel3.TabIndex = 9;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(-1, -2);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(39, 39);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 10;
            pictureBox2.TabStop = false;
            // 
            // tb_username
            // 
            tb_username.BorderStyle = BorderStyle.None;
            tb_username.Location = new Point(41, 6);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(392, 20);
            tb_username.TabIndex = 0;
            tb_username.TextChanged += tb_username_TextChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.White;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(40, 37);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(97, 97);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            panel2.BackgroundImage = (Image)resources.GetObject("panel2.BackgroundImage");
            panel2.BackgroundImageLayout = ImageLayout.None;
            panel2.Location = new Point(560, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(474, 498);
            panel2.TabIndex = 9;
            // 
            // DangNhap
            // 
            AcceptButton = btn_dangnhap;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightCyan;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(1036, 503);
            Controls.Add(panel2);
            Controls.Add(panel1);
            ForeColor = Color.DarkBlue;
            Name = "DangNhap";
            Text = "DangNhap";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TextBox tb_passwd;
        private Label label1;
        private Label label2;
        private LinkLabel linkLabel_dangky;
        private Button btn_dangnhap;
        private LinkLabel linkLabel_forgetpasswd;
        private Panel panel1;
        private PictureBox pictureBox1;
        private Panel panel2;
        private TextBox tb_username;
        private Panel panel3;
        private PictureBox pictureBox2;
        private Panel panel4;
        private PictureBox pictureBox3;
    }
}