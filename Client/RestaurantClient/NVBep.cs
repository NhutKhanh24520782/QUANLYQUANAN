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
using System.Data.SqlClient; // Hoặc Microsoft.Data.SqlClient
using OfficeOpenXml;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RestaurantClient
{
    public partial class NVBep : Form
    {
        // ==================== CONSTANTS & FIELDS ====================
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;

        private int _currentUserId;
        private string _currentUserName;
        private List<NguoiDung> _danhSachDauBep = new List<NguoiDung>();
        private GridViewManager<KitchenOrderData>? _ordersManager;
        private GridViewManager<KitchenDishData>? _dishesManager;
        private System.Windows.Forms.Timer? _autoRefreshTimer;
        private System.Windows.Forms.Timer? _clockTimer; // ✅ THÊM DÒNG NÀY
        private KitchenOrderDetailData? _currentOrderDetail;
        private KitchenDishData? _selectedDish;

        // ==================== INITIALIZATION ====================
        public NVBep(int userId, string userName)
        {
            _currentUserId = userId;
            _currentUserName = userName;

            InitializeComponent();
            InitializeGridViewManagers();
            InitializeComboBoxes();
            InitializeAutoRefreshTimer(); 
            InitializeClockTimer(); // ✅ THÊM DÒNG NÀY
            UpdateUserInfo();
            InitializeEmptyDataGridView(); // THÊM DÒNG NÀY
            LoadKitchenUserInfo();

            // Thiết lập ngày mặc định (7 ngày gần nhất)
            dateTimePicker_tungay.Value = DateTime.Now.AddDays(-7);
            dateTimePicker_denngay.Value = DateTime.Now;

            // Tải danh sách đầu bếp
            _ = LoadDanhSachDauBepAsync();

            // Tải dữ liệu thống kê ban đầu
            _ = TaiDuLieuThongKeAsync();

            // Đảm bảo form hiển thị tab quản lý đầu tiên
            tc_nvbep.SelectedIndex = 0;

            // Load orders ngay sau khi khởi tạo xong
            _ = LoadOrdersOnStartup();
        }
        // ✅ THÊM HÀM MỚI: Khởi tạo timer cập nhật đồng hồ
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
        private DateTime GetVietnamTime()
        {
            // Azure SQL lưu UTC, nên chúng ta cần chuyển đổi
            try
            {
                TimeZoneInfo vietnamTimeZone;
                try
                {
                    vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                }
                catch
                {
                    vietnamTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                        "Vietnam",
                        TimeSpan.FromHours(7),
                        "Vietnam Time",
                        "Vietnam Time");
                }

                // Chuyển từ UTC sang Việt Nam
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            }
            catch
            {
                // Fallback
                return DateTime.Now;
            }
        }

        // Thêm hàm chuyển đổi khi gửi request
        private DateTime ConvertToUtcForAzure(DateTime vietnamTime)
        {
            try
            {
                TimeZoneInfo vietnamTimeZone;
                try
                {
                    vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                }
                catch
                {
                    vietnamTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                        "Vietnam",
                        TimeSpan.FromHours(7),
                        "Vietnam Time",
                        "Vietnam Time");
                }

                // Chuyển từ Việt Nam sang UTC
                return TimeZoneInfo.ConvertTimeToUtc(vietnamTime, vietnamTimeZone);
            }
            catch
            {
                // Fallback: trừ 7 giờ
                return vietnamTime.AddHours(-7);
            }
        }

        private void NVBep_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            cb_thongkedaubep.DisplayMember = "HoTen";
        }

        private void InitializeEmptyDataGridView()
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new Action(InitializeEmptyDataGridView));
                return;
            }

            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add("Đang tải dữ liệu...");
        }
        // Trong GridViewManager.cs - thêm method này nếu chưa có
        private async Task LoadOrdersOnStartup()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (_ordersManager != null)
                {
                    // 1. Tải dữ liệu từ server
                    bool success = await _ordersManager.LoadDataAsync();

                    if (success)
                    {
                        // Gọi bind để chắc chắn
                        _ordersManager.BindToGridView();

                        Console.WriteLine($"✅ Đã tải thành công. Số dòng: {_ordersManager.GetRowCount()}");
                    }
                    else
                    {
                        ShowError("Không thể tải danh sách đơn hàng");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
        private void InitializeGridViewManagers()
        {
            // Manager cho danh sách đơn hàng (Form chính)
            _ordersManager = new GridViewManager<KitchenOrderData>(
                dataGridView1,
                LoadKitchenOrdersFromServer,
                order => new
                {
                    STT = _ordersManager?.GetRowCount() + 1 ?? 1,
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

            ConfigureOrdersGridView();
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
            this.Shown += async (s, e) =>
            {
                // Tự động load đơn hàng khi form hiển thị
                if (_ordersManager != null)
                {
                    await _ordersManager.LoadDataAsync();
                }
            };
        }

        private void ConfigureOrdersGridView()
        {
            dataGridView1.AutoGenerateColumns = true; // Để true để tự động tạo cột
            dataGridView1.Columns.Clear();

            // Định nghĩa các cột với DataPropertyName ĐÚNG
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "STT",
                HeaderText = "STT",
                DataPropertyName = "STT", // ✅ ĐÚNG
                Width = 50,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaDonHang",
                HeaderText = "Mã đơn",
                DataPropertyName = "MaDonHang", // ✅ SỬA: "MaDonHang" thay vì "STT"
                Width = 80,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Ban",
                HeaderText = "Bàn",
                DataPropertyName = "Ban", // ✅ SỬA: "Ban" thay vì "STT"
                Width = 80,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThoiGian",
                HeaderText = "Thời gian",
                DataPropertyName = "ThoiGian", // ✅ SỬA: "ThoiGian" thay vì "STT"
                Width = 100,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Mon",
                HeaderText = "Số món",
                DataPropertyName = "Mon", // ✅ SỬA: "Mon" thay vì "STT"
                Width = 70,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                DataPropertyName = "TrangThai", // ✅ SỬA: "TrangThai" thay vì "STT"
                Width = 120,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "UuTien",
                HeaderText = "Ưu tiên",
                DataPropertyName = "UuTien", // ✅ SỬA: "UuTien" thay vì "STT"
                Width = 80,
                ReadOnly = true
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cho",
                HeaderText = "Thời gian chờ",
                DataPropertyName = "Cho", // ✅ SỬA: "Cho" thay vì "STT"
                Width = 100,
                ReadOnly = true
            });
        }
        private async Task<bool> CheckServerConnection()
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.ReceiveTimeout = 3000;
                    client.SendTimeout = 3000;

                    await client.ConnectAsync(SERVER_IP, SERVER_PORT);
                    return client.Connected;
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task LoadOrdersWithRetry(int retryCount = 3)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    if (await CheckServerConnection())
                    {
                        if (_ordersManager != null)
                        {
                            await _ordersManager.LoadDataAsync();
                            return;
                        }
                    }
                    else
                    {
                        if (i < retryCount - 1)
                        {
                            await Task.Delay(2000); // Chờ 2 giây trước khi thử lại
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi tải đơn hàng (lần {i + 1}): {ex.Message}");

                    if (i == retryCount - 1)
                    {
                        ShowError($"Không thể kết nối đến server sau {retryCount} lần thử");
                    }
                }
            }
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

        // =========THONG KE BEP===============
        // Hàm chuyển đổi từ GetThongKeBepResponse sang ThongKeBepDayDuResult
        private ThongKeBepDayDuResult ConvertResponseToResult(GetThongKeBepResponse response)
        {
            var result = new ThongKeBepDayDuResult
            {
                Success = response.Success,
                Message = response.Message,
                TongQuan = new ThongKeBepTongQuan
                {
                    TongDon = response.TongQuan.TongDon,
                    DonHoanThanh = response.TongQuan.DonHoanThanh,
                    TyLeHoanThanh = response.TongQuan.TyLeHoanThanh,
                    TongMon = response.TongQuan.TongMon,
                    ThoiGianTrungBinh = response.TongQuan.ThoiGianTrungBinh
                }
            };

            // Chuyển đổi danh sách đầu bếp
            result.DanhSachDauBep = response.DanhSachDauBep.Select(d => new ThongKeDauBep
            {
                MaNguoiDung = d.MaNguoiDung,
                HoTen = d.HoTen,
                TongDon = d.TongDon,
                DonHoanThanh = d.DonHoanThanh,
                TyLeHoanThanh = d.TyLeHoanThanh,
                ThoiGianTrungBinh = d.ThoiGianTrungBinh,
                DanhGiaHieuSuat = d.DanhGiaHieuSuat
            }).ToList();

            // Chuyển đổi top món ăn
            result.TopMonAn = response.TopMonAn.Select(m => new TopMonAnThongKe
            {
                TenMon = m.TenMon,
                TenLoai = m.TenLoai,
                SoLuong = m.SoLuong,
                SoDon = m.SoDon,
                TyLe = m.TyLe
            }).ToList();

            return result;
        }

        private async Task TaiDuLieuThongKeAsync()
        {
            try
            {
                // Hiển thị loading
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(async () => await TaiDuLieuThongKeAsync()));
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;

                // Lấy thông tin lọc
                DateTime tuNgay = dateTimePicker_tungay.Value.Date;
                DateTime denNgay = dateTimePicker_denngay.Value.Date;

                // Kiểm tra ngày hợp lệ
                if (tuNgay > denNgay)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc!",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy mã đầu bếp nếu được chọn
                int? maNhanVien = null;
                if (cb_thongkedaubep.SelectedItem is NguoiDung selectedDauBep &&
                    cb_thongkedaubep.SelectedIndex > 0)
                {
                    maNhanVien = selectedDauBep.MaNguoiDung;
                }

                // Gọi phương thức trong cùng class
                var result = await GetThongKeBepFromServer(tuNgay, denNgay, maNhanVien);

                if (result.Success)
                {
                    // Cập nhật giao diện
                    CapNhatGiaoDienThongKe(result);
                }
                else
                {
                    MessageBox.Show($"Lỗi: {result.Message}", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thống kê: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private async Task LoadDanhSachDauBepAsync()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(async () => await LoadDanhSachDauBepAsync()));
                    return;
                }

                // Thêm "Tất cả"
                cb_thongkedaubep.Items.Clear();
                cb_thongkedaubep.Items.Add("Tất cả");

                // Lấy danh sách đầu bếp từ server
                var danhSachDauBep = await GetDanhSachDauBepFromServer();

                // Thêm vào combobox
                foreach (var dauBep in danhSachDauBep)
                {
                    cb_thongkedaubep.Items.Add(dauBep);
                }

                // Chọn mặc định là "Tất cả"
                cb_thongkedaubep.SelectedIndex = 0;
                cb_thongkedaubep.DisplayMember = "HoTen";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách đầu bếp: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CapNhatGiaoDienThongKe(ThongKeBepDayDuResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<ThongKeBepDayDuResult>(CapNhatGiaoDienThongKe), result);
                return;
            }

            // Cập nhật tổng quan
            var tongQuan = result.TongQuan;

            lbl_tongdon.Text = $"Tổng đơn:   {tongQuan.TongDon} 📋";
            lbl_HoanThanh.Text = $"Hoàn thành: {Math.Round(tongQuan.TyLeHoanThanh, 1)}% ✅";
            lbl_TongMon.Text = $"Tổng món:   {tongQuan.TongMon} 🍽️";

            // Tính đơn trung bình ngày
            double soNgay = (dateTimePicker_denngay.Value - dateTimePicker_tungay.Value).TotalDays + 1;
            double donTB = soNgay > 0 ? (double)tongQuan.TongDon / soNgay : 0;
            lbl_DonTB.Text = $"Đơn TB/ngày: {Math.Round(donTB, 1)} 📈";

            // Thời gian trung bình
            string thoiGianTB = tongQuan.ThoiGianTrungBinh.HasValue ?
                $"{Math.Round(tongQuan.ThoiGianTrungBinh.Value, 1)}p" : "0p";
            lbl_ThoiGianTB.Text = $"Thời gian TB: {thoiGianTB} ⏱️";

            // Hiệu suất
            string hieuSuat = "⭐☆☆☆☆";
            if (tongQuan.ThoiGianTrungBinh.HasValue && tongQuan.TyLeHoanThanh > 0)
            {
                double diem =
                   ((double)tongQuan.TyLeHoanThanh / 100.0 * 0.7) +
                   ((30.0 - Math.Min((double)tongQuan.ThoiGianTrungBinh.Value, 30.0)) / 30.0 * 0.3);

                if (diem >= 0.8) hieuSuat = "⭐⭐⭐⭐⭐";
                else if (diem >= 0.6) hieuSuat = "⭐⭐⭐⭐☆";
                else if (diem >= 0.4) hieuSuat = "⭐⭐⭐☆☆";
                else if (diem >= 0.2) hieuSuat = "⭐⭐☆☆☆";
            }
            lbl_HieuSuat.Text = $"Hiệu suất: {hieuSuat}";

            // Cập nhật ListView cho top đầu bếp
            listView1.Items.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;

            // Thêm cột cho ListView
            listView1.Columns.Clear();
            listView1.Columns.Add("STT", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Tên đầu bếp", 200, HorizontalAlignment.Left);
            listView1.Columns.Add("Tổng đơn", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Đơn HT", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Tỷ lệ HT", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Thời gian TB", 100, HorizontalAlignment.Center);
            listView1.Columns.Add("Hiệu suất", 100, HorizontalAlignment.Center);

            // Thêm dữ liệu vào ListView
            int stt = 1;
            foreach (var dauBep in result.DanhSachDauBep.OrderByDescending(x => x.TongDon))
            {
                ListViewItem item = new ListViewItem(stt.ToString());
                item.SubItems.Add(dauBep.HoTen ?? "");
                item.SubItems.Add(dauBep.TongDon.ToString());
                item.SubItems.Add(dauBep.DonHoanThanh.ToString());
                item.SubItems.Add($"{Math.Round(dauBep.TyLeHoanThanh, 1)}%");
                item.SubItems.Add(dauBep.ThoiGianTrungBinh.HasValue ?
                    $"{Math.Round(dauBep.ThoiGianTrungBinh.Value, 1)}p" : "N/A");
                item.SubItems.Add(dauBep.DanhGiaHieuSuat ?? "");

                // Tô màu cho hàng đầu tiên (top 1)
                if (stt == 1)
                {
                    item.BackColor = Color.LightGoldenrodYellow;
                    item.Font = new Font(listView1.Font, FontStyle.Bold);
                }

                listView1.Items.Add(item);
                stt++;
            }

            // Cập nhật ListView cho top món phổ biến
            listView_topmonphobien.Items.Clear();
            listView_topmonphobien.View = View.Details;
            listView_topmonphobien.GridLines = true;
            listView_topmonphobien.FullRowSelect = true;

            // Thêm cột cho ListView
            listView_topmonphobien.Columns.Clear();
            listView_topmonphobien.Columns.Add("STT", 50, HorizontalAlignment.Center);
            listView_topmonphobien.Columns.Add("Tên món", 250, HorizontalAlignment.Left);
            listView_topmonphobien.Columns.Add("Loại", 100, HorizontalAlignment.Left);
            listView_topmonphobien.Columns.Add("Số lượng", 80, HorizontalAlignment.Center);
            listView_topmonphobien.Columns.Add("Số đơn", 80, HorizontalAlignment.Center);
            listView_topmonphobien.Columns.Add("Tỷ lệ", 80, HorizontalAlignment.Center);

            // Thêm dữ liệu vào ListView
            stt = 1;
            foreach (var mon in result.TopMonAn.OrderByDescending(x => x.SoLuong))
            {
                ListViewItem item = new ListViewItem(stt.ToString());
                item.SubItems.Add(mon.TenMon ?? "");
                item.SubItems.Add(mon.TenLoai ?? "");
                item.SubItems.Add(mon.SoLuong.ToString());
                item.SubItems.Add(mon.SoDon.ToString());
                item.SubItems.Add($"{Math.Round(mon.TyLe, 1)}%");

                // Tô màu cho 3 món đầu
                if (stt <= 3)
                {
                    if (stt == 1) item.BackColor = Color.LightGoldenrodYellow;
                    else if (stt == 2) item.BackColor = Color.LightGray;
                    else if (stt == 3) item.BackColor = Color.LightSalmon;
                    item.Font = new Font(listView_topmonphobien.Font, FontStyle.Bold);
                }

                listView_topmonphobien.Items.Add(item);
                stt++;
            }

            // Cập nhật tiêu đề với thông tin ngày thống kê
            lbl_titlethongke.Text = $"THỐNG KÊ TỪ {dateTimePicker_tungay.Value:dd/MM/yyyy} ĐẾN {dateTimePicker_denngay.Value:dd/MM/yyyy}";

            // Hiển thị số lượng đầu bếp
            if (cb_thongkedaubep.SelectedIndex == 0) // Tất cả
            {
                lbl_topdaubep.Text = $"TOP ĐẦU BẾP ({result.DanhSachDauBep.Count} đầu bếp)";
            }
            else if (cb_thongkedaubep.SelectedItem is NguoiDung selectedDauBep)
            {
                lbl_topdaubep.Text = $"THỐNG KÊ ĐẦU BẾP: {selectedDauBep.HoTen}";
            }
        }


        private async void LoadDauBepComboBox()
        {
            try
            {
                // Sử dụng hàm đã định nghĩa để lấy danh sách
                var danhSachDauBep = await GetDanhSachDauBepFromServer();

                // Đảm bảo không null
                _danhSachDauBep = danhSachDauBep ?? new List<NguoiDung>();

                // Thêm chính mình nếu chưa có trong danh sách và là đầu bếp
                if (!_danhSachDauBep.Any(d => d.MaNguoiDung == _currentUserId))
                {
                    // Kiểm tra xem người dùng hiện tại có phải là đầu bếp không
                    // (Có thể cần truy vấn database để kiểm tra, tạm thời giả sử là đầu bếp)
                    _danhSachDauBep.Add(new NguoiDung
                    {
                        MaNguoiDung = _currentUserId,
                        HoTen = _currentUserName
                    });
                }

                // Clear và thêm vào combobox
                cb_daubep.Items.Clear();

                // Thêm danh sách đầu bếp vào combobox
                foreach (var dauBep in _danhSachDauBep)
                {
                    cb_daubep.Items.Add(dauBep.HoTen ?? "");
                }

                // Chọn mặc định là chính mình (nếu có trong danh sách)
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

                // Nếu không tìm thấy, chọn đầu tiên
                if (!found && cb_daubep.Items.Count > 0)
                {
                    cb_daubep.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải danh sách đầu bếp: {ex.Message}");

                // Fallback: thêm chính mình
                if (cb_daubep.InvokeRequired)
                {
                    cb_daubep.Invoke(new Action(() =>
                    {
                        cb_daubep.Items.Clear();
                        cb_daubep.Items.Add(_currentUserName);
                        cb_daubep.SelectedIndex = 0;
                    }));
                }
                else
                {
                    cb_daubep.Items.Clear();
                    cb_daubep.Items.Add(_currentUserName);
                    cb_daubep.SelectedIndex = 0;
                }
            }
        }
        private void InitializeTimeComboBox()
        {
            cb_timedukien.Items.Clear();

            // DÙNG GIỜ VIỆT NAM
            var now = GetVietnamTime();

            // Làm tròn lên 15 phút gần nhất
            int minutes = now.Minute;
            int roundedMinutes = ((minutes / 15) + 1) * 15;
            var startTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddMinutes(roundedMinutes);

            // Tạo danh sách từ thời gian làm tròn đến 2 giờ sau
            for (int i = 0; i <= 8; i++) // 2 giờ * 4 (mỗi 15 phút)
            {
                var time = startTime.AddMinutes(i * 15);
                cb_timedukien.Items.Add(time.ToString("HH:mm"));
            }

            // Mặc định chọn slot đầu tiên (gần nhất)
            if (cb_timedukien.Items.Count > 0)
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
                    if (_ordersManager != null)
                    {
                        await _ordersManager.RefreshAsync();
                    }
                }
            };
            _autoRefreshTimer.Start();
        }

        private void UpdateUserInfo()
        {
            if (lbl_userInfo.InvokeRequired)
            {
                lbl_userInfo.Invoke(new Action(UpdateUserInfo));
                return;
            }

            DateTime vietnamTime = GetVietnamTime();

            // Hiển thị đầy đủ thông tin
            lbl_userInfo.Text = $"👨‍🍳 {_currentUserName} • {vietnamTime:HH:mm:ss dd/MM/yyyy} (UTC+7)";
        }
        private async Task<ThongKeBepDayDuResult> GetThongKeBepFromServer(DateTime tuNgay, DateTime denNgay, int? maNhanVien = null)
        {
            var request = new GetThongKeBepRequest
            {
                TuNgay = tuNgay,
                DenNgay = denNgay,
                MaNhanVienBep = maNhanVien
            };

            var json = JsonConvert.SerializeObject(request) + "\n";

            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(SERVER_IP, SERVER_PORT);

                    using (var stream = client.GetStream())
                    using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        await writer.WriteLineAsync(json.TrimEnd('\n'));
                        var responseJson = await reader.ReadLineAsync();

                        if (!string.IsNullOrEmpty(responseJson))
                        {
                            var response = JsonConvert.DeserializeObject<GetThongKeBepResponse>(responseJson);
                            if (response != null && response.Success)
                            {
                                // CHUYỂN ĐỔI TỪ GetThongKeBepResponse SANG ThongKeBepDayDuResult
                                return ConvertResponseToResult(response);
                            }
                        }

                        return new ThongKeBepDayDuResult
                        {
                            Success = false,
                            Message = "Không nhận được phản hồi từ server"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ThongKeBepDayDuResult
                {
                    Success = false,
                    Message = $"Lỗi kết nối: {ex.Message}"
                };
            }
        }

        private async Task<List<NguoiDung>> GetDanhSachDauBepFromServer()
        {
            var request = new GetDanhSachDauBepRequest();
            var json = JsonConvert.SerializeObject(request) + "\n";

            try
            {
                using (var client = new TcpClient())
                {
                    await client.ConnectAsync(SERVER_IP, SERVER_PORT);

                    using (var stream = client.GetStream())
                    using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        await writer.WriteLineAsync(json.TrimEnd('\n'));
                        var responseJson = await reader.ReadLineAsync();

                        if (!string.IsNullOrEmpty(responseJson))
                        {
                            var result = JsonConvert.DeserializeObject<GetDanhSachDauBepResponse>(responseJson);
                            if (result != null && result.Success)
                            {
                                return result.DanhSachDauBep ?? new List<NguoiDung>();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDanhSachDauBep: {ex}");
            }

            return new List<NguoiDung>();
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

        private async Task LoadThongKeAsync()
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

            // Cập nhật trạng thái hiện tại
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

            // Cập nhật độ ưu tiên
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

            // Làm mới danh sách thời gian dự kiến với giờ Việt Nam
            InitializeTimeComboBox();

            // ✅ SỬA: Cập nhật thời gian dự kiến (nếu có) - CHUYỂN ĐỔI TỪ UTC SANG VIỆT NAM
            if (dish.ThoiGianDuKien.HasValue)
            {
                // Giả sử ThoiGianDuKien trong database là UTC, chuyển sang Việt Nam
                DateTime thoiGianVietnam = ConvertFromUtcToVietnamTime(dish.ThoiGianDuKien.Value);
                string timeString = thoiGianVietnam.ToString("HH:mm");

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

        // THÊM HÀM CHUYỂN ĐỔI TỪ UTC SANG GIỜ VIỆT NAM
        private DateTime ConvertFromUtcToVietnamTime(DateTime utcTime)
        {
            try
            {
                TimeZoneInfo vietnamTimeZone;
                try
                {
                    vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                }
                catch (TimeZoneNotFoundException)
                {
                    try
                    {
                        vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        // Fallback: tạo múi giờ UTC+7
                        vietnamTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                            "Vietnam",
                            TimeSpan.FromHours(7),
                            "Vietnam Time",
                            "Vietnam Time");
                    }
                }

                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, vietnamTimeZone);
            }
            catch
            {
                // Fallback: cộng 7 giờ
                return utcTime.AddHours(7);
            }
        }
        // ==================== EVENT HANDLERS ====================
        private void DataGridView_Orders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Kiểm tra và thoát sớm nếu không cần định dạng
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];
            var order = _ordersManager?.GetAllDisplayedItems().ElementAtOrDefault(e.RowIndex);

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
                }

                // 3. Định dạng màu theo ưu tiên (Sử dụng giá trị đã tính toán/có sẵn)
                if (dataGridView1.Columns.Contains("UuTien") && e.ColumnIndex == dataGridView1.Columns["UuTien"].Index)
                {
                    e.CellStyle.ForeColor = GetPriorityColor(order.UuTienCaoNhat);
                }
            }
        }

        private void DataGridView_Orders_SelectionChanged(object sender, EventArgs e)
        {
            var selectedOrder = _ordersManager?.GetSelectedItem();
            if (selectedOrder != null)
            {
                // Có thể highlight hoặc hiển thị thông tin nhanh
                // Không tự động mở chi tiết, phải double click
            }
        }

        private void DataGridView_Dishes_SelectionChanged(object sender, EventArgs e)
        {
            var selectedDish = _dishesManager?.GetSelectedItem();
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
                var dish = _dishesManager?.GetItemAtRow(e.RowIndex);
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

            var dish = _dishesManager?.GetItemAtRow(e.RowIndex);
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

            var selectedOrder = _ordersManager?.GetSelectedItem();
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
                var selectedDish = _dishesManager?.GetSelectedItem();
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
            if (_ordersManager != null)
            {
                await _ordersManager.RefreshAsync();
            }
        }

        private async void ComboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_ordersManager != null)
            {
                await _ordersManager.RefreshAsync();
            }
        }

        private void tb_numberTable_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (_ordersManager != null)
                {
                    _ = _ordersManager.RefreshAsync();
                }
            }
        }

        // ==================== BUTTON HANDLERS ====================
        private async void btn_refresh_Click(object sender, EventArgs e)
        {
            await ExecuteAsync(btn_refresh, "Đang làm mới...", async () =>
            {
                if (_ordersManager != null)
                {
                    Console.WriteLine("Người dùng nhấn nút Refresh...");
                    bool success = await _ordersManager.RefreshAsync();

                    if (success)
                    {
                        ShowSuccess($"Đã làm mới danh sách đơn hàng ({_ordersManager.GetRowCount()} đơn)");
                    }
                    else
                    {
                        ShowError("Không thể tải lại danh sách đơn hàng");
                    }
                }
            });
        }

        private async void btn_xemthongke_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Chuyển sang tab thống kê
                tc_nvbep.SelectedIndex = 2; // Giả sử tab thống kê là index 2

                // 2. Tự động tải thống kê với khoảng thời gian mặc định
                _ = TaiDuLieuThongKeAsync();

                // 3. Hiển thị thông báo (tuỳ chọn)
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        ShowSuccess("Đang tải dữ liệu thống kê...");
                    }));
                }
                else
                {
                    ShowSuccess("Đang tải dữ liệu thống kê...");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi chuyển tab thống kê: {ex.Message}");
            }
        }
        private async void btn_taithongke_Click(object sender, EventArgs e)
        {
            await ExecuteAsync(btn_taithongke, "🔄 Đang tải...", async () =>
            {
                await TaiDuLieuThongKeAsync();

                // Hiển thị thông báo thành công
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        ShowSuccess("Đã tải thống kê mới nhất!");
                    }));
                }
                else
                {
                    ShowSuccess("Đã tải thống kê mới nhất!");
                }
            });
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

            string? trangThaiMoi = ConvertDisplayToStatus(cb_status.SelectedItem?.ToString());
            int uuTienMoi = cb_uutien.SelectedIndex + 1;
            string? tenDauBep = cb_daubep.SelectedItem?.ToString();
            int maDauBep = GetMaDauBep(tenDauBep);

            if (string.IsNullOrEmpty(trangThaiMoi))
            {
                ShowWarning("Vui lòng chọn trạng thái!");
                return;
            }

            DateTime? thoiGianDuKienUtc = null;
            string? timeString = cb_timedukien.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(timeString))
            {
                try
                {
                    if (TimeSpan.TryParse(timeString, out TimeSpan timeOfDay))
                    {
                        // Lấy giờ Việt Nam hiện tại
                        DateTime vietnamNow = GetVietnamTime();

                        // Tạo thời gian Việt Nam
                        DateTime vietnamTime = vietnamNow.Date.Add(timeOfDay);

                        // Nếu đã qua trong ngày, cộng thêm 1 ngày
                        if (vietnamTime < vietnamNow)
                        {
                            vietnamTime = vietnamTime.AddDays(1);
                        }

                        // ✅ QUAN TRỌNG: Chuyển sang UTC trước khi gửi lên Azure
                        thoiGianDuKienUtc = ConvertToUtcForAzure(vietnamTime);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi parse thời gian: {ex.Message}");
                    thoiGianDuKienUtc = null;
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
                        ThoiGianDuKienHoanThanh = thoiGianDuKienUtc, // Đã là UTC
                        UuTien = uuTienMoi,
                        GuiThongBao = true
                    };

                    var response = await SendRequest<UpdateDishStatusRequest, UpdateDishStatusResponse>(request);

                    if (response?.Success == true)
                    {
                        ShowSuccess($"Đã cập nhật trạng thái '{_selectedDish.TenMon}' thành '{response.TrangThaiMoi}'");

                        // Refresh
                        if (_dishesManager != null) await _dishesManager.RefreshAsync();
                        if (_ordersManager != null) await _ordersManager.RefreshAsync();

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
        private DateTime ConvertToUtc(DateTime vietnamTime)
        {
            try
            {
                TimeZoneInfo vietnamTimeZone;
                try
                {
                    vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                }
                catch (TimeZoneNotFoundException)
                {
                    try
                    {
                        vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        vietnamTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                            "Vietnam",
                            TimeSpan.FromHours(7),
                            "Vietnam Time",
                            "Vietnam Time");
                    }
                }

                return TimeZoneInfo.ConvertTimeToUtc(vietnamTime, vietnamTimeZone);
            }
            catch
            {
                // Fallback: trừ 7 giờ
                return vietnamTime.AddHours(-7);
            }
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
                        if (_dishesManager != null)
                        {
                            await _dishesManager.RefreshAsync();
                        }
                        if (_ordersManager != null)
                        {
                            await _ordersManager.RefreshAsync();
                        }

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
                bool success = false;
                if (_dishesManager != null)
                {
                    success = await _dishesManager.LoadDataAsync();
                }

                if (success)
                {
                    // Hiển thị thông tin đơn hàng
                    UpdateOrderDetailDisplay();

                    // Nếu có món, chọn món đầu tiên
                    var dishes = _dishesManager?.GetAllDisplayedItems();
                    if (dishes != null && dishes.Count > 0)
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
            Console.WriteLine($"GetStatusColor called with: {trangThai}"); // DEBUG

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

        private string ConvertFilterToStatus(string? filterText)
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

        private string ConvertSortToServer(string? sortText)
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

        private string? ConvertDisplayToStatus(string? displayText)
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

        private int GetMaDauBep(string? tenDauBep)
        {
            if (string.IsNullOrEmpty(tenDauBep)) return _currentUserId;

            var dauBep = _danhSachDauBep?.FirstOrDefault(d =>
                d.HoTen != null && d.HoTen.Equals(tenDauBep, StringComparison.OrdinalIgnoreCase));

            return dauBep?.MaNguoiDung ?? _currentUserId;
        }

        // ==================== NETWORK & EXECUTION METHODS ====================
        private static async Task<TResponse?> SendRequest<TRequest, TResponse>(
            TRequest request, CancellationToken cancellationToken = default)
        {
            string json = JsonConvert.SerializeObject(request) + "\n";

            using (var client = new TcpClient())
            {
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                // Kết nối với timeout
                var connectTask = client.ConnectAsync(SERVER_IP, SERVER_PORT);
                var timeoutTask = Task.Delay(5000, cancellationToken);

                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                if (completedTask == timeoutTask)
                {
                    throw new TimeoutException("Kết nối đến server timeout.");
                }

                using (var stream = client.GetStream())
                using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(json.TrimEnd('\n'));

                    // Đọc phản hồi với timeout
                    var readTask = reader.ReadLineAsync();
                    timeoutTask = Task.Delay(5000, cancellationToken);

                    completedTask = await Task.WhenAny(readTask, timeoutTask);
                    if (completedTask == timeoutTask)
                    {
                        throw new TimeoutException("Đọc phản hồi từ server timeout.");
                    }

                    string? responseJson = await readTask;

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
                Console.WriteLine("Bắt đầu tải đơn hàng từ server...");

                var request = new GetKitchenOrdersRequest
                {
                    TrangThai = ConvertFilterToStatus(comboBox1.SelectedItem?.ToString()),
                    TimKiemBan = tb_numberTable.Text.Trim(),
                    SapXep = ConvertSortToServer(cb_sapxep.SelectedItem?.ToString()),
                    MaNhanVienBep = _currentUserId
                };

                Console.WriteLine($"Gửi request: TrangThai={request.TrangThai}, MaNhanVienBep={request.MaNhanVienBep}");

                var response = await SendRequest<GetKitchenOrdersRequest, GetKitchenOrdersResponse>(request);

                if (response?.Success == true)
                {
                    var orders = response.DonHang ?? new List<KitchenOrderData>();

                    Console.WriteLine($"Nhận được {orders.Count} đơn hàng từ server");


                    // Sắp xếp
                    if (ConvertSortToServer(cb_sapxep.SelectedItem?.ToString()) == "ThoiGianCho")
                    {
                        orders = orders.OrderBy(o => o.NgayOrder).ToList();
                    }

                    UpdateThongKeDisplay(response.ThongKe ?? new ThongKeBep());

                    // Hiển thị thông báo nếu không có đơn hàng
                    if (orders.Count == 0)
                    {
                        Console.WriteLine("Không có đơn hàng nào phù hợp với điều kiện lọc");

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                            {
                                // Có thể hiển thị thông báo trên DataGridView
                                dataGridView1.Rows.Clear();
                                dataGridView1.Rows.Add("Không có đơn hàng nào");
                            }));
                        }
                    }

                    return orders;
                }
                else
                {
                    string errorMsg = response?.Message ?? "Không thể tải danh sách đơn hàng";
                    Console.WriteLine($"Lỗi từ server: {errorMsg}");

                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            ShowError(errorMsg);
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(errorMsg);
                        }));
                    }

                    return new List<KitchenOrderData>();
                }
            }
            catch (SocketException)
            {
                string errorMsg = "Không thể kết nối đến server (127.0.0.1:5000).";
                Console.WriteLine(errorMsg);

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        ShowError(errorMsg);
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add(errorMsg);
                    }));
                }

                return new List<KitchenOrderData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi tải đơn hàng: {ex.Message}");

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        ShowError($"Lỗi tải đơn hàng: {ex.Message}");
                        dataGridView1.Rows.Clear();
                        dataGridView1.Rows.Add("Lỗi tải dữ liệu");
                    }));
                }

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
        private void NVBep_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the auto refresh timer
            if (_autoRefreshTimer != null)
            {
                _autoRefreshTimer.Stop();
                _autoRefreshTimer.Dispose();
            }

            // ✅ THÊM: Stop the clock timer
            if (_clockTimer != null)
            {
                _clockTimer.Stop();
                _clockTimer.Dispose();
            }
        }

        private void btn_xuatbaocao_Click(object sender, EventArgs e)
        {
            // Xuất báo cáo Excel
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx|All Files|*.*";
                saveFileDialog.Title = "Xuất báo cáo thống kê bếp";
                saveFileDialog.FileName = $"BaoCaoThongKeBep_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    XuatBaoCaoExcel(saveFileDialog.FileName);
                    MessageBox.Show($"Đã xuất báo cáo thành công!\nĐường dẫn: {saveFileDialog.FileName}",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void XuatBaoCaoExcel(string filePath)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    // Tạo worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Thống kê bếp");

                    // Tiêu đề
                    worksheet.Cells["A1"].Value = "BÁO CÁO THỐNG KÊ HIỆU SUẤT BẾP";
                    worksheet.Cells["A1:D1"].Merge = true;
                    worksheet.Cells["A1"].Style.Font.Size = 16;
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    // Thời gian thống kê
                    worksheet.Cells["A2"].Value = $"Từ ngày: {dateTimePicker_tungay.Value:dd/MM/yyyy}";
                    worksheet.Cells["B2"].Value = $"Đến ngày: {dateTimePicker_denngay.Value:dd/MM/yyyy}";

                    // Tổng quan
                    worksheet.Cells["A4"].Value = "TỔNG QUAN";
                    worksheet.Cells["A4"].Style.Font.Bold = true;

                    worksheet.Cells["A5"].Value = "Tổng đơn:";
                    worksheet.Cells["B5"].Value = lbl_tongdon.Text.Replace("📋", "").Trim();

                    worksheet.Cells["A6"].Value = "Hoàn thành:";
                    worksheet.Cells["B6"].Value = lbl_HoanThanh.Text.Replace("✅", "").Trim();

                    worksheet.Cells["A7"].Value = "Tổng món:";
                    worksheet.Cells["B7"].Value = lbl_TongMon.Text.Replace("🍽️", "").Trim();

                    worksheet.Cells["A8"].Value = "Đơn TB/ngày:";
                    worksheet.Cells["B8"].Value = lbl_DonTB.Text.Replace("📈", "").Trim();

                    worksheet.Cells["A9"].Value = "Thời gian TB:";
                    worksheet.Cells["B9"].Value = lbl_ThoiGianTB.Text.Replace("⏱️", "").Trim();

                    worksheet.Cells["A10"].Value = "Hiệu suất:";
                    worksheet.Cells["B10"].Value = lbl_HieuSuat.Text.Replace("⭐", "").Trim();

                    // Top đầu bếp
                    worksheet.Cells["D4"].Value = "TOP ĐẦU BẾP";
                    worksheet.Cells["D4"].Style.Font.Bold = true;

                    // Header cho top đầu bếp
                    string[] headersDauBep = { "STT", "Tên đầu bếp", "Tổng đơn", "Đơn HT", "Tỷ lệ HT", "Thời gian TB", "Hiệu suất" };
                    for (int i = 0; i < headersDauBep.Length; i++)
                    {
                        worksheet.Cells[5, 4 + i].Value = headersDauBep[i];
                        worksheet.Cells[5, 4 + i].Style.Font.Bold = true;
                        worksheet.Cells[5, 4 + i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    // Dữ liệu top đầu bếp
                    int row = 6;
                    foreach (ListViewItem item in listView1.Items)
                    {
                        for (int col = 0; col < item.SubItems.Count; col++)
                        {
                            worksheet.Cells[row, 4 + col].Value = item.SubItems[col].Text;
                        }
                        row++;
                    }

                    // Top món phổ biến
                    worksheet.Cells["A15"].Value = "TOP MÓN PHỔ BIẾN";
                    worksheet.Cells["A15"].Style.Font.Bold = true;

                    // Header cho top món
                    string[] headersMon = { "STT", "Tên món", "Loại", "Số lượng", "Số đơn", "Tỷ lệ" };
                    for (int i = 0; i < headersMon.Length; i++)
                    {
                        worksheet.Cells[16, 1 + i].Value = headersMon[i];
                        worksheet.Cells[16, 1 + i].Style.Font.Bold = true;
                        worksheet.Cells[16, 1 + i].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    // Dữ liệu top món
                    row = 17;
                    foreach (ListViewItem item in listView_topmonphobien.Items)
                    {
                        for (int col = 0; col < item.SubItems.Count; col++)
                        {
                            worksheet.Cells[row, 1 + col].Value = item.SubItems[col].Text;
                        }
                        row++;
                    }

                    // Điều chỉnh độ rộng cột
                    worksheet.Cells.AutoFitColumns();

                    // Lưu file
                    package.SaveAs(new FileInfo(filePath));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xuất Excel: {ex.Message}", ex);
            }
        }
        private void LoadKitchenUserInfo()
        {
            try
            {
                // Nếu form được khởi tạo trong thread khác thì invoke
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(LoadKitchenUserInfo));
                    return;
                }

                // Nếu bạn có 1 class tĩnh lưu current user, ví dụ CurrentUser hoặc Program.CurrentUser
                // Mình đưa 2 phương án: ưu tiên CurrentUser (nếu có), nếu không thì dùng _currentUserName/_currentUserId

                // PHƯƠNG ÁN A: nếu có lớp CurrentUser (static) chứa thông tin
                try
                {
                    // Thay thế tên thuộc tính theo cách bạn lưu (Username, Email, FullName, Role)
                    if (typeof(CurrentUser) != null) // guard chỉ để đọc code dễ hiểu
                    {
                        // Nếu CurrentUser có properties public
                        textbox_usernamebep.Text = CurrentUser.Username ?? "";
                        textbox_emailbep.Text = CurrentUser.Email ?? "";
                        textbox_tenbep.Text = CurrentUser.FullName ?? "";
                        textbox_chucvubep.Text = CurrentUser.Role ?? "";
                        return;
                    }
                }
                catch { /* nếu không có CurrentUser thì fallback bên dưới */ }

                // PHƯƠNG ÁN B: fallback dùng biến nội bộ đã có trong form
                textbox_usernamebep.Text = _currentUserName ?? "";
                // Nếu không có email/fullname trong form, giữ trống hoặc gán từ server khi có API
                textbox_emailbep.Text = ""; // gán nếu bạn có biến chứa email
                textbox_tenbep.Text = "";   // gán nếu bạn có biến chứa full name
                textbox_chucvubep.Text = ""; // gán vai trò (role) nếu biết
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị thông tin tài khoản: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button_dangxuatnvbep_Click(object sender, EventArgs e)
        {
            // Hỏi xác nhận
            var confirm = MessageBox.Show("Bạn có chắc muốn đăng xuất?",
                                          "Xác nhận",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                // Nếu có timer tự động refresh thì stop lại
                try
                {
                    if (_autoRefreshTimer != null)
                    {
                        _autoRefreshTimer.Stop();
                        _autoRefreshTimer.Tick -= null; // không bắt buộc nhưng cố gắng remove handlers nếu cần
                        _autoRefreshTimer.Dispose();
                        _autoRefreshTimer = null;
                    }
                }
                catch { /* ignore */ }

                // Nếu có GridViewManager hoặc resource cần dispose thì xử lý (nếu class hỗ trợ Dispose)
                try
                {
                    // Nếu GridViewManager implements IDisposable, dispose nó.
                    // Nếu không, bạn có thể gán về null để GC thu dọn.
                    (_ordersManager as IDisposable)?.Dispose();
                    (_dishesManager as IDisposable)?.Dispose();
                }
                catch { /* ignore */ }

                // Reset thông tin người dùng (nếu bạn có class CurrentUser tĩnh)
                try
                {
                    // Nếu project bạn có class CurrentUser, reset các thuộc tính
                    CurrentUser.Id = 0;
                    CurrentUser.Username = "";
                    CurrentUser.Email = "";
                    CurrentUser.FullName = "";
                    CurrentUser.Role = "";
                }
                catch { /* nếu không có CurrentUser thì bỏ qua */ }

                // --- TUỲ CHỌN: gọi API logout tới server ---
                // Nếu server cần biết user logout, bạn có thể gửi request logout ở đây.
                // Mình để ví dụ comment (bạn cần có model LogoutRequest/LogoutResponse):
                /*
                try
                {
                    var logoutRequest = new LogoutRequest { MaNguoiDung = _currentUserId };
                    var logoutResp = await SendRequest<LogoutRequest, LogoutResponse>(logoutRequest);
                    // xử lý logoutResp nếu cần
                }
                catch (Exception ex)
                {
                    // không bắt buộc phải fail nếu logout server lỗi
                    Console.WriteLine("Lỗi gửi logout lên server: " + ex.Message);
                }
                */

                // Mở form đăng nhập lại (tên form của bạn có thể khác: DangNhap, FormLogin, ...)
                try
                {
                    var loginForm = new DangNhap(); // đổi tên nếu form đăng nhập của bạn khác
                    loginForm.StartPosition = FormStartPosition.CenterScreen;
                    loginForm.Show();
                }
                catch (Exception ex)
                {
                    // Nếu không thể mở lại form login, ít nhất thoát app
                    Console.WriteLine("Không mở được form đăng nhập: " + ex.Message);
                }

                // Đóng form NVBep hiện tại (sẽ trở về login hoặc đóng app)
                this.Close();

                // Nếu bạn muốn restart app hoàn toàn thay vì show login, dùng:
                // Application.Restart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng xuất: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tp_thongke_Click(object sender, EventArgs e)
        {

        }
    }


}