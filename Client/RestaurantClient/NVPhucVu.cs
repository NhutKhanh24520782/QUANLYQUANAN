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
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RestaurantClient
{
    public partial class NVPhucVu : Form
    {
        // ==================== CONSTANTS & FIELDS ====================
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;
        private const string SEARCH_BILL_PLACEHOLDER = "Tìm theo mã hóa đơn...";

        private int _currentUserId;
        private string _currentUserName;
        private List<CategoryData> _danhSachLoaiMon;
        private List<BanAnData> _danhSachBan;
        private List<CartItem> _gioHang = new List<CartItem>();
        private GridViewManager<PendingPaymentData> _billManager;
        private GridViewManager<MenuItemData> _ordermonManager;
        private System.Windows.Forms.Timer _checkPaymentTimer;
        private int _pendingMaHD = 0; // Lưu mã HD đang chờ
        private System.Windows.Forms.Timer _autoRefreshTimer; // 🔥 ĐÃ ĐƯỢC SỬ DỤNG
        private System.Windows.Forms.Timer? _clockTimer; // ✅ THÊM DÒNG NÀY
        #region CHAT FIELDS
        private System.Windows.Forms.Timer _chatRefreshTimer;      // Timer polling tin nhắn mới
        private int _selectedChatUserId = 0;                        // ID user đang chat
        private string _selectedChatUserName = "";                  // Tên user đang chat
        private DateTime _lastMessageTime = DateTime.MinValue;      // Thời gian tin nhắn cuối
        private List<ChatUserData> _chatUsers = new List<ChatUserData>();
        private const int CHAT_REFRESH_INTERVAL = 3000;             // 3 giây polling
        private HashSet<int> _displayedMessageIds = new HashSet<int>();
        private HashSet<int> _notifiedMessageIds = new HashSet<int>(); // Track tin đã popup
        #endregion

        // ==================== INITIALIZATION ====================
        public NVPhucVu(int userId, string userName)
        {
            _currentUserId = userId;
            _currentUserName = userName;
            InitializeComponent();
            cb_trangthai.Items.Clear();
            cb_trangthai.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_trangthai.Items.AddRange(new string[] { "Tất cả", "Hoàn thành", "Đang chế biến" });
      
            cb_banan.SelectedIndex = -1;
            cb_trangthai.SelectedIndex = -1;
            pb_QR.Click += pb_QR_Click;
            cb_trangthai.SelectedIndexChanged += (s, e) => btn_lammoi_Click_1(null, null);
            SetupMasterDetailView();
            InitializeGridViewManager();
            InitializePaymentControls();
            InitializeAutoRefreshTimer(); // 🔥 BỔ SUNG: Khởi tạo Timer
            _checkPaymentTimer = new System.Windows.Forms.Timer();
            _checkPaymentTimer.Interval = 3000; // Kiểm tra mỗi 3 giây
            _checkPaymentTimer.Tick += CheckPaymentTimer_Tick;
            LoadPendingBills();
            LoadMenuItems();
            InitializeCategoryComboBox();
            InitializeTableComboBox();
            InitializeClockTimer(); // ✅ THÊM DÒNG NÀY
            UpdateUserInfo();
            LoadNVInfo();
            InitializeChatFeature();

        }
        // 🔥 THÊM HÀM CHUYỂN ĐỔI MÚI GIỜ
        
        private void InitializeClockTimer()
        {
            _clockTimer = new System.Windows.Forms.Timer();
            _clockTimer.Interval = 1000; // Cập nhật mỗi giây
            _clockTimer.Tick += (s, e) =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(UpdateUserInfo));
                }
                else
                {
                    UpdateUserInfo();
                }
            };
            _clockTimer.Start();
        }
        private void SetupMasterDetailView()
        {
            // --- CẤU HÌNH BẢNG ĐƠN HÀNG (BÊN TRÁI) ---
            dgv_DonHangTongQuan.AutoGenerateColumns = false;
            dgv_DonHangTongQuan.Columns.Clear();
            dgv_DonHangTongQuan.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Đăng ký sự kiện tô màu (QUAN TRỌNG)
            dgv_DonHangTongQuan.CellFormatting -= Dgv_DonHangTongQuan_CellFormatting; // Xóa cũ để tránh trùng
            dgv_DonHangTongQuan.CellFormatting += Dgv_DonHangTongQuan_CellFormatting; // Thêm mới

            // Thêm các cột (DataPropertyName phải khớp với KitchenOrderData)
            dgv_DonHangTongQuan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaDonHang", HeaderText = "Mã Đơn", Width = 80 });
            dgv_DonHangTongQuan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TenBan", HeaderText = "Bàn", Width = 70 });
            dgv_DonHangTongQuan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ThoiGianDisplay", HeaderText = "Giờ gọi", Width = 100 });

            // Cột trạng thái (Width 140 để đủ chỗ hiển thị chữ)
            dgv_DonHangTongQuan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TrangThaiDon", HeaderText = "Trạng Thái", Width = 250 });

            dgv_DonHangTongQuan.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TongSoMon", HeaderText = "Số Món", Width = 60 });

            dgv_DonHangTongQuan.SelectionChanged += Dgv_DonHangTongQuan_SelectionChanged;

            // --- CẤU HÌNH BẢNG CHI TIẾT (BÊN PHẢI) ---
            lv_ChiTietDon.Columns.Clear();
            lv_ChiTietDon.View = View.Details;
            lv_ChiTietDon.GridLines = true;
            lv_ChiTietDon.FullRowSelect = true;

            lv_ChiTietDon.Columns.Add("Tên Món", 220);
            lv_ChiTietDon.Columns.Add("SL", 40);
            lv_ChiTietDon.Columns.Add("Ghi Chú", 400);
            lv_ChiTietDon.Columns.Add("Trạng Thái", 120);

            // Tạo Group
            lv_ChiTietDon.Groups.Add(new ListViewGroup("HoanThanh", "[1] MÓN ĐÃ HOÀN THÀNH"));
            lv_ChiTietDon.Groups.Add(new ListViewGroup("DangCheBien", "[2] MÓN ĐANG CHẾ BIẾN"));
            lv_ChiTietDon.Groups.Add(new ListViewGroup("ChoXacNhan", "[3] MÓN CHỜ XÁC NHẬN"));
            lv_ChiTietDon.Groups.Add(new ListViewGroup("CoVanDe", "[4] MÓN CÓ VẤN ĐỀ / HỦY"));
        }

        private async void Dgv_DonHangTongQuan_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_DonHangTongQuan.SelectedRows.Count == 0) return;

            // Lấy object data từ dòng đang chọn
            var selectedOrder = dgv_DonHangTongQuan.SelectedRows[0].DataBoundItem as KitchenOrderData;
            if (selectedOrder == null) return;

            await LoadOrderDetailToListView(selectedOrder.MaDonHang);
        }

        private async Task LoadOrderDetailToListView(int maDonHang)
        {
            try
            {
                var request = new GetOrderDetailRequest { MaDonHang = maDonHang };
                var response = await SendRequest<GetOrderDetailRequest, GetOrderDetailResponse>(request);

                if (response != null && response.Success && response.ChiTietDonHang != null)
                {
                    lv_ChiTietDon.Items.Clear();
                    var details = response.ChiTietDonHang.DanhSachMon;

                    foreach (var item in details)
                    {
                        // Tạo dòng cho ListView
                        ListViewItem row = new ListViewItem(item.TenMon);
                        row.SubItems.Add(item.SoLuong.ToString());
                        row.SubItems.Add(item.GhiChuKhach); // Hoặc GhiChuBep
                        row.SubItems.Add(TranslateStatus(item.TrangThai));

                        // 1. PHÂN NHÓM (GROUP)
                        switch (item.TrangThai)
                        {
                            case "HoanThanh":
                                row.Group = lv_ChiTietDon.Groups["HoanThanh"];
                                row.ForeColor = Color.DarkGreen; // Chữ xanh
                                row.BackColor = Color.LightGreen;   // Nền xanh nhạt
                                row.ImageKey = "check"; // Nếu bạn có ImageList
                                break;

                            case "DangCheBien":
                                row.Group = lv_ChiTietDon.Groups["DangCheBien"];
                                row.ForeColor = Color.DarkGoldenrod; // Chữ vàng đậm
                                row.BackColor = Color.LightYellow;   // Nền vàng nhạt
                                break;

                            case "ChoXacNhan":
                                row.Group = lv_ChiTietDon.Groups["ChoXacNhan"];
                                row.ForeColor = Color.Gray;
                                row.BackColor = Color.LightGray; // Nền xám nhạt
                                break;

                            case "CoVanDe":
                            case "Huy":
                                row.Group = lv_ChiTietDon.Groups["CoVanDe"];
                                row.ForeColor = Color.DarkRed;
                                row.BackColor = Color.LightCoral; // Nền đỏ nhạt
                                row.Font = new Font(lv_ChiTietDon.Font, FontStyle.Strikeout); // Gạch ngang nếu hủy
                                break;
                        }

                        lv_ChiTietDon.Items.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tải chi tiết: " + ex.Message);
            }
        }

        private string TranslateStatus(string status)
        {
            return status switch
            {
                "HoanThanh" => "Đã xong",
                "DangCheBien" => "Đang làm",
                "ChoXacNhan" => "Chờ bếp",
                "CoVanDe" => "Có sự cố",
                "Huy" => "Đã hủy",
                _ => status
            };
        }
        private void InitializeGridViewManager()
        {
            // Khởi tạo GridViewManager với PendingPaymentData
            _billManager = new GridViewManager<PendingPaymentData>(
                dataGridView_thanhtoan,
                LoadPendingBillsFromServer,
                payment => new
                {
                    MaHD = payment.MaHD,
                    MaBanAn = payment.MaBanAn,
                    TenBan = payment.TenBan,
                    MaNhanVien = payment.MaNhanVien,
                    TenNhanVien = payment.TenNhanVien,
                    NgayTao = payment.NgayTao,
                    TongTien = payment.TongTien,
                    SoMon = payment.SoMon,
                    TrangThai = ConvertBillStatusToVietnamese(payment.TrangThai)
                },
                "MaHD"
            );

            // Event handlers
            dataGridView_thanhtoan.SelectionChanged += DataGridView_Bills_SelectionChanged;
            dataGridView_thanhtoan.CellFormatting += DataGridView_Bills_CellFormatting;

            // Sort sẽ được gọi sau khi load data lần đầu (trong LoadPendingBills)
            _ordermonManager = new GridViewManager<MenuItemData>(
                dataGridView_mon,
                LoadMenuFromServer,
                mon => new
                {
                    MaMon = mon.MaMon,
                    TenMon = mon.TenMon,
                    Gia = mon.Gia,
                    TrangThai = mon.TrangThai,
                    MaLoaiMon = mon.MaLoaiMon
                },
                "MaMon"
            );
            dataGridView_mon.CellFormatting += DataGridView_Menu_CellFormatting;
        }
        private void DataGridView_Menu_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView_mon.Columns["Gia"] != null &&
                e.ColumnIndex == dataGridView_mon.Columns["Gia"].Index &&
                e.Value != null)
            {

                if (decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("N0") + " VNĐ";
                    e.FormattingApplied = true;
                }
                DataGridViewRow row = dataGridView_mon.Rows[e.RowIndex];
                if (row.Cells["TrangThai"].Value != null)
                {
                    string status = row.Cells["TrangThai"].Value.ToString();
                    if (status == "ConMon")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.SelectionBackColor = Color.Green;
                    }
                    else if (status == "HetMon")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                        row.DefaultCellStyle.SelectionBackColor = Color.Red;
                    }
                }
            }
        }
        private async void LoadMenuItems()
        {
            try
            {
                await _ordermonManager.LoadDataAsync();

                // Kiểm tra kết quả
                var menuData = _ordermonManager.GetCachedData();
                if (menuData?.Count > 0)
                {
                    Console.WriteLine($"✅ Đã tải {menuData.Count} món ăn khi khởi động");
                }
                else
                {
                    Console.WriteLine("⚠️ Không có món ăn nào được tải");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi tải menu: {ex.Message}");
                // Có thể show thông báo nhẹ nếu cần
            }
        }
        private async Task<List<MenuItemData>> LoadMenuFromServer()
        {
            try
            {
                var request = new GetMenuRequest { };
                var response = await SendRequest<GetMenuRequest, GetMenuResponse>(request);

                if (response?.Success == true)
                {
                    // 🔥 DEBUG: Log số lượng món ăn nhận được
                    Console.WriteLine($"Nhận được {response.Items?.Count ?? 0} món ăn từ server");

                    var sortedMenu = response.Items?
                        .OrderBy(m => m.MaMon)
                        .ToList() ?? new List<MenuItemData>();

                    return sortedMenu;
                }
                else
                {
                    ShowError(response?.Message ?? "Không thể tải danh sách món ăn");
                    return new List<MenuItemData>();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi tải menu: {ex.Message}");
                return new List<MenuItemData>();
            }
        }
        private void InitializeCategoryComboBox()
        {
            if (cb_nameDish == null) return;

            cb_nameDish.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_nameDish.SelectedIndexChanged += Cb_nameDish_SelectedIndexChanged;

            LoadCategories();
        }
        private async void LoadCategories()
        {
            try
            {
                var request = new GetCategoriesRequest { };
                var response = await SendRequest<GetCategoriesRequest, GetCategoriesResponse>(request);

                if (response?.Success == true && response.Categories != null)
                {
                    _danhSachLoaiMon = response.Categories;

                    var displayList = new List<CategoryData>
            {
                new CategoryData { MaLoaiMon = 0, TenLoai = "Tất cả các món" }
            };
                    displayList.AddRange(_danhSachLoaiMon);

                    cb_nameDish.DataSource = displayList;
                    cb_nameDish.DisplayMember = "TenLoai";
                    cb_nameDish.ValueMember = "MaLoaiMon";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải loại món: {ex.Message}");
            }
        }
        private async void Cb_nameDish_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_nameDish.SelectedValue == null) return;

            try
            {
                int selectedCategoryId = (int)cb_nameDish.SelectedValue;

                if (selectedCategoryId == 0)
                {
                    await _ordermonManager.LoadDataAsync();
                }
                else
                {
                    await FilterMenuByCategory(selectedCategoryId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lọc món: {ex.Message}");
            }
        }
        private async Task FilterMenuByCategory(int categoryId)
        {
            try
            {
                var request = new GetMenuByCategoryRequest
                {
                    MaLoaiMon = categoryId
                };
                var response = await SendRequest<GetMenuByCategoryRequest, GetMenuResponse>(request);

                if (response?.Success == true)
                {
                    _ordermonManager.UpdateDataSource(response.Items);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lọc món từ server: {ex.Message}");
            }
        }
        //================== combobox chọn bàn
        private void InitializeTableComboBox()
        {
            if (cb_banOrder == null) return;

            cb_banOrder.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_banOrder.SelectedIndexChanged += Cb_banOrder_SelectedIndexChanged;

            if (cb_banan == null) return;

            cb_banan.DropDownStyle = ComboBoxStyle.DropDownList;
            cb_banan.SelectedIndexChanged += cb_banan_SelectedIndexChanged;

            LoadTables();

        }
        private async void LoadTables()
        {
            try
            {
                var request = new GetTablesRequest { };
                var response = await SendRequest<GetTablesRequest, GetTablesResponse>(request);

                if (response?.Success == true && response.ListBan != null)
                {
                    _danhSachBan = response.ListBan
                    .Where(b => b.TrangThai != "An")
                    .ToList();

                    // Hiển thị tên bàn và mã bàn
                    cb_banOrder.DataSource = _danhSachBan;
                    cb_banOrder.DisplayMember = "TenBan";
                    cb_banOrder.ValueMember = "MaBanAn";

                    cb_banan.DataSource = _danhSachBan;
                    cb_banan.DisplayMember = "TenBan";
                    cb_banan.ValueMember = "MaBanAn";

                    Console.WriteLine($"✅ Đã tải {_danhSachBan.Count} bàn ăn");

                    // Tự động chọn bàn đầu tiên nếu có
                    if (_danhSachBan.Count > 0)
                    {
                        cb_banOrder.SelectedIndex = 0;

                    }
                    cb_banan.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi tải danh sách bàn: {ex.Message}");
                ShowError("Không thể tải danh sách bàn ăn");
            }
        }
        private void Cb_banOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_banOrder.SelectedValue == null) return;

            try
            {
                int selectedTableId = (int)cb_banOrder.SelectedValue;
                var selectedTable = _danhSachBan.FirstOrDefault(b => b.MaBanAn == selectedTableId);

                if (selectedTable != null)
                {
                    UpdateTableStatusDisplay(selectedTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi hiển thị trạng thái bàn: {ex.Message}");
            }
        }
        private void cb_banan_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_banan.SelectedValue == null) return;

            try
            {
                int selectedTableId = (int)cb_banan.SelectedValue;
                var selectedTable = _danhSachBan.FirstOrDefault(b => b.MaBanAn == selectedTableId);

                if (selectedTable != null)
                {
                    UpdateTableStatusDisplay(selectedTable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi hiển thị trạng thái bàn: {ex.Message}");
            }
        }
        private void UpdateTableStatusDisplay(BanAnData table)
        {
            if (lbl_trangthaiban == null) return;

            string statusText = "";
            Color statusColor = Color.Black;

            switch (table.TrangThai)
            {
                case "Trong":
                    statusText = " Bàn trống";
                    statusColor = Color.Green;
                    break;
                case "DangSuDung":
                    statusText = " Đang có khách";
                    statusColor = Color.Orange;
                    break;
                case "DaDat":
                    statusText = " Đã đặt trước";
                    statusColor = Color.Blue;
                    break;
                case "An":
                    statusText = " Đóng";
                    statusColor = Color.Gray;
                    break;
                default:
                    statusText = table.TrangThai ?? "Không xác định";
                    statusColor = Color.Black;
                    break;
            }

            lbl_trangthaiban.Text = $"{statusText}";
            //lbl_trangthaiban.ForeColor = statusColor;

            // Hiển thị thêm thông tin nếu có
            if (table.SoChoNgoi.HasValue)
            {
                lbl_trangthaiban.Text += $" | {table.SoChoNgoi} chỗ";
            }
        }
        //=============== THÊM MÓN ==============
        private void btn_themmon_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedMon = _ordermonManager.GetSelectedItem();

                if (selectedMon == null)
                {
                    ShowWarning("Vui lòng chọn món cần thêm!");
                    return;
                }

                // Lấy số lượng từ numeric
                int soLuong = (int)nm_soluong.Value;

                if (soLuong <= 0)
                {
                    ShowWarning("Số lượng phải lớn hơn 0!");
                    return;
                }

                if (selectedMon.TrangThai == "ConMon")
                {
                    AddToCart(selectedMon, soLuong);

                    ShowSuccess($"Đã thêm {soLuong} '{selectedMon.TenMon}' vào giỏ hàng");

                    // Reset số lượng về 1 sau khi thêm
                    nm_soluong.Value = 1;
                }
                else
                {
                    ShowWarning("Đã hết món đang chọn !");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi thêm món: {ex.Message}");
            }
        }
        private void AddToCart(MenuItemData mon, int soLuong)
        {
            // Kiểm tra xem món đã có trong giỏ chưa
            var existingItem = _gioHang.FirstOrDefault(item => item.MaMon == mon.MaMon);

            if (existingItem != null)
            {
                // Nếu đã có, cộng thêm số lượng
                existingItem.SoLuong += soLuong;
            }
            else
            {
                // Nếu chưa có, thêm mới với số lượng
                _gioHang.Add(new CartItem
                {
                    MaMon = mon.MaMon,
                    TenMon = mon.TenMon,
                    Gia = mon.Gia,
                    SoLuong = soLuong
                });
            }

            // Cập nhật hiển thị giỏ hàng
            UpdateCartDisplay();
        }
        private void UpdateCartDisplay()
        {
            try
            {
                // Hiển thị lên dataGridView_giohang
                var displayData = _gioHang.Select(item => new
                {
                    MaMon = item.MaMon,
                    TenMon = item.TenMon,
                    DonGia = item.Gia,
                    SoLuong = item.SoLuong
                }).ToList();

                if (dataGridView_giohang.InvokeRequired)
                {
                    dataGridView_giohang.Invoke(new Action(() =>
                    {
                        dataGridView_giohang.DataSource = displayData;
                        FormatCartGridView();
                    }));
                }
                else
                {
                    dataGridView_giohang.DataSource = displayData;
                    FormatCartGridView();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật giỏ hàng: {ex.Message}");
            }
        }
        private void FormatCartGridView()
        {
            if (dataGridView_giohang.Columns.Count == 0) return;
            if (dataGridView_giohang.Columns["DonGia"] != null)
            {
                dataGridView_giohang.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dataGridView_giohang.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridView_giohang.Columns["SoLuong"] != null)
            {
                dataGridView_giohang.Columns["SoLuong"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dataGridView_giohang.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        //=================== xóa món ===========
        private void btn_xoamon_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra có dòng nào được chọn trong giỏ hàng không
                if (dataGridView_giohang.SelectedRows.Count == 0)
                {
                    ShowWarning("Vui lòng chọn món cần xóa trong giỏ hàng!");
                    return;
                }

                // Lấy mã món từ dòng được chọn
                var selectedRow = dataGridView_giohang.SelectedRows[0];
                int maMon = Convert.ToInt32(selectedRow.Cells["MaMon"].Value);
                string tenMon = selectedRow.Cells["TenMon"].Value.ToString();

                // Xác nhận xóa
                if (Confirm($"Bạn có chắc muốn xóa '{tenMon}' khỏi giỏ hàng?"))
                {
                    // Xóa món khỏi giỏ hàng
                    RemoveFromCart(maMon);
                    ShowSuccess($"Đã xóa '{tenMon}' khỏi giỏ hàng");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi xóa món: {ex.Message}");
            }
        }
        private void RemoveFromCart(int maMon)
        {
            // Tìm và xóa món khỏi giỏ hàng
            var itemToRemove = _gioHang.FirstOrDefault(item => item.MaMon == maMon);
            if (itemToRemove != null)
            {
                _gioHang.Remove(itemToRemove);

                // Cập nhật hiển thị giỏ hàng
                UpdateCartDisplay();
            }
        }
        // 🔥 BỔ SUNG: Khởi tạo Auto Refresh Timer
        private void InitializeAutoRefreshTimer()
        {
            _autoRefreshTimer = new System.Windows.Forms.Timer();
            _autoRefreshTimer.Interval = 30000; // 30 giây
            _autoRefreshTimer.Tick += async (s, e) =>
            {
                // Chỉ làm mới nếu form đang hiển thị và không có ô nào được chọn
                if (this.Visible && dataGridView_thanhtoan.SelectedRows.Count == 0)
                {
                    await _billManager.RefreshAsync();
                }
            };
            _autoRefreshTimer.Start();
        }

        private void InitializePaymentControls()
        {
            // Setup payment method checkboxes - CHỈ CHO CHỌN 1
            checkBox_tienmat.CheckedChanged += PaymentMethod_CheckedChanged;
            checkBox_chuyenkhoan.CheckedChanged += PaymentMethod_CheckedChanged;

            // 🔥 BẮT ĐẦU VỚI TIỀN MẶT ĐƯỢC CHỌN VÀ CHUYỂN KHOẢN KHÔNG ĐƯỢC CHỌN
            checkBox_tienmat.Checked = true;
            checkBox_chuyenkhoan.Checked = false;

            SetupSearchBox();

            btn_ttoan.Enabled = false;
            UpdateWelcomeMessage();
            InitializeInfoTextBoxes();
            InitializeTongTienLabel();
        }
        private void InitializeInfoTextBoxes()
        {
            // Đảm bảo các textbox có style nhất quán
            var infoTextBoxes = new[] { tb_idBill, tb_idTable, tb_dateBill, tb_tongtien };
            foreach (var textBox in infoTextBoxes)
            {
                if (textBox != null)
                {
                    textBox.ReadOnly = true;
                    textBox.BackColor = Color.WhiteSmoke;
                    textBox.Font = new Font("Segoe UI", 9f, FontStyle.Regular); // Không in đậm
                }
            }
        }

        private void InitializeTongTienLabel()
        {
            // Đảm bảo label tổng tiền có style giống form Admin
            if (label_tongtien != null)
            {
                label_tongtien.Text = "0 VNĐ";
                label_tongtien.ForeColor = Color.Red;
                label_tongtien.Font = new Font("Segoe UI", 10f, FontStyle.Regular); // Không in đậm
                label_tongtien.TextAlign = ContentAlignment.MiddleCenter;
            }
        }
        private void PaymentMethod_CheckedChanged(object sender, EventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox == null) return;

            if (checkbox.Checked)
            {
                // Nếu checkbox này được chọn, bỏ chọn checkbox kia
                if (checkbox == checkBox_tienmat)
                {
                    checkBox_chuyenkhoan.Checked = false;
                }
                else if (checkbox == checkBox_chuyenkhoan)
                {
                    checkBox_tienmat.Checked = false;
                    // Hiển thị thông báo về QR code
                    ShowInfo("Sau khi thanh toán, hệ thống sẽ hiển thị QR code để khách hàng quét.");
                }
            }
            else
            {
                // 🔥 SỬA LỖI: Đảm bảo luôn có ít nhất một phương thức được chọn
                if (checkbox == checkBox_tienmat && !checkBox_chuyenkhoan.Checked)
                {
                    // Tự động chọn lại TienMat nếu cả hai đều bị bỏ chọn
                    checkBox_tienmat.Checked = true;
                }
                else if (checkbox == checkBox_chuyenkhoan && !checkBox_tienmat.Checked)
                {
                    // Tự động chọn lại TienMat nếu cả hai đều bị bỏ chọn
                    checkBox_tienmat.Checked = true;
                }
            }
        }

        private void UpdateWelcomeMessage()
        {
            // Tìm và cập nhật label chào mừng
            var welcomeLabel = FindControlRecursive<Label>(this, "lblWelcome");
            if (welcomeLabel != null)
            {
                welcomeLabel.Text = $"Xin chào: {_currentUserName} - Nhân viên phục vụ";
                welcomeLabel.Font = new Font("Segoe UI", 9f, FontStyle.Regular); // Không in đậm
            }
        }

        private void SetupSearchBox()
        {
            var searchBox = FindControlRecursive<TextBox>(this, "tb_searchBill");
            if (searchBox != null)
            {
                searchBox.Text = SEARCH_BILL_PLACEHOLDER;
                searchBox.ForeColor = Color.Gray;
                searchBox.Font = new Font("Segoe UI", 9f, FontStyle.Regular); // Không in đậm

                searchBox.Enter += (s, e) =>
                {
                    if (searchBox.Text == SEARCH_BILL_PLACEHOLDER)
                    {
                        searchBox.Text = "";
                        searchBox.ForeColor = Color.Black;
                    }
                };

                searchBox.Leave += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(searchBox.Text))
                    {
                        searchBox.Text = SEARCH_BILL_PLACEHOLDER;
                        searchBox.ForeColor = Color.Gray;
                    }
                };

                // Cho phép tìm kiếm bằng Enter
                searchBox.KeyPress += (s, e) =>
                {
                    if (e.KeyChar == (char)Keys.Enter)
                    {
                        btn_searchBill_Click(s, e);
                        e.Handled = true;
                    }
                };
            }
        }

        // Helper method để tìm control đệ quy
        private T FindControlRecursive<T>(Control parent, string controlName) where T : Control
        {
            foreach (Control control in parent.Controls)
            {
                if (control is T && control.Name == controlName)
                    return (T)control;

                var found = FindControlRecursive<T>(control, controlName);
                if (found != null)
                    return found;
            }
            return null;
        }

        // ==================== DATA LOADING ====================
        // Trong NVPhucVu.cs

        private async Task<List<PendingPaymentData>> LoadPendingBillsFromServer()
        {
            try
            {
                var request = new GetPendingPaymentsRequest
                {
                    MaNhanVien = _currentUserId // Chỉ hiển thị bill của nhân viên này
                };

                var response = await SendRequest<GetPendingPaymentsRequest, GetPendingPaymentsResponse>(request);

                if (response?.Success == true)
                {
                    var convertedPayments = response.PendingPayments
                        .Select(p =>
                        {
                            // Chuyển từ UTC sang giờ Việt Nam để hiển thị
                            p.NgayTao = (p.NgayTao);
                            return p;
                        })
                        .OrderBy(p => p.MaHD)
                        .ToList();

                    UpdateStatusLabel(convertedPayments.Count);
                    return convertedPayments;
                }
                else
                {
                    ShowError(response?.Message ?? "Không thể tải danh sách hóa đơn");
                    return new List<PendingPaymentData>();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi kết nối: {ex.Message}");
                return new List<PendingPaymentData>();
            }
        }
        private void UpdateStatusLabel(int count)
        {
            // Tìm status label trong controls
            var statusLabel = FindControlRecursive<Label>(this, "lblStatus");
            if (statusLabel != null)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        statusLabel.Text = $"Đang có {count} hóa đơn chờ thanh toán";
                        statusLabel.ForeColor = count > 0 ? Color.Red : Color.Green;
                        statusLabel.Font = new Font("Segoe UI", 9f, FontStyle.Regular); // Không in đậm
                    }));
                }
                else
                {
                    statusLabel.Text = $"Đang có {count} hóa đơn chờ thanh toán";
                    statusLabel.ForeColor = count > 0 ? Color.Red : Color.Green;
                    statusLabel.Font = new Font("Segoe UI", 9f, FontStyle.Regular); // Không in đậm
                }
            }
        }
        // ✅ SỬA HÀM UpdateUserInfo để hiển thị giây
        private void UpdateUserInfo()
        {
            if (lbl_userInfo.InvokeRequired)
            {
                lbl_userInfo.Invoke(new Action(UpdateUserInfo));
                return;
            }

            // Dùng giờ Việt Nam thay vì DateTime.Now
            DateTime vietnamTime = GetVietnamTime();
            lbl_userInfo.Text = $"Chào, {_currentUserName} • {vietnamTime:HH:mm:ss dd/MM/yyyy}";
        }

        // ==================== EVENT HANDLERS ====================
        private void DataGridView_Bills_SelectionChanged(object sender, EventArgs e)
        {
            var selectedPayment = _billManager.GetSelectedItem();
            if (selectedPayment != null)
            {
                ShowBillDetails(selectedPayment);
                btn_ttoan.Enabled = true;
            }
            else
            {
                btn_ttoan.Enabled = false;
                ClearBillDetails();
            }
        }

        private void DataGridView_Bills_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView_thanhtoan.Columns["TrangThai"] != null)
            {
                DataGridViewRow row = dataGridView_thanhtoan.Rows[e.RowIndex];
                if (row.Cells["TrangThai"].Value != null)
                {
                    string status = row.Cells["TrangThai"].Value.ToString();
                    //Color backColor = GetBillStatusColor(status);

                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.SelectionBackColor = Color.Green; // Màu khi được chọn
                }
            }

            // Định dạng cột tổng tiền
            if (e.ColumnIndex == dataGridView_thanhtoan.Columns["TongTien"].Index && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = value.ToString("N0") + " VNĐ";
                    e.FormattingApplied = true;
                }
            }

            if (e.ColumnIndex == dataGridView_thanhtoan.Columns["NgayTao"].Index && e.Value != null)
            {
                if (DateTime.TryParse(e.Value.ToString(), out DateTime date))
                {
                    // Chuyển sang giờ Việt Nam trước khi hiển thị
                    DateTime vnTime = (date);
                    e.Value = vnTime.ToString("HH:mm dd/MM/yyyy");
                    e.FormattingApplied = true;
                }
            }

            // Đảm bảo font không in đậm
            if (e.RowIndex >= 0 && dataGridView_thanhtoan.Rows[e.RowIndex].DefaultCellStyle.Font != null)
            {
                dataGridView_thanhtoan.Rows[e.RowIndex].DefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
            }
        }

        private void ShowBillDetails(PendingPaymentData payment)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<PendingPaymentData>(ShowBillDetails), payment);
                    return;
                }
                // 🔥 SỬA LỖI: Đảm bảo hiển thị đúng thời gian đã được chuyển đổi
                if (tb_dateBill != null)
                {
                    // Hiển thị giờ Việt Nam
                    DateTime displayTime = (payment.NgayTao);
                    tb_dateBill.Text = displayTime.ToString("HH:mm dd/MM/yyyy");
                }

                if (tb_idBill != null)
                {
                    tb_idBill.Text = payment.MaHD.ToString();
                }

                if (tb_idTable != null)
                {
                    tb_idTable.Text = payment.MaBanAn.ToString();
                }


                if (tb_tongtien != null)
                {
                    tb_tongtien.Text = payment.TongTien.ToString("N0") + " VNĐ";
                }

                // 🔥 QUAN TRỌNG: HIỂN THỊ TỔNG TIỀN LÊN LABEL_TONGTIEN
                if (label_tongtien != null)
                {
                    label_tongtien.Text = payment.TongTien.ToString("N0") + " VNĐ";
                    label_tongtien.ForeColor = Color.Red;
                }

                // Hiển thị thêm thông tin nếu có control
                var lblTenBan = FindControlRecursive<Label>(this, "lblTenBan");
                if (lblTenBan != null)
                {
                    lblTenBan.Text = payment.TenBan;
                }

                var lblSoMon = FindControlRecursive<Label>(this, "lblSoMon");
                if (lblSoMon != null)
                {
                    lblSoMon.Text = payment.SoMon.ToString();
                }

                // Hiển thị thông tin nhân viên
                var lblNhanVien = FindControlRecursive<Label>(this, "lblNhanVien");
                if (lblNhanVien != null)
                {
                    lblNhanVien.Text = payment.TenNhanVien;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi hiển thị bill: {ex.Message}");
            }
        }

        // ==================== PAYMENT METHODS ====================
        private async void btn_ttoan_Click(object sender, EventArgs e)
        {
            var selectedPayment = _billManager.GetSelectedItem();
            if (selectedPayment == null)
            {
                ShowWarning("Vui lòng chọn hóa đơn cần thanh toán!");
                return;
            }

            // Validate payment method - CHỈ ĐƯỢC CHỌN 1
            if (!checkBox_tienmat.Checked && !checkBox_chuyenkhoan.Checked)
            {
                ShowWarning("Vui lòng chọn phương thức thanh toán!");
                return;
            }

            string paymentMethod = checkBox_tienmat.Checked ? "TienMat" : "ChuyenKhoan";
            if (paymentMethod == "TienMat")
            {
                pb_QR.Visible = false; // Chọn tiền mặt thì ẩn QR đi
            }

            // Xác nhận thanh toán
            if (!Confirm($"Xác nhận thanh toán hóa đơn #{selectedPayment.MaHD}?\n" +
                        $"Bàn: {selectedPayment.TenBan}\n" +
                        $"Tổng tiền: {selectedPayment.TongTien:N0} VNĐ\n" +
                        $"Số món: {selectedPayment.SoMon}"))
                return;

            await ExecuteAsync(btn_ttoan, "Đang xử lý...", async () =>
            {
                try
                {
                    // 🔥 SỬA LỖI: Sử dụng đúng tên property theo database
                    var request = new ProcessPaymentRequest
                    {
                        MaHD = selectedPayment.MaHD,
                        MaNhanVien = _currentUserId,
                        PhuongThucThanhToan = paymentMethod,
                        SoTienThanhToan = selectedPayment.TongTien,
                        SoTienNhan = paymentMethod == "TienMat" ? selectedPayment.TongTien : 0
                    };

                    // 🔥 DEBUG: Log request để kiểm tra
                    Console.WriteLine($"Sending payment request: MaHD={request.MaHD}, MaNhanVien={request.MaNhanVien}, PhuongThucThanhToan={request.PhuongThucThanhToan}");

                    var response = await SendRequest<ProcessPaymentRequest, ProcessPaymentResponse>(request);

                    if (response?.Success == true)
                    {
                        //string successMessage = $"Thanh toán thành công!\nMã giao dịch: {response.MaGiaoDich}";

                        if (paymentMethod == "TienMat" && response.SoTienThua > 0)
                        {
                            string successMessage1 = $"Thanh toán thành công!\nMã giao dịch: {response.MaGiaoDich}";
                            successMessage1 += $"\nTiền thừa: {response.SoTienThua:N0} VNĐ";
                            ShowSuccess(successMessage1);
                        }

                        //ShowSuccess(successMessage);

                        // 🔥 QUAN TRỌNG: Refresh danh sách -> hóa đơn đã thanh toán sẽ ẩn đi
                        await _billManager.RefreshAsync();

                        // Clear form
                        ClearBillDetails();
                        btn_ttoan.Enabled = false;

                        // Hiển thị QR code nếu là chuyển khoản
                        if (paymentMethod == "ChuyenKhoan")
                        {
                            if (!Confirm($"Xác nhận thanh toán CK cho bàn {selectedPayment.TenBan}?")) return;

                            await ExecuteAsync(btn_ttoan, "Đang lấy QR...", async () =>
                            {
                                // Gọi API tạo thanh toán (Server sẽ trả về DangXuLy)
                                var request = new ProcessPaymentRequest
                                {
                                    MaHD = selectedPayment.MaHD,
                                    MaNhanVien = _currentUserId,
                                    PhuongThucThanhToan = "ChuyenKhoan",
                                    SoTienThanhToan = selectedPayment.TongTien
                                };

                                var response = await SendRequest<ProcessPaymentRequest, ProcessPaymentResponse>(request);

                                if (response?.Success == true)
                                {
                                    // 1. Hiện thị QR code
                                    HienThiMaQR(selectedPayment.TongTien, $"HD{selectedPayment.MaHD}");
                                    if (panel_qrthanhtoan != null) panel_qrthanhtoan.Visible = true;

                                    // 2. Thông báo trạng thái chờ
                                    ShowInfo("Vui lòng đợi khách quét QR. Hệ thống sẽ tự động xác nhận khi tiền về.");

                                    // 3. BẮT ĐẦU ĐẾM GIỜ KIỂM TRA
                                    _pendingMaHD = selectedPayment.MaHD;
                                    _checkPaymentTimer.Start();
                                    btn_ttoan.Enabled = false; // Khóa nút lại
                                }
                                else
                                {
                                    ShowError(response?.Message ?? "Lỗi tạo giao dịch");
                                }
                            });
                        }
                    }
                    else
                    {
                        // Xử lý lỗi từ server
                        string errorMessage = response?.Message ?? "Thanh toán thất bại";

                        // Kiểm tra xem có phải lỗi format string không
                        if (errorMessage.Contains("was not in a correct format"))
                        {
                            errorMessage = "Lỗi xử lý dữ liệu thanh toán. Vui lòng thử lại hoặc liên hệ quản lý.";
                        }
                        else if (errorMessage.Contains("TRF"))
                        {
                            errorMessage = "Lỗi tạo mã giao dịch. Vui lòng thử lại.";
                        }

                        ShowError(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi kết nối hoặc lỗi khác
                    string errorMessage = $"Lỗi thanh toán: {ex.Message}";

                    // Kiểm tra xem có phải lỗi format string không
                    if (ex.Message.Contains("was not in a correct format") ||
                        (ex.InnerException != null && ex.InnerException.Message.Contains("was not in a correct format")))
                    {
                        errorMessage = "Lỗi xử lý dữ liệu thanh toán. Vui lòng thử lại hoặc liên hệ quản lý.";
                    }

                    ShowError(errorMessage);
                }

            });
        }
        private async void CheckPaymentTimer_Tick(object sender, EventArgs e)
        {
            if (_pendingMaHD == 0) return;

            try
            {
                // Gửi request hỏi Server
                var request = new CheckTransferStatusRequest { MaHD = _pendingMaHD };

                // Lưu ý: Cần viết hàm SendRequest nhẹ hơn không block UI, 
                // nhưng tạm thời dùng hàm cũ cũng được.
                var response = await SendRequest<CheckTransferStatusRequest, CheckTransferStatusResponse>(request);

                if (response != null && response.IsPaid)
                {
                    // === THANH TOÁN THÀNH CÔNG ===
                    _checkPaymentTimer.Stop(); // Dừng kiểm tra
                    _pendingMaHD = 0;

                    // Ẩn QR và báo thành công
                    if (panel_qrthanhtoan != null) panel_qrthanhtoan.Visible = false;
                    ShowSuccess("Thanh toán thành công! Tiền đã về tài khoản.");

                    // Làm mới danh sách và mở lại nút
                    await _billManager.RefreshAsync();
                    ClearBillDetails();
                    btn_ttoan.Enabled = true;
                }
                else
                {
                    // Chưa có tiền -> Console log nhẹ (không hiện popup làm phiền)
                    Console.WriteLine("Đang đợi tiền về...");
                }
            }
            catch
            {
                // Lỗi mạng thì cứ lờ đi, chờ lần check sau
            }
        }
        private void ShowQRCode(decimal amount, string transactionNo)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<decimal, string>(ShowQRCode), amount, transactionNo);
                    return;
                }

                if (panel_qrthanhtoan != null)
                {
                    panel_qrthanhtoan.Visible = true;

                    // Tạo QR code đơn giản với thông tin giao dịch
                    using (var bmp = new Bitmap(200, 200))
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.White);
                        g.DrawString($"QR Payment\n{amount:N0} VNĐ\nMã: {transactionNo}",
                                    new Font("Arial", 7, FontStyle.Regular), // Không in đậm
                                    Brushes.Black,
                                    new PointF(5, 50));

                        panel_qrthanhtoan.BackgroundImage = new Bitmap(bmp);
                    }

                    // Tự động ẩn QR sau 10 giây
                    var timer = new System.Windows.Forms.Timer();
                    timer.Interval = 10000;
                    timer.Tick += (s, e) =>
                    {
                        if (panel_qrthanhtoan != null)
                        {
                            panel_qrthanhtoan.Visible = false;
                            panel_qrthanhtoan.BackgroundImage = null;
                        }
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tạo QR: {ex.Message}");
            }
        }

        // ==================== BUTTON HANDLERS ====================
        private async void btn_lammoi_Click(object sender, EventArgs e)
        {
            await ExecuteAsync(btn_lammoi, "Đang tải...", async () =>
            {
                await _billManager.RefreshAsync();

                var cachedData = _billManager.GetCachedData();
                ShowSuccess($"Đã tải {cachedData?.Count ?? 0} hóa đơn chờ thanh toán");
            });
        }

        private async void btn_searchBill_Click(object sender, EventArgs e)
        {
            string keyword = "";
            var searchBox = FindControlRecursive<TextBox>(this, "tb_searchBill");

            if (searchBox != null && searchBox.Text != SEARCH_BILL_PLACEHOLDER)
            {
                keyword = searchBox.Text.Trim();
            }

            if (string.IsNullOrEmpty(keyword))
            {
                await _billManager.LoadDataAsync();
                return;
            }

            // 🔥 CẢI THIỆN TÌM KIẾM: Tìm theo mã hóa đơn, mã bàn, tên bàn, hoặc tên nhân viên
            _billManager.FilterLocal(payment =>
                payment.MaHD.ToString().Contains(keyword) ||
                payment.MaBanAn.ToString().Contains(keyword) ||
                (payment.TenBan != null && payment.TenBan.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                (payment.TenNhanVien != null && payment.TenNhanVien.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            );

            var filteredCount = _billManager.GetRowCount();
            if (filteredCount > 0)
            {
                // Tự động chọn bill đầu tiên
                if (dataGridView_thanhtoan.Rows.Count > 0)
                {
                    dataGridView_thanhtoan.Rows[0].Selected = true;
                }
                ShowSuccess($"Tìm thấy {filteredCount} hóa đơn phù hợp");
            }
            else
            {
                ClearBillDetails();
                ShowWarning("Không tìm thấy hóa đơn nào khớp!");
            }
        }
        private void ClearBillDetails()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ClearBillDetails));
                return;
            }

            // XÓA THÔNG TIN TRONG CÁC TEXTBOX
            if (tb_idBill != null)
            {
                tb_idBill.Text = "";
            }
            if (tb_idTable != null)
            {
                tb_idTable.Text = "";
            }
            if (tb_dateBill != null)
            {
                tb_dateBill.Text = "";
            }
            if (tb_tongtien != null)
            {
                tb_tongtien.Text = "";
            }

            // 🔥 QUAN TRỌNG: RESET LABEL TỔNG TIỀN
            if (label_tongtien != null)
            {
                label_tongtien.Text = "0 VNĐ";
                label_tongtien.ForeColor = Color.Black;
            }

            if (panel_qrthanhtoan != null)
            {
                panel_qrthanhtoan.Visible = false;
                panel_qrthanhtoan.BackgroundImage = null;
            }

            // Clear các label khác
            var lblTenBan = FindControlRecursive<Label>(this, "lblTenBan");
            if (lblTenBan != null)
            {
                lblTenBan.Text = "";
            }

            var lblSoMon = FindControlRecursive<Label>(this, "lblSoMon");
            if (lblSoMon != null)
            {
                lblSoMon.Text = "";
            }

            var lblNhanVien = FindControlRecursive<Label>(this, "lblNhanVien");
            if (lblNhanVien != null)
            {
                lblNhanVien.Text = "";
            }
        }

        private string ConvertBillStatusToVietnamese(string sqlStatus)
        {
            var statusMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ChuaThanhToan", "Chưa thanh toán" },
                { "DaThanhToan", "Đã thanh toán" },
                { "Huy", "Hủy" }
            };

            return statusMap.TryGetValue(sqlStatus, out string vietnameseStatus) ? vietnameseStatus : sqlStatus;
        }

        // ==================== NETWORK & EXECUTION ====================
        private async Task<TResponse> SendRequest<TRequest, TResponse>(TRequest request)
        {
            string json = JsonConvert.SerializeObject(request) + "\n";

            using (var client = new TcpClient())
            {
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                await client.ConnectAsync(SERVER_IP, SERVER_PORT);

                using (var stream = client.GetStream())
                using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(json.TrimEnd('\n'));
                    string responseJson = await reader.ReadLineAsync();
                    return JsonConvert.DeserializeObject<TResponse>(responseJson);
                }
            }
        }

        private async Task ExecuteAsync(System.Windows.Forms.Button button, string loadingText, Func<Task> action)
        {
            string originalText = button.Text;
            button.Enabled = false;
            button.Text = loadingText;
            button.Font = new Font("Segoe UI", 9f, FontStyle.Regular); // Không in đậm
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
                button.Font = new Font("Segoe UI", 9f, FontStyle.Regular); // Không in đậm
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

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Thông tin",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== LOAD INITIAL DATA ====================
        private async void LoadPendingBills()
        {
            await _billManager.LoadDataAsync();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop the auto refresh timer
            _autoRefreshTimer?.Stop();
            _autoRefreshTimer?.Dispose();

            // ✅ THÊM: Stop the clock timer
            if (_clockTimer != null)
            {
                _clockTimer.Stop();
                _clockTimer.Dispose();
            }
            CleanupChatResources();
            base.OnFormClosing(e);
        }

        private void dataGridView_mon_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        //================= ĐẶT BÀN==============
        private async void btn_guiorder_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đã chọn bàn chưa
                if (cb_banOrder.SelectedValue == null)
                {
                    ShowWarning("Vui lòng chọn bàn!");
                    return;
                }

                // Kiểm tra giỏ hàng có món không
                if (_gioHang.Count == 0)
                {
                    ShowWarning("Giỏ hàng trống! Vui lòng thêm món.");
                    return;
                }

                int maBan = (int)cb_banOrder.SelectedValue;
                var selectedTable = _danhSachBan.FirstOrDefault(b => b.MaBanAn == maBan);

                if (selectedTable == null)
                {
                    ShowWarning("Không tìm thấy thông tin bàn!");
                    return;
                }

                // Kiểm tra trạng thái bàn
                if (selectedTable.TrangThai == "CoNguoi")
                {
                    ShowWarning("Bàn này đang có khách! Không thể thêm order.");
                    return;
                }

                if (selectedTable.TrangThai == "Dong")
                {
                    ShowWarning("Bàn này đã đóng! Không thể thêm order.");
                    return;
                }

                // Tính tổng tiền
                decimal tongTien = _gioHang.Sum(item => item.Gia * item.SoLuong);

                // Hiển thị thông tin xác nhận
                string confirmMessage = $"Xác nhận gửi order cho bàn '{selectedTable.TenBan}'?\n\n";
                confirmMessage += $"Số món: {_gioHang.Sum(item => item.SoLuong)}\n";
                confirmMessage += $"Tổng tiền: {tongTien:N0} VNĐ\n\n";
                confirmMessage += "Chi tiết:\n";

                foreach (var item in _gioHang)
                {
                    confirmMessage += $"- {item.TenMon} x{item.SoLuong}\n";
                }

                if (Confirm(confirmMessage))
                {
                    await ExecuteAsync(btn_guiorder, "Đang gửi order...", async () =>
                    {
                        // Tạo request thêm hóa đơn
                        var request = new CreateOrderRequest
                        {
                            MaBanAn = maBan,
                            MaNhanVien = _currentUserId,
                            TongTien = tongTien,
                            NgayOrder = ConvertVietnamToUtc(GetVietnamTime()),  // ← ĐÚNG: Lưu UTC
                            ChiTietOrder = _gioHang.Select(item => new ChiTietOrder
                            {
                                MaMon = item.MaMon,
                                SoLuong = item.SoLuong,
                                DonGia = item.Gia
                            }).ToList()
                        };

                        var response = await SendRequest<CreateOrderRequest, CreateOrderResponse>(request);

                        if (response?.Success == true)
                        {
                            ShowSuccess($"Đã gửi order thành công!\nMã hóa đơn: {response.MaHoaDon}");

                            // ✅ THÊM: Gửi thông báo tự động cho Bếp
                            int soMon = _gioHang.Sum(item => item.SoLuong);
                            await SendNotificationToBepAsync(selectedTable.TenBan, soMon, tongTien);

                            // Cập nhật trạng thái bàn thành "CoNguoi"
                            await UpdateTableStatus(maBan, "CoNguoi");

                            // Xóa giỏ hàng
                            ClearCart();

                            // Refresh danh sách bàn
                            await RefreshTableList();
                        }

                        else
                        {
                            ShowError(response?.Message ?? "Lỗi gửi order!");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi gửi order: {ex.Message}");
            }
        }
        private async Task UpdateTableStatus(int maBan, string trangThai)
        {
            try
            {
                var table = _danhSachBan.FirstOrDefault(b => b.MaBanAn == maBan);
                if (table == null) return;

                var request = new UpdateTableRequest
                {
                    MaBanAn = maBan,
                    TenBan = table.TenBan,
                    SoChoNgoi = table.SoChoNgoi,
                    TrangThai = trangThai,
                    MaNhanVien = _currentUserId
                };

                var response = await SendRequest<UpdateTableRequest, UpdateTableResponse>(request);

                if (response?.Success == true)
                {
                    table.TrangThai = trangThai;
                    UpdateTableStatusDisplay(table);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi cập nhật trạng thái bàn: {ex.Message}");
            }
        }
        private void ClearCart()
        {
            _gioHang.Clear();
            UpdateCartDisplay();
            Console.WriteLine("✅ Đã xóa giỏ hàng");
        }

        private void btn_datban_Click(object sender, EventArgs e)
        {
            try
            {
                if (cb_banOrder.SelectedValue == null)
                {
                    ShowWarning("Vui lòng chọn bàn cần đặt!");
                    return;
                }

                int maBan = (int)cb_banOrder.SelectedValue;
                var selectedTable = _danhSachBan.FirstOrDefault(b => b.MaBanAn == maBan);

                if (selectedTable == null)
                {
                    ShowWarning("Không tìm thấy thông tin bàn!");
                    return;
                }

                // Kiểm tra trạng thái hiện tại
                if (selectedTable.TrangThai == "DaDat")
                {
                    ShowWarning("Bàn này đã được đặt trước!");
                    return;
                }

                if (selectedTable.TrangThai == "CoNguoi")
                {
                    ShowWarning("Bàn này đang có khách!");
                    return;
                }

                // Xác nhận đặt bàn
                if (Confirm($"Xác nhận đặt trước bàn '{selectedTable.TenBan}'?"))
                {
                    Task datban;
                    datban = ExecuteAsync(btn_datban, "Đang đặt bàn...", async () =>
                    {
                        // Gửi request cập nhật trạng thái bàn
                        var request = new UpdateTableRequest
                        {
                            MaBanAn = maBan,
                            TenBan = selectedTable.TenBan,
                            SoChoNgoi = selectedTable.SoChoNgoi,
                            TrangThai = "DaDat",
                            MaNhanVien = _currentUserId
                        };

                        var response = await SendRequest<UpdateTableRequest, UpdateTableResponse>(request);

                        if (response?.Success == true)
                        {
                            ShowSuccess($"Đã đặt trước bàn '{selectedTable.TenBan}' thành công!");

                            // Cập nhật local data và hiển thị
                            selectedTable.TrangThai = "DaDat";
                            UpdateTableStatusDisplay(selectedTable);

                            // Refresh danh sách bàn (tùy chọn)
                            await RefreshTableList();
                        }
                        else
                        {
                            ShowError(response?.Message ?? "Lỗi đặt bàn!");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi đặt bàn: {ex.Message}");
            }
        }
        private async Task RefreshTableList()
        {
            try
            {
                var request = new GetTablesRequest { };
                var response = await SendRequest<GetTablesRequest, GetTablesResponse>(request);

                if (response?.Success == true && response.ListBan != null)
                {
                    _danhSachBan = response.ListBan;

                    // Giữ lại selection hiện tại
                    var currentSelection = cb_banOrder.SelectedValue;

                    cb_banOrder.DataSource = null;
                    cb_banOrder.DataSource = _danhSachBan;
                    cb_banOrder.DisplayMember = "TenBan";
                    cb_banOrder.ValueMember = "MaBanAn";

                    // Khôi phục selection
                    if (currentSelection != null)
                    {
                        cb_banOrder.SelectedValue = currentSelection;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi refresh danh sách bàn: {ex.Message}");
            }
        }

        private async void btn_huyban_Click(object sender, EventArgs e)
        {
            try
            {
                if (cb_banOrder.SelectedValue == null)
                {
                    ShowWarning("Vui lòng chọn bàn cần hủy!");
                    return;
                }

                int maBan = (int)cb_banOrder.SelectedValue;
                var selectedTable = _danhSachBan.FirstOrDefault(b => b.MaBanAn == maBan);

                if (selectedTable == null)
                {
                    ShowWarning("Không tìm thấy thông tin bàn!");
                    return;
                }

                // Kiểm tra trạng thái hiện tại
                if (selectedTable.TrangThai != "DaDat")
                {
                    ShowWarning("Bàn này chưa được đặt trước!");
                    return;
                }

                // Xác nhận hủy đặt bàn
                if (Confirm($"Xác nhận hủy đặt trước bàn '{selectedTable.TenBan}'?"))
                {
                    await ExecuteAsync(btn_huyban, "Đang hủy đặt...", async () =>
                    {
                        // Gửi request cập nhật trạng thái bàn về "Trong"
                        var request = new UpdateTableRequest
                        {
                            MaBanAn = maBan,
                            TenBan = selectedTable.TenBan,
                            SoChoNgoi = selectedTable.SoChoNgoi,
                            TrangThai = "Trong",
                            MaNhanVien = _currentUserId
                        };

                        var response = await SendRequest<UpdateTableRequest, UpdateTableResponse>(request);

                        if (response?.Success == true)
                        {
                            ShowSuccess($"Đã hủy đặt trước bàn '{selectedTable.TenBan}' thành công!");

                            // Cập nhật local data và hiển thị
                            selectedTable.TrangThai = "Trong";
                            UpdateTableStatusDisplay(selectedTable);

                            // Refresh danh sách bàn (tùy chọn)
                            await RefreshTableList();
                        }
                        else
                        {
                            ShowError(response?.Message ?? "Lỗi hủy đặt bàn!");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi hủy đặt bàn: {ex.Message}");
            }
        }

        private void cb_banOrder_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void cb_trangthai_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        /*private async Task LoadTableDetailsToListView(int maBan, string trangThai)
        {
            try
            {
                // Setup Cột (Thêm cột Bàn vào đầu tiên)
                if (listView1.Columns.Count == 0)
                {
                    listView1.View = View.Details;
                    listView1.GridLines = true;
                    listView1.FullRowSelect = true;

                    listView1.Columns.Add("Bàn", 50);       // 🔥 Cột 0: Mã Bàn
                    listView1.Columns.Add("Tên Món", 250);
                    listView1.Columns.Add("SL", 50);
                    listView1.Columns.Add("Đơn Giá", 250);
                    listView1.Columns.Add("Thành Tiền", 250);
                    listView1.Columns.Add("Thời Gian", 150);
                    listView1.Columns.Add("Trạng Thái Món", 350);
                }

                listView1.Items.Clear();

                var request = new GetTableDetailRequest { MaBanAn = maBan, TrangThai = trangThai };
                var response = await SendRequest<GetTableDetailRequest, GetTableDetailResponse>(request);

                if (response != null && response.Success)
                {
                    var danhSachDaGop = response.Orders
                .GroupBy(x => new
                {
                    x.MaBanAn,
                    x.TenMon,
                    x.DonGia,
                    x.TrangThai,
                    // Mẹo: Chuyển thời gian sang chuỗi "ngày-giờ-phút" để gộp hết các dòng lệch giây lại
                    ThoiGianKey = x.ThoiGianGoi.ToString("yyyyMMddHHmm")
                })
                .Select(g => new TableOrderDetailData
                {
                    MaBanAn = g.Key.MaBanAn,
                    TenMon = g.Key.TenMon,
                    DonGia = g.Key.DonGia,
                    TrangThai = g.Key.TrangThai,

                    // Cộng dồn số lượng
                    SoLuong = g.Sum(x => x.SoLuong),

                    // Lấy thời gian của dòng đầu tiên để hiển thị
                    ThoiGianGoi = g.First().ThoiGianGoi
                })
                .OrderByDescending(x => x.ThoiGianGoi) // Sắp xếp mới nhất lên đầu cho gọn
                .ToList();
                    foreach (var item in response.Orders)
                    {
                        // 🔥 Đổ Mã Bàn vào cột đầu tiên
                        ListViewItem row = new ListViewItem(item.MaBanAn.ToString());

                        row.SubItems.Add(item.TenMon);
                        row.SubItems.Add(item.SoLuong.ToString());
                        row.SubItems.Add(item.DonGia.ToString("N0"));
                        row.SubItems.Add(item.ThanhTien.ToString("N0"));
                        row.SubItems.Add(item.ThoiGianGoi.ToString("HH:mm"));
                        string tenHienThi = "";
                        switch (item.TrangThai)
                        {
                            case "ChuaLenMon": tenHienThi = "Chưa lên món"; break;
                            case "HoanThanh": tenHienThi = "Đã lên món"; break;
                            //case "DaDat": tenHienThi = "Đã đặt trước"; break;
                            default: tenHienThi = item.TrangThai; break; // Nếu lạ thì hiện nguyên gốc
                        }
                        row.SubItems.Add(tenHienThi);
                        if (item.TrangThai == "Đã lên món")
                        {
                            // Màu xanh lá (dùng LightGreen để chữ đen vẫn dễ đọc)
                            row.BackColor = Color.LightGreen;
                        }
                        else if (item.TrangThai == "Chưa lên món")
                        {
                            // (Tùy chọn) Màu vàng nhạt cho món chưa lên để dễ phân biệt
                            row.BackColor = Color.LightYellow;
                        }
                        else
                        {
                            // Màu trắng mặc định
                            row.BackColor = Color.White;
                        }
                        listView1.Items.Add(row);
                    }
                }

                // Thêm vào bảng
                //listView1.Items.Add(row);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }*/
        // 1. Thêm từ khóa 'async' vào trước 'void' 👇
        private async void cb_banan_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Kiểm tra null để tránh lỗi vặt
            if (cb_banan.SelectedValue == null) return;

            // Lấy Mã bàn
            if (int.TryParse(cb_banan.SelectedValue.ToString(), out int maBan))
            {
                // 2. Lấy trạng thái hiện tại (nếu chưa chọn thì mặc định là lấy hết "")
                string trangThai = "";
                if (cb_trangthai.SelectedItem != null)
                {
                    // Logic đơn giản để lấy code trạng thái
                    string val = cb_trangthai.SelectedItem.ToString();
                    if (val == "Chưa thanh toán") trangThai = "ChuaThanhToan";
                    else if (val == "Đã thanh toán") trangThai = "DaThanhToan";
                    else if (val == "Đã đặt trước") trangThai = "DaDat";
                }

                // 3. Gọi hàm (Đã sửa để truyền đủ 2 tham số: Mã Bàn + Trạng Thái)
                //await LoadTableDetailsToListView(maBan, trangThai);
            }
        }

        // 1. Hàm quy đổi trạng thái
        // [NVPhucVu.cs]
        // [NVPhucVu.cs] - Tìm hàm GetTrangThaiTuComboBox

        private string GetTrangThaiTuComboBox()
        {
            if (cb_trangthai.SelectedItem == null) return "";

            string luaChon = cb_trangthai.SelectedItem.ToString();
            switch (luaChon)
            {
                case "Tất cả":
                    return ""; // Lấy hết

                // 🔥 SỬA: Map chữ "Đang chế biến" trên giao diện thành mã "DangCheBien" trong SQL
                case "Đang chế biến":
                    return "DangCheBien";

                // 🔥 SỬA: Map chữ "Hoàn thành" trên giao diện thành mã "HoanThanh" trong SQL
                case "Hoàn thành":
                    return "HoanThanh";

                default:
                    return "";
            }
        }
        // 2. Sự kiện bộ lọc chung (Dùng cho cả cb_banan và cb_trangthai)
        /*private async void OnFilterChanged(object sender, EventArgs e)
        {
            // Lấy mã bàn
            int maBan = 0;
            if (cb_banan.SelectedValue != null)
            {
                int.TryParse(cb_banan.SelectedValue.ToString(), out maBan);
            }

            // Lấy trạng thái
            string trangThai = GetTrangThaiTuComboBox();

            // 🔥 SỬA ĐIỀU KIỆN: Chỉ cần KHÔNG CHỌN BÀN là xóa trắng ngay
            if (maBan == 0)
            {
                listView1.Items.Clear();
                return; // Dừng, không tải gì cả
            }

            // Nếu đã chọn bàn thì mới tải
           // await LoadTableDetailsToListView(maBan, trangThai);
        }*/

        private async void btn_lammoi_Click_1(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy mã bàn đang được chọn (nếu có)
                int maBan = 0;
                if (cb_banan.SelectedValue != null)
                {
                    if (int.TryParse(cb_banan.SelectedValue.ToString(), out int id))
                    {
                        maBan = id;
                    }
                }

                // 2. Lấy trạng thái lọc đang chọn (nếu có)
                // Hàm GetTrangThaiTuComboBox() chúng ta đã viết ở các bước trước
                string trangThai = GetTrangThaiTuComboBox();

                // 3. Tải lại dữ liệu vào DataGridView
                // (Đây là hàm hiển thị có ảnh và màu sắc bạn đã làm)
                await LoadTableDetailsToGrid(maBan, trangThai);

                // 4. (Tùy chọn) Cập nhật lại danh sách bàn để xem bàn nào mới có khách/trống
                // LoadTables(); 

                // Thông báo nhẹ dưới Console để biết đã chạy (hoặc dùng MessageBox nếu muốn)
                Console.WriteLine($"Đã làm mới dữ liệu lúc {DateTime.Now:HH:mm:ss}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi làm mới: " + ex.Message);
            }
        }
        // Hàm tải dữ liệu chi tiết món ăn lên DataGridView (hoặc ListView)
        // Hàm này nằm trong class NVPhucVu
        // [NVPhucVu.cs]

        private async Task LoadTableDetailsToGrid(int maBan, string trangThai)
        {
            try
            {
                // 1. Lấy tên bàn để tìm kiếm (vì API GetKitchenOrders dùng tên bàn chứ không dùng ID)
                string tenBanCanTim = "";

                // Tìm object bàn trong danh sách đã tải để lấy tên
                var banObj = _danhSachBan?.FirstOrDefault(b => b.MaBanAn == maBan);
                if (banObj != null)
                {
                    tenBanCanTim = banObj.TenBan;
                }

                // 2. Tạo request lấy danh sách ĐƠN HÀNG (đúng chuẩn cho dgv_DonHangTongQuan)
                var request = new GetKitchenOrdersRequest
                {
                    TrangThai = string.IsNullOrEmpty(trangThai) ? "TatCa" : trangThai,
                    TimKiemBan = tenBanCanTim, // Server sẽ tìm theo tên bàn (LIKE query)
                    SapXep = "ThoiGian"
                };

                // 3. Gọi Server
                var response = await SendRequest<GetKitchenOrdersRequest, GetKitchenOrdersResponse>(request);

                if (response != null && response.Success)
                {
                    dgv_DonHangTongQuan.AutoGenerateColumns = false; // Giữ nguyên cột đã design

                    // Đổ đúng dữ liệu KitchenOrderData vào Grid
                    dgv_DonHangTongQuan.DataSource = response.DonHang;
                }
                else
                {
                    dgv_DonHangTongQuan.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }
        // [NVPhucVu.cs] - Thêm hàm này vào trong class

        // [NVPhucVu.cs]

        private void Dgv_DonHangTongQuan_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Chỉ xử lý khi có dữ liệu và đúng cột cần thiết
            if (e.RowIndex < 0 || e.Value == null) return;

            // Lấy dòng hiện tại
            DataGridViewRow row = dgv_DonHangTongQuan.Rows[e.RowIndex];

            // --- XỬ LÝ CỘT TRẠNG THÁI (Hiển thị chữ Tiếng Việt) ---
            if (dgv_DonHangTongQuan.Columns[e.ColumnIndex].DataPropertyName == "TrangThaiDon")
            {
                string rawStatus = e.Value.ToString();

                switch (rawStatus)
                {
                    case "DangCheBien":
                        e.Value = "Đang chế biến";
                        e.CellStyle.ForeColor = Color.Blue;
                        // Tô màu nền VÀNG NHẠT cho dòng đang chế biến
                        row.DefaultCellStyle.BackColor = Color.LightYellow;
                        break;

                    case "HoanThanh":
                        e.Value = "Hoàn thành";
                        e.CellStyle.ForeColor = Color.DarkGreen;
                        // Tô màu nền XANH LÁ NHẠT cho dòng đã xong
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        break;

                    case "ChoXacNhan":
                        e.Value = "Chờ xác nhận";
                        e.CellStyle.ForeColor = Color.DarkOrange;
                        row.DefaultCellStyle.BackColor = Color.White;
                        break;

                    case "Huy":
                        e.Value = "Đã hủy";
                        e.CellStyle.ForeColor = Color.Gray;
                        row.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                        break;
                }

                // In đậm chữ trạng thái
                e.CellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                e.FormattingApplied = true;
            }
        }
        private void btn_xoahet_Click(object sender, EventArgs e)
        {
            // 1. Đưa các bộ lọc về mặc định (Rỗng)
            // Việc này sẽ tự động kích hoạt sự kiện SelectedIndexChanged
            // và code xử lý của chúng ta đã có check null nên nó sẽ tự dừng tải dữ liệu.
            cb_banan.SelectedIndex = -1;
            cb_trangthai.SelectedIndex = -1;

            // 2. Xóa dữ liệu trong bảng danh sách đơn hàng (Bảng bên trái)
            // Lưu ý: Thay 'dgv_DonHangTongQuan' bằng tên DataGridView thực tế của bạn
            if (dgv_DonHangTongQuan.DataSource != null)
            {
                dgv_DonHangTongQuan.DataSource = null;
            }
            else
            {
                dgv_DonHangTongQuan.Rows.Clear();
            }

            // 3. Xóa dữ liệu phần Chi tiết (Bên phải)
            // --- NẾU BẠN DÙNG LISTVIEW (Master-Detail) ---
            if (lv_ChiTietDon != null)
            {
                lv_ChiTietDon.Items.Clear();
            }

            // --- NẾU BẠN DÙNG PANEL ẢNH (Cách cũ) ---
            // (Bỏ comment phần này nếu bạn dùng Panel ảnh)
            /*
            if (lbl_TenMonCT != null) lbl_TenMonCT.Text = "";
            if (lbl_GiaCT != null) lbl_GiaCT.Text = "";
            if (lbl_TrangThaiCT != null) lbl_TrangThaiCT.Text = "";
            if (pb_MonAn != null) pb_MonAn.Image = null;
            */
        }
        //=============== QR =============================
        // Hàm hiển thị QR Code sử dụng API VietQR
        // Hàm gọi API VietQR và hiển thị lên PictureBox
        private void HienThiMaQR(decimal soTien, string noiDung)
        {
            try
            {
                // Bắt buộc dùng TLS 1.2 để tải ảnh từ https
                System.Net.ServicePointManager.SecurityProtocol =
                    System.Net.SecurityProtocolType.Tls12 |
                    System.Net.SecurityProtocolType.Tls11 |
                    System.Net.SecurityProtocolType.Tls;

                // 1. Cấu hình tài khoản (Thay bằng thông tin của bạn)
                string nganHang = "ICB"; // VietinBank
                string soTaiKhoan = "0933200298";
                string tenChuTaiKhoan = "NGUYEN QUOC TRUONG";

                // 2. Xử lý dữ liệu
                string amount = ((int)soTien).ToString();

                // Encode nội dung sang định dạng URL (để tránh lỗi ký tự đặc biệt)
                // VietQR sẽ tự động hiển thị đúng khi quét
                string addInfo = Uri.EscapeDataString(noiDung);
                string accountName = Uri.EscapeDataString(tenChuTaiKhoan);

                // 3. Tạo link API VietQR (Dùng template compact2 cho đẹp và chuẩn)
                string apiUrl = $"https://img.vietqr.io/image/{nganHang}-{soTaiKhoan}-compact2.png?amount={amount}&addInfo={addInfo}&accountName={accountName}";

                // 4. Hiển thị lên UI
                if (panel_qrthanhtoan != null) panel_qrthanhtoan.Visible = true;

                pb_QR.Visible = true;
                pb_QR.Image = null; // Xóa ảnh cũ để tránh nhầm lẫn
                pb_QR.SizeMode = PictureBoxSizeMode.Zoom;
                pb_QR.BringToFront();

                // Tải ảnh bất đồng bộ
                pb_QR.LoadAsync(apiUrl);

                Console.WriteLine("Link QR đã tạo: " + apiUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo QR: " + ex.Message);
            }
        }

        private void pb_QR_Click(object sender, EventArgs e)
        {
            if (pb_QR.Image == null) return;

            Form zoomForm = new Form();
            zoomForm.StartPosition = FormStartPosition.CenterScreen;
            zoomForm.Size = new Size(600, 600);
            zoomForm.FormBorderStyle = FormBorderStyle.None;
            zoomForm.BackColor = Color.White;

            PictureBox pbZoom = new PictureBox();
            pbZoom.Image = pb_QR.Image;
            pbZoom.Dock = DockStyle.Fill;
            pbZoom.SizeMode = PictureBoxSizeMode.Zoom;
            pbZoom.Cursor = Cursors.Hand;

            // Bấm vào ảnh to hoặc bấm ESC thì tắt
            pbZoom.Click += (s, args) => zoomForm.Close();
            zoomForm.KeyDown += (s, args) => { if (args.KeyCode == Keys.Escape) zoomForm.Close(); };

            zoomForm.Controls.Add(pbZoom);
            zoomForm.ShowDialog();
        }

        private void checkBox_chuyenkhoan_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void LoadNVInfo()
        {
            try
            {
                // Gán dữ liệu từ CurrentUser
                textbox_usernamephucvu.Text = CurrentUser.Username ?? "";
                textbox_emailphucvu.Text = CurrentUser.Email ?? "";
                textbox_tenphucvu.Text = CurrentUser.FullName ?? "";
                textbox_rolepv.Text = CurrentUser.Role ?? "PhucVu";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị thông tin tài khoản: " + ex.Message);
            }
        }

        private async void button_DangXuatPhucVu_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            try
            {
                button_DangXuatPhucVu.Enabled = false;
                button_DangXuatPhucVu.Text = "Đang đăng xuất...";

                // 1. DỪNG TẤT CẢ TIMER
                StopAllTimers();

                // 2. GỬI REQUEST LOGOUT VỚI TOKEN ĐẾN SERVER
                await SendLogoutRequestAsync();

                // 3. XÓA THÔNG TIN USER LOCAL
                CurrentUser.Clear();

                // 4. MỞ FORM ĐĂNG NHẬP
                var loginForm = new DangNhap();
                loginForm.StartPosition = FormStartPosition.CenterScreen;
                loginForm.Show();

                // 5. ĐÓNG FORM NVPHUCVU
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng xuất: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button_DangXuatPhucVu.Enabled = true;
                button_DangXuatPhucVu.Text = "Đăng xuất";
            }
        }

        private void dgv_DonHangTongQuan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView_thanhtoan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        #region CHAT INITIALIZATION

        /// <summary>
        /// Khởi tạo chức năng Chat
        /// </summary>
        private void InitializeChatFeature()
        {
            try
            {
                // Setup ListView Users
                SetupChatUserListView();

                // Setup RichTextBox Messages
                SetupChatMessagesBox();

                // Setup Controls
                SetupChatControls();

                // Đăng ký sự kiện
                RegisterChatEvents();

                // Khởi tạo Timer polling
                InitializeChatRefreshTimer();

                // Load danh sách user
                LoadChatUsers();

                Console.WriteLine("✅ Khởi tạo Chat thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khởi tạo Chat: {ex.Message}");
            }
        }

        /// <summary>
        /// Setup ListView hiển thị danh sách user
        /// </summary>
        private void SetupChatUserListView()
        {
            if (lv_Users == null) return;

            lv_Users.View = View.Details;
            lv_Users.FullRowSelect = true;
            lv_Users.GridLines = true;
            lv_Users.MultiSelect = false;

            // Xóa cột cũ và thêm cột mới
            lv_Users.Columns.Clear();
            lv_Users.Columns.Add("Tên", 180);
            lv_Users.Columns.Add("Vai trò", 100);
            lv_Users.Columns.Add("", 40); // Cột trạng thái online + số tin chưa đọc

            // Cho phép sắp xếp
            lv_Users.Sorting = SortOrder.None;
        }

        /// <summary>
        /// Setup RichTextBox hiển thị tin nhắn
        /// </summary>
        private void SetupChatMessagesBox()
        {
            if (rtb_ChatMessages == null) return;

            rtb_ChatMessages.ReadOnly = true;
            rtb_ChatMessages.BackColor = Color.White;
            rtb_ChatMessages.Font = new Font("Segoe UI", 10f);
            rtb_ChatMessages.BorderStyle = BorderStyle.None;

            // Hiển thị thông báo ban đầu
            rtb_ChatMessages.Clear();
            AppendColoredText("💬 Chọn một người để bắt đầu chat\n", Color.Gray, true);
        }

        /// <summary>
        /// Setup các controls khác
        /// </summary>
        private void SetupChatControls()
        {
            // TextBox tìm kiếm
            if (txt_SearchUser != null)
            {
                txt_SearchUser.PlaceholderText = "🔍 Tìm kiếm...";
                txt_SearchUser.Font = new Font("Segoe UI", 9f);
            }

            // TextBox nhập tin nhắn
            if (txt_ChatMessage != null)
            {
                txt_ChatMessage.PlaceholderText = "Nhập tin nhắn...";
                txt_ChatMessage.Font = new Font("Segoe UI", 10f);
                txt_ChatMessage.Enabled = false; // Disable cho đến khi chọn người chat
            }

            // Button gửi
            if (btn_SendChat != null)
            {
                btn_SendChat.Enabled = false;
                btn_SendChat.BackColor = Color.DodgerBlue;
                btn_SendChat.ForeColor = Color.White;
                btn_SendChat.FlatStyle = FlatStyle.Flat;
            }

            // CheckBox gửi tất cả
            if (chk_SendAll != null)
            {
                chk_SendAll.Text = "📢 Gửi cho tất cả";
            }

            // Label header
            if (lbl_ChatTitle != null)
            {
                lbl_ChatTitle.Text = "💬 Chọn người để chat";
                lbl_ChatTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            }

            if (lbl_ChatRole != null)
            {
                lbl_ChatRole.Text = "";
                lbl_ChatRole.ForeColor = Color.DimGray;
            }

            // Label online count
            UpdateOnlineCount();
        }

        /// <summary>
        /// Đăng ký các sự kiện
        /// </summary>
        private void RegisterChatEvents()
        {
            // Sự kiện chọn user trong ListView
            if (lv_Users != null)
            {
                lv_Users.SelectedIndexChanged += LvUsers_SelectedIndexChanged;
                lv_Users.DoubleClick += LvUsers_DoubleClick;
            }

            // Sự kiện gửi tin nhắn
            if (btn_SendChat != null)
            {
                btn_SendChat.Click += BtnSendChat_Click;
            }

            // Sự kiện nhấn Enter để gửi
            if (txt_ChatMessage != null)
            {
                txt_ChatMessage.KeyPress += TxtChatMessage_KeyPress;
            }

            // Sự kiện tìm kiếm
            if (txt_SearchUser != null)
            {
                txt_SearchUser.TextChanged += TxtSearchUser_TextChanged;
            }

            // Sự kiện checkbox gửi tất cả
            if (chk_SendAll != null)
            {
                chk_SendAll.CheckedChanged += ChkSendAll_CheckedChanged;
            }

            // Sự kiện làm mới
            if (btn_RefreshUsers != null)
            {
                btn_RefreshUsers.Click += BtnRefreshUsers_Click;
            }
        }

        /// <summary>
        /// Khởi tạo Timer polling tin nhắn mới
        /// </summary>
        private void InitializeChatRefreshTimer()
        {
            _chatRefreshTimer = new System.Windows.Forms.Timer();
            _chatRefreshTimer.Interval = CHAT_REFRESH_INTERVAL;
            _chatRefreshTimer.Tick += async (s, e) => await CheckNewMessagesAsync();
            _chatRefreshTimer.Start();
        }

        #endregion

        #region CHAT DATA LOADING

        /// <summary>
        /// Load danh sách user để chat
        /// </summary>
        private async void LoadChatUsers(string searchKeyword = "")
        {
            try
            {
                var request = new GetChatUsersRequest
                {
                    MaNguoiDungHienTai = _currentUserId,
                    TimKiem = searchKeyword
                };

                var response = await SendRequest<GetChatUsersRequest, GetChatUsersResponse>(request);

                if (response?.Success == true)
                {
                    _chatUsers = response.Users;
                    DisplayChatUsers(_chatUsers);
                    UpdateOnlineCount();
                }
                else
                {
                    Console.WriteLine($"Lỗi load chat users: {response?.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi LoadChatUsers: {ex.Message}");
            }
        }

        /// <summary>
        /// Hiển thị danh sách user lên ListView
        /// </summary>
        private void DisplayChatUsers(List<ChatUserData> users)
        {
            if (lv_Users == null) return;

            if (lv_Users.InvokeRequired)
            {
                lv_Users.Invoke(new Action(() => DisplayChatUsers(users)));
                return;
            }

            lv_Users.Items.Clear();

            foreach (var user in users)
            {
                ListViewItem item = new ListViewItem(user.HoTen);
                item.SubItems.Add(user.VaiTroDisplay);

                // Hiển thị số tin chưa đọc hoặc trạng thái online
                string statusText = user.SoTinChuaDoc > 0 ?
                    $"({user.SoTinChuaDoc})" :
                    (user.DangOnline ? "●" : "○");
                item.SubItems.Add(statusText);

                // Màu sắc theo vai trò
                switch (user.VaiTro)
                {
                    case "Admin":
                        item.ForeColor = Color.DarkRed;
                        break;
                    case "Bep":
                        item.ForeColor = Color.DarkOrange;
                        break;
                    case "PhucVu":
                        item.ForeColor = Color.DarkBlue;
                        break;
                }

                // Highlight nếu có tin chưa đọc
                if (user.SoTinChuaDoc > 0)
                {
                    item.BackColor = Color.LightYellow;
                    item.Font = new Font(lv_Users.Font, FontStyle.Bold);
                }

                // Lưu MaNguoiDung vào Tag
                item.Tag = user.MaNguoiDung;

                lv_Users.Items.Add(item);
            }
        }

        /// <summary>
        /// Cập nhật số user online
        /// </summary>
        private void UpdateOnlineCount()
        {
            if (lbl_OnlineCount == null) return;

            if (lbl_OnlineCount.InvokeRequired)
            {
                lbl_OnlineCount.Invoke(new Action(UpdateOnlineCount));
                return;
            }

            int online = _chatUsers?.Count(u => u.DangOnline) ?? 0;
            int total = _chatUsers?.Count ?? 0;
            lbl_OnlineCount.Text = $"Online: {online}/{total}";
        }

        /// <summary>
        /// Load tin nhắn chat với user được chọn
        /// </summary>
        private async Task LoadChatMessagesAsync(int maNguoiChat)
        {
            try
            {
                var request = new GetChatMessagesRequest
                {
                    MaNguoiDung1 = _currentUserId,
                    MaNguoiDung2 = maNguoiChat,
                    SoLuong = 100
                };

                var response = await SendRequest<GetChatMessagesRequest, GetChatMessagesResponse>(request);

                if (response?.Success == true)
                {
                    DisplayChatMessages(response.Messages);

                    // Cập nhật thời gian tin nhắn cuối
                    if (response.Messages.Count > 0)
                    {
                        _lastMessageTime = response.Messages.Max(m => m.ThoiGian);
                    }

                    // Đánh dấu đã đọc
                    await MarkMessagesAsReadAsync(maNguoiChat);

                    // Refresh lại danh sách user để cập nhật số tin chưa đọc
                    LoadChatUsers(txt_SearchUser?.Text ?? "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi LoadChatMessages: {ex.Message}");
            }
        }

        /// <summary>
        /// Hiển thị tin nhắn lên RichTextBox
        /// </summary>
        private void DisplayChatMessages(List<ChatMessageData> messages)
        {
            if (rtb_ChatMessages == null) return;

            if (rtb_ChatMessages.InvokeRequired)
            {
                rtb_ChatMessages.Invoke(new Action(() => DisplayChatMessages(messages)));
                return;
            }

            rtb_ChatMessages.Clear();
            _displayedMessageIds.Clear();  // ✅ THÊM: Reset danh sách tin đã hiển thị

            if (messages.Count == 0)
            {
                AppendColoredText("💬 Chưa có tin nhắn nào. Hãy bắt đầu cuộc trò chuyện!\n", Color.Gray, true);
                return;
            }

            DateTime? lastDate = null;

            foreach (var msg in messages)
            {
                // ✅ THÊM: Đánh dấu tin nhắn đã hiển thị
                _displayedMessageIds.Add(msg.MaTinNhan);

                // Hiển thị ngày nếu khác ngày trước
                if (!lastDate.HasValue || msg.ThoiGian.Date != lastDate.Value.Date)
                {
                    AppendColoredText($"\n─── {msg.ThoiGian:dd/MM/yyyy} ───\n", Color.Gray, true);
                    lastDate = msg.ThoiGian.Date;
                }

                bool isMine = msg.MaNguoiGui == _currentUserId;
                string broadcastPrefix = msg.LaTinBroadcast ? "[📢 TẤT CẢ] " : "";

                if (isMine)
                {
                    AppendColoredText($"[{msg.ThoiGianDisplay}] ", Color.Gray, false);
                    AppendColoredText("Bạn: ", Color.DarkGreen, true);
                    AppendColoredText($"{broadcastPrefix}{msg.NoiDung}\n", Color.Black, false);
                }
                else
                {
                    AppendColoredText($"[{msg.ThoiGianDisplay}] ", Color.Gray, false);

                    Color nameColor = msg.VaiTroNguoiGui switch
                    {
                        "Admin" => Color.DarkRed,
                        "Bep" => Color.DarkOrange,
                        _ => Color.DarkBlue
                    };

                    AppendColoredText($"{msg.TenNguoiGui}: ", nameColor, true);
                    AppendColoredText($"{broadcastPrefix}{msg.NoiDung}\n", Color.Black, false);
                }
            }

            // Cuộn xuống cuối
            rtb_ChatMessages.SelectionStart = rtb_ChatMessages.Text.Length;
            rtb_ChatMessages.ScrollToCaret();
        }

        /// <summary>
        /// Thêm text có màu vào RichTextBox
        /// </summary>
        private void AppendColoredText(string text, Color color, bool bold)
        {
            if (rtb_ChatMessages == null) return;

            int start = rtb_ChatMessages.TextLength;
            rtb_ChatMessages.AppendText(text);
            rtb_ChatMessages.Select(start, text.Length);
            rtb_ChatMessages.SelectionColor = color;

            if (bold)
            {
                rtb_ChatMessages.SelectionFont = new Font(rtb_ChatMessages.Font, FontStyle.Bold);
            }
            else
            {
                rtb_ChatMessages.SelectionFont = new Font(rtb_ChatMessages.Font, FontStyle.Regular);
            }

            rtb_ChatMessages.SelectionLength = 0;
        }

        #endregion

        #region CHAT SEND & RECEIVE

        /// <summary>
        /// Gửi tin nhắn chat
        /// </summary>
        private async Task SendChatMessageAsync()
        {
            try
            {
                string noiDung = txt_ChatMessage?.Text?.Trim() ?? "";

                if (string.IsNullOrEmpty(noiDung))
                {
                    return;
                }

                bool guiTatCa = chk_SendAll?.Checked ?? false;

                // Kiểm tra: phải chọn người nhận hoặc chọn gửi tất cả
                if (!guiTatCa && _selectedChatUserId == 0)
                {
                    ShowWarning("Vui lòng chọn người nhận hoặc chọn 'Gửi cho tất cả'!");
                    return;
                }

                // Disable controls khi đang gửi
                if (btn_SendChat != null) btn_SendChat.Enabled = false;
                if (txt_ChatMessage != null) txt_ChatMessage.Enabled = false;

                var request = new SendChatMessageRequest
                {
                    MaNguoiGui = _currentUserId,
                    MaNguoiNhan = guiTatCa ? 0 : _selectedChatUserId,
                    NoiDung = noiDung,
                    GuiTatCa = guiTatCa
                };

                var response = await SendRequest<SendChatMessageRequest, SendChatMessageResponse>(request);

                if (response?.Success == true)
                {
                    // Xóa textbox
                    if (txt_ChatMessage != null) txt_ChatMessage.Clear();

                    // Thêm tin nhắn vào hiển thị ngay lập tức
                    AppendSentMessage(noiDung, guiTatCa);

                    // Cập nhật thời gian tin nhắn cuối
                    _lastMessageTime = response.ThoiGianGui;

                    Console.WriteLine($"✅ Đã gửi tin nhắn: {noiDung.Substring(0, Math.Min(20, noiDung.Length))}...");
                }
                else
                {
                    ShowError(response?.Message ?? "Lỗi gửi tin nhắn");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi gửi tin nhắn: {ex.Message}");
            }
            finally
            {
                // Enable lại controls
                if (btn_SendChat != null) btn_SendChat.Enabled = true;
                if (txt_ChatMessage != null)
                {
                    txt_ChatMessage.Enabled = true;
                    txt_ChatMessage.Focus();
                }
            }
        }

        /// <summary>
        /// Thêm tin nhắn vừa gửi vào RichTextBox
        /// </summary>
        private void AppendSentMessage(string noiDung, bool guiTatCa)
        {
            if (rtb_ChatMessages == null) return;

            if (rtb_ChatMessages.InvokeRequired)
            {
                rtb_ChatMessages.Invoke(new Action(() => AppendSentMessage(noiDung, guiTatCa)));
                return;
            }

            string timeStr = DateTime.Now.ToString("HH:mm");
            string prefix = guiTatCa ? "[📢 TẤT CẢ] " : "";

            AppendColoredText($"[{timeStr}] ", Color.Gray, false);
            AppendColoredText("Bạn: ", Color.DarkGreen, true);
            AppendColoredText($"{prefix}{noiDung}\n", Color.Black, false);

            // Cuộn xuống cuối
            rtb_ChatMessages.SelectionStart = rtb_ChatMessages.Text.Length;
            rtb_ChatMessages.ScrollToCaret();
        }

        /// <summary>
        /// Kiểm tra tin nhắn mới (polling)
        /// </summary>
        private async Task CheckNewMessagesAsync()
        {
            try
            {
                // Chỉ kiểm tra nếu đang ở tab Chat HOẶC cần nhận thông báo nền
                // if (tc_main.SelectedTab.Name != "tabChat") return; // (Tuỳ logic của bạn)

                var request = new CheckNewMessagesRequest
                {
                    MaNguoiDung = _currentUserId,
                    TuThoiGian = _lastMessageTime
                };

                var response = await SendRequest<CheckNewMessagesRequest, CheckNewMessagesResponse>(request);

                if (response?.Success == true && response.CoTinMoi)
                {
                    bool hasNewMessages = false;

                    foreach (var msg in response.TinNhanMoi)
                    {
                        if (_displayedMessageIds.Contains(msg.MaTinNhan)) continue;

                        _displayedMessageIds.Add(msg.MaTinNhan);
                        hasNewMessages = true;

                        if (msg.ThoiGian > _lastMessageTime) _lastMessageTime = msg.ThoiGian;

                        // 1. Nếu đang chat với người này -> Hiện vào khung chat
                        if (msg.MaNguoiGui == _selectedChatUserId || msg.LaTinBroadcast)
                        {
                            AppendReceivedMessage(msg);
                        }

                        // 2. Nếu tin nhắn từ người khác (ví dụ từ Bếp) -> Hiện THÔNG BÁO
                        if (msg.MaNguoiGui != _selectedChatUserId)
                        {
                            // Cập nhật danh sách (để hiện số đỏ)
                            LoadChatUsers(txt_SearchUser.Text);

                            // ✅ THÊM: Hiện Popup thông báo nếu là tin từ Bếp hoặc có icon chuông
                            if (msg.NoiDung.Contains("🔔") || msg.VaiTroNguoiGui == "Bep")
                            {
                                // Kiểm tra đã thông báo chưa - QUAN TRỌNG!
                                if (!_notifiedMessageIds.Contains(msg.MaTinNhan))
                                {
                                    _notifiedMessageIds.Add(msg.MaTinNhan);
                                    string thongBao = $"🔔 BẾP NHẮN: {msg.TenNguoiGui}\n{msg.NoiDung}";
                                    MessageBox.Show(thongBao, "Thông báo món ăn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi CheckNewMessages: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm tin nhắn nhận được vào RichTextBox
        /// </summary>
        private void AppendReceivedMessage(ChatMessageData msg)
        {
            if (rtb_ChatMessages.InvokeRequired)
            {
                rtb_ChatMessages.Invoke(new Action(() => AppendReceivedMessage(msg)));
                return;
            }

            // Màu sắc theo vai trò
            Color nameColor = msg.VaiTroNguoiGui switch
            {
                "Admin" => Color.DarkRed,
                "Bep" => Color.DarkOrange, // Màu cam cho Bếp
                _ => Color.DarkBlue
            };

            string broadcastPrefix = msg.LaTinBroadcast ? "[📢 TẤT CẢ] " : "";

            // ✅ QUAN TRỌNG: msg.ThoiGianDisplay đã được xử lý đúng ở Server/DBAccess
            // Không dùng DateTime.Now ở đây
            AppendColoredText($"[{msg.ThoiGianDisplay}] ", Color.Gray, false);
            AppendColoredText($"{msg.TenNguoiGui}: ", nameColor, true);
            AppendColoredText($"{broadcastPrefix}{msg.NoiDung}\n", Color.Black, false);

            rtb_ChatMessages.SelectionStart = rtb_ChatMessages.Text.Length;
            rtb_ChatMessages.ScrollToCaret();
        }

        /// <summary>
        /// Đánh dấu tin nhắn đã đọc
        /// </summary>
        private async Task MarkMessagesAsReadAsync(int maNguoiGui)
        {
            try
            {
                var request = new MarkMessagesReadRequest
                {
                    MaNguoiNhan = _currentUserId,
                    MaNguoiGui = maNguoiGui
                };

                await SendRequest<MarkMessagesReadRequest, MarkMessagesReadResponse>(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi MarkMessagesAsRead: {ex.Message}");
            }
        }

        #endregion
        #region AUTO NOTIFICATION

        /// <summary>
        /// Gửi thông báo tự động cho nhân viên Bếp khi có đơn hàng mới
        /// </summary>
        private async Task SendNotificationToBepAsync(string tenBan, int soMon, decimal tongTien)
        {
            try
            {
                string noiDung = $"🆕 ĐƠN MỚI [{tenBan}]: {soMon} món - {tongTien:N0} VNĐ";

                // Gửi cho tất cả nhân viên Bếp
                await SendChatNotificationAsync(noiDung, "Bep");

                Console.WriteLine($"✅ Đã gửi thông báo cho Bếp: {noiDung}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi SendNotificationToBep: {ex.Message}");
            }
        }

        /// <summary>
        /// Gửi thông báo qua chat cho vai trò cụ thể
        /// </summary>
        private async Task SendChatNotificationAsync(string noiDung, string vaiTroNhan)
        {
            try
            {
                // Lấy danh sách user theo vai trò
                var usersToNotify = _chatUsers?.Where(u => u.VaiTro == vaiTroNhan).ToList();

                if (usersToNotify == null || usersToNotify.Count == 0)
                {
                    // Nếu không có danh sách, gửi broadcast
                    var request = new SendChatMessageRequest
                    {
                        MaNguoiGui = _currentUserId,
                        MaNguoiNhan = 0,
                        NoiDung = $"🔔 {noiDung}",
                        GuiTatCa = true
                    };

                    await SendRequest<SendChatMessageRequest, SendChatMessageResponse>(request);
                }
                else
                {
                    // Gửi cho từng user theo vai trò
                    foreach (var user in usersToNotify)
                    {
                        var request = new SendChatMessageRequest
                        {
                            MaNguoiGui = _currentUserId,
                            MaNguoiNhan = user.MaNguoiDung,
                            NoiDung = $"🔔 {noiDung}",
                            GuiTatCa = false
                        };

                        await SendRequest<SendChatMessageRequest, SendChatMessageResponse>(request);
                    }
                }

                Console.WriteLine($"✅ Đã gửi thông báo chat: {noiDung}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi gửi thông báo chat: {ex.Message}");
            }
        }

        /// <summary>
        /// Gửi thông báo khi món đã được phục vụ (lên bàn)
        /// </summary>
        private async Task SendServedNotificationAsync(string tenBan, string tenMon)
        {
            try
            {
                string noiDung = $"🍽️ [{tenBan}] Đã phục vụ: {tenMon}";
                await SendChatNotificationAsync(noiDung, "Bep");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi gửi thông báo: {ex.Message}");
            }
        }

        #endregion

        #region CHAT EVENT HANDLERS

        /// <summary>
        /// Sự kiện chọn user trong ListView
        /// </summary>
        private async void LvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_Users?.SelectedItems.Count == 0) return;

            var selectedItem = lv_Users.SelectedItems[0];
            if (selectedItem.Tag == null) return;

            _selectedChatUserId = (int)selectedItem.Tag;
            _selectedChatUserName = selectedItem.Text;

            // Cập nhật header
            UpdateChatHeader(_selectedChatUserName, selectedItem.SubItems[1].Text);

            // Enable controls
            if (txt_ChatMessage != null) txt_ChatMessage.Enabled = true;
            if (btn_SendChat != null) btn_SendChat.Enabled = true;

            // Bỏ chọn "Gửi tất cả" khi chọn người cụ thể
            if (chk_SendAll != null) chk_SendAll.Checked = false;

            // Load tin nhắn
            await LoadChatMessagesAsync(_selectedChatUserId);
        }

        /// <summary>
        /// Sự kiện double click vào user
        /// </summary>
        private void LvUsers_DoubleClick(object sender, EventArgs e)
        {
            // Focus vào textbox để nhập tin nhắn
            txt_ChatMessage?.Focus();
        }

        /// <summary>
        /// Sự kiện click nút Gửi
        /// </summary>
        private async void BtnSendChat_Click(object sender, EventArgs e)
        {
            await SendChatMessageAsync();
        }

        /// <summary>
        /// Sự kiện nhấn phím trong textbox tin nhắn
        /// </summary>
        private async void TxtChatMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Nhấn Enter để gửi (Shift+Enter để xuống dòng)
            if (e.KeyChar == (char)Keys.Enter && !ModifierKeys.HasFlag(Keys.Shift))
            {
                e.Handled = true; // Ngăn xuống dòng
                await SendChatMessageAsync();
            }
        }

        /// <summary>
        /// Sự kiện thay đổi text tìm kiếm
        /// </summary>
        private void TxtSearchUser_TextChanged(object sender, EventArgs e)
        {
            // Delay để không gọi API liên tục khi đang gõ
            // Có thể dùng Timer để debounce, nhưng đơn giản ta gọi trực tiếp
            string keyword = txt_SearchUser?.Text?.Trim() ?? "";
            FilterChatUsers(keyword);
        }

        /// <summary>
        /// Lọc danh sách user theo từ khóa (local)
        /// </summary>
        private void FilterChatUsers(string keyword)
        {
            if (_chatUsers == null) return;

            var filtered = string.IsNullOrEmpty(keyword) ?
                _chatUsers :
                _chatUsers.Where(u => u.HoTen.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            DisplayChatUsers(filtered);
        }

        /// <summary>
        /// Sự kiện thay đổi checkbox "Gửi tất cả"
        /// </summary>
        private void ChkSendAll_CheckedChanged(object sender, EventArgs e)
        {
            bool guiTatCa = chk_SendAll?.Checked ?? false;

            if (guiTatCa)
            {
                // Bỏ chọn user trong ListView
                if (lv_Users != null)
                {
                    lv_Users.SelectedItems.Clear();
                }

                _selectedChatUserId = 0;
                _selectedChatUserName = "";

                // Cập nhật header
                if (lbl_ChatTitle != null) lbl_ChatTitle.Text = "📢 Gửi tin nhắn cho TẤT CẢ";
                if (lbl_ChatRole != null) lbl_ChatRole.Text = "Tin nhắn sẽ được gửi đến tất cả nhân viên";

                // Enable controls
                if (txt_ChatMessage != null) txt_ChatMessage.Enabled = true;
                if (btn_SendChat != null) btn_SendChat.Enabled = true;

                // Hiển thị hướng dẫn
                if (rtb_ChatMessages != null)
                {
                    rtb_ChatMessages.Clear();
                    AppendColoredText("📢 CHẾ ĐỘ GỬI TẤT CẢ\n\n", Color.DarkOrange, true);
                    AppendColoredText("Tin nhắn của bạn sẽ được gửi đến TẤT CẢ nhân viên trong hệ thống.\n\n", Color.Gray, false);
                    AppendColoredText("Lưu ý: Chỉ sử dụng khi có thông báo quan trọng!\n", Color.Red, false);
                }
            }
            else
            {
                // Reset về trạng thái ban đầu nếu chưa chọn ai
                if (_selectedChatUserId == 0)
                {
                    if (lbl_ChatTitle != null) lbl_ChatTitle.Text = "💬 Chọn người để chat";
                    if (lbl_ChatRole != null) lbl_ChatRole.Text = "";

                    if (txt_ChatMessage != null) txt_ChatMessage.Enabled = false;
                    if (btn_SendChat != null) btn_SendChat.Enabled = false;

                    if (rtb_ChatMessages != null)
                    {
                        rtb_ChatMessages.Clear();
                        AppendColoredText("💬 Chọn một người để bắt đầu chat\n", Color.Gray, true);
                    }
                }
            }
        }

        /// <summary>
        /// Sự kiện click nút Làm mới
        /// </summary>
        private async void BtnRefreshUsers_Click(object sender, EventArgs e)
        {
            if (btn_RefreshUsers != null)
            {
                btn_RefreshUsers.Enabled = false;
                btn_RefreshUsers.Text = "Đang tải...";
            }

            try
            {
                LoadChatUsers(txt_SearchUser?.Text ?? "");

                // Nếu đang chat với ai đó, refresh tin nhắn
                if (_selectedChatUserId > 0)
                {
                    await LoadChatMessagesAsync(_selectedChatUserId);
                }
            }
            finally
            {
                if (btn_RefreshUsers != null)
                {
                    btn_RefreshUsers.Enabled = true;
                    btn_RefreshUsers.Text = "🔄 Làm mới";
                }
            }
        }

        /// <summary>
        /// Cập nhật header khi chọn người chat
        /// </summary>
        private void UpdateChatHeader(string tenNguoi, string vaiTro)
        {
            if (lbl_ChatTitle != null)
            {
                lbl_ChatTitle.Text = $"💬 Chat với: {tenNguoi}";
            }

            if (lbl_ChatRole != null)
            {
                lbl_ChatRole.Text = $"Vai trò: {vaiTro}";
            }
        }

        #endregion

        #region CHAT CLEANUP

        /// <summary>
        /// Dọn dẹp resources khi đóng form
        /// Thêm vào phương thức OnFormClosing đã có
        /// </summary>
        private void CleanupChatResources()
        {
            if (_chatRefreshTimer != null)
            {
                _chatRefreshTimer.Stop();
                _chatRefreshTimer.Dispose();
                _chatRefreshTimer = null;
            }
        }

        #endregion

        private async void btn_ThongBao_PhucVu_Click(object sender, EventArgs e)
        {
            try
            {
                // Hiển thị form nhập thông báo
                string noiDung = Microsoft.VisualBasic.Interaction.InputBox(
                    "Nhập nội dung thông báo gửi cho Bếp:",
                    "📢 Gửi Thông Báo",
                    ""
                );

                if (string.IsNullOrWhiteSpace(noiDung))
                {
                    return;
                }

                // Gửi thông báo
                await SendChatNotificationAsync(noiDung, "Bep");
                ShowSuccess("Đã gửi thông báo cho tất cả nhân viên Bếp!");
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi gửi thông báo: {ex.Message}");
            }
        }

        #region TIMEZONE HELPERS

        /// <summary>
        /// Lấy thời gian hiện tại theo múi giờ Việt Nam (UTC+7)
        /// </summary>
        private DateTime GetVietnamTime()
        {
            try
            {
                TimeZoneInfo vietnamZone;
                try
                {
                    vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                }
                catch
                {
                    try
                    {
                        vietnamZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                    }
                    catch
                    {
                        vietnamZone = TimeZoneInfo.CreateCustomTimeZone(
                            "Vietnam", TimeSpan.FromHours(7), "Vietnam Time", "Vietnam Time");
                    }
                }
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamZone);
            }
            catch
            {
                return DateTime.UtcNow.AddHours(7);
            }
        }

        /// <summary>
        /// Chuyển giờ Việt Nam sang UTC (để lưu vào database)
        /// </summary>
        private DateTime ConvertVietnamToUtc(DateTime vietnamTime)
        {
            try
            {
                if (vietnamTime.Kind == DateTimeKind.Utc)
                    return vietnamTime;
                return vietnamTime.AddHours(-7);
            }
            catch
            {
                return vietnamTime.AddHours(-7);
            }
        }

        /// <summary>
        /// Chuyển UTC sang giờ Việt Nam (để hiển thị)
        /// </summary>
        private DateTime ConvertUtcToVietnam(DateTime utcTime)
        {
            try
            {
                if (utcTime.Kind == DateTimeKind.Local)
                    return utcTime;
                return utcTime.AddHours(7);
            }
            catch
            {
                return utcTime.AddHours(7);
            }
        }

        #endregion
        // ========== KẾT THÚC ĐOẠN CODE THÊM ==========
        #region TOKEN AUTHENTICATION

        private System.Windows.Forms.Timer? _tokenRefreshTimer;

        /// <summary>
        /// Khởi tạo timer tự động refresh token
        /// </summary>
        private void InitializeTokenRefreshTimer()
        {
            _tokenRefreshTimer = new System.Windows.Forms.Timer();
            _tokenRefreshTimer.Interval = 30 * 60 * 1000; // 30 phút
            _tokenRefreshTimer.Tick += async (s, e) => await RefreshTokenAsync();
            _tokenRefreshTimer.Start();
            Console.WriteLine("✅ [NVPhucVu] Đã khởi tạo Token Refresh Timer");
        }

        /// <summary>
        /// Tự động refresh token khi sắp hết hạn
        /// </summary>
        private async Task RefreshTokenAsync()
        {
            try
            {
                if (!CurrentUser.IsTokenValid())
                {
                    Console.WriteLine("⚠️ Token đã hết hạn, cần đăng nhập lại");
                    ForceLogout("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
                    return;
                }

                if (CurrentUser.IsTokenExpiringSoon())
                {
                    Console.WriteLine("🔄 Token sắp hết hạn, đang refresh...");

                    var request = new RefreshTokenRequest
                    {
                        MaNguoiDung = CurrentUser.Id,
                        Token = CurrentUser.Token
                    };

                    var response = await SendRequest<RefreshTokenRequest, RefreshTokenResponse>(request);

                    if (response?.Success == true)
                    {
                        CurrentUser.Token = response.NewToken;
                        CurrentUser.TokenExpiry = response.TokenExpiry;
                        Console.WriteLine($"✅ Token đã được refresh, hết hạn mới: {response.TokenExpiry:HH:mm:ss dd/MM/yyyy}");
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Refresh token thất bại: {response?.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi refresh token: {ex.Message}");
            }
        }

        /// <summary>
        /// Gửi request logout đến server
        /// </summary>
        private async Task SendLogoutRequestAsync()
        {
            try
            {
                if (CurrentUser.Id <= 0 || string.IsNullOrEmpty(CurrentUser.Token))
                {
                    Console.WriteLine("⚠️ Không có user/token để logout");
                    return;
                }

                var request = new LogoutRequest
                {
                    MaNguoiDung = CurrentUser.Id,
                    Token = CurrentUser.Token
                };

                var response = await SendRequest<LogoutRequest, LogoutResponse>(request);

                if (response?.Success == true)
                {
                    Console.WriteLine($"✅ Server xác nhận logout lúc: {response.ThoiGianDangXuat:HH:mm:ss dd/MM/yyyy}");
                }
                else
                {
                    Console.WriteLine($"⚠️ Logout response: {response?.Message ?? "Không có phản hồi"}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Không thể gửi logout request: {ex.Message}");
            }
        }

        /// <summary>
        /// Buộc đăng xuất khi token hết hạn
        /// </summary>
        private void ForceLogout(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ForceLogout(message)));
                return;
            }

            MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            StopAllTimers();
            CurrentUser.Clear();

            var loginForm = new DangNhap();
            loginForm.StartPosition = FormStartPosition.CenterScreen;
            loginForm.Show();

            this.Close();
        }

        /// <summary>
        /// Dừng tất cả timer
        /// </summary>
        private void StopAllTimers()
        {
            try
            {
                _autoRefreshTimer?.Stop();
                _autoRefreshTimer?.Dispose();
                _autoRefreshTimer = null;

                _clockTimer?.Stop();
                _clockTimer?.Dispose();
                _clockTimer = null;

                _chatRefreshTimer?.Stop();
                _chatRefreshTimer?.Dispose();
                _chatRefreshTimer = null;

                _tokenRefreshTimer?.Stop();
                _tokenRefreshTimer?.Dispose();
                _tokenRefreshTimer = null;

                _checkPaymentTimer?.Stop();
                _checkPaymentTimer?.Dispose();

                Console.WriteLine("✅ Đã dừng tất cả timer");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi dừng timer: {ex.Message}");
            }
        }

        #endregion
    }
}
    
