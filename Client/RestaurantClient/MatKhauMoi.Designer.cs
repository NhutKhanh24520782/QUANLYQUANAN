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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MatKhauMoi));
            label1 = new Label();
            label2 = new Label();
            tb_newPass = new TextBox();
            tb_confirmPass = new TextBox();
            btn_hoanTat = new Button();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(164, 61);
            label1.Name = "label1";
            label1.Size = new Size(170, 25);
            label1.TabIndex = 0;
            label1.Text = "Nhập mật khẩu mới";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(164, 138);
            label2.Name = "label2";
            label2.Size = new Size(156, 25);
            label2.TabIndex = 1;
            label2.Text = "Nhập lại mật khẩu";
            // 
            // tb_newPass
            // 
            tb_newPass.Location = new Point(190, 89);
            tb_newPass.Name = "tb_newPass";
            tb_newPass.Size = new Size(355, 27);
            tb_newPass.TabIndex = 2;
            // 
            // tb_confirmPass
            // 
            tb_confirmPass.Location = new Point(190, 166);
            tb_confirmPass.Name = "tb_confirmPass";
            tb_confirmPass.Size = new Size(355, 27);
            tb_confirmPass.TabIndex = 3;
            // 
            // btn_hoanTat
            // 
            btn_hoanTat.BackColor = Color.FromArgb(128, 64, 64);
            btn_hoanTat.FlatStyle = FlatStyle.Flat;
            btn_hoanTat.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btn_hoanTat.ForeColor = Color.White;
            btn_hoanTat.Location = new Point(282, 218);
            btn_hoanTat.Name = "btn_hoanTat";
            btn_hoanTat.Size = new Size(157, 45);
            btn_hoanTat.TabIndex = 4;
            btn_hoanTat.Text = "Hoàn tất";
            btn_hoanTat.UseVisualStyleBackColor = false;
            btn_hoanTat.Click += btn_hoanTat_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(164, 89);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(29, 27);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(164, 166);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(29, 27);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 6;
            pictureBox2.TabStop = false;
            // 
            // MatKhauMoi
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 358);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(tb_newPass);
            Controls.Add(label1);
            Controls.Add(tb_confirmPass);
            Controls.Add(label2);
            Controls.Add(btn_hoanTat);
            Name = "MatKhauMoi";
            Text = "MatKhauMoi";
            Load += MatKhauMoi_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox tb_newPass;
        private TextBox tb_confirmPass;
        private Button btn_hoanTat;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
    }
}