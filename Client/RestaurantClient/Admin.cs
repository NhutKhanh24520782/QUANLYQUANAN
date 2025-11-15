using Models.Request;
using Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        // private GridViewManager<MenuData> _menuManager;
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
            InitializeControls();
            LoadAllData();
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

            // Gắn event handler
            dataGridView_emp.SelectionChanged += (s, e) =>
            {
                var selected = _employeeManager.GetSelectedItem();
                if (selected != null)
                    ShowEmployeeDetails(selected);
            };

            // Menu GridView (uncomment khi cần)
            // _menuManager = new GridViewManager<MenuData>(
            //     dataGridView_menu,
            //     LoadMenuFromServer,
            //     menu => new { ... },
            //     "MaMon"
            // );
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
            // await _menuManager.LoadDataAsync();
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