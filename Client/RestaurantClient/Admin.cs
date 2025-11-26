using Microsoft.VisualBasic.Devices;
using Models.Database;
using Models.Request;
using Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RestaurantClient
{
    public partial class Admin : Form
    {
        // ==================== GRIDVIEW MANAGERS ====================
        private GridViewManager<EmployeeData> _employeeManager;
        private GridViewManager<DoanhThuTheoBan> _doanhThuManager;
        private GridViewManager<BillData> _billManager;
        private GridViewManager<MenuItemData> _menuManager;
        private GridViewManager<Models.Database.Database.BanAn> _tableManager;

        // ✅ MANAGER CHO BÀN ĂN
        private GridViewManager<Models.Database.Database.BanAn> _tableManager;

        // ==================== CONSTANTS ====================
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;
        private const string SEARCH_PLACEHOLDER = "Tìm theo tên hoặc email...";
        private const string SEARCH_BILL = "Tìm theo mã bàn hoặc nhân viên...";

        // ==================== INITIALIZATION ====================
        public Admin()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Trống");
            comboBox1.Items.Add("Có người");
            comboBox1.Items.Add("Đã đặt");
            comboBox1.SelectedIndex = 0; // Chọn mặc định là Trống
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList; // Không cho gõ bậy
            InitializeGridViewManagers();
            InitializeDoanhThuControls();
            InitializeBillTab();
            InitializeControls();
            LoadAllData();
        }

        private void InitializeBillTab()
        {
            dataGridView_bill.CellFormatting += DataGridView_Bill_CellFormatting;
            dataGridView_bill.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_bill.MultiSelect = false;
            dataGridView_bill.ReadOnly = true;

            var searchBox = this.Controls.Find("tb_searchBill", true).FirstOrDefault() as TextBox;
            if (searchBox != null)
            {
                SetupSearchBox(searchBox, SEARCH_BILL);
            }
        }

        private void InitializeDoanhThuControls()
        {
            dtp_tuNgay.Value = DateTime.Today;
            dtp_denNgay.Value = DateTime.Today;
            btn_xuatbaocao.Enabled = false;
        }

        private void InitializeGridViewManagers()
        {
            // 1. Employee GridView
            _employeeManager = new GridViewManager<EmployeeData>(
                dataGridView_emp,
                LoadEmployeesFromServer,
                emp => new
                {
                    MaNV = emp.MaNguoiDung,
                    TenDangNhap = emp.TenDangNhap,
                    HoTen = emp.HoTen,
                    Email = emp.Email,
                    ViTri = GetRoleDisplay(emp.VaiTro),
                    NgayTao = emp.NgayTao.ToString("dd/MM/yyyy"),
                    TrangThai = emp.TrangThai ? "✓" : "✗"
                },
                "MaNguoiDung"
            );

            // 2. Doanh Thu GridView
            _doanhThuManager = new GridViewManager<DoanhThuTheoBan>(
                dataGridView_doanhthu,
                LoadDoanhThuFromServer,
                dt => new
                {
                    TenBan = dt.TenBan,
                    MaBanAn = dt.MaBanAn,
                    SoLuongHoaDon = dt.SoLuongHoaDon,
                    DoanhThu = dt.DoanhThu,
                    HoaDonLonNhat = dt.HoaDonLonNhat,
                    HoaDonNhoNhat = dt.HoaDonNhoNhat,
                    DoanhThuTB = dt.DoanhThuTB
                },
                "TenBan"
            );

            // 3. Bill GridView
            _billManager = new GridViewManager<BillData>(
                dataGridView_bill,
                LoadBillsAsListAsync,
                x => new
                {
                    MaHoaDon = x.MaHoaDon,
                    MaBanAn = x.MaBanAn,
                    MaNhanVien = x.MaNhanVien,
                    Ngay = x.NgayXuatHoaDon,
                    TongTien = x.TongTien,
                    TrangThai = x.TrangThai
                },
                "MaHoaDon"
            );

            // 4. Menu GridView
            _menuManager = new GridViewManager<MenuItemData>(
                dataGridView_menu,
                LoadMenuFromServer,
                food => new
                {
                    food.MaMon,
                    food.TenMon,
                    food.Gia,
                    TrangThai = food.TrangThai == "ConMon" ? "Còn món" : "Hết món"
                },
                "MaMon"
            );

            // 5. ✅ TABLE GRIDVIEW (BÀN ĂN)
            _tableManager = new GridViewManager<Models.Database.Database.BanAn>(
                dataGridView3,
                LoadTablesFromServer,
                table => new
                {
                    ID = table.MaBan,
                    TenBan = table.TenBan,
                    // Hiển thị tiếng Việt đẹp trên lưới
                    TrangThai = ConvertStatusToVietnamese(table.TrangThai)
                },
                "MaBan"
            );

            // --- GẮN EVENT HANDLERS ---

            // Employee Events
            dataGridView_emp.SelectionChanged += (s, e) =>
            {
                var selected = _employeeManager.GetSelectedItem();
                if (selected != null) ShowEmployeeDetails(selected);
            };
            dataGridView_emp.CellFormatting += DataGridView_Employee_CellFormatting;

            // Doanh Thu Events
            dataGridView_doanhthu.SelectionChanged += (s, e) =>
            {
                var selected = _doanhThuManager.GetSelectedItem();
                if (selected != null) ShowDoanhThuDetails(selected);
            };

            // Bill Events
            dataGridView_bill.SelectionChanged += (s, e) =>
            {
                var selected = _billManager.GetSelectedItem();
                if (selected != null) ShowBillDetails(selected);
            };

            // Menu Events
            dataGridView_menu.SelectionChanged += (s, e) =>
            {
                var item = _menuManager.GetSelectedItem();
                if (item != null)
                {
                    tb_nameFood.Text = item.TenMon;
                    nm_priceFood.Value = item.Gia;
                    cb_statusFood.SelectedItem = item.TrangThai == "ConMon" ? "Còn món" : "Hết món";
                }
            };
            dataGridView_menu.CellFormatting += DataGridView_Menu_CellFormatting;
            //==========================================================================
            // --- CẤU HÌNH BẢNG BÀN ĂN (dataGridView3) ---

            // ✅ Table Events (Sự kiện chọn dòng Bàn ăn)
            dataGridView3.SelectionChanged += (s, e) =>
            {
                var selected = _tableManager.GetSelectedItem();
                if (selected != null)
                {
                    textBox1.Text = selected.MaBan.ToString();
                    tb_nameTable.Text = selected.TenBan;

                    // Map ngược từ SQL về ComboBox
                    string dbStatus = selected.TrangThai; // "DangSuDung", "Trong"...
                    if (dbStatus == "DangSuDung") comboBox1.SelectedItem = "Có người";
                    else if (dbStatus == "DaDat") comboBox1.SelectedItem = "Đã đặt";
                    else comboBox1.SelectedItem = "Trống"; // Default "Trong"
                }
            };

            // ✅ Format màu Bàn ăn (Sửa logic check màu)
            dataGridView3.CellFormatting += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    var row = dataGridView3.Rows[e.RowIndex];
                    // Lấy giá trị hiển thị (đã convert sang tiếng Việt ở trên)
                    var status = row.Cells["TrangThai"].Value?.ToString();

                    if (status == "Có người") row.DefaultCellStyle.BackColor = Color.LightSalmon;
                    else if (status == "Đã đặt") row.DefaultCellStyle.BackColor = Color.LightYellow;
                    else row.DefaultCellStyle.BackColor = Color.LightGreen; // Trống
                }
            };
        }

        // ✅ Hàm hỗ trợ chuyển đổi hiển thị trên lưới
        private string ConvertStatusToVietnamese(string sqlStatus)
        {
            if (sqlStatus == "DangSuDung") return "Có người";
            if (sqlStatus == "DaDat") return "Đã đặt";
            return "Trống"; // "Trong"
        }

        private void InitializeControls()
        {
            cb_position.Items.AddRange(new[] { "Admin", "PhucVu", "Bep" });
            cb_statusFood.Items.AddRange(new[] { "Còn món", "Hết món" });

            // ✅ Init combobox bàn ăn (Giao diện tiếng Việt)
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new[] { "Trống", "Có người", "Đã đặt" });
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;

            tb_password.PasswordChar = '●';
            SetupSearchBox(tb_searchHuman, SEARCH_PLACEHOLDER);
        }

        private void SetupSearchBox(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private async void LoadAllData()
        {
            await _employeeManager.LoadDataAsync();
            await _billManager.LoadDataAsync();
            await _menuManager.LoadDataAsync();
            await _tableManager.LoadDataAsync(); // ✅ Load Bàn
        }

        // ==================== DATA LOADING ====================

        private Task<List<EmployeeData>> LoadEmployeesFromServer()
        {
            return LoadEmployeesFromServer("", "");
        }

        private async Task<List<EmployeeData>> LoadEmployeesFromServer(string keyword, string role)
        {
            var request = new GetEmployeesRequest { Keyword = keyword, VaiTro = role };
            var response = await SendRequest<GetEmployeesRequest, GetEmployeesResponse>(request);
            if (response?.Success == true) return response.Employees;
            ShowError("Không thể tải dữ liệu nhân viên");
            return new List<EmployeeData>();
        }

        private async Task<List<DoanhThuTheoBan>> LoadDoanhThuFromServer()
        {
            DateTime tuNgay = dtp_tuNgay.Value.Date;
            DateTime denNgay = dtp_denNgay.Value.Date.AddDays(1).AddSeconds(-1);
            return await LoadDoanhThuFromServerWithDates(tuNgay, denNgay);
        }

        private async Task<List<DoanhThuTheoBan>> LoadDoanhThuFromServerWithDates(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                var request = new ThongKeDoanhThuRequest { TuNgay = tuNgay, DenNgay = denNgay };
                var response = await SendRequest<ThongKeDoanhThuRequest, ThongKeDoanhThuResponse>(request);

                if (response?.Success == true)
                {
                    UpdateTongDoanhThu(response.TongDoanhThu.tongDoanhThu);
                    return response.DoanhThuTheoBan;
                }
                else
                {
                    ShowError(response?.Message ?? "Không thể tải dữ liệu doanh thu");
                    return new List<DoanhThuTheoBan>();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Lỗi kết nối: {ex.Message}");
                return new List<DoanhThuTheoBan>();
            }
        }

        // ✅ Load Bàn từ Server
        private Task<List<Models.Database.Database.BanAn>> LoadTablesFromServer()
        {
            return LoadTablesFromServer("");
        }

        private async Task<List<Models.Database.Database.BanAn>> LoadTablesFromServer(string keyword)
        {
            try
            {
                // Gửi JSON thủ công
                JObject jsonRequest = new JObject();
                jsonRequest["Type"] = "GetTables";

                string resString = await SendRequestRaw(jsonRequest.ToString(Formatting.None));

                var json = JObject.Parse(resString);
                if (json["Success"] != null && (bool)json["Success"])
                {
                    var list = json["ListBan"].ToObject<List<Models.Database.Database.BanAn>>();
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        return list.Where(t => t.TenBan.Contains(keyword) || t.MaBan.ToString().Contains(keyword)).ToList();
                    }
                    return list;
                }
            }
            catch { }
            return new List<Models.Database.Database.BanAn>();
        }

        private void UpdateTongDoanhThu(decimal tongDoanhThu)
        {
            if (tongDoanhThu > 0)
            {
                lbl_sumdoanhthu.Text = tongDoanhThu.ToString("N0") + " VNĐ";
                lbl_sumdoanhthu.ForeColor = Color.Red;
            }
            else
            {
                lbl_sumdoanhthu.Text = "---";
                lbl_sumdoanhthu.ForeColor = Color.Gray;
            }
        }

        private void UpdateDoanhThuUI()
        {
            var cachedData = _doanhThuManager.GetCachedData();
            if (cachedData != null && cachedData.Count > 0)
            {
                var tongDoanhThu = cachedData.Sum(x => x.DoanhThu);
                lbl_sumdoanhthu.Text = tongDoanhThu.ToString("N0") + " VNĐ";
                lbl_sumdoanhthu.ForeColor = Color.Red;
                btn_xuatbaocao.Enabled = true;
                FormatDoanhThuGridView();
                ShowSuccess($"Đã tải {_doanhThuManager.GetRowCount()} bàn");
            }
            else
            {
                btn_xuatbaocao.Enabled = false;
                lbl_sumdoanhthu.Text = "---";
                lbl_sumdoanhthu.ForeColor = Color.Gray;
            }
        }

        private void FormatDoanhThuGridView()
        {
            foreach (DataGridViewRow row in dataGridView_doanhthu.Rows)
            {
                if (row.Cells["DoanhThu"].Value != null)
                {
                    var doanhThu = Convert.ToDecimal(row.Cells["DoanhThu"].Value);
                    if (doanhThu > 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.White;
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                    }
                }
            }
        }

        private async Task<List<BillData>> LoadBillsAsListAsync()
        {
            var request = new GetBillRequest { };
            var response = await SendRequest<GetBillRequest, GetBillResponse>(request);
            return response.Success ? response.Bills : new List<BillData>();
        }

        private Task<List<MenuItemData>> LoadMenuFromServer() => LoadMenuFromServer("");

        private async Task<List<MenuItemData>> LoadMenuFromServer(string keyword = "")
        {
            var req = new SearchMenuRequest { Keyword = keyword };
            var res = await SendRequest<SearchMenuRequest, GetMenuResponse>(req);
            return res?.Success == true ? res.Items : new List<MenuItemData>();

        }

        // --- HELPER BÀN ĂN ---
        private string MapStatusToSQL(string statusViet)
        {
            if (statusViet == "Có người") return "DangSuDung";
            if (statusViet == "Đã đặt") return "DaDat";
            return "Trong";
        }

        private string ConvertStatusToVietnamese(string sqlStatus)
        {
            if (sqlStatus == "DangSuDung") return "Có người";
            if (sqlStatus == "DaDat") return "Đã đặt";
            return "Trống";
        }

        // =======================================================================
        // 🔥 XÓA HẾT CÁC HÀM LoadTablesFromServer CŨ VÀ DÁN ĐÈ ĐOẠN NÀY VÀO 🔥
        // =======================================================================

        // HÀM 1: Không tham số (Dùng cho nút Xem, Thêm, Xóa, Sửa gọi lại)
        private Task<List<Models.Database.Database.BanAn>> LoadTablesFromServer() => LoadTablesFromServer("");

        private async Task<List<Models.Database.Database.BanAn>> LoadTablesFromServer(string keyword)
        {
            try
            {
                // Gói yêu cầu vào object ẩn danh để có { Type, Data }
                var wrapper = new { Type = "GetTables", Data = new object() };

                // Dùng SendRequest chuẩn, nhận về JObject
                var json = await SendRequest<object, JObject>(wrapper);

                if (json != null && (bool)json["Success"])
                {
                    var list = json["ListBan"].ToObject<List<Models.Database.Database.BanAn>>();

                    // Dịch dữ liệu trước khi hiển thị
                    foreach (var item in list) item.TrangThai = ConvertStatusToVietnamese(item.TrangThai);

                    // Tìm kiếm Client-side
                    if (!string.IsNullOrEmpty(keyword))
                        return list.Where(t => t.TenBan.ToLower().Contains(keyword.ToLower()) || t.MaBan.ToString().Contains(keyword)).ToList();

                    return list;
                }
            }
            catch { }
            return new List<Models.Database.Database.BanAn>();
        }

        // ==================== DISPLAY METHODS ====================

        private void ShowEmployeeDetails(EmployeeData employee)
        {
            tb_nameHuman.Text = employee.HoTen;
            tb_emailHuman.Text = employee.Email;
            dateTimePicker3.Value = employee.NgayTao;
            cb_position.SelectedItem = cb_position.Items.Cast<string>()
                .FirstOrDefault(item => item.Equals(employee.VaiTro, StringComparison.OrdinalIgnoreCase));
            tb_password.Text = "******";
            tb_password.Enabled = false;
            tb_password.BackColor = Color.LightGray;
        }

        private void ShowDoanhThuDetails(DoanhThuTheoBan dt)
        {
            if (dt == null) return;
            MessageBox.Show($"Bàn: {dt.TenBan}\nDoanh thu: {dt.DoanhThu:N0} VNĐ", "Chi Tiết", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DataGridView_Bill_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView_bill.Rows[e.RowIndex];
                if (row.Cells["TrangThai"].Value != null)
                {
                    string status = row.Cells["TrangThai"].Value.ToString();
                    if (status == "DaThanhToan")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.SelectionBackColor = Color.Green;
                    }
                    else if (status == "ChuaThanhToan" || status == "Huy")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                        row.DefaultCellStyle.SelectionBackColor = Color.Red;
                    }
                }
            }
        }

        private void DataGridView_Employee_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dataGridView_emp.Rows[e.RowIndex];
                if (row.Cells["TrangThai"].Value != null)
                {
                    string status = row.Cells["TrangThai"].Value.ToString();
                    if (status == "✓")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.SelectionBackColor = Color.Green;
                    }
                    else if (status == "✗")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                        row.DefaultCellStyle.SelectionBackColor = Color.Red;
                    }
                }
            }
        }

        private void DataGridView_Menu_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView_menu.Columns["TrangThai"] != null)
            {
                var row = dataGridView_menu.Rows[e.RowIndex];
                var status = row.Cells["TrangThai"].Value?.ToString();
                row.DefaultCellStyle.BackColor = status == "Hết món" ? Color.LightSalmon : Color.LightGreen;
            }
        }

        private void ShowBillDetails(BillData bill)
        {
            if (bill == null) return;
            try
            {
                tb_idBill.Text = bill.MaHoaDon.ToString();
                tb_idHuman.Text = bill.MaNhanVien.ToString();
                tb_idTable.Text = bill.MaBanAn.ToString();
                tb_dateBill.Text = bill.NgayXuatHoaDon.ToString("HH:mm dd/MM/yyyy");
                if (this.Controls.Find("tb_tongTien", true).FirstOrDefault() is TextBox tbTongTien)
                {
                    tbTongTien.Text = bill.TongTien.ToString("N0") + " VNĐ";
                }
            }
            catch { }
        }

        private void ClearForm()
        {
            tb_nameHuman.Clear();
            tb_emailHuman.Clear();
            tb_password.Clear();
            tb_password.Enabled = true;
            tb_password.BackColor = Color.White;
            cb_position.SelectedIndex = -1;
            dateTimePicker3.Value = DateTime.Now;
            _employeeManager.ClearSelection();
        }

        private string GetRoleDisplay(string role)
        {
            var mapping = new Dictionary<string, string> { { "Admin", "Quản trị" }, { "PhucVu", "Phục vụ" }, { "Bep", "Bếp" } };
            return mapping.ContainsKey(role) ? mapping[role] : role;
        }

        private void XuatExcelTrucTiep()
        {
            var cachedData = _doanhThuManager.GetCachedData();
            if (cachedData == null || cachedData.Count == 0)
            {
                ShowWarning("Không có dữ liệu để xuất báo cáo!");
                return;
            }
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveDialog.FileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("DoanhThu");
                            worksheet.Cells["A1"].Value = "BÁO CÁO DOANH THU";
                            int row = 2;
                            foreach (var item in cachedData)
                            {
                                worksheet.Cells[row, 1].Value = item.TenBan;
                                worksheet.Cells[row, 2].Value = item.DoanhThu;
                                row++;
                            }
                            package.SaveAs(new FileInfo(saveDialog.FileName));
                        }
                        ShowSuccess("Xuất thành công!");
                    }
                    catch (Exception ex) { ShowError($"Lỗi: {ex.Message}"); }
                }
            }
        }

        private void ClearBillTextBoxes()
        {
            tb_idBill.Text = ""; tb_idHuman.Text = ""; tb_idTable.Text = ""; tb_dateBill.Text = ""; tb_searchBill.Text = "";
        }

        private void ClearFoodForm()
        {
            tb_nameFood.Clear(); nm_priceFood.Value = 0; tb_searchFood.Clear(); cb_statusFood.SelectedIndex = 0; _menuManager.ClearSelection();
        }

        // ==================== CRUD OPERATIONS (EMPLOYEE) ====================

        private async void btn_addHuman_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(true, out string err)) { ShowWarning(err); return; }
            if (!Confirm($"Thêm nhân viên: {tb_nameHuman.Text}?")) return;

            await ExecuteAsync(btn_addHuman, "Đang thêm...", async () =>
            {
                var req = new AddEmployeeRequest
                {
                    TenDangNhap = tb_emailHuman.Text.Split('@')[0],
                    MatKhau = tb_password.Text,
                    HoTen = tb_nameHuman.Text,
                    Email = tb_emailHuman.Text,
                    VaiTro = cb_position.SelectedItem.ToString(),
                    NgayVaoLam = dateTimePicker3.Value
                };
                var res = await SendRequest<AddEmployeeRequest, AddEmployeeResponse>(req);
                if (res?.Success == true) { ShowSuccess("Thêm thành công"); ClearForm(); await _employeeManager.RefreshAsync(); }
                else ShowError(res?.Message);
            });
        }

        private async void btn_editHuman_Click(object sender, EventArgs e)
        {
            var sel = _employeeManager.GetSelectedItem();
            if (sel == null) { ShowWarning("Chọn nhân viên"); return; }
            if (!ValidateInput(false, out string err)) { ShowWarning(err); return; }

            await ExecuteAsync(btn_editHuman, "Cập nhật...", async () => {
                var req = new UpdateEmployeeRequest { MaNguoiDung = sel.MaNguoiDung, HoTen = tb_nameHuman.Text, Email = tb_emailHuman.Text, VaiTro = cb_position.SelectedItem.ToString(), TrangThai = true };
                var res = await SendRequest<UpdateEmployeeRequest, UpdateEmployeeResponse>(req);
                if (res?.Success == true) { ShowSuccess("Cập nhật xong"); await _employeeManager.RefreshAsync(); }
            });
        }

        private async void btn_deleteHuman_Click(object sender, EventArgs e)
        {
            var sel = _employeeManager.GetSelectedItem();
            if (sel == null || !Confirm("Xóa nhân viên này?")) return;
            await ExecuteAsync(btn_deleteHuman, "Xóa...", async () => {
                var res = await SendRequest<DeleteEmployeeRequest, DeleteEmployeeResponse>(new DeleteEmployeeRequest { MaNguoiDung = sel.MaNguoiDung });
                if (res?.Success == true) { ShowSuccess("Đã xóa"); await _employeeManager.RefreshAsync(); }
            });
        }

        private async void btn_viewHuman_Click(object sender, EventArgs e) => await _employeeManager.LoadDataAsync();
        private async void btn_searchHuman_Click(object sender, EventArgs e) => await _employeeManager.LoadDataAsync(() => LoadEmployeesFromServer(tb_searchHuman.Text == SEARCH_PLACEHOLDER ? "" : tb_searchHuman.Text, cb_position.SelectedItem?.ToString() ?? ""));

        // 2. MÓN ĂN
        private async void btn_addFood_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_nameFood.Text)) { ShowWarning("Nhập tên món"); return; }
            var req = new AddMenuRequest { TenMon = tb_nameFood.Text, Gia = nm_priceFood.Value, TrangThai = cb_statusFood.SelectedItem?.ToString() == "Còn món" ? "ConMon" : "HetMon" };
            var res = await SendRequest<AddMenuRequest, AddMenuResponse>(req);
            if (res?.Success == true) { ShowSuccess("Thêm món thành công"); ClearFoodForm(); await _menuManager.RefreshAsync(); }
        }

        private async void btn_editFood_Click(object sender, EventArgs e)
        {
            var sel = _menuManager.GetSelectedItem();
            if (sel == null) { ShowWarning("Chọn món"); return; }
            if (!Confirm("Cập nhật món này?")) return;

            var req = new UpdateMenuRequest { MaMon = sel.MaMon, TenMon = tb_nameFood.Text, Gia = nm_priceFood.Value, TrangThai = cb_statusFood.SelectedItem?.ToString() == "Còn món" ? "ConMon" : "HetMon" };
            var res = await SendRequest<UpdateMenuRequest, UpdateMenuResponse>(req);
            if (res?.Success == true) { ShowSuccess("Đã cập nhật"); ClearFoodForm(); await _menuManager.RefreshAsync(); }
        }

        private async void btn_deleteFood_Click(object sender, EventArgs e)
        {
            var sel = _menuManager.GetSelectedItem();
            if (sel == null || !Confirm("Ẩn món này?")) return;
            var req = new UpdateMenuStatusRequest { MaMon = sel.MaMon, TrangThai = "HetMon" };
            var res = await SendRequest<UpdateMenuStatusRequest, UpdateMenuResponse>(req);
            if (res?.Success == true) { ShowSuccess("Đã ẩn món"); ClearFoodForm(); await _menuManager.RefreshAsync(); }
        }

        private async void btn_viewFood_Click(object sender, EventArgs e) => await _menuManager.RefreshAsync();
        private async void btn_searchFood_Click(object sender, EventArgs e) => await _menuManager.LoadDataAsync(() => LoadMenuFromServer(tb_searchFood.Text));

        // ✅ 3. TABLE (BÀN ĂN) - ĐÃ MAP DỮ LIỆU CHO SQL
        private async void btn_addTable_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(tb_nameTable.Text))
            {
                ShowWarning("Vui lòng nhập ID và Tên bàn");
                return;
            }

            try
            {
                // 🟢 Mapping trạng thái
                string statusHienThi = comboBox1.SelectedItem?.ToString() ?? "Trống";
                string statusGuiDi = "Trong";

                if (statusHienThi == "Có người") statusGuiDi = "DangSuDung";
                else if (statusHienThi == "Đã đặt") statusGuiDi = "DaDat";

                var req = new AddTableRequest
                {
                    MaBan = int.Parse(textBox1.Text),
                    TenBan = tb_nameTable.Text,
                    TrangThai = statusGuiDi
                };

                await ExecuteAsync(btn_addTable, "Đang thêm...", async () =>
                {
                    string res = await SendRequestRaw(JsonConvert.SerializeObject(new { Type = "AddTable", Data = req }));
                    var resp = JsonConvert.DeserializeObject<TableActionResponse>(res);
                    if (resp.Success) { ShowSuccess(resp.Message); await _tableManager.RefreshAsync(); }
                    else ShowError(resp.Message);
                });
            }
            catch (FormatException) { ShowError("ID phải là số"); }
        }

        private async void btn_editTable_Click(object sender, EventArgs e)
        {
            var selected = _tableManager.GetSelectedItem();
            if (selected == null) { ShowWarning("Chọn bàn cần sửa"); return; }

            try
            {
                // 🟢 Mapping trạng thái
                string statusHienThi = comboBox1.SelectedItem?.ToString() ?? "Trống";
                string statusGuiDi = "Trong";

                if (statusHienThi == "Có người") statusGuiDi = "DangSuDung";
                else if (statusHienThi == "Đã đặt") statusGuiDi = "DaDat";

                var req = new UpdateTableRequest
                {
                    MaBan = int.Parse(textBox1.Text),
                    TenBan = tb_nameTable.Text,
                    TrangThai = statusGuiDi
                };

                await ExecuteAsync((Button)sender, "Đang sửa...", async () =>
                {
                    string res = await SendRequestRaw(JsonConvert.SerializeObject(new { Type = "UpdateTable", Data = req }));
                    var resp = JsonConvert.DeserializeObject<TableActionResponse>(res);
                    if (resp.Success) { ShowSuccess(resp.Message); await _tableManager.RefreshAsync(); }
                    else ShowError(resp.Message);
                });
            }
            catch { ShowError("ID phải là số"); }
        }

        private async void btn_deleteTable_Click(object sender, EventArgs e)
        {
            var selected = _tableManager.GetSelectedItem();
            if (selected == null) { ShowWarning("Chọn bàn cần xóa"); return; }
            if (!Confirm("Xóa bàn này?")) return;

            await ExecuteAsync((Button)sender, "Đang xóa...", async () =>
            {
                var req = new DeleteTableRequest { MaBan = selected.MaBan };
                string res = await SendRequestRaw(JsonConvert.SerializeObject(new { Type = "DeleteTable", Data = req }));
                var resp = JsonConvert.DeserializeObject<TableActionResponse>(res);
                if (resp.Success) { ShowSuccess(resp.Message); await _tableManager.RefreshAsync(); }
                else ShowError(resp.Message);
            });
        }

        private async void btn_viewTable_Click(object sender, EventArgs e) => await _tableManager.RefreshAsync();

        private async void btn_searchTable_Click(object sender, EventArgs e)
        {
            string kw = "";
            if (this.Controls.Find("tb_searchTable", true).FirstOrDefault() is TextBox tb) kw = tb.Text;
            await _tableManager.LoadDataAsync(() => LoadTablesFromServer(kw));
        }

        // ==================== VALIDATION & HELPER ====================

        private bool ValidateInput(bool isAddMode, out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(tb_nameHuman.Text)) { error = "Nhập tên NV"; return false; }
            if (string.IsNullOrWhiteSpace(tb_emailHuman.Text)) { error = "Nhập Email"; return false; }
            if (isAddMode && string.IsNullOrWhiteSpace(tb_password.Text)) { error = "Nhập pass"; return false; }
            return true;
        }

        private bool IsValidEmail(string email)
        {
            try { var addr = new System.Net.Mail.MailAddress(email); return addr.Address == email.Trim(); } catch { return false; }
        }

        // ✅ HÀM GỬI REQUEST THƯỜNG
        private async Task<TResponse> SendRequest<TRequest, TResponse>(TRequest request)
        {
            string json = JsonConvert.SerializeObject(request);
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(SERVER_IP, SERVER_PORT);
                using (var stream = client.GetStream())
                using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(json);
                    string res = await reader.ReadLineAsync();
                    return JsonConvert.DeserializeObject<TResponse>(res);
                }
            }
        }

        // ✅ HÀM GỬI REQUEST RAW (Cho Bàn Ăn)
        private async Task<string> SendRequestRaw(string jsonRequest)
        {
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(SERVER_IP, SERVER_PORT);
                using (var stream = client.GetStream())
                using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    await writer.WriteLineAsync(jsonRequest);
                    return await reader.ReadLineAsync();
                }
            }
        }

        private async Task ExecuteAsync(Button button, string loadingText, Func<Task> action)
        {
            string originalText = button.Text;
            button.Enabled = false;
            button.Text = loadingText;
            Cursor = Cursors.WaitCursor;
            try { await action(); }
            catch (Exception ex) { ShowError(ex.Message); }
            finally { button.Enabled = true; button.Text = originalText; Cursor = Cursors.Default; }
        }

        private bool Confirm(string msg) => MessageBox.Show(msg, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        private void ShowSuccess(string msg) => MessageBox.Show(msg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void ShowError(string msg) => MessageBox.Show(msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void ShowWarning(string msg) => MessageBox.Show(msg, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        // Các event handler khác
        private void btn_XemDoanhThu_Click(object sender, EventArgs e) => btn_XemDoanhThu_Click(sender, e);
        private async void btn_viewBill_Click(object sender, EventArgs e) => await _billManager.LoadDataAsync();
        private void btn_resetBill_Click(object sender, EventArgs e) => ClearBillTextBoxes();
        private void btn_searchBill_Click(object sender, EventArgs e) => btn_searchBill_Click(sender, e);
        private void btn_xuatbaocao_Click(object sender, EventArgs e) => XuatExcelTrucTiep();
    }
}