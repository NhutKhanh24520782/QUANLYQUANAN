using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestaurantServer
{
    public class WebhookServer
    {
        private HttpListener _listener;
        private bool _isRunning = false;

        public void Start(int port)
        {
            Console.WriteLine("✅ Webhook Server đang lắng nghe tại cổng 8888 (Cho SePay)...");
            Console.WriteLine("--> Hãy chạy ngrok: ngrok http 8888");
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://*:{port}/"); // Lắng nghe mọi IP trên cổng này
            try
            {
                _listener.Start();
                _isRunning = true;
                Console.WriteLine($"🌍 Webhook Server (SePay) đang chạy tại cổng {port}...");

                // Chạy luồng lắng nghe riêng
                Task.Run(() => ListenLoop());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khởi động Webhook: {ex.Message}");
                Console.WriteLine("👉 Hãy thử chạy Visual Studio bằng quyền Admin!");
            }
        }

        private async Task ListenLoop()
        {
            while (_isRunning)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = ProcessRequest(context); // Xử lý request ở luồng phụ để không chặn
                }
                catch (Exception ex)
                {
                    if (_isRunning) Console.WriteLine($"Lỗi Webhook Listener: {ex.Message}");
                }
            }
        }

        private async Task ProcessRequest(HttpListenerContext context)
        {
            try
            {
                var request = context.Request;
                var response = context.Response;

                // Chỉ xử lý method POST
                if (request.HttpMethod == "POST")
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string jsonBody = await reader.ReadToEndAsync();

                        // Xử lý logic SePay
                        HandleSePayWebhook(jsonBody);
                    }
                }

                // Luôn trả về 200 OK cho SePay vui lòng
                string responseString = "{\"success\": true}";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xử lý request: {ex.Message}");
            }
        }

        private void HandleSePayWebhook(string jsonBody)
        {
            try
            {
                Console.WriteLine("\n🔔 [WEBHOOK] Nhận được thông báo thanh toán:");
                Console.WriteLine(jsonBody);

                // 1. Parse JSON
                JObject data = JObject.Parse(jsonBody);

                // Lấy các trường quan trọng từ SePay
                string transferContent = data["transferContent"]?.ToString() ?? ""; // Nội dung CK
                decimal transferAmount = (decimal)(data["transferAmount"] ?? 0);    // Số tiền

                // 2. Phân tích nội dung để tìm Mã Hóa Đơn (VD: "HD102")
                // Regex tìm chữ HD hoặc hd theo sau là số
                Match match = Regex.Match(transferContent, @"(HD|hd)(\d+)", RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    int maHD = int.Parse(match.Groups[2].Value);
                    Console.WriteLine($"=> Phát hiện thanh toán cho Hóa Đơn #{maHD} - Số tiền: {transferAmount:N0}");

                    // 3. GỌI HÀM DATABASE CỦA BẠN ĐỂ CẬP NHẬT
                    DatabaseAccess.ConfirmPaymentWebhook(maHD, transferAmount);
                }
                else
                {
                    Console.WriteLine("⚠️ Không tìm thấy mã hóa đơn (HD...) trong nội dung chuyển khoản.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi phân tích dữ liệu SePay: {ex.Message}");
            }
        }
    }
}