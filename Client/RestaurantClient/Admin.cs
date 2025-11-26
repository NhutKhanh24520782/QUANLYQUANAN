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
            // Setup events cho tab hóa đơn
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
            // Employee GridView
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
                "MaNguoiDung" // Tên property ID
            );
            _doanhThuManager = new GridViewManager<DoanhThuTheoBan>(
                dataGridView_doanhthu, // DataGridView trong panel1
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
            _billManager = new GridViewManager<BillData>(
        dataGridView_bill,
        LoadBillsAsListAsync,
        x => new
        {
            MaHoaDon = x.MaHoaDon,
            MaBanAn = x.MaBanAn,
            MaNhanVien = x.MaNhanVien,
            Ngay = x.NgayXuatHoaDon,
            TongTien = x.TongTien, // ✅ THÊM NẾU CÓ
            TrangThai = x.TrangThai // ✅ THÊM NẾU CÓ
        },
        "MaHoaDon"
    );
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
            // Gắn event handler
            dataGridView_emp.SelectionChanged += (s, e) =>
            {
                var selected = _employeeManager.GetSelectedItem();
                if (selected != null)
                    ShowEmployeeDetails(selected);
            };

            dataGridView_emp.CellFormatting += DataGridView_Employee_CellFormatting;

            dataGridView_doanhthu.SelectionChanged += (s, e) =>
            {
                var selected = _doanhThuManager.GetSelectedItem();
                if (selected != null)
                    ShowDoanhThuDetails(selected);
            };

            dataGridView_bill.SelectionChanged += (s, e) =>
            {
                var selected = _billManager.GetSelectedItem();
                if (selected != null)
                    ShowBillDetails(selected);
            };
            dataGridView_menu.SelectionChanged += (s, e) =>
            {
                var item = _menuManager.GetSelectedItem();
                if (item != null)
                {
                    tb_nameFood.Text = item.TenMon;
                    nm_priceFood.Value = item.Gia;

                    // ✅ SET TRẠNG THÁI CHO COMBOBOX
                    cb_statusFood.SelectedItem = item.TrangThai == "ConMon" ? "Còn món" : "Hết món";
                }
            };
            dataGridView_menu.CellFormatting += DataGridView_Menu_CellFormatting;
            //==========================================================================
            // --- CẤU HÌNH BẢNG BÀN ĂN (dataGridView3) ---

            _tableManager = new GridViewManager<Models.Database.Database.BanAn>(
                dataGridView3, // Tên bảng trong Designer của bạn
                LoadTablesFromServer,
                table => new
                {
                    ID = table.MaBan,
                    TenBan = table.TenBan,
                    // Dịch trạng thái SQL (DangSuDung) -> Tiếng Việt (Có người) để hiện lên bảng
                    TrangThai = ConvertStatusToVietnamese(table.TrangThai)
                },
                "MaBan"
            );

            // 1. Sự kiện chọn dòng -> Đổ dữ liệu ngược lại ô nhập
            dataGridView3.SelectionChanged += (s, e) =>
            {
                var selected = _tableManager.GetSelectedItem();
                if (selected != null)
                {
                    textBox1.Text = selected.MaBan.ToString();
                    tb_nameTable.Text = selected.TenBan;

                    // 🛑 QUAN TRỌNG: Dịch từ SQL -> Tiếng Việt trước khi gán vào ComboBox
                    string statusViet = ConvertStatusToVietnamese(selected.TrangThai);

                    if (comboBox1.Items.Contains(statusViet))
                        comboBox1.SelectedItem = statusViet;
                }
            };

            // 2. Sự kiện tô màu (Trang trí)
            dataGridView3.CellFormatting += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    var row = dataGridView3.Rows[e.RowIndex];
                    var status = row.Cells["TrangThai"].Value?.ToString();

                    if (status == "Có người") row.DefaultCellStyle.BackColor = Color.LightSalmon;
                    else if (status == "Đã đặt") row.DefaultCellStyle.BackColor = Color.LightYellow;
                    else row.DefaultCellStyle.BackColor = Color.LightGreen;
                }
            };
        }

        private void InitializeControls()
        {
            cb_position.Items.AddRange(new[] { "Admin", "PhucVu", "Bep" });
            cb_statusFood.Items.AddRange(new[] { "Còn món", "Hết món" }); // ✅ THÊM
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
            //await _doanhThuManager.LoadDataAsync();
            await _billManager.LoadDataAsync(); // ✅ THÊM: Tự động load bills
            await _menuManager.LoadDataAsync();
            await _tableManager.LoadDataAsync();
            
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

            if (response?.Success == true)
                return response.Employees;

            ShowError("Không thể tải dữ liệu nhân viên");
            return new List<EmployeeData>();
        }
        private async Task<List<DoanhThuTheoBan>> LoadDoanhThuFromServer()
        {
            DateTime tuNgay = dtp_tuNgay.Value.Date;
            DateTime denNgay = dtp_denNgay.Value.Date.AddDays(1).AddSeconds(-1);

            return await LoadDoanhThuFromServerWithDates(tuNgay, denNgay);
        }

        // Phương thức phụ trợ có tham số ngày
        private async Task<List<DoanhThuTheoBan>> LoadDoanhThuFromServerWithDates(DateTime tuNgay, DateTime denNgay)
        {
            try
            {
                var request = new ThongKeDoanhThuRequest
                {
                    TuNgay = tuNgay,
                    DenNgay = denNgay
                };

                var response = await SendRequest<ThongKeDoanhThuRequest, ThongKeDoanhThuResponse>(request);

                if (response?.Success == true)
                {
                    // Cập nhật tổng doanh thu lên Label
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
                var dataCount = _doanhThuManager.GetRowCount();
                var tongHoaDon = cachedData.Sum(x => x.SoLuongHoaDon);
                var soBanCoDoanhThu = cachedData.Count(x => x.DoanhThu > 0);
                var tongDoanhThu = cachedData.Sum(x => x.DoanhThu);
                // Cập nhật label tổng doanh thu trong panel1
                lbl_sumdoanhthu.Text = tongDoanhThu.ToString("N0") + " VNĐ";
                lbl_sumdoanhthu.ForeColor = Color.Red;
                // Enable nút xuất báo cáo trong panel2
                btn_xuatbaocao.Enabled = true;
                FormatDoanhThuGridView();
                ShowSuccess($"Đã tải {dataCount} bàn | {soBanCoDoanhThu} bàn có doanh thu | {tongHoaDon} hóa đơn");
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

            if (response.Success)
            {
                return response.Bills;
            }
            else
            {
                // Có thể log lỗi hoặc thông báo nhẹ nếu cần
                return new List<BillData>();
            }
        }

        private Task<List<MenuItemData>> LoadMenuFromServer()
            => LoadMenuFromServer("");

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

            // Set combobox
            cb_position.SelectedItem = cb_position.Items.Cast<string>()
                .FirstOrDefault(item => item.Equals(employee.VaiTro, StringComparison.OrdinalIgnoreCase));

            // Disable password khi edit
            tb_password.Text = "******";
            tb_password.Enabled = false;
            tb_password.BackColor = Color.LightGray;
        }
        private void ShowDoanhThuDetails(DoanhThuTheoBan doanhThu)
        {
            if (doanhThu == null) return;

            string message = $"📊 CHI TIẾT DOANH THU BÀN\n\n" +
                            $"🔸 Tên bàn: {doanhThu.TenBan}\n" +
                            $"🔸 Số hóa đơn: {doanhThu.SoLuongHoaDon}\n" +
                            $"🔸 Doanh thu: {doanhThu.DoanhThu:N0} VNĐ\n" +
                            $"🔸 Hóa đơn lớn nhất: {doanhThu.HoaDonLonNhat:N0} VNĐ\n" +
                            $"🔸 Hóa đơn nhỏ nhất: {doanhThu.HoaDonNhoNhat:N0} VNĐ\n" +
                            $"🔸 Doanh thu trung bình: {doanhThu.DoanhThuTB:N0} VNĐ";

            MessageBox.Show(message, "Chi Tiết Doanh Thu",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DataGridView_Bill_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_bill.Rows[e.RowIndex];
                if (row.Cells["TrangThai"].Value != null)
                {
                    string status = row.Cells["TrangThai"].Value.ToString();
                    if (status == "DaThanhToan")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.SelectionBackColor = Color.Green; // Màu khi được chọn
                    }
                    else if (status == "ChuaThanhToan" || status == "Huy")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                        row.DefaultCellStyle.SelectionBackColor = Color.Red; // Màu khi được chọn
                    }
                }
            }
        }

        private void DataGridView_Employee_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {

                DataGridViewRow row = dataGridView_emp.Rows[e.RowIndex];
                if (row.Cells["TrangThai"].Value != null)
                {
                    string status = row.Cells["TrangThai"].Value.ToString();
                    if (status == "✓")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen;
                        row.DefaultCellStyle.SelectionBackColor = Color.Green; // Màu khi được chọn
                    }
                    else if (status == "✗")
                    {
                        row.DefaultCellStyle.BackColor = Color.LightSalmon;
                        row.DefaultCellStyle.SelectionBackColor = Color.Red; // Màu khi được chọn
                    }
                }
            }
        }
        private void DataGridView_Menu_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView_menu.Columns["TrangThai"] != null)
            {
                DataGridViewRow row = dataGridView_menu.Rows[e.RowIndex];
                if (row.Cells["TrangThai"].Value?.ToString() == "Hết món")
                {
                    row.DefaultCellStyle.BackColor = Color.LightSalmon;
                    row.DefaultCellStyle.SelectionBackColor = Color.Red; // Màu khi được chọn
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.SelectionBackColor = Color.Green; // Màu khi được chọn
                }
            }
        }
        private void ShowBillDetails(BillData bill)
        {
            if (bill == null) return;

            try
            {
                Console.WriteLine($"🔍 Đang hiển thị bill: {bill.MaHoaDon}");

                // ✅ HIỂN THỊ LÊN TEXTBOX
                tb_idBill.Text = bill.MaHoaDon.ToString();
                tb_idHuman.Text = bill.MaNhanVien.ToString();
                tb_idTable.Text = bill.MaBanAn.ToString();
                tb_dateBill.Text = bill.NgayXuatHoaDon.ToString("HH:mm dd/MM/yyyy");

                // ✅ THÊM: Hiển thị thông tin khác nếu có
                if (this.Controls.Find("tb_tongTien", true).FirstOrDefault() is TextBox tbTongTien)
                {
                    tbTongTien.Text = bill.TongTien.ToString("N0") + " VNĐ";
                }

                Console.WriteLine($"✅ Đã hiển thị bill {bill.MaHoaDon} lên form");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi hiển thị bill: {ex.Message}");
            }
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
            var mapping = new Dictionary<string, string>
            {
                { "Admin", "Quản trị" },
                { "PhucVu", "Phục vụ" },
                { "Bep", "Bếp" }
            };
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
                        // 🔥 EPPLUS 7.x: KHÔNG CẦN SET LICENSE CONTEXT!
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("DoanhThu");

                            // Tiêu đề
                            worksheet.Cells["A1:F1"].Merge = true;
                            worksheet.Cells["A1"].Value = "BÁO CÁO DOANH THU";
                            worksheet.Cells["A1"].Style.Font.Bold = true;
                            worksheet.Cells["A1"].Style.Font.Size = 14;
                            worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            // Thời gian
                            worksheet.Cells["A2"].Value = $"Từ: {dtp_tuNgay.Value:dd/MM/yyyy} - Đến: {dtp_denNgay.Value:dd/MM/yyyy}";
                            worksheet.Cells["A2:F2"].Merge = true;
                            worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            // Header
                            string[] headers = { "Tên Bàn", "Số HĐ", "Doanh Thu", "HĐ Lớn Nhất", "HĐ Nhỏ Nhất", "Doanh Thu TB" };
                            for (int i = 0; i < headers.Length; i++)
                            {
                                var cell = worksheet.Cells[3, i + 1];
                                cell.Value = headers[i];
                                cell.Style.Font.Bold = true;
                                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            }

                            // Data
                            int row = 4;
                            foreach (var item in cachedData)
                            {
                                worksheet.Cells[row, 1].Value = item.TenBan;
                                worksheet.Cells[row, 2].Value = item.SoLuongHoaDon;
                                worksheet.Cells[row, 3].Value = item.DoanhThu;
                                worksheet.Cells[row, 4].Value = item.HoaDonLonNhat;
                                worksheet.Cells[row, 5].Value = item.HoaDonNhoNhat;
                                worksheet.Cells[row, 6].Value = item.DoanhThuTB;

                                // Format số
                                for (int col = 3; col <= 6; col++)
                                {
                                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
                                }

                                row++;
                            }

                            // Tổng cộng
                            worksheet.Cells[row, 1].Value = "TỔNG CỘNG";
                            worksheet.Cells[row, 1].Style.Font.Bold = true;
                            worksheet.Cells[row, 3].Formula = $"SUM(C4:C{row - 1})";
                            worksheet.Cells[row, 3].Style.Numberformat.Format = "#,##0";
                            worksheet.Cells[row, 3].Style.Font.Bold = true;

                            // Auto fit
                            worksheet.Cells[1, 1, row, 6].AutoFitColumns();

                            // Lưu file
                            package.SaveAs(new FileInfo(saveDialog.FileName));
                        }

                        ShowSuccess($"Đã xuất Excel thành công!\n{saveDialog.FileName}");

                        // Hỏi mở file
                        if (MessageBox.Show("Bạn có muốn mở file Excel ngay bây giờ?",
                            "Xuất thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = saveDialog.FileName,
                                UseShellExecute = true
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Lỗi xuất Excel: {ex.Message}");
                    }
                }
            }
        }
        private void ClearBillTextBoxes()
        {
            tb_idBill.Text = "";
            tb_idHuman.Text = "";
            tb_idTable.Text = "";
            tb_dateBill.Text = "";
            tb_searchBill.Text = "";
        }
        private void ClearFoodForm()
        {
            tb_nameFood.Clear();
            nm_priceFood.Value = 0;
            tb_searchFood.Clear();
            cb_statusFood.SelectedIndex = 0; // ✅ Mặc định "Còn món"

            _menuManager.ClearSelection();
        }
        // ==================== CRUD OPERATIONS ====================

        private async void btn_addHuman_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(isAddMode: true, out string error))
            {
                ShowWarning(error);
                return;
            }

            if (!Confirm($"Thêm nhân viên: {tb_nameHuman.Text}?"))
                return;

            await ExecuteAsync(btn_addHuman, "Đang thêm...", async () =>
            {
                var request = new AddEmployeeRequest
                {
                    TenDangNhap = tb_emailHuman.Text.Split('@')[0],
                    MatKhau = tb_password.Text,
                    HoTen = tb_nameHuman.Text.Trim(),
                    Email = tb_emailHuman.Text.Trim(),
                    VaiTro = cb_position.SelectedItem.ToString(),
                    NgayVaoLam = dateTimePicker3.Value
                };

                var response = await SendRequest<AddEmployeeRequest, AddEmployeeResponse>(request);

                if (response?.Success == true)
                {
                    ShowSuccess($"Thêm thành công! Mã NV: {response.MaNguoiDung}");
                    ClearForm();
                    await _employeeManager.RefreshAsync();
                }
                else
                {
                    ShowError(response?.Message ?? "Thêm thất bại");
                }
            });
        }

        private async void btn_editHuman_Click(object sender, EventArgs e)
        {
            var selected = _employeeManager.GetSelectedItem();
            if (selected == null)
            {
                ShowWarning("Vui lòng chọn nhân viên cần sửa!");
                return;
            }

            if (!ValidateInput(isAddMode: false, out string error))
            {
                ShowWarning(error);
                return;
            }

            if (!Confirm($"Cập nhật: {tb_nameHuman.Text}?"))
                return;

            await ExecuteAsync(btn_editHuman, "Đang cập nhật...", async () =>
            {
                var request = new UpdateEmployeeRequest
                {
                    MaNguoiDung = selected.MaNguoiDung,
                    HoTen = tb_nameHuman.Text.Trim(),
                    Email = tb_emailHuman.Text.Trim(),
                    VaiTro = cb_position.SelectedItem.ToString(),
                    TrangThai = true
                };

                var response = await SendRequest<UpdateEmployeeRequest, UpdateEmployeeResponse>(request);

                if (response?.Success == true)
                {
                    ShowSuccess("Cập nhật thành công!");
                    ClearForm();
                    await _employeeManager.RefreshAsync();
                }
                else
                {
                    ShowError(response?.Message ?? "Cập nhật thất bại");
                }
            });
        }

        private async void btn_deleteHuman_Click(object sender, EventArgs e)
        {
            var selected = _employeeManager.GetSelectedItem();
            if (selected == null)
            {
                ShowWarning("Vui lòng chọn nhân viên cần xóa!");
                return;
            }

            if (!Confirm($"⚠️ XÓA nhân viên:\n{selected.HoTen}?\n\nHành động không thể hoàn tác!"))
                return;

            await ExecuteAsync(btn_deleteHuman, "Đang xóa...", async () =>
            {
                var request = new DeleteEmployeeRequest { MaNguoiDung = selected.MaNguoiDung };
                var response = await SendRequest<DeleteEmployeeRequest, DeleteEmployeeResponse>(request);

                if (response?.Success == true)
                {
                    ShowSuccess("Xóa thành công!");
                    ClearForm();
                    await _employeeManager.RefreshAsync();
                }
                else
                {
                    ShowError(response?.Message ?? "Xóa thất bại");
                }
            });
        }

        private async void btn_viewHuman_Click(object sender, EventArgs e)
        {

            await ExecuteAsync(btn_viewHuman, "Đang tải...", async () =>
            {
                await _employeeManager.LoadDataAsync();
                ClearForm();

                var cachedData = _employeeManager.GetCachedData();
                ShowSuccess($"Đã tải {cachedData?.Count ?? 0} nhân viên");
            });
        }

        private async void btn_searchHuman_Click(object sender, EventArgs e)
        {
            string keyword = tb_searchHuman.Text == SEARCH_PLACEHOLDER
                ? ""
                : tb_searchHuman.Text.Trim();
            string role = cb_position.SelectedItem?.ToString() ?? "";

            await _employeeManager.LoadDataAsync(() => LoadEmployeesFromServer(keyword, role));
        }
        private async void btn_XemDoanhThu_ClickAsync(object sender, EventArgs e)
        {
            if (dtp_tuNgay.Value > dtp_denNgay.Value)
            {
                ShowWarning("Từ ngày không được lớn hơn đến ngày!");
                return;
            }
            // Hiển thị panel1 khi có dữ liệu
            panel1.Visible = true;

            await ExecuteAsync((Button)sender, "Đang tải...", async () =>
            {
                // SỬ DỤNG REFRESHASYNC() - GIỐNG HỆT EMPLOYEE
                await _doanhThuManager.RefreshAsync();

                // Cập nhật UI sau khi load
                UpdateDoanhThuUI();
            });
        }

        private void btn_xuatbaocao_Click(object sender, EventArgs e)
        {
            XuatExcelTrucTiep();
        }

        private async void btn_viewBill_Click(object sender, EventArgs e)
        {
            await ExecuteAsync(btn_viewBill, "Đang tải mới...", async () =>
            {
                await _billManager.LoadDataAsync();
                ClearBillTextBoxes();

                var cachedData = _billManager.GetCachedData();
                ShowSuccess($"Đã tải mới {cachedData?.Count ?? 0} hóa đơn");
            });
        }

        private async void btn_resetBill_Click(object sender, EventArgs e)
        {

            await _billManager.LoadDataAsync();
            ClearBillTextBoxes();
        }
        private void btn_searchBill_Click(object sender, EventArgs e)
        {

            string keyword = tb_searchBill.Text == SEARCH_BILL ? "" : tb_searchBill.Text.Trim();
            _billManager.FilterLocal(bill =>
            {
                if (string.IsNullOrEmpty(keyword)) return true;
                bool matchMaHD = bill.MaHoaDon.ToString().Contains(keyword);
                bool matchMaNV = bill.MaNhanVien.ToString().Contains(keyword);
                return matchMaHD || matchMaNV;
            });
            if (dataGridView_bill.Rows.Count > 0)
            {
                dataGridView_bill.Rows[0].Selected = true;

                var selectedBill = _billManager.GetSelectedItem();

                if (selectedBill != null)
                {
                    tb_idBill.Text = selectedBill.MaHoaDon.ToString();
                    tb_idHuman.Text = selectedBill.MaNhanVien.ToString();
                    tb_idTable.Text = selectedBill.MaBanAn.ToString();
                    tb_dateBill.Text = selectedBill.NgayXuatHoaDon.ToString("HH:mm dd/MM/yyyy");
                }
            }
            else
            {
                ClearBillTextBoxes();
                MessageBox.Show("Không tìm thấy hóa đơn nào khớp mã HĐ hoặc mã NV này!");
            }
        }
        private async void btn_viewFood_Click(object sender, EventArgs e)
        {
            await ExecuteAsync(btn_viewFood, "Đang tải...", async () =>
            {
                await _menuManager.RefreshAsync();
                ClearFoodForm();

                var cachedData = _menuManager.GetCachedData();
                ShowSuccess($"Đã tải {cachedData?.Count ?? 0} món ăn");
            });
        }

        private async void btn_searchFood_Click(object sender, EventArgs e)
        {
            string keyword = tb_searchFood.Text.Trim();

            // ✅ SỬ DỤNG GRIDVIEWMANAGER ĐỂ FILTER
            await _menuManager.LoadDataAsync(() => LoadMenuFromServer(keyword));

            var filteredCount = _menuManager.GetRowCount();
            ShowSuccess($"Tìm thấy {filteredCount} món phù hợp");
        }
        private async void btn_addFood_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_nameFood.Text))
            {
                ShowWarning("Tên món không được trống!");
                return;
            }

            if (cb_statusFood.SelectedItem == null)
            {
                ShowWarning("Chọn trạng thái món!");
                return;
            }

            // ✅ CHUYỂN ĐỔI TRẠNG THÁI
            string trangThai = cb_statusFood.SelectedItem.ToString() == "Còn món" ? "ConMon" : "HetMon";

            var req = new AddMenuRequest
            {
                TenMon = tb_nameFood.Text.Trim(),
                Gia = nm_priceFood.Value,
                TrangThai = trangThai // ✅ THÊM TRẠNG THÁI
            };

            var res = await SendRequest<AddMenuRequest, AddMenuResponse>(req);

            if (res?.Success == true)
            {
                ShowSuccess("Thêm món thành công!");
                await _menuManager.RefreshAsync();
                ClearFoodForm(); // ✅ XÓA FORM SAU KHI THÊM
            }
            else ShowError(res?.Message);
        }
        private async void btn_editFood_Click(object sender, EventArgs e)
        {
            var selectedFood = _menuManager.GetSelectedItem();
            if (selectedFood == null)
            {
                ShowWarning("Vui lòng chọn món cần sửa!");
                return;
            }

            // VALIDATE DỮ LIỆU
            if (string.IsNullOrWhiteSpace(tb_nameFood.Text))
            {
                ShowWarning("Nhập tên món!");
                tb_nameFood.Focus();
                return;
            }

            if (nm_priceFood.Value <= 0)
            {
                ShowWarning("Giá món phải lớn hơn 0!");
                nm_priceFood.Focus();
                return;
            }

            if (cb_statusFood.SelectedItem == null)
            {
                ShowWarning("Chọn trạng thái món!");
                return;
            }

            // XÁC NHẬN CẬP NHẬT
            if (!Confirm($"Cập nhật món: {selectedFood.TenMon}?"))
                return;

            await ExecuteAsync(btn_editFood, "Đang cập nhật...", async () =>
            {
                // ✅ CHUYỂN ĐỔI TRẠNG THÁI TỪ COMBOBOX
                string trangThai = cb_statusFood.SelectedItem.ToString() == "Còn món" ? "ConMon" : "HetMon";

                var req = new UpdateMenuRequest
                {
                    MaMon = selectedFood.MaMon,
                    TenMon = tb_nameFood.Text.Trim(),
                    Gia = nm_priceFood.Value,
                    TrangThai = trangThai // ✅ ĐẢM BẢO CÓ TRƯỜNG NÀY
                };

                Console.WriteLine($"🔄 Gửi update: MaMon={req.MaMon}, TrangThai={req.TrangThai}"); // DEBUG

                var res = await SendRequest<UpdateMenuRequest, UpdateMenuResponse>(req);

                if (res?.Success == true)
                {
                    ShowSuccess($"✅ Đã cập nhật món: {req.TenMon} -> {trangThai}");
                    await _menuManager.RefreshAsync(); // ✅ QUAN TRỌNG: Refresh để thấy thay đổi
                    ClearFoodForm();
                }
                else
                {
                    ShowError(res?.Message ?? "Cập nhật thất bại");
                }
            });
        }
        private async void btn_deleteFood_Click(object sender, EventArgs e)
        {
            var selectedFood = _menuManager.GetSelectedItem();
            if (selectedFood == null)
            {
                ShowWarning("Vui lòng chọn món cần ẩn!");
                return;
            }

            // XÁC NHẬN ẨN MÓN
            if (!Confirm($"ẨN MÓN: {selectedFood.TenMon}?\n\nMón sẽ chuyển sang 'Hết món'."))
                return;

            await ExecuteAsync(btn_deleteFood, "Đang ẩn món...", async () =>
            {
                // ✅ GỬI REQUEST CẬP NHẬT STATUS THAY VÌ XÓA
                var req = new UpdateMenuStatusRequest
                {
                    MaMon = selectedFood.MaMon,
                    TrangThai = "HetMon"
                };

                var res = await SendRequest<UpdateMenuStatusRequest, UpdateMenuResponse>(req);

                if (res?.Success == true)
                {
                    ShowSuccess($"✅ Đã ẩn món: {selectedFood.TenMon}");
                    await _menuManager.RefreshAsync(); // Refresh để hiển thị status mới
                    ClearFoodForm();
                }
                else
                {
                    ShowError(res?.Message ?? "Ẩn món thất bại");
                }
            });
        }
        // ==================== VALIDATION ====================

        private bool ValidateInput(bool isAddMode, out string error)
        {
            if (string.IsNullOrWhiteSpace(tb_nameHuman.Text))
            {
                error = "Vui lòng nhập tên nhân viên";
                return false;
            }

            if (string.IsNullOrWhiteSpace(tb_emailHuman.Text))
            {
                error = "Vui lòng nhập email";
                return false;
            }

            if (!IsValidEmail(tb_emailHuman.Text))
            {
                error = "Email không hợp lệ";
                return false;
            }

            if (cb_position.SelectedIndex < 0)
            {
                error = "Vui lòng chọn vị trí";
                return false;
            }

            if (isAddMode)
            {
                if (string.IsNullOrWhiteSpace(tb_password.Text))
                {
                    error = "Vui lòng nhập mật khẩu";
                    return false;
                }

                if (tb_password.Text.Length < 6)
                {
                    error = "Mật khẩu phải có ít nhất 6 ký tự";
                    return false;
                }
            }

            error = "";
            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email.Trim() && email.Contains("@");
            }
            catch
            {
                return false;
            }
        }

        // ==================== NETWORK ====================
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
        //==================== TABLE ===============================
        // 1. Hàm chuyển đổi trạng thái để khớp với SQL của bạn
       


        // ==================== HELPER METHODS ====================
        // --- HELPER BÀN ĂN ---
       
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

        private async void btn_addTable_Click(object sender, EventArgs e)
        {


            // ID trong SQL là tự tăng, nhưng cứ parse để tránh lỗi format
            if (string.IsNullOrEmpty(textBox1.Text)) return;

            try
            {
                string status = MapStatusToSQL(comboBox1.SelectedItem?.ToString() ?? "Trống");
                var req = new AddTableRequest
                {
                    MaBan = int.Parse(textBox1.Text),
                    TenBan = tb_nameTable.Text,
                    TrangThai = status
                };

                await ExecuteAsync(btn_addTable, "...", async () =>
                {
                    // 1. Gói dữ liệu (Wrapper)
                    var wrapper = new { Type = "AddTable", Data = req };

                    // 2. Gọi SendRequest chuẩn (hứng về class AddTableResponse của bạn)
                    var resp = await SendRequest<object, AddTableResponse>(wrapper);

                    if (resp != null && resp.Success)
                    {
                        ShowSuccess(resp.Message);

                        // 3. ✅ Xóa dữ liệu trên ô nhập liệu
                        textBox1.Clear();
                        tb_nameTable.Clear();
                        comboBox1.SelectedIndex = 0;

                        // 4. ✅ Load lại bảng (hiện vào ô màu xám)
                        await _tableManager.RefreshAsync();
                    }
                    else ShowError(resp?.Message ?? "Thất bại");
                });
            }
            catch { ShowError("ID phải là số"); }
        }

        private async void btn_editTable_Click(object sender, EventArgs e)
        {
            if (_tableManager.GetSelectedItem() == null) { ShowWarning("Chọn bàn cần sửa"); return; }

            try
            {
                string status = MapStatusToSQL(comboBox1.SelectedItem?.ToString() ?? "Trống");
                var req = new UpdateTableRequest
                {
                    MaBan = int.Parse(textBox1.Text),
                    TenBan = tb_nameTable.Text,
                    TrangThai = status
                };

                await ExecuteAsync((Button)sender, "...", async () =>
                {
                    var wrapper = new { Type = "UpdateTable", Data = req };
                    var resp = await SendRequest<object, UpdateTableResponse>(wrapper);

                    if (resp != null && resp.Success)
                    {
                        ShowSuccess(resp.Message);
                        // Xóa ô nhập và load lại
                        textBox1.Clear(); tb_nameTable.Clear();
                        await _tableManager.RefreshAsync();
                    }
                    else ShowError(resp?.Message ?? "Thất bại");
                });
            }
            catch { ShowError("ID phải là số"); }
        }

        private async void btn_deleteTable_Click(object sender, EventArgs e)
        {
            var s = _tableManager.GetSelectedItem();
            if (s == null || !Confirm("Xóa bàn này?")) return;

            await ExecuteAsync((Button)sender, "...", async () =>
            {
                var req = new DeleteTableRequest { MaBan = s.MaBan };
                var wrapper = new { Type = "DeleteTable", Data = req };

                // Dùng BaseResponse hứng kết quả
                var resp = await SendRequest<object, BaseResponse>(wrapper);

                if (resp != null && resp.Success)
                {
                    ShowSuccess(resp.Message);
                    // Xóa ô nhập và load lại
                    textBox1.Clear(); tb_nameTable.Clear();
                    await _tableManager.RefreshAsync();
                }
                else ShowError(resp?.Message ?? "Lỗi (Có thể bàn đang có hóa đơn)");
            });
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await ExecuteAsync((Button)sender, "...", async () => {
                await _tableManager.RefreshAsync();
                textBox1.Clear(); tb_nameTable.Clear();
                ShowSuccess("Đã tải lại");
            });
        }

        private async void btn_searchTable_Click(object sender, EventArgs e)
        {
            string kw = "";
            if (this.Controls.Find("tb_searchTable", true).FirstOrDefault() is TextBox tb)
                kw = tb.Text.Trim();

            await _tableManager.LoadDataAsync(() => LoadTablesFromServer(kw));
        }
    }
}