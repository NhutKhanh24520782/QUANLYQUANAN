namespace RestaurantClient
{
    partial class NhapEmail
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
            tb_email = new TextBox();
            btn_send = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(78, 60);
            label1.Name = "label1";
            label1.Size = new Size(112, 28);
            label1.TabIndex = 0;
            label1.Text = "Nhập email";
            // 
            // tb_email
            // 
            tb_email.Location = new Point(196, 64);
            tb_email.Name = "tb_email";
            tb_email.Size = new Size(543, 27);
            tb_email.TabIndex = 1;
            // 
            // btn_send
            // 
            btn_send.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_send.Location = new Point(336, 147);
            btn_send.Name = "btn_send";
            btn_send.Size = new Size(139, 57);
            btn_send.TabIndex = 2;
            btn_send.Text = "Gửi mã OTP";
            btn_send.UseVisualStyleBackColor = true;
            btn_send.Click += btn_send_Click;
            // 
            // NhapEmail
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(793, 258);
            Controls.Add(btn_send);
            Controls.Add(tb_email);
            Controls.Add(label1);
            Name = "NhapEmail";
            Text = "NhapEmail";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox tb_email;
        private Button btn_send;
    }
}