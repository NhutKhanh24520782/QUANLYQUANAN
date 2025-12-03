using Models.Database;
using Models.Request;
using Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class NVBep : Form
    {
        // ==================== CONSTANTS & FIELDS ====================
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;

        private int _currentUserId;
        private string _currentUserName;
        private List<NguoiDung> _danhSachDauBep;
        private GridViewManager<KitchenOrderData> _ordersManager;
        private GridViewManager<KitchenDishData> _dishesManager;
        private System.Windows.Forms.Timer _autoRefreshTimer;
        private KitchenOrderDetailData _currentOrderDetail;
        private KitchenDishData _selectedDish;

        // ==================== INITIALIZATION ====================
        public NVBep(int userId, string userName)
        {
            _currentUserId = userId;
            _currentUserName = userName;

            InitializeComponent();
            InitializeGridViewManagers();
            InitializeComboBoxes();
            InitializeAutoRefreshTimer();
            LoadInitialData();
            UpdateUserInfo();
        }

        private void InitializeGridViewManagers()
        {
            // Manager cho danh sách đơn hàng (Form chính)
            _ordersManager = new GridViewManager<KitchenOrderData>(
                dataGridView1,
                LoadKitchenOrdersFromServer,
                order => new
                {
                    STT = _ordersManager.GetRowCount() + 1,
                    MaDonHang = order.MaDonHangDisplay,
                    Ban = order.TenBan,
                    ThoiGian = order.ThoiGianDisplay,
                    Mon = order.TongSoMon,
                    TrangThai = order.TrangThaiDisplay,
                    UuTien = order.UuTienDisplay,
                    Cho = order.ThoiGianChoDisplay
                },
                "MaDonHang"
            );

            _dishesManager = new GridViewManager<KitchenDishData>(
                 dataGridView_dishs,
                 LoadOrderDishesFromServer,
                 dish => new
                 {
                     MaChiTiet = dish.MaChiTiet,
                     Icon = GetDishIcon(dish.TenMon),
                     TenMon = dish.TenMon,
                     SoLuong = dish.SoLuongDisplay,
                     TrangThai = dish.TrangThaiDisplay,
                     ThoiGian = dish.ThoiGianDisplay,
                     UuTien = dish.UuTienDisplay
                 },
                 "MaChiTiet"
             );

            ConfigureDishesGridView(); // ✅ Gọi sau khi khởi tạo manager

            // Event handlers
            dataGridView1.SelectionChanged += DataGridView_Orders_SelectionChanged;
            dataGridView1.CellFormatting += DataGridView_Orders_CellFormatting;
            dataGridView1.CellDoubleClick += DataGridView_Orders_CellDoubleClick;

            dataGridView_dishs.SelectionChanged += DataGridView_Dishes_SelectionChanged;
            dataGridView_dishs.CellFormatting += DataGridView_Dishes_CellFormatting;
            dataGridView_dishs.CellContentClick += DataGridView_Dishes_CellContentClick;

            // Key events for shortcuts
            dataGridView_dishs.KeyDown += DataGridView_Dishes_KeyDown;
        }

        private void ConfigureDishesGridView()
        {
            dataGridView_dishs.AutoGenerateColumns = false;
            dataGridView_dishs.Columns.Clear();

            // ✅ THÊM DataPropertyName cho tất cả các cột
            dataGridView_dishs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Icon",
                HeaderText = "",
                DataPropertyName = "Icon", // ✅ THÊM
                Width = 40,
                ReadOnly = true
            });

            dataGridView_dishs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenMon",
                HeaderText = "Món ăn",
                DataPropertyName = "TenMon", // ✅ THÊM
                Width = 200,
                ReadOnly = true
            });

            dataGridView_dishs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "SL",
                DataPropertyName = "SoLuong", // ✅ THÊM
                Width = 50,
                ReadOnly = true
            });

            dataGridView_dishs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                DataPropertyName = "TrangThai", // ✅ THÊM
                Width = 120,
                ReadOnly = true
            });

            dataGridView_dishs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThoiGian",
                HeaderText = "Thời gian",
                DataPropertyName = "ThoiGian", // ✅ THÊM
                Width = 80,
                ReadOnly = true
            });

            dataGridView_dishs.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UuTien",
                HeaderText = "Ưu tiên",
                DataPropertyName = "UuTien", // ✅ THÊM
                Width = 60,
                ReadOnly = true
            });

            dataGridView_dishs.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "Action",
                HeaderText = "Thao tác",
                Text = "🔄",
                UseColumnTextForButtonValue = true,
                Width = 70,
                FlatStyle = FlatStyle.Flat
            });
        }
        private void InitializeComboBoxes()
        {
            // ComboBox lọc trạng thái (Form chính)
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[] {
                "Tất cả",
                "Chờ xác nhận",
                "Đang chế biến",
                "Hoàn thành",
                "Có vấn đề",
                "Hủy"
            });
            comboBox1.SelectedIndex = 0;
            comboBox1.SelectedIndexChanged += ComboBoxFilter_SelectedIndexChanged;

            // ComboBox sắp xếp
            cb_sapxep.Items.Clear();
            cb_sapxep.Items.AddRange(new string[] {
                "Thời gian",
                "Ưu tiên",
                "Bàn",
                "Thời gian chờ"
            });
            cb_sapxep.SelectedIndex = 0;
            cb_sapxep.SelectedIndexChanged += ComboBoxSort_SelectedIndexChanged;

            // ComboBox trạng thái món (Form cập nhật)
            cb_status.Items.Clear();
            cb_status.Items.AddRange(new string[] {
                "⏳ Chờ xác nhận",
                "👨‍🍳 Đang chế biến",
                "✅ Hoàn thành",
                "⚠️ Có vấn đề",
                "❌ Hủy"
            });

            // Sửa lại tên hiển thị trong NVBep.InitializeComboBoxes để khớp với UuTien 1-5
            cb_uutien.Items.Clear();
            cb_uutien.Items.AddRange(new string[] {
                "⭐ Thấp (1)",      // UuTien 1
                "🔥 Bình thường (2)", // UuTien 2
                "🔥🔥 Trung bình (3)", // UuTien 3
                "🔥🔥🔥 Cao (4)",     // UuTien 4
                "🔥🔥🔥🔥 Khẩn cấp (5)" // UuTien 5
            });
            cb_uutien.SelectedIndex = 0;

            // ComboBox đầu bếp
            LoadDauBepComboBox();

            // ComboBox thời gian dự kiến
            InitializeTimeComboBox();
        }
        // Trong NVBep.cs

        private async void LoadDauBepComboBox()
        {
            try
            {
                var request = new GetEmployeesRequest
                {
                    VaiTro = "Bep"
                };
                var response = await SendRequest<GetEmployeesRequest, GetEmployeesResponse>(request);

                if (response?.Success == true && response.Employees != null)
                {
                    _danhSachDauBep = response.Employees
                        .Select(e => new NguoiDung
                        {
                            MaNguoiDung = e.MaNguoiDung,
                            HoTen = e.HoTen
                        })
                        .ToList();

                    // Thêm chính mình nếu chưa có trong danh sách
                    if (!_danhSachDauBep.Any(d => d.MaNguoiDung == _currentUserId))
                    {
                        _danhSachDauBep.Add(new NguoiDung
                        {
                            MaNguoiDung = _currentUserId,
                            HoTen = _currentUserName
                        });
                    }

                    // Clear và thêm vào combobox
                    cb_daubep.Items.Clear();
                    cb_daubep.Items.AddRange(_danhSachDauBep
                        .Select(c => c.HoTen)
                        .ToArray());

                    // Chọn mặc định là chính mình
                    for (int i = 0; i < cb_daubep.Items.Count; i++)
                    {
                        if (cb_daubep.Items[i].ToString() == _currentUserName)
                        {
                            cb_daubep.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    // Nếu không lấy được, thêm chính mình
                    cb_daubep.Items.Clear();
                    cb_daubep.Items.Add(_currentUserName);
                    cb_daubep.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải danh sách đầu bếp: {ex.Message}");
                // Fallback: thêm chính mình
                cb_daubep.Items.Clear();
                cb_daubep.Items.Add(_currentUserName);
                cb_daubep.SelectedIndex = 0;
            }
        }
        private void InitializeTimeComboBox()
        {
            cb_timedukien.Items.Clear();

            // Tạo danh sách giờ từ hiện tại đến 2 giờ sau
            var now = DateTime.Now;
            for (int i = 0; i <= 8; i++) // 2 giờ * 4 (mỗi 15 phút)
            {
                var time = now.AddMinutes(i * 15);
                cb_timedukien.Items.Add(time.ToString("HH:mm"));
            }

            // Mặc định chọn thời gian sau 30 phút
            var defaultTime = now.AddMinutes(30).ToString("HH:mm");
            if (cb_timedukien.Items.Contains(defaultTime))
            {
                cb_timedukien.SelectedItem = defaultTime;
            }
            else
            {
                cb_timedukien.SelectedIndex = 0;
            }
        }

        private void InitializeAutoRefreshTimer()
        {
            _autoRefreshTimer = new System.Windows.Forms.Timer();
            _autoRefreshTimer.Interval = 30000; // 30 giây
            _autoRefreshTimer.Tick += async (s, e) =>
            {
                if (this.Visible && tc_nvbep.SelectedIndex == 0) // Chỉ refresh khi ở tab quản lý
                {
                    await _ordersManager.RefreshAsync();
                }
            };
            _autoRefreshTimer.Start();
        }

        private void UpdateUserInfo()
        {
            lbl_userInfo.Text = $"Chào, {_currentUserName} • {DateTime.Now:HH:mm dd/MM/yyyy}";
        }

        private async void LoadInitialData()
        {
            try
            {
                await _ordersManager.LoadDataAsync();

                // Load statistics
                await LoadThongKe();
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi tải dữ liệu ban đầu: {ex.Message}");
            }
        }

     

        private async Task<List<KitchenDishData>> LoadOrderDishesFromServer()
        {
            if (_currentOrderDetail == null || _currentOrderDetail.MaDonHang == 0)
                return new List<KitchenDishData>();

            try
            {
                var request = new GetOrderDetailRequest
                {
                    MaDonHang = _currentOrderDetail.MaDonHang
                };

                var response = await SendRequest<GetOrderDetailRequest, GetOrderDetailResponse>(request);

                if (response?.Success == true)
                {
                    _currentOrderDetail = response.ChiTietDonHang;
                    UpdateOrderDetailDisplay();

                    return response.ChiTietDonHang?.DanhSachMon ?? new List<KitchenDishData>();
                }
                else
                {
                    ShowError(response?.Message ?? "Không thể tải chi tiết đơn hàng");
                    return new List<KitchenDishData>();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi tải chi tiết: {ex.Message}");
                return new List<KitchenDishData>();
            }
        }

        private async Task LoadThongKe()
        {
            try
            {
                var request = new GetKitchenStatisticsRequest
                {
                    TuNgay = DateTime.Today,
                    DenNgay = DateTime.Today.AddDays(1).AddSeconds(-1)
                };

                var response = await SendRequest<GetKitchenStatisticsRequest, GetKitchenStatisticsResponse>(request);

                if (response?.Success == true)
                {
                    // Có thể hiển thị thống kê nếu cần
                    Console.WriteLine($"Thống kê hôm nay: {response.ThongKe.TongSoDon} đơn, {response.ThongKe.TongSoMon} món");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải thống kê: {ex.Message}");
            }
        }

        // ==================== DISPLAY UPDATE METHODS ====================
        private void UpdateThongKeDisplay(ThongKeBep thongKe)
        {
            if (lbl_thongke.InvokeRequired)
            {
                lbl_thongke.Invoke(new Action<ThongKeBep>(UpdateThongKeDisplay), thongKe);
                return;
            }

            lbl_thongke.Text = thongKe.DisplayText;
        }

        private void UpdateOrderDetailDisplay()
        {
            if (_currentOrderDetail == null) return;

            if (lbl_orderinfo.InvokeRequired)
            {
                lbl_orderinfo.Invoke(new Action(UpdateOrderDetailDisplay));
                return;
            }

            // Cập nhật thông tin đơn hàng
            lbl_orderinfo.Text = $"📋 {_currentOrderDetail.MaDonHangDisplay} - {_currentOrderDetail.TenBan} • {_currentOrderDetail.ThoiGianDisplay} • Ước tính: {_currentOrderDetail.ThoiGianConLaiDisplay}";
            lbl_orderdetails.Text = $"NV Order: {_currentOrderDetail.TenNhanVienOrder} • Trạng thái: {ConvertStatusToDisplay(_currentOrderDetail.TrangThaiDon)}";
        }
        private void UpdateUpdatePanel(KitchenDishData dish)
        {
            if (dish == null) return;

            _selectedDish = dish;

            // Cập nhật tiêu đề
            lbl_updateTitle.Text = $"⚙️ CẬP NHẬT TRẠNG THÁI: {dish.TenMon} ×{dish.SoLuong}";

            // Cập nhật trạng thái hiện tại - FIX: Sử dụng GetStatusDisplayItem
            string statusDisplay = GetStatusDisplayItem(dish.TrangThai);
            int statusIndex = -1;
            for (int i = 0; i < cb_status.Items.Count; i++)
            {
                if (cb_status.Items[i].ToString() == statusDisplay)
                {
                    statusIndex = i;
                    break;
                }
            }
            if (statusIndex >= 0)
            {
                cb_status.SelectedIndex = statusIndex;
            }

            // Cập nhật độ ưu tiên - FIX: UuTien từ 1-5, SelectedIndex từ 0-4
            cb_uutien.SelectedIndex = Math.Clamp(dish.UuTien - 1, 0, 4);

            // Cập nhật đầu bếp
            if (!string.IsNullOrEmpty(dish.TenNhanVienCheBien))
            {
                bool found = false;
                for (int i = 0; i < cb_daubep.Items.Count; i++)
                {
                    if (cb_daubep.Items[i].ToString() == dish.TenNhanVienCheBien)
                    {
                        cb_daubep.SelectedIndex = i;
                        found = true;
                        break;
                    }
                }
                if (!found && cb_daubep.Items.Count > 0)
                {
                    cb_daubep.SelectedIndex = 0;
                }
            }
            else
            {
                // Tìm tên của _currentUserName trong combobox
                bool found = false;
                for (int i = 0; i < cb_daubep.Items.Count; i++)
                {
                    if (cb_daubep.Items[i].ToString() == _currentUserName)
                    {
                        cb_daubep.SelectedIndex = i;
                        found = true;
                        break;
                    }
                }
                if (!found && cb_daubep.Items.Count > 0)
                {
                    cb_daubep.SelectedIndex = 0;
                }
            }

            // Cập nhật thời gian dự kiến
            if (dish.ThoiGianDuKien.HasValue)
            {
                string timeString = dish.ThoiGianDuKien.Value.ToString("HH:mm");
                bool found = false;
                for (int i = 0; i < cb_timedukien.Items.Count; i++)
                {
                    if (cb_timedukien.Items[i].ToString() == timeString)
                    {
                        cb_timedukien.SelectedIndex = i;
                        found = true;
                        break;
                    }
                }
                if (!found && cb_timedukien.Items.Count > 0)
                {
                    cb_timedukien.SelectedIndex = 0;
                }
            }
            else if (cb_timedukien.Items.Count > 0)
            {
                cb_timedukien.SelectedIndex = 0;
            }

            // Cập nhật ghi chú
            tb_ghichu.Text = dish.GhiChuBep ?? "";

            // Hiển thị panel
            panel_update.Visible = true;
            panel_update.BringToFront();
        }
        // ==================== EVENT HANDLERS ====================
        // Trong NVBep.cs

        private void DataGridView_Orders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Kiểm tra và thoát sớm nếu không cần định dạng
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            var order = _ordersManager.GetAllDisplayedItems().ElementAtOrDefault(e.RowIndex);

            if (order != null)
            {

                if (order.ThoiGianCho.TotalMinutes > 15)
                {
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }

                // 2. Định dạng màu theo trạng thái (Sử dụng giá trị đã tính toán/có sẵn)
                if (dataGridView1.Columns.Contains("TrangThai") && e.ColumnIndex == dataGridView1.Columns["TrangThai"].Index)
                {
                    e.CellStyle.ForeColor = GetStatusColor(order.TrangThaiDon);
                    // e.FormattingApplied = true; // Có thể bỏ qua
                }

                // 3. Định dạng màu theo ưu tiên (Sử dụng giá trị đã tính toán/có sẵn)
                if (dataGridView1.Columns.Contains("UuTien") && e.ColumnIndex == dataGridView1.Columns["UuTien"].Index)
                {
                    e.CellStyle.ForeColor = GetPriorityColor(order.UuTienCaoNhat);
                    // e.FormattingApplied = true; // Có thể bỏ qua
                }
            }
        }
        private void DataGridView_Orders_SelectionChanged(object sender, EventArgs e)
        {
            var selectedOrder = _ordersManager.GetSelectedItem();
            if (selectedOrder != null)
            {
                // Có thể highlight hoặc hiển thị thông tin nhanh
                // Không tự động mở chi tiết, phải double click
            }
        }
        private void DataGridView_Dishes_SelectionChanged(object sender, EventArgs e)
        {
            var selectedDish = _dishesManager.GetSelectedItem();
            if (selectedDish != null)
            {
                // Highlight dòng được chọn
                foreach (DataGridViewRow row in dataGridView_dishs.Rows)
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }

                if (dataGridView_dishs.CurrentRow != null)
                {
                    dataGridView_dishs.CurrentRow.DefaultCellStyle.BackColor = Color.LightBlue;
                }

                // Hiển thị panel cập nhật
                UpdateUpdatePanel(selectedDish);
            }
        }

        // ✅ FIX: CellContentClick an toàn hơn
        private void DataGridView_Dishes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridView_dishs.Columns.Contains("Action") &&
                e.ColumnIndex == dataGridView_dishs.Columns["Action"].Index)
            {
                var dish = _dishesManager.GetItemAtRow(e.RowIndex);
                if (dish != null)
                {
                    UpdateUpdatePanel(dish);
                    dataGridView_dishs.Rows[e.RowIndex].Selected = true;
                }
            }
        }

        // ✅ FIX: CellFormatting xử lý tất cả dữ liệu hiển thị
        private void DataGridView_Dishes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var dish = _dishesManager.GetItemAtRow(e.RowIndex);
            if (dish == null) return;

            var columnName = dataGridView_dishs.Columns[e.ColumnIndex].Name;

            switch (columnName)
            {
                case "Icon":
                    e.Value = GetDishIcon(dish.TenMon);
                    e.FormattingApplied = true;
                    break;

                case "TenMon":
                    e.Value = dish.TenMon;
                    e.FormattingApplied = true;
                    break;

                case "SoLuong":
                    e.Value = dish.SoLuongDisplay;
                    e.FormattingApplied = true;
                    break;

                case "TrangThai":
                    e.Value = dish.TrangThaiDisplay;
                    e.CellStyle.ForeColor = GetDishStatusColor(dish.TrangThai);
                    e.FormattingApplied = true;
                    break;

                case "ThoiGian":
                    e.Value = dish.ThoiGianDisplay;
                    e.FormattingApplied = true;
                    break;

                case "UuTien":
                    e.Value = dish.UuTienDisplay;
                    e.CellStyle.ForeColor = GetPriorityColor(dish.UuTien);
                    e.FormattingApplied = true;
                    break;
            }
        }
        private void DataGridView_Orders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var selectedOrder = _ordersManager.GetSelectedItem();
            if (selectedOrder != null)
            {
                // Chuyển sang tab chi tiết
                tc_nvbep.SelectedIndex = 1;

                // Load chi tiết đơn hàng
                LoadOrderDetail(selectedOrder.MaDonHang);
            }
        }
        private void DataGridView_Dishes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                var selectedDish = _dishesManager.GetSelectedItem();
                if (selectedDish != null)
                {
                    UpdateUpdatePanel(selectedDish);
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Enter && panel_update.Visible)
            {
                btn_luuthaydoi_Click(sender, e);
                e.Handled = true;
            }
        }

        private async void ComboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            await _ordersManager.RefreshAsync();
        }

        private async void ComboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            await _ordersManager.RefreshAsync();
        }

        private void tb_numberTable_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                _ordersManager.RefreshAsync();
            }
        }

        // ==================== BUTTON HANDLERS ====================
        private async void btn_refresh_Click(object sender, EventArgs e)
        {
            await ExecuteAsync(btn_refresh, "Đang làm mới...", async () =>
            {
                await _ordersManager.RefreshAsync();
                ShowSuccess("Đã làm mới danh sách đơn hàng");
            });
        }

        private async void btn_xemthongke_Click(object sender, EventArgs e)
        {
            // Mở form thống kê (có thể implement sau)
            MessageBox.Show("Tính năng thống kê đang được phát triển", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btn_caidat_Click(object sender, EventArgs e)
        {
            // Mở form cài đặt (có thể implement sau)
            MessageBox.Show("Tính năng cài đặt đang được phát triển", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void btn_luuthaydoi_Click(object sender, EventArgs e)
        {
            if (_selectedDish == null || _currentOrderDetail == null)
            {
                ShowWarning("Vui lòng chọn món cần cập nhật!");
                return;
            }

            string trangThaiMoi = ConvertDisplayToStatus(cb_status.SelectedItem?.ToString());
            int uuTienMoi = cb_uutien.SelectedIndex + 1; // 1-5
            string tenDauBep = cb_daubep.SelectedItem?.ToString();
            int maDauBep = GetMaDauBep(tenDauBep);

            if (string.IsNullOrEmpty(trangThaiMoi))
            {
                ShowWarning("Vui lòng chọn trạng thái!");
                return;
            }

            DateTime? thoiGianDuKien = null;
            if (!string.IsNullOrEmpty(cb_timedukien.SelectedItem?.ToString()))
            {
                try
                {
                    // Parse time từ string HH:mm
                    string timeString = cb_timedukien.SelectedItem.ToString();
                    if (TimeSpan.TryParse(timeString, out TimeSpan timeOfDay))
                    {
                        // Kết hợp với ngày hiện tại
                        thoiGianDuKien = DateTime.Today.Add(timeOfDay);

                        // Nếu thời gian đã qua trong ngày, cộng thêm 1 ngày
                        if (thoiGianDuKien < DateTime.Now)
                        {
                            thoiGianDuKien = thoiGianDuKien.Value.AddDays(1);
                        }
                    }
                }
                catch
                {
                    // Nếu parse lỗi, để null
                    thoiGianDuKien = null;
                }
            }

            await ExecuteAsync(btn_luuthaydoi, "Đang lưu...", async () =>
            {
                try
                {
                    var request = new UpdateDishStatusRequest
                    {
                        MaDonHang = _currentOrderDetail.MaDonHang,
                        MaChiTiet = _selectedDish.MaChiTiet,
                        TrangThaiMoi = trangThaiMoi,
                        MaNhanVienBep = maDauBep,
                        GhiChuBep = tb_ghichu.Text.Trim(),
                        ThoiGianDuKienHoanThanh = thoiGianDuKien,
                        UuTien = uuTienMoi,
                        GuiThongBao = true
                    };

                    var response = await SendRequest<UpdateDishStatusRequest, UpdateDishStatusResponse>(request);

                    if (response?.Success == true)
                    {
                        ShowSuccess($"Đã cập nhật trạng thái '{_selectedDish.TenMon}' thành '{response.TrangThaiMoi}'");

                        // Refresh danh sách món
                        await _dishesManager.RefreshAsync();

                        // Cập nhật lại danh sách đơn hàng
                        await _ordersManager.RefreshAsync();

                        // Ẩn panel cập nhật
                        panel_update.Visible = false;
                    }
                    else
                    {
                        ShowError(response?.Message ?? "Cập nhật thất bại");
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Lỗi cập nhật: {ex.Message}");
                }
            });
        }
        private async void btn_huymon_Click(object sender, EventArgs e)
        {
            if (_selectedDish == null || _currentOrderDetail == null)
            {
                ShowWarning("Vui lòng chọn món cần hủy!");
                return;
            }

            if (!Confirm($"Xác nhận hủy món '{_selectedDish.TenMon}'?"))
                return;

            await ExecuteAsync(btn_huymon, "Đang hủy...", async () =>
            {
                try
                {
                    var request = new UpdateDishStatusRequest
                    {
                        MaDonHang = _currentOrderDetail.MaDonHang,
                        MaChiTiet = _selectedDish.MaChiTiet,
                        TrangThaiMoi = "Huy",
                        MaNhanVienBep = _currentUserId,
                        GhiChuBep = $"Đã hủy: {tb_ghichu.Text.Trim()}",
                        GuiThongBao = true
                    };

                    var response = await SendRequest<UpdateDishStatusRequest, UpdateDishStatusResponse>(request);

                    if (response?.Success == true)
                    {
                        ShowSuccess($"Đã hủy món '{_selectedDish.TenMon}'");

                        // Refresh danh sách
                        await _dishesManager.RefreshAsync();
                        await _ordersManager.RefreshAsync();

                        // Ẩn panel cập nhật
                        panel_update.Visible = false;
                    }
                    else
                    {
                        ShowError(response?.Message ?? "Hủy thất bại");
                    }
                }
                catch (Exception ex)
                {
                    ShowError($"Lỗi hủy món: {ex.Message}");
                }
            });
        }

        private void btn_sendmess_Click(object sender, EventArgs e)
        {
            // Gửi tin nhắn cho phục vụ
            if (_currentOrderDetail == null)
            {
                ShowWarning("Không có đơn hàng nào được chọn!");
                return;
            }

            // Mở form chat (có thể implement sau)
            MessageBox.Show($"Tính năng chat với phục vụ bàn {_currentOrderDetail.TenBan} đang được phát triển",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tc_nvbep_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tc_nvbep.SelectedIndex == 0) // Quay lại tab quản lý
            {
                _currentOrderDetail = null;
                _selectedDish = null;
                panel_update.Visible = false;
            }
        }

        // ==================== HELPER METHODS ====================
        private async void LoadOrderDetail(int maDonHang)
        {
            try
            {
                _currentOrderDetail = new KitchenOrderDetailData { MaDonHang = maDonHang };

                // Hiển thị loading
                dataGridView_dishs.DataSource = null;
                dataGridView_dishs.Rows.Clear();

                // Refresh danh sách món
                bool success = await _dishesManager.LoadDataAsync();

                if (success)
                {
                    // Hiển thị thông tin đơn hàng
                    UpdateOrderDetailDisplay();

                    // Nếu có món, chọn món đầu tiên
                    var dishes = _dishesManager.GetAllDisplayedItems();
                    if (dishes.Count > 0)
                    {
                        dataGridView_dishs.Rows[0].Selected = true;
                        UpdateUpdatePanel(dishes[0]);
                    }
                }
                else
                {
                    ShowError("Không thể tải chi tiết đơn hàng");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi tải chi tiết: {ex.Message}");
            }
        }

        private string GetDishIcon(string tenMon)
        {
            // Map tên món với icon phù hợp
            if (tenMon.Contains("phở") || tenMon.Contains("Phở"))
                return "🍜";
            else if (tenMon.Contains("bún") || tenMon.Contains("Bún"))
                return "🍝";
            else if (tenMon.Contains("nước") || tenMon.Contains("Nước"))
                return "🍹";
            else if (tenMon.Contains("cơm") || tenMon.Contains("Cơm"))
                return "🍚";
            else if (tenMon.Contains("gỏi") || tenMon.Contains("Gỏi"))
                return "🥗";
            else
                return "🍽️";
        }

        private Color GetStatusColor(string trangThai)
        {
            return trangThai switch
            {
                "ChoXacNhan" => Color.Orange,
                "DangCheBien" => Color.DodgerBlue,
                "HoanThanh" => Color.Green,
                "CoVanDe" => Color.Red,
                "Huy" => Color.Gray,
                _ => Color.Black
            };
        }

        private Color GetDishStatusColor(string trangThai)
        {
            return trangThai switch
            {
                "ChoXacNhan" => Color.Orange,
                "DangCheBien" => Color.DodgerBlue,
                "HoanThanh" => Color.Green,
                "CoVanDe" => Color.Red,
                "Huy" => Color.Gray,
                _ => Color.Black
            };
        }

        private Color GetPriorityColor(int uuTien)
        {
            return uuTien switch
            {
                1 => Color.Gray,
                2 => Color.Orange,
                3 => Color.OrangeRed,
                4 => Color.Red,
                5 => Color.DarkRed,
                _ => Color.Black
            };
        }

        private string ConvertFilterToStatus(string filterText)
        {
            return filterText switch
            {
                "Tất cả" => "TatCa",
                "Chờ xác nhận" => "ChoXacNhan",
                "Đang chế biến" => "DangCheBien",
                "Hoàn thành" => "HoanThanh",
                "Có vấn đề" => "CoVanDe",
                "Hủy" => "Huy",
                _ => "TatCa"
            };
        }

        private string ConvertSortToServer(string sortText)
        {
            return sortText switch
            {
                "Thời gian" => "ThoiGian",
                "Ưu tiên" => "UuTien",
                "Bàn" => "Ban",
                "Thời gian chờ" => "ThoiGianCho",
                _ => "ThoiGian"
            };
        }

        private string ConvertStatusToDisplay(string trangThai)
        {
            return trangThai switch
            {
                "ChoXacNhan" => "⏳ Chờ xác nhận",
                "DangCheBien" => "👨‍🍳 Đang chế biến",
                "HoanThanh" => "✅ Hoàn thành",
                "CoVanDe" => "⚠️ Có vấn đề",
                "Huy" => "❌ Hủy",
                _ => trangThai
            };
        }

        private string ConvertDisplayToStatus(string displayText)
        {
            if (string.IsNullOrEmpty(displayText)) return "ChoXacNhan";

            return displayText switch
            {
                "⏳ Chờ xác nhận" => "ChoXacNhan",
                "👨‍🍳 Đang chế biến" => "DangCheBien",
                "✅ Hoàn thành" => "HoanThanh",
                "⚠️ Có vấn đề" => "CoVanDe",
                "❌ Hủy" => "Huy",
                _ => displayText.Replace(" ", "").Replace("⏳", "").Replace("👨‍🍳", "").Replace("✅", "").Replace("⚠️", "").Replace("❌", "")
            };
        }

        private string GetStatusDisplayItem(string trangThai)
        {
            return trangThai switch
            {
                "ChoXacNhan" => "⏳ Chờ xác nhận",
                "DangCheBien" => "👨‍🍳 Đang chế biến",
                "HoanThanh" => "✅ Hoàn thành",
                "CoVanDe" => "⚠️ Có vấn đề",
                "Huy" => "❌ Hủy",
                _ => "⏳ Chờ xác nhận"
            };
        }

        private int GetMaDauBep(string tenDauBep)
        {
            if (string.IsNullOrEmpty(tenDauBep)) return _currentUserId;

            var dauBep = _danhSachDauBep?.FirstOrDefault(d =>
                d.HoTen != null && d.HoTen.Equals(tenDauBep, StringComparison.OrdinalIgnoreCase));

            return dauBep?.MaNguoiDung ?? _currentUserId;
        }
        // ==================== NETWORK & EXECUTION METHODS ====================
        // ĐỊNH NGHĨA LẠI HÀM CŨ: Thêm CancellationToken
        private static async Task<TResponse> SendRequest<TRequest, TResponse>(
            TRequest request, CancellationToken cancellationToken = default)
        {
            string json = JsonConvert.SerializeObject(request) + "\n";

            using (var client = new TcpClient())
            {
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                // Sử dụng ConnectAsync với Cancellation Token nếu Framework hỗ trợ, 
                // nếu không, ta phải tự quản lý timeout.
                // Dùng Task.Run để chuyển block sang background thread nếu cần (tạm thời không dùng để giữ nguyên logic gốc)
                await client.ConnectAsync(SERVER_IP, SERVER_PORT); // Không có CT, dùng Task.Delay để mô phỏng timeout

                // Nếu bạn đang dùng .NET Core/5+ thì có thể dùng Task.WaitAsync(timeout)
                // Với WinForms cổ điển, ta phụ thuộc vào ReceiveTimeout/SendTimeout của TcpClient.

                using (var stream = client.GetStream())
                using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Bỏ dòng FIX cũ, sử dụng cách gọi WriteLineAsync đơn giản hơn
                    await writer.WriteLineAsync(json.TrimEnd('\n'));

                    // Đọc phản hồi (vẫn phụ thuộc vào ReceiveTimeout)
                    string responseJson = await reader.ReadLineAsync();

                    if (responseJson == null)
                        throw new TimeoutException("Không nhận được phản hồi hoặc kết nối bị đóng.");

                    return JsonConvert.DeserializeObject<TResponse>(responseJson);
                }
            }
        }

        // ==================== DATA LOADING METHODS ====================
        private async Task<List<KitchenOrderData>> LoadKitchenOrdersFromServer()
        {
            try
            {
                var request = new GetKitchenOrdersRequest
                {
                    TrangThai = ConvertFilterToStatus(comboBox1.SelectedItem?.ToString()),
                    TimKiemBan = tb_numberTable.Text.Trim(),
                    SapXep = ConvertSortToServer(cb_sapxep.SelectedItem?.ToString()),
                    MaNhanVienBep = _currentUserId
                };

                var response = await SendRequest<GetKitchenOrdersRequest, GetKitchenOrdersResponse>(request);

                if (response?.Success == true)
                {
                    var orders = response.DonHang;

                    // FIX: Sắp xếp Thời gian chờ trên Client
                    if (ConvertSortToServer(cb_sapxep.SelectedItem?.ToString()) == "ThoiGianCho")
                    {
                        // Thời gian chờ càng lâu (NgayOrder càng cũ) thì càng ưu tiên lên đầu
                        orders = orders.OrderBy(o => o.NgayOrder).ToList();
                    }

                    UpdateThongKeDisplay(response.ThongKe);
                    return orders;
                }
                else
                {
                    ShowError(response?.Message ?? "Không thể tải danh sách đơn hàng");
                    return new List<KitchenOrderData>();
                }
            }
            catch (SocketException)
            {
                ShowError("Không thể kết nối đến server (127.0.0.1:5000).");
                return new List<KitchenOrderData>();
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi tải đơn hàng: {ex.Message}");
                return new List<KitchenOrderData>();
            }
        }

        private async Task ExecuteAsync(Button button, string loadingText, Func<Task> action)
        {
            string originalText = button.Text;
            button.Enabled = false;
            button.Text = loadingText;
            Cursor = Cursors.WaitCursor;

            try
            {
                await action();
            }
            catch (SocketException)
            {
                ShowError("Không thể kết nối đến server!");
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi: {ex.Message}");
            }
            finally
            {
                button.Enabled = true;
                button.Text = originalText;
                Cursor = Cursors.Default;
            }
        }

        private bool Confirm(string message)
        {
            return MessageBox.Show(message, "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Cảnh báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // ==================== FORM EVENTS ====================
        private void NVBep_Load(object sender, EventArgs e)
        {
            // Center the form
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void NVBep_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the timer
            if (_autoRefreshTimer != null)
            {
                _autoRefreshTimer.Stop();
                _autoRefreshTimer.Dispose();
            }
        }
      

    }
}