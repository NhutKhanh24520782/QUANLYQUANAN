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
    public partial class NVPhucVu : Form
    {
        // ==================== CONSTANTS & FIELDS ====================
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;
        private const string SEARCH_BILL_PLACEHOLDER = "Tìm theo mã hóa đơn...";

        private int _currentUserId;
        private string _currentUserName;

        private GridViewManager<PendingPaymentData> _billManager;
        private System.Windows.Forms.Timer _autoRefreshTimer; // 🔥 ĐÃ ĐƯỢC SỬ DỤNG

        // ==================== INITIALIZATION ====================
        public NVPhucVu(int userId, string userName)
        {
            _currentUserId = userId;
            _currentUserName = userName;

            InitializeComponent();
            InitializeGridViewManager();
            InitializePaymentControls();
            InitializeAutoRefreshTimer(); // 🔥 BỔ SUNG: Khởi tạo Timer
            LoadPendingBills();
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
                    // Sắp xếp tăng dần theo mã hóa đơn trước khi trả về
                    var sortedPayments = response.PendingPayments
                        .OrderBy(p => p.MaHD)
                        .ToList();

                    UpdateStatusLabel(sortedPayments.Count);
                    return sortedPayments;
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

            // Định dạng cột ngày tháng
            if (e.ColumnIndex == dataGridView_thanhtoan.Columns["NgayTao"].Index && e.Value != null)
            {
                if (DateTime.TryParse(e.Value.ToString(), out DateTime date))
                {
                    e.Value = date.ToString("HH:mm dd/MM/yyyy");
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

                // HIỂN THỊ THÔNG TIN CHI TIẾT VÀO CÁC TEXTBOX
                if (tb_idBill != null)
                {
                    tb_idBill.Text = payment.MaHD.ToString();
                }

                if (tb_idTable != null)
                {
                    tb_idTable.Text = payment.MaBanAn.ToString();
                }

                if (tb_dateBill != null)
                {
                    tb_dateBill.Text = payment.NgayTao.ToString("HH:mm dd/MM/yyyy");
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
                        string successMessage = $"Thanh toán thành công!\nMã giao dịch: {response.MaGiaoDich}";

                        if (paymentMethod == "TienMat" && response.SoTienThua > 0)
                        {
                            successMessage += $"\nTiền thừa: {response.SoTienThua:N0} VNĐ";
                        }

                        ShowSuccess(successMessage);

                        // 🔥 QUAN TRỌNG: Refresh danh sách -> hóa đơn đã thanh toán sẽ ẩn đi
                        await _billManager.RefreshAsync();

                        // Clear form
                        ClearBillDetails();
                        btn_ttoan.Enabled = false;

                        // Hiển thị QR code nếu là chuyển khoản
                        if (paymentMethod == "ChuyenKhoan")
                        {
                            ShowQRCode(selectedPayment.TongTien, response.MaGiaoDich.ToString() ?? "N/A");
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

        private void  ShowQRCode(decimal amount, string transactionNo)
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

        private async Task ExecuteAsync(Button button, string loadingText, Func<Task> action)
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
            _autoRefreshTimer?.Stop(); // 🔥 ĐẢM BẢO DỪNG TIMER
            _autoRefreshTimer?.Dispose();
            base.OnFormClosing(e);
        }

    }
}