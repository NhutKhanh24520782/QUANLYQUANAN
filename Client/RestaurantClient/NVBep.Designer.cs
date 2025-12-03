namespace RestaurantClient
{
    partial class NVBep
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
            tc_nvbep = new TabControl();
            tp_quanlyvaxuly = new TabPage();
            btn_xemthongke = new Button();
            btn_caidat = new Button();
            btn_refresh = new Button();
            lbl_thongke = new Label();
            dataGridView1 = new DataGridView();
            cb_sapxep = new ComboBox();
            label1 = new Label();
            tb_numberTable = new TextBox();
            lbl_findTable = new Label();
            lbl_filterType = new Label();
            comboBox1 = new ComboBox();
            lbl_userInfo = new Label();
            lbl_title = new Label();
            tb_chat = new TabPage();
            panel_update = new Panel();
            btn_huymon = new Button();
            btn_sendmess = new Button();
            btn_luuthaydoi = new Button();
            tb_ghichu = new TextBox();
            label5 = new Label();
            cb_timedukien = new ComboBox();
            label4 = new Label();
            cb_daubep = new ComboBox();
            label3 = new Label();
            cb_uutien = new ComboBox();
            label2 = new Label();
            cb_status = new ComboBox();
            lbl_status = new Label();
            lbl_updateTitle = new Label();
            dataGridView_dishs = new DataGridView();
            lbl_orderdetails = new Label();
            lbl_orderinfo = new Label();
            tc_nvbep.SuspendLayout();
            tp_quanlyvaxuly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tb_chat.SuspendLayout();
            panel_update.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView_dishs).BeginInit();
            SuspendLayout();
            // 
            // tc_nvbep
            // 
            tc_nvbep.Controls.Add(tp_quanlyvaxuly);
            tc_nvbep.Controls.Add(tb_chat);
            tc_nvbep.Location = new Point(4, 3);
            tc_nvbep.Name = "tc_nvbep";
            tc_nvbep.SelectedIndex = 0;
            tc_nvbep.Size = new Size(1291, 581);
            tc_nvbep.TabIndex = 0;
            tc_nvbep.SelectedIndexChanged += tc_nvbep_SelectedIndexChanged;
            // 
            // tp_quanlyvaxuly
            // 
            tp_quanlyvaxuly.Controls.Add(btn_xemthongke);
            tp_quanlyvaxuly.Controls.Add(btn_caidat);
            tp_quanlyvaxuly.Controls.Add(btn_refresh);
            tp_quanlyvaxuly.Controls.Add(lbl_thongke);
            tp_quanlyvaxuly.Controls.Add(dataGridView1);
            tp_quanlyvaxuly.Controls.Add(cb_sapxep);
            tp_quanlyvaxuly.Controls.Add(label1);
            tp_quanlyvaxuly.Controls.Add(tb_numberTable);
            tp_quanlyvaxuly.Controls.Add(lbl_findTable);
            tp_quanlyvaxuly.Controls.Add(lbl_filterType);
            tp_quanlyvaxuly.Controls.Add(comboBox1);
            tp_quanlyvaxuly.Controls.Add(lbl_userInfo);
            tp_quanlyvaxuly.Controls.Add(lbl_title);
            tp_quanlyvaxuly.Location = new Point(4, 29);
            tp_quanlyvaxuly.Name = "tp_quanlyvaxuly";
            tp_quanlyvaxuly.Padding = new Padding(3);
            tp_quanlyvaxuly.Size = new Size(1283, 548);
            tp_quanlyvaxuly.TabIndex = 0;
            tp_quanlyvaxuly.Text = "Quản lý & xử lý đơn";
            tp_quanlyvaxuly.UseVisualStyleBackColor = true;
            // 
            // btn_xemthongke
            // 
            btn_xemthongke.AutoSize = true;
            btn_xemthongke.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_xemthongke.Location = new Point(884, 491);
            btn_xemthongke.Name = "btn_xemthongke";
            btn_xemthongke.Size = new Size(133, 48);
            btn_xemthongke.TabIndex = 33;
            btn_xemthongke.Text = "Xem thống kê";
            btn_xemthongke.UseVisualStyleBackColor = true;
            btn_xemthongke.Click += btn_xemthongke_Click;
            // 
            // btn_caidat
            // 
            btn_caidat.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_caidat.Location = new Point(1074, 491);
            btn_caidat.Name = "btn_caidat";
            btn_caidat.Size = new Size(118, 48);
            btn_caidat.TabIndex = 32;
            btn_caidat.Text = "Cài đặt";
            btn_caidat.UseVisualStyleBackColor = true;
            btn_caidat.Click += btn_caidat_Click;
            // 
            // btn_refresh
            // 
            btn_refresh.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_refresh.Location = new Point(697, 491);
            btn_refresh.Name = "btn_refresh";
            btn_refresh.Size = new Size(118, 48);
            btn_refresh.TabIndex = 31;
            btn_refresh.Text = "Làm mới";
            btn_refresh.UseVisualStyleBackColor = true;
            btn_refresh.Click += btn_refresh_Click;
            // 
            // lbl_thongke
            // 
            lbl_thongke.AutoSize = true;
            lbl_thongke.Font = new Font("Segoe UI", 13.2000008F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_thongke.Location = new Point(17, 457);
            lbl_thongke.Name = "lbl_thongke";
            lbl_thongke.Size = new Size(671, 31);
            lbl_thongke.TabIndex = 30;
            lbl_thongke.Text = "Tổng: 14 món • ⏳ 5 chờ • 👨‍🍳 6 đang • ✅ 3 xong • ⚠️ 1 vấn đề";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(17, 148);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(1257, 306);
            dataGridView1.TabIndex = 29;
            // 
            // cb_sapxep
            // 
            cb_sapxep.FormattingEnabled = true;
            cb_sapxep.Location = new Point(687, 103);
            cb_sapxep.Name = "cb_sapxep";
            cb_sapxep.Size = new Size(151, 28);
            cb_sapxep.TabIndex = 28;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 13.2000008F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(581, 100);
            label1.Name = "label1";
            label1.Size = new Size(100, 31);
            label1.TabIndex = 27;
            label1.Text = "Sắp xếp:";
            // 
            // tb_numberTable
            // 
            tb_numberTable.Location = new Point(393, 105);
            tb_numberTable.Name = "tb_numberTable";
            tb_numberTable.PlaceholderText = "Nhập số bàn...";
            tb_numberTable.Size = new Size(141, 27);
            tb_numberTable.TabIndex = 26;
            // 
            // lbl_findTable
            // 
            lbl_findTable.AutoSize = true;
            lbl_findTable.Font = new Font("Segoe UI", 13.2000008F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_findTable.Location = new Point(285, 100);
            lbl_findTable.Name = "lbl_findTable";
            lbl_findTable.Size = new Size(102, 31);
            lbl_findTable.TabIndex = 25;
            lbl_findTable.Text = "Tìm bàn:";
            // 
            // lbl_filterType
            // 
            lbl_filterType.AutoSize = true;
            lbl_filterType.Font = new Font("Segoe UI", 13.2000008F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_filterType.Location = new Point(18, 100);
            lbl_filterType.Name = "lbl_filterType";
            lbl_filterType.Size = new Size(54, 31);
            lbl_filterType.TabIndex = 24;
            lbl_filterType.Text = "Lọc:";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(78, 103);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(151, 28);
            comboBox1.TabIndex = 23;
            // 
            // lbl_userInfo
            // 
            lbl_userInfo.AutoSize = true;
            lbl_userInfo.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_userInfo.Location = new Point(485, 53);
            lbl_userInfo.Name = "lbl_userInfo";
            lbl_userInfo.Size = new Size(330, 28);
            lbl_userInfo.TabIndex = 22;
            lbl_userInfo.Text = "Chào, Trần Văn B • 14:30 02/12/2024";
            // 
            // lbl_title
            // 
            lbl_title.AutoSize = true;
            lbl_title.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl_title.Location = new Point(468, 12);
            lbl_title.Name = "lbl_title";
            lbl_title.Size = new Size(347, 31);
            lbl_title.TabIndex = 21;
            lbl_title.Text = "👨‍🍳 QUẢN LÝ ĐƠN HÀNG - BẾP";
            // 
            // tb_chat
            // 
            tb_chat.Controls.Add(panel_update);
            tb_chat.Controls.Add(dataGridView_dishs);
            tb_chat.Controls.Add(lbl_orderdetails);
            tb_chat.Controls.Add(lbl_orderinfo);
            tb_chat.Location = new Point(4, 29);
            tb_chat.Name = "tb_chat";
            tb_chat.Padding = new Padding(3);
            tb_chat.Size = new Size(1283, 548);
            tb_chat.TabIndex = 1;
            tb_chat.Text = "Chi tiết đơn & cập nhật";
            tb_chat.UseVisualStyleBackColor = true;
            // 
            // panel_update
            // 
            panel_update.Controls.Add(btn_huymon);
            panel_update.Controls.Add(btn_sendmess);
            panel_update.Controls.Add(btn_luuthaydoi);
            panel_update.Controls.Add(tb_ghichu);
            panel_update.Controls.Add(label5);
            panel_update.Controls.Add(cb_timedukien);
            panel_update.Controls.Add(label4);
            panel_update.Controls.Add(cb_daubep);
            panel_update.Controls.Add(label3);
            panel_update.Controls.Add(cb_uutien);
            panel_update.Controls.Add(label2);
            panel_update.Controls.Add(cb_status);
            panel_update.Controls.Add(lbl_status);
            panel_update.Controls.Add(lbl_updateTitle);
            panel_update.Location = new Point(741, 79);
            panel_update.Name = "panel_update";
            panel_update.Size = new Size(533, 466);
            panel_update.TabIndex = 3;
            // 
            // btn_huymon
            // 
            btn_huymon.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_huymon.Location = new Point(204, 400);
            btn_huymon.Name = "btn_huymon";
            btn_huymon.Size = new Size(114, 38);
            btn_huymon.TabIndex = 13;
            btn_huymon.Text = "Hủy món";
            btn_huymon.UseVisualStyleBackColor = true;
            btn_huymon.Click += btn_huymon_Click;
            // 
            // btn_sendmess
            // 
            btn_sendmess.AutoSize = true;
            btn_sendmess.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_sendmess.Location = new Point(378, 400);
            btn_sendmess.Name = "btn_sendmess";
            btn_sendmess.Size = new Size(119, 38);
            btn_sendmess.TabIndex = 12;
            btn_sendmess.Text = "Gửi tin nhắn";
            btn_sendmess.UseVisualStyleBackColor = true;
            // 
            // btn_luuthaydoi
            // 
            btn_luuthaydoi.AutoSize = true;
            btn_luuthaydoi.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_luuthaydoi.Location = new Point(23, 400);
            btn_luuthaydoi.Name = "btn_luuthaydoi";
            btn_luuthaydoi.Size = new Size(121, 38);
            btn_luuthaydoi.TabIndex = 11;
            btn_luuthaydoi.Text = "Lưu thay đổi";
            btn_luuthaydoi.UseVisualStyleBackColor = true;
            btn_luuthaydoi.Click += btn_luuthaydoi_Click;
            // 
            // tb_ghichu
            // 
            tb_ghichu.Location = new Point(120, 311);
            tb_ghichu.Name = "tb_ghichu";
            tb_ghichu.Size = new Size(410, 27);
            tb_ghichu.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(7, 315);
            label5.Name = "label5";
            label5.Size = new Size(107, 23);
            label5.TabIndex = 9;
            label5.Text = "Ghi chú bếp:";
            // 
            // cb_timedukien
            // 
            cb_timedukien.FormattingEnabled = true;
            cb_timedukien.Location = new Point(131, 205);
            cb_timedukien.Name = "cb_timedukien";
            cb_timedukien.Size = new Size(151, 28);
            cb_timedukien.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(3, 205);
            label4.Name = "label4";
            label4.Size = new Size(122, 25);
            label4.TabIndex = 7;
            label4.Text = "Dự kiến xong:";
            // 
            // cb_daubep
            // 
            cb_daubep.FormattingEnabled = true;
            cb_daubep.Location = new Point(108, 137);
            cb_daubep.Name = "cb_daubep";
            cb_daubep.Size = new Size(151, 28);
            cb_daubep.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(3, 136);
            label3.Name = "label3";
            label3.Size = new Size(84, 25);
            label3.TabIndex = 5;
            label3.Text = "Đầu bếp:";
            // 
            // cb_uutien
            // 
            cb_uutien.FormattingEnabled = true;
            cb_uutien.Location = new Point(364, 65);
            cb_uutien.Name = "cb_uutien";
            cb_uutien.Size = new Size(151, 28);
            cb_uutien.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(274, 65);
            label2.Name = "label2";
            label2.Size = new Size(73, 25);
            label2.TabIndex = 3;
            label2.Text = "Ưu tiên:";
            // 
            // cb_status
            // 
            cb_status.FormattingEnabled = true;
            cb_status.Location = new Point(108, 66);
            cb_status.Name = "cb_status";
            cb_status.Size = new Size(151, 28);
            cb_status.TabIndex = 2;
            // 
            // lbl_status
            // 
            lbl_status.AutoSize = true;
            lbl_status.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_status.Location = new Point(3, 65);
            lbl_status.Name = "lbl_status";
            lbl_status.Size = new Size(98, 25);
            lbl_status.TabIndex = 1;
            lbl_status.Text = "Trạng thái: ";
            // 
            // lbl_updateTitle
            // 
            lbl_updateTitle.AutoSize = true;
            lbl_updateTitle.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl_updateTitle.Location = new Point(120, 15);
            lbl_updateTitle.Name = "lbl_updateTitle";
            lbl_updateTitle.Size = new Size(333, 23);
            lbl_updateTitle.TabIndex = 0;
            lbl_updateTitle.Text = "⚙️ CẬP NHẬT TRẠNG THÁI: Bún chả ×3";
            // 
            // dataGridView_dishs
            // 
            dataGridView_dishs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_dishs.Location = new Point(6, 79);
            dataGridView_dishs.Name = "dataGridView_dishs";
            dataGridView_dishs.RowHeadersWidth = 51;
            dataGridView_dishs.Size = new Size(729, 460);
            dataGridView_dishs.TabIndex = 2;
            // 
            // lbl_orderdetails
            // 
            lbl_orderdetails.AutoSize = true;
            lbl_orderdetails.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_orderdetails.Location = new Point(427, 37);
            lbl_orderdetails.Name = "lbl_orderdetails";
            lbl_orderdetails.Size = new Size(448, 25);
            lbl_orderdetails.TabIndex = 1;
            lbl_orderdetails.Text = "NV Order: Nguyễn Văn A • Trạng thái: ⏳ Chờ xác nhận";
            // 
            // lbl_orderinfo
            // 
            lbl_orderinfo.AutoSize = true;
            lbl_orderinfo.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_orderinfo.Location = new Point(449, 12);
            lbl_orderinfo.Name = "lbl_orderinfo";
            lbl_orderinfo.Size = new Size(381, 25);
            lbl_orderinfo.TabIndex = 0;
            lbl_orderinfo.Text = "ĐƠN #012 - BÀN 1 • 14:25 • Ước tính: 15 phút";
            // 
            // NVBep
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1294, 583);
            Controls.Add(tc_nvbep);
            Name = "NVBep";
            Text = "NVBep";
            tc_nvbep.ResumeLayout(false);
            tp_quanlyvaxuly.ResumeLayout(false);
            tp_quanlyvaxuly.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tb_chat.ResumeLayout(false);
            tb_chat.PerformLayout();
            panel_update.ResumeLayout(false);
            panel_update.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView_dishs).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tc_nvbep;
        private TabPage tp_quanlyvaxuly;
        private TabPage tb_chat;
        private Label lbl_title;
        private Label lbl_findTable;
        private Label lbl_filterType;
        private ComboBox comboBox1;
        private Label lbl_userInfo;
        private DataGridView dataGridView1;
        private ComboBox cb_sapxep;
        private Label label1;
        private TextBox tb_numberTable;
        private Label lbl_thongke;
        private Button btn_xemthongke;
        private Button btn_caidat;
        private Button btn_refresh;
        private Label lbl_orderinfo;
        private DataGridView dataGridView_dishs;
        private Label lbl_orderdetails;
        private Panel panel_update;
        private ComboBox cb_status;
        private Label lbl_status;
        private Label lbl_updateTitle;
        private ComboBox cb_uutien;
        private Label label2;
        private TextBox tb_ghichu;
        private Label label5;
        private ComboBox cb_timedukien;
        private Label label4;
        private ComboBox cb_daubep;
        private Label label3;
        private Button btn_huymon;
        private Button btn_sendmess;
        private Button btn_luuthaydoi;
    }
}