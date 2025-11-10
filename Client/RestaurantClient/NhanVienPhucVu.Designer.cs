namespace RestaurantClient
{
    partial class NhanVienPhucVu
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
            gb_thongtinban = new GroupBox();
            lbl_chonban = new Label();
            comboBox1 = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            lbl_trangthai = new Label();
            btn_datban = new Button();
            button2 = new Button();
            gb_thucdonorder = new GroupBox();
            listView1 = new ListView();
            cb_namedish = new ComboBox();
            label3 = new Label();
            tb_soluongmon = new TextBox();
            btn_themmon = new Button();
            btn_xoamon = new Button();
            btn_guiorder = new Button();
            gb_chitietorder = new GroupBox();
            listView2 = new ListView();
            lbl_tongtien = new Label();
            lbl_thongtintongtien = new Label();
            btn_thanhtoan = new Button();
            gb_thongtinban.SuspendLayout();
            gb_thucdonorder.SuspendLayout();
            gb_chitietorder.SuspendLayout();
            SuspendLayout();
            // 
            // gb_thongtinban
            // 
            gb_thongtinban.Controls.Add(button2);
            gb_thongtinban.Controls.Add(btn_datban);
            gb_thongtinban.Controls.Add(lbl_trangthai);
            gb_thongtinban.Controls.Add(label2);
            gb_thongtinban.Controls.Add(label1);
            gb_thongtinban.Controls.Add(comboBox1);
            gb_thongtinban.Controls.Add(lbl_chonban);
            gb_thongtinban.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gb_thongtinban.Location = new Point(12, 12);
            gb_thongtinban.Name = "gb_thongtinban";
            gb_thongtinban.Size = new Size(466, 169);
            gb_thongtinban.TabIndex = 0;
            gb_thongtinban.TabStop = false;
            gb_thongtinban.Text = "Thông tin bàn ăn";
            gb_thongtinban.Enter += gb_thongtinban_Enter;
            // 
            // lbl_chonban
            // 
            lbl_chonban.AutoSize = true;
            lbl_chonban.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_chonban.Location = new Point(6, 39);
            lbl_chonban.Name = "lbl_chonban";
            lbl_chonban.Size = new Size(93, 25);
            lbl_chonban.TabIndex = 0;
            lbl_chonban.Text = "Chọn bàn:";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(151, 33);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(151, 36);
            comboBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(0, 88);
            label1.Name = "label1";
            label1.Size = new Size(128, 25);
            label1.TabIndex = 2;
            label1.Text = "Trạng thái bàn:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(151, 88);
            label2.Name = "label2";
            label2.Size = new Size(0, 25);
            label2.TabIndex = 3;
            // 
            // lbl_trangthai
            // 
            lbl_trangthai.AutoSize = true;
            lbl_trangthai.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_trangthai.Location = new Point(151, 88);
            lbl_trangthai.Name = "lbl_trangthai";
            lbl_trangthai.Size = new Size(0, 25);
            lbl_trangthai.TabIndex = 4;
            // 
            // btn_datban
            // 
            btn_datban.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_datban.Location = new Point(57, 128);
            btn_datban.Name = "btn_datban";
            btn_datban.Size = new Size(94, 29);
            btn_datban.TabIndex = 5;
            btn_datban.Text = "Đặt bàn";
            btn_datban.UseVisualStyleBackColor = true;
            btn_datban.Click += btn_datban_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(185, 128);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 6;
            button2.Text = "Hủy bàn";
            button2.UseVisualStyleBackColor = true;
            // 
            // gb_thucdonorder
            // 
            gb_thucdonorder.Controls.Add(btn_guiorder);
            gb_thucdonorder.Controls.Add(btn_xoamon);
            gb_thucdonorder.Controls.Add(btn_themmon);
            gb_thucdonorder.Controls.Add(tb_soluongmon);
            gb_thucdonorder.Controls.Add(label3);
            gb_thucdonorder.Controls.Add(cb_namedish);
            gb_thucdonorder.Controls.Add(listView1);
            gb_thucdonorder.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gb_thucdonorder.Location = new Point(484, 12);
            gb_thucdonorder.Name = "gb_thucdonorder";
            gb_thucdonorder.Size = new Size(659, 477);
            gb_thucdonorder.TabIndex = 1;
            gb_thucdonorder.TabStop = false;
            gb_thucdonorder.Text = "Thực đơn & Order món";
            // 
            // listView1
            // 
            listView1.Location = new Point(24, 100);
            listView1.Name = "listView1";
            listView1.Size = new Size(615, 231);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // cb_namedish
            // 
            cb_namedish.FormattingEnabled = true;
            cb_namedish.Location = new Point(42, 30);
            cb_namedish.Name = "cb_namedish";
            cb_namedish.Size = new Size(311, 33);
            cb_namedish.TabIndex = 1;
            cb_namedish.Text = "Tên món";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(389, 33);
            label3.Name = "label3";
            label3.Size = new Size(89, 25);
            label3.TabIndex = 7;
            label3.Text = "Số lượng:";
            // 
            // tb_soluongmon
            // 
            tb_soluongmon.Location = new Point(495, 33);
            tb_soluongmon.Name = "tb_soluongmon";
            tb_soluongmon.Size = new Size(125, 31);
            tb_soluongmon.TabIndex = 8;
            // 
            // btn_themmon
            // 
            btn_themmon.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_themmon.Location = new Point(68, 362);
            btn_themmon.Name = "btn_themmon";
            btn_themmon.Size = new Size(95, 65);
            btn_themmon.TabIndex = 8;
            btn_themmon.Text = "Thêm món";
            btn_themmon.UseVisualStyleBackColor = true;
            // 
            // btn_xoamon
            // 
            btn_xoamon.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_xoamon.Location = new Point(209, 362);
            btn_xoamon.Name = "btn_xoamon";
            btn_xoamon.Size = new Size(95, 65);
            btn_xoamon.TabIndex = 9;
            btn_xoamon.Text = "Xóa Món";
            btn_xoamon.UseVisualStyleBackColor = true;
            // 
            // btn_guiorder
            // 
            btn_guiorder.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_guiorder.Location = new Point(351, 362);
            btn_guiorder.Name = "btn_guiorder";
            btn_guiorder.Size = new Size(95, 65);
            btn_guiorder.TabIndex = 10;
            btn_guiorder.Text = "Gửi Order";
            btn_guiorder.UseVisualStyleBackColor = true;
            // 
            // gb_chitietorder
            // 
            gb_chitietorder.Controls.Add(btn_thanhtoan);
            gb_chitietorder.Controls.Add(lbl_thongtintongtien);
            gb_chitietorder.Controls.Add(lbl_tongtien);
            gb_chitietorder.Controls.Add(listView2);
            gb_chitietorder.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gb_chitietorder.Location = new Point(12, 187);
            gb_chitietorder.Name = "gb_chitietorder";
            gb_chitietorder.Size = new Size(466, 302);
            gb_chitietorder.TabIndex = 2;
            gb_chitietorder.TabStop = false;
            gb_chitietorder.Text = "Chi tiết order hiện tại";
            // 
            // listView2
            // 
            listView2.Location = new Point(6, 30);
            listView2.Name = "listView2";
            listView2.Size = new Size(454, 172);
            listView2.TabIndex = 0;
            listView2.UseCompatibleStateImageBehavior = false;
            // 
            // lbl_tongtien
            // 
            lbl_tongtien.AutoSize = true;
            lbl_tongtien.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_tongtien.Location = new Point(6, 205);
            lbl_tongtien.Name = "lbl_tongtien";
            lbl_tongtien.Size = new Size(91, 25);
            lbl_tongtien.TabIndex = 3;
            lbl_tongtien.Text = "Tổng tiền:";
            // 
            // lbl_thongtintongtien
            // 
            lbl_thongtintongtien.AutoSize = true;
            lbl_thongtintongtien.Location = new Point(103, 205);
            lbl_thongtintongtien.Name = "lbl_thongtintongtien";
            lbl_thongtintongtien.Size = new Size(54, 25);
            lbl_thongtintongtien.TabIndex = 4;
            lbl_thongtintongtien.Text = "------";
            // 
            // btn_thanhtoan
            // 
            btn_thanhtoan.Location = new Point(243, 214);
            btn_thanhtoan.Name = "btn_thanhtoan";
            btn_thanhtoan.Size = new Size(126, 56);
            btn_thanhtoan.TabIndex = 5;
            btn_thanhtoan.Text = "Thanh toán";
            btn_thanhtoan.UseVisualStyleBackColor = true;
            // 
            // NhanVienPhucVu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1155, 501);
            Controls.Add(gb_chitietorder);
            Controls.Add(gb_thucdonorder);
            Controls.Add(gb_thongtinban);
            Name = "NhanVienPhucVu";
            Text = "NhanVienPhucVu";
            Load += NhanVienPhucVu_Load;
            gb_thongtinban.ResumeLayout(false);
            gb_thongtinban.PerformLayout();
            gb_thucdonorder.ResumeLayout(false);
            gb_thucdonorder.PerformLayout();
            gb_chitietorder.ResumeLayout(false);
            gb_chitietorder.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gb_thongtinban;
        private Label label1;
        private ComboBox comboBox1;
        private Label lbl_chonban;
        private Label label2;
        private Button button2;
        private Button btn_datban;
        private Label lbl_trangthai;
        private GroupBox gb_thucdonorder;
        private Label label3;
        private ComboBox cb_namedish;
        private ListView listView1;
        private TextBox tb_soluongmon;
        private Button btn_guiorder;
        private Button btn_xoamon;
        private Button btn_themmon;
        private GroupBox gb_chitietorder;
        private Button btn_thanhtoan;
        private Label lbl_thongtintongtien;
        private Label lbl_tongtien;
        private ListView listView2;
    }
}