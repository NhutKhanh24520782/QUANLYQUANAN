namespace RestaurantClient
{
    partial class NVPhucVu
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
            tabControl1 = new TabControl();
            tp_phucvuqly = new TabPage();
            gb_thucdonorder = new GroupBox();
            btn_guiorder = new Button();
            btn_xoamon = new Button();
            btn_themmon = new Button();
            tb_soluongmon = new TextBox();
            label3 = new Label();
            cb_namedish = new ComboBox();
            listView1 = new ListView();
            gb_thongtinban = new GroupBox();
            button2 = new Button();
            btn_datban = new Button();
            lbl_trangthai = new Label();
            label2 = new Label();
            label1 = new Label();
            comboBox1 = new ComboBox();
            lbl_chonban = new Label();
            gb_chitietorder = new GroupBox();
            btn_thanhtoan = new Button();
            lbl_thongtintongtien = new Label();
            lbl_tongtien = new Label();
            listView2 = new ListView();
            tb_theodoidon = new TabPage();
            gb_thongtin = new GroupBox();
            label4 = new Label();
            tb_chat = new TabPage();
            button1 = new Button();
            textBox1 = new TextBox();
            dataGridView1 = new DataGridView();
            lbl_boloc = new Label();
            lbl_ban = new Label();
            cb_ban = new ComboBox();
            cb_state = new ComboBox();
            label5 = new Label();
            btn_lammoi = new Button();
            listView3 = new ListView();
            label6 = new Label();
            tabControl1.SuspendLayout();
            tp_phucvuqly.SuspendLayout();
            gb_thucdonorder.SuspendLayout();
            gb_thongtinban.SuspendLayout();
            gb_chitietorder.SuspendLayout();
            tb_theodoidon.SuspendLayout();
            gb_thongtin.SuspendLayout();
            tb_chat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tp_phucvuqly);
            tabControl1.Controls.Add(tb_theodoidon);
            tabControl1.Controls.Add(tb_chat);
            tabControl1.Location = new Point(-1, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1333, 596);
            tabControl1.TabIndex = 0;
            // 
            // tp_phucvuqly
            // 
            tp_phucvuqly.Controls.Add(gb_thucdonorder);
            tp_phucvuqly.Controls.Add(gb_thongtinban);
            tp_phucvuqly.Controls.Add(gb_chitietorder);
            tp_phucvuqly.Location = new Point(4, 29);
            tp_phucvuqly.Name = "tp_phucvuqly";
            tp_phucvuqly.Padding = new Padding(3);
            tp_phucvuqly.Size = new Size(1325, 563);
            tp_phucvuqly.TabIndex = 0;
            tp_phucvuqly.Text = "Order món và đặt bàn";
            tp_phucvuqly.UseVisualStyleBackColor = true;
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
            gb_thucdonorder.Location = new Point(647, 25);
            gb_thucdonorder.Name = "gb_thucdonorder";
            gb_thucdonorder.Size = new Size(682, 542);
            gb_thucdonorder.TabIndex = 4;
            gb_thucdonorder.TabStop = false;
            gb_thucdonorder.Text = "Thực đơn & Order món";
            // 
            // btn_guiorder
            // 
            btn_guiorder.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_guiorder.Location = new Point(555, 442);
            btn_guiorder.Name = "btn_guiorder";
            btn_guiorder.Size = new Size(95, 65);
            btn_guiorder.TabIndex = 10;
            btn_guiorder.Text = "Gửi Order";
            btn_guiorder.UseVisualStyleBackColor = true;
            // 
            // btn_xoamon
            // 
            btn_xoamon.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_xoamon.Location = new Point(423, 442);
            btn_xoamon.Name = "btn_xoamon";
            btn_xoamon.Size = new Size(95, 65);
            btn_xoamon.TabIndex = 9;
            btn_xoamon.Text = "Xóa Món";
            btn_xoamon.UseVisualStyleBackColor = true;
            // 
            // btn_themmon
            // 
            btn_themmon.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_themmon.Location = new Point(293, 442);
            btn_themmon.Name = "btn_themmon";
            btn_themmon.Size = new Size(95, 65);
            btn_themmon.TabIndex = 8;
            btn_themmon.Text = "Thêm món";
            btn_themmon.UseVisualStyleBackColor = true;
            // 
            // tb_soluongmon
            // 
            tb_soluongmon.Location = new Point(495, 33);
            tb_soluongmon.Name = "tb_soluongmon";
            tb_soluongmon.Size = new Size(125, 31);
            tb_soluongmon.TabIndex = 8;
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
            // cb_namedish
            // 
            cb_namedish.FormattingEnabled = true;
            cb_namedish.Location = new Point(42, 30);
            cb_namedish.Name = "cb_namedish";
            cb_namedish.Size = new Size(311, 33);
            cb_namedish.TabIndex = 1;
            cb_namedish.Text = "Tên món";
            // 
            // listView1
            // 
            listView1.Location = new Point(24, 100);
            listView1.Name = "listView1";
            listView1.Size = new Size(648, 314);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
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
            gb_thongtinban.Location = new Point(20, 25);
            gb_thongtinban.Name = "gb_thongtinban";
            gb_thongtinban.Size = new Size(609, 169);
            gb_thongtinban.TabIndex = 3;
            gb_thongtinban.TabStop = false;
            gb_thongtinban.Text = "Thông tin bàn ăn";
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(279, 122);
            button2.Name = "button2";
            button2.Size = new Size(112, 41);
            button2.TabIndex = 6;
            button2.Text = "Hủy bàn";
            button2.UseVisualStyleBackColor = true;
            // 
            // btn_datban
            // 
            btn_datban.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_datban.Location = new Point(103, 122);
            btn_datban.Name = "btn_datban";
            btn_datban.Size = new Size(112, 41);
            btn_datban.TabIndex = 5;
            btn_datban.Text = "Đặt bàn";
            btn_datban.UseVisualStyleBackColor = true;
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
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(151, 88);
            label2.Name = "label2";
            label2.Size = new Size(0, 25);
            label2.TabIndex = 3;
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
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(151, 33);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(223, 36);
            comboBox1.TabIndex = 1;
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
            // gb_chitietorder
            // 
            gb_chitietorder.Controls.Add(btn_thanhtoan);
            gb_chitietorder.Controls.Add(lbl_thongtintongtien);
            gb_chitietorder.Controls.Add(lbl_tongtien);
            gb_chitietorder.Controls.Add(listView2);
            gb_chitietorder.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gb_chitietorder.Location = new Point(20, 200);
            gb_chitietorder.Name = "gb_chitietorder";
            gb_chitietorder.Size = new Size(621, 357);
            gb_chitietorder.TabIndex = 5;
            gb_chitietorder.TabStop = false;
            gb_chitietorder.Text = "Chi tiết order hiện tại";
            // 
            // btn_thanhtoan
            // 
            btn_thanhtoan.Location = new Point(442, 285);
            btn_thanhtoan.Name = "btn_thanhtoan";
            btn_thanhtoan.Size = new Size(126, 56);
            btn_thanhtoan.TabIndex = 5;
            btn_thanhtoan.Text = "Thanh toán";
            btn_thanhtoan.UseVisualStyleBackColor = true;
            // 
            // lbl_thongtintongtien
            // 
            lbl_thongtintongtien.AutoSize = true;
            lbl_thongtintongtien.Location = new Point(103, 285);
            lbl_thongtintongtien.Name = "lbl_thongtintongtien";
            lbl_thongtintongtien.Size = new Size(54, 25);
            lbl_thongtintongtien.TabIndex = 4;
            lbl_thongtintongtien.Text = "------";
            // 
            // lbl_tongtien
            // 
            lbl_tongtien.AutoSize = true;
            lbl_tongtien.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_tongtien.Location = new Point(0, 285);
            lbl_tongtien.Name = "lbl_tongtien";
            lbl_tongtien.Size = new Size(91, 25);
            lbl_tongtien.TabIndex = 3;
            lbl_tongtien.Text = "Tổng tiền:";
            // 
            // listView2
            // 
            listView2.Location = new Point(6, 30);
            listView2.Name = "listView2";
            listView2.Size = new Size(603, 252);
            listView2.TabIndex = 0;
            listView2.UseCompatibleStateImageBehavior = false;
            // 
            // tb_theodoidon
            // 
            tb_theodoidon.Controls.Add(gb_thongtin);
            tb_theodoidon.Controls.Add(label4);
            tb_theodoidon.Location = new Point(4, 29);
            tb_theodoidon.Name = "tb_theodoidon";
            tb_theodoidon.Padding = new Padding(3);
            tb_theodoidon.Size = new Size(1325, 563);
            tb_theodoidon.TabIndex = 1;
            tb_theodoidon.Text = "Theo dõi đơn hàng";
            tb_theodoidon.UseVisualStyleBackColor = true;
            // 
            // gb_thongtin
            // 
            gb_thongtin.Controls.Add(label6);
            gb_thongtin.Controls.Add(listView3);
            gb_thongtin.Controls.Add(btn_lammoi);
            gb_thongtin.Controls.Add(cb_state);
            gb_thongtin.Controls.Add(label5);
            gb_thongtin.Controls.Add(cb_ban);
            gb_thongtin.Controls.Add(lbl_ban);
            gb_thongtin.Controls.Add(lbl_boloc);
            gb_thongtin.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gb_thongtin.Location = new Point(6, 6);
            gb_thongtin.Name = "gb_thongtin";
            gb_thongtin.Size = new Size(1313, 551);
            gb_thongtin.TabIndex = 1;
            gb_thongtin.TabStop = false;
            gb_thongtin.Text = "Thông tin đơn hàng";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(59, 111);
            label4.Name = "label4";
            label4.Size = new Size(0, 20);
            label4.TabIndex = 0;
            // 
            // tb_chat
            // 
            tb_chat.Controls.Add(button1);
            tb_chat.Controls.Add(textBox1);
            tb_chat.Controls.Add(dataGridView1);
            tb_chat.Location = new Point(4, 29);
            tb_chat.Name = "tb_chat";
            tb_chat.Padding = new Padding(3);
            tb_chat.Size = new Size(1325, 563);
            tb_chat.TabIndex = 2;
            tb_chat.Text = "Chat";
            tb_chat.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(1034, 418);
            button1.Name = "button1";
            button1.Size = new Size(225, 65);
            button1.TabIndex = 5;
            button1.Text = "Gửi";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(-4, 418);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(1006, 65);
            textBox1.TabIndex = 4;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(-4, 15);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(1280, 397);
            dataGridView1.TabIndex = 3;
            // 
            // lbl_boloc
            // 
            lbl_boloc.AutoSize = true;
            lbl_boloc.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl_boloc.Location = new Point(17, 63);
            lbl_boloc.Name = "lbl_boloc";
            lbl_boloc.Size = new Size(76, 28);
            lbl_boloc.TabIndex = 0;
            lbl_boloc.Text = "Bộ lọc:";
            // 
            // lbl_ban
            // 
            lbl_ban.AutoSize = true;
            lbl_ban.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_ban.Location = new Point(111, 66);
            lbl_ban.Name = "lbl_ban";
            lbl_ban.Size = new Size(69, 25);
            lbl_ban.TabIndex = 1;
            lbl_ban.Text = "Bàn ăn:";
            // 
            // cb_ban
            // 
            cb_ban.FormattingEnabled = true;
            cb_ban.Location = new Point(186, 60);
            cb_ban.Name = "cb_ban";
            cb_ban.Size = new Size(216, 36);
            cb_ban.TabIndex = 2;
            // 
            // cb_state
            // 
            cb_state.FormattingEnabled = true;
            cb_state.Location = new Point(612, 63);
            cb_state.Name = "cb_state";
            cb_state.Size = new Size(216, 36);
            cb_state.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(513, 66);
            label5.Name = "label5";
            label5.Size = new Size(93, 25);
            label5.TabIndex = 3;
            label5.Text = "Trạng thái:";
            // 
            // btn_lammoi
            // 
            btn_lammoi.Location = new Point(913, 55);
            btn_lammoi.Name = "btn_lammoi";
            btn_lammoi.Size = new Size(173, 53);
            btn_lammoi.TabIndex = 5;
            btn_lammoi.Text = "Làm mới";
            btn_lammoi.UseVisualStyleBackColor = true;
            // 
            // listView3
            // 
            listView3.Location = new Point(17, 151);
            listView3.Name = "listView3";
            listView3.Size = new Size(1290, 368);
            listView3.TabIndex = 6;
            listView3.UseCompatibleStateImageBehavior = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(554, 120);
            label6.Name = "label6";
            label6.Size = new Size(233, 31);
            label6.TabIndex = 7;
            label6.Text = "Danh sách đơn hàng";
            // 
            // NVPhucVu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1340, 602);
            Controls.Add(tabControl1);
            Name = "NVPhucVu";
            Text = "NVPhucVu";
            tabControl1.ResumeLayout(false);
            tp_phucvuqly.ResumeLayout(false);
            gb_thucdonorder.ResumeLayout(false);
            gb_thucdonorder.PerformLayout();
            gb_thongtinban.ResumeLayout(false);
            gb_thongtinban.PerformLayout();
            gb_chitietorder.ResumeLayout(false);
            gb_chitietorder.PerformLayout();
            tb_theodoidon.ResumeLayout(false);
            tb_theodoidon.PerformLayout();
            gb_thongtin.ResumeLayout(false);
            gb_thongtin.PerformLayout();
            tb_chat.ResumeLayout(false);
            tb_chat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tp_phucvuqly;
        private TabPage tb_theodoidon;
        private TabPage tp_chat;
        private GroupBox gb_thucdonorder;
        private Button btn_guiorder;
        private Button btn_xoamon;
        private Button btn_themmon;
        private TextBox tb_soluongmon;
        private Label label3;
        private ComboBox cb_namedish;
        private ListView listView1;
        private GroupBox gb_thongtinban;
        private Button button2;
        private Button btn_datban;
        private Label lbl_trangthai;
        private Label label2;
        private Label label1;
        private ComboBox comboBox1;
        private Label lbl_chonban;
        private GroupBox gb_chitietorder;
        private Button btn_thanhtoan;
        private Label lbl_thongtintongtien;
        private Label lbl_tongtien;
        private ListView listView2;
        private TextBox tb_chatphucvu;
        private DataGridView dataGridView_chatphucvu;
        private Button btn_sendphucvu;
        private TabPage tb_chat;
        private Button button1;
        private TextBox textBox1;
        private DataGridView dataGridView1;
        private Label label4;
        private GroupBox gb_thongtin;
        private Label lbl_boloc;
        private ComboBox cb_state;
        private Label label5;
        private ComboBox cb_ban;
        private Label lbl_ban;
        private Label label6;
        private ListView listView3;
        private Button btn_lammoi;
    }
}