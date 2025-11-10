namespace RestaurantClient
{
    partial class MatKhauMoi
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
            label2 = new Label();
            tb_newPass = new TextBox();
            tb_confirmPass = new TextBox();
            btn_hoanTat = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(104, 87);
            label1.Name = "label1";
            label1.Size = new Size(185, 28);
            label1.TabIndex = 0;
            label1.Text = "Nhập mật khẩu mới";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(118, 141);
            label2.Name = "label2";
            label2.Size = new Size(171, 28);
            label2.TabIndex = 1;
            label2.Text = "Nhập lại mật khẩu";
            // 
            // tb_newPass
            // 
            tb_newPass.Location = new Point(295, 91);
            tb_newPass.Name = "tb_newPass";
            tb_newPass.Size = new Size(306, 27);
            tb_newPass.TabIndex = 2;
            // 
            // tb_confirmPass
            // 
            tb_confirmPass.Location = new Point(295, 145);
            tb_confirmPass.Name = "tb_confirmPass";
            tb_confirmPass.Size = new Size(306, 27);
            tb_confirmPass.TabIndex = 3;
            // 
            // btn_hoanTat
            // 
            btn_hoanTat.BackColor = SystemColors.ActiveCaption;
            btn_hoanTat.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_hoanTat.Location = new Point(299, 221);
            btn_hoanTat.Name = "btn_hoanTat";
            btn_hoanTat.Size = new Size(157, 45);
            btn_hoanTat.TabIndex = 4;
            btn_hoanTat.Text = "Hoàn tất";
            btn_hoanTat.UseVisualStyleBackColor = false;
            btn_hoanTat.Click += btn_hoanTat_Click;
            // 
            // MatKhauMoi
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Cornsilk;
            ClientSize = new Size(800, 358);
            Controls.Add(btn_hoanTat);
            Controls.Add(tb_confirmPass);
            Controls.Add(tb_newPass);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "MatKhauMoi";
            Text = "MatKhauMoi";
            Load += MatKhauMoi_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox tb_newPass;
        private TextBox tb_confirmPass;
        private Button btn_hoanTat;
    }
}