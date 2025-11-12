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
            tb_chat = new TabPage();
            btn_hoanthanh = new Button();
            btn_dangchebien = new Button();
            btn_choxacnhan = new Button();
            label7 = new Label();
            label6 = new Label();
            listView3 = new ListView();
            btn_huydon = new Button();
            btn_sendBep = new Button();
            tb_chatBep = new TextBox();
            dataGridView_chatBep = new DataGridView();
            tc_nvbep.SuspendLayout();
            tp_quanlyvaxuly.SuspendLayout();
            tb_chat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView_chatBep).BeginInit();
            SuspendLayout();
            // 
            // tc_nvbep
            // 
            tc_nvbep.Controls.Add(tp_quanlyvaxuly);
            tc_nvbep.Controls.Add(tb_chat);
            tc_nvbep.Location = new Point(4, 4);
            tc_nvbep.Name = "tc_nvbep";
            tc_nvbep.SelectedIndex = 0;
            tc_nvbep.Size = new Size(1170, 548);
            tc_nvbep.TabIndex = 0;
            tc_nvbep.SelectedIndexChanged += tc_nvbep_SelectedIndexChanged;
            // 
            // tp_quanlyvaxuly
            // 
            tp_quanlyvaxuly.Controls.Add(btn_huydon);
            tp_quanlyvaxuly.Controls.Add(btn_hoanthanh);
            tp_quanlyvaxuly.Controls.Add(btn_dangchebien);
            tp_quanlyvaxuly.Controls.Add(btn_choxacnhan);
            tp_quanlyvaxuly.Controls.Add(label7);
            tp_quanlyvaxuly.Controls.Add(label6);
            tp_quanlyvaxuly.Controls.Add(listView3);
            tp_quanlyvaxuly.Location = new Point(4, 29);
            tp_quanlyvaxuly.Name = "tp_quanlyvaxuly";
            tp_quanlyvaxuly.Padding = new Padding(3);
            tp_quanlyvaxuly.Size = new Size(1162, 515);
            tp_quanlyvaxuly.TabIndex = 0;
            tp_quanlyvaxuly.Text = "Quản lý & xử lý đơn";
            tp_quanlyvaxuly.UseVisualStyleBackColor = true;
            // 
            // tb_chat
            // 
            tb_chat.Controls.Add(btn_sendBep);
            tb_chat.Controls.Add(tb_chatBep);
            tb_chat.Controls.Add(dataGridView_chatBep);
            tb_chat.Location = new Point(4, 29);
            tb_chat.Name = "tb_chat";
            tb_chat.Padding = new Padding(3);
            tb_chat.Size = new Size(1162, 515);
            tb_chat.TabIndex = 1;
            tb_chat.Text = "Chat";
            tb_chat.UseVisualStyleBackColor = true;
            // 
            // btn_hoanthanh
            // 
            btn_hoanthanh.AutoSize = true;
            btn_hoanthanh.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_hoanthanh.Location = new Point(648, 427);
            btn_hoanthanh.Name = "btn_hoanthanh";
            btn_hoanthanh.Size = new Size(158, 51);
            btn_hoanthanh.TabIndex = 25;
            btn_hoanthanh.Text = "Hoàn thành";
            btn_hoanthanh.UseVisualStyleBackColor = true;
            // 
            // btn_dangchebien
            // 
            btn_dangchebien.AutoSize = true;
            btn_dangchebien.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_dangchebien.Location = new Point(442, 427);
            btn_dangchebien.Name = "btn_dangchebien";
            btn_dangchebien.Size = new Size(158, 51);
            btn_dangchebien.TabIndex = 24;
            btn_dangchebien.Text = "Đang chế biến";
            btn_dangchebien.UseVisualStyleBackColor = true;
            // 
            // btn_choxacnhan
            // 
            btn_choxacnhan.AutoSize = true;
            btn_choxacnhan.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_choxacnhan.Location = new Point(243, 427);
            btn_choxacnhan.Name = "btn_choxacnhan";
            btn_choxacnhan.Size = new Size(150, 51);
            btn_choxacnhan.TabIndex = 23;
            btn_choxacnhan.Text = "Chờ xác nhận";
            btn_choxacnhan.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label7.Location = new Point(23, 438);
            label7.Name = "label7";
            label7.Size = new Size(201, 28);
            label7.TabIndex = 22;
            label7.Text = "Cập nhật trạng thái:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(442, 18);
            label6.Name = "label6";
            label6.Size = new Size(332, 28);
            label6.TabIndex = 21;
            label6.Text = "Danh sách đơn hàng cần chế biến";
            // 
            // listView3
            // 
            listView3.Location = new Point(23, 58);
            listView3.Name = "listView3";
            listView3.Size = new Size(1116, 354);
            listView3.TabIndex = 20;
            listView3.UseCompatibleStateImageBehavior = false;
            // 
            // btn_huydon
            // 
            btn_huydon.AutoSize = true;
            btn_huydon.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_huydon.Location = new Point(854, 427);
            btn_huydon.Name = "btn_huydon";
            btn_huydon.Size = new Size(158, 51);
            btn_huydon.TabIndex = 26;
            btn_huydon.Text = "Hủy đơn";
            btn_huydon.UseVisualStyleBackColor = true;
            // 
            // btn_sendBep
            // 
            btn_sendBep.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_sendBep.Location = new Point(932, 426);
            btn_sendBep.Name = "btn_sendBep";
            btn_sendBep.Size = new Size(225, 65);
            btn_sendBep.TabIndex = 8;
            btn_sendBep.Text = "Gửi";
            btn_sendBep.UseVisualStyleBackColor = true;
            // 
            // tb_chatBep
            // 
            tb_chatBep.Location = new Point(1, 426);
            tb_chatBep.Multiline = true;
            tb_chatBep.Name = "tb_chatBep";
            tb_chatBep.Size = new Size(925, 65);
            tb_chatBep.TabIndex = 7;
            // 
            // dataGridView_chatBep
            // 
            dataGridView_chatBep.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_chatBep.Location = new Point(1, 23);
            dataGridView_chatBep.Name = "dataGridView_chatBep";
            dataGridView_chatBep.RowHeadersWidth = 51;
            dataGridView_chatBep.Size = new Size(1161, 397);
            dataGridView_chatBep.TabIndex = 6;
            // 
            // NVBep
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1174, 552);
            Controls.Add(tc_nvbep);
            Name = "NVBep";
            Text = "NVBep";
            tc_nvbep.ResumeLayout(false);
            tp_quanlyvaxuly.ResumeLayout(false);
            tp_quanlyvaxuly.PerformLayout();
            tb_chat.ResumeLayout(false);
            tb_chat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView_chatBep).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tc_nvbep;
        private TabPage tp_quanlyvaxuly;
        private TabPage tb_chat;
        private Button btn_huydon;
        private Button btn_hoanthanh;
        private Button btn_dangchebien;
        private Button btn_choxacnhan;
        private Label label7;
        private Label label6;
        private ListView listView3;
        private Button btn_sendBep;
        private TextBox tb_chatBep;
        private DataGridView dataGridView_chatBep;
    }
}