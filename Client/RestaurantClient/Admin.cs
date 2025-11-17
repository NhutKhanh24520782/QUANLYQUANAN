using Models.Database;
using Models.Request;
using Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;

namespace RestaurantClient
{
    public partial class Admin : Form
    {
        // ==================== GRIDVIEW MANAGERS ====================
        private GridViewManager<EmployeeData> _employeeManager;
        private GridViewManager<DoanhThuTheoBan> _doanhThuManager;
        // private GridViewManager<TableData> _tableManager;

        // ==================== CONSTANTS ====================
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;
        private const string SEARCH_PLACEHOLDER = "Tìm theo tên hoặc email...";

        // ==================== INITIALIZATION ====================
        public Admin()
        {
            InitializeComponent();
            InitializeGridViewManagers();
            InitializeDoanhThuControls(); 
            InitializeControls();
            LoadAllData();
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
            // Doanh Thu GridView - Sử dụng dataGridView_doanhthu trong panel1
            _doanhThuManager = new GridViewManager<DoanhThuTheoBan>(
                dataGridView_doanhthu, // DataGridView trong panel1
                LoadDoanhThuFromServer,
                dt => new
                {
                    TenBan = dt.TenBan,
                    SoLuongHoaDon = dt.SoLuongHoaDon,
                    DoanhThu = dt.DoanhThu,
                    HoaDonLonNhat = dt.HoaDonLonNhat,
                    HoaDonNhoNhat = dt.HoaDonNhoNhat,
                    DoanhThuTB = dt.DoanhThuTB
                },
                "TenBan"
            );

            // Gắn event handler
            dataGridView_emp.SelectionChanged += (s, e) =>
            {
                var selected = _employeeManager.GetSelectedItem();
                if (selected != null)
                    ShowEmployeeDetails(selected);
            };

            dataGridView_doanhthu.SelectionChanged += (s, e) =>
            {
                var selected = _doanhThuManager.GetSelectedItem();
                if (selected != null)
                    ShowDoanhThuDetails(selected);
            };
        }

        private void InitializeControls()
        {
            cb_position.Items.AddRange(new[] { "Admin", "PhucVu", "Bep" });
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
            await _doanhThuManager.LoadDataAsync();
            // await _tableManager.LoadDataAsync();
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
            await _employeeManager.RefreshAsync();
            ClearForm();
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

        private async void btn_xuatbaocao_Click(object sender, EventArgs e)
        {
            XuatExcelTrucTiep();
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

        // ==================== HELPER METHODS ====================
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

       
       
    }
}