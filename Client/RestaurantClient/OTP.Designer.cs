namespace RestaurantClient
{
    partial class OTP
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
            label1 = new Label();
            tb_otp = new TextBox();
            linkLabel1 = new LinkLabel();
            llResendCode = new LinkLabel();
            btnXacNhanOTP = new Button();
            lblTimer = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(70, 102);
            label1.Name = "label1";
            label1.Size = new Size(132, 28);
            label1.TabIndex = 0;
            label1.Text = "Nhập mã OTP";
            // 
            // tb_otp
            // 
            tb_otp.Location = new Point(208, 103);
            tb_otp.Name = "tb_otp";
            tb_otp.Size = new Size(294, 27);
            tb_otp.TabIndex = 1;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            linkLabel1.Location = new Point(296, 214);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(108, 28);
            linkLabel1.TabIndex = 2;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Gửi lại OTP";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // llResendCode
            // 
            llResendCode.AutoSize = true;
            llResendCode.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            llResendCode.Location = new Point(271, 245);
            llResendCode.Name = "llResendCode";
            llResendCode.Size = new Size(0, 28);
            llResendCode.TabIndex = 3;
            // 
            // btnXacNhanOTP
            // 
            btnXacNhanOTP.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnXacNhanOTP.Location = new Point(271, 159);
            btnXacNhanOTP.Name = "btnXacNhanOTP";
            btnXacNhanOTP.Size = new Size(166, 39);
            btnXacNhanOTP.TabIndex = 4;
            btnXacNhanOTP.Text = "Xác nhận OTP";
            btnXacNhanOTP.UseVisualStyleBackColor = true;
            btnXacNhanOTP.Click += btnXacNhanOTP_Click;
            // 
            // lblTimer
            // 
            lblTimer.AutoSize = true;
            lblTimer.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTimer.Location = new Point(306, 292);
            lblTimer.Name = "lblTimer";
            lblTimer.Size = new Size(0, 23);
            lblTimer.TabIndex = 5;
            // 
            // OTP
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 321);
            Controls.Add(lblTimer);
            Controls.Add(btnXacNhanOTP);
            Controls.Add(llResendCode);
            Controls.Add(linkLabel1);
            Controls.Add(tb_otp);
            Controls.Add(label1);
            Name = "OTP";
            Text = "OTP";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox tb_otp;
        private LinkLabel linkLabel1;
        private LinkLabel llResendCode;
        private Button btnXacNhanOTP;
        private Label lblTimer;
    }
}