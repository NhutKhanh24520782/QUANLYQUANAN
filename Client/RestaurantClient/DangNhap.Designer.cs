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
            lbl_username = new Label();
            lbl_passwd = new Label();
            groupBox_dangnhap.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox_dangnhap
            // 
            groupBox_dangnhap.Controls.Add(lbl_passwd);
            groupBox_dangnhap.Controls.Add(lbl_username);
            groupBox_dangnhap.Location = new Point(117, 161);
            groupBox_dangnhap.Name = "groupBox_dangnhap";
            groupBox_dangnhap.Size = new Size(563, 147);
            groupBox_dangnhap.TabIndex = 0;
            groupBox_dangnhap.TabStop = false;
            groupBox_dangnhap.Text = "Thông tin đăng nhập";
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
            // lbl_passwd
            // 
            lbl_passwd.AutoSize = true;
            lbl_passwd.Location = new Point(43, 95);
            lbl_passwd.Name = "lbl_passwd";
            lbl_passwd.Size = new Size(50, 20);
            lbl_passwd.TabIndex = 1;
            lbl_passwd.Text = "label2";
            // 
            // DangNhap
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox_dangnhap);
            Name = "DangNhap";
            Text = "DangNhap";
            groupBox_dangnhap.ResumeLayout(false);
            groupBox_dangnhap.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox_dangnhap;
        private Label lbl_passwd;
        private Label lbl_username;
    }
}