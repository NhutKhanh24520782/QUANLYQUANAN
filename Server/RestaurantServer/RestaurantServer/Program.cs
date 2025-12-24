using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.WriteLine("=== KHỞI ĐỘNG HỆ THỐNG SERVER NHÀ HÀNG ===");

            // ---------------------------------------------------------
            // 1. Chạy TCP Server (Cổng 5000) - Dành cho App WinForms
            // ---------------------------------------------------------
            // Nên chạy trong Task.Run để không chặn luồng chính nếu server.Start() bị block
            Task.Run(() =>
            {
                try
                {
                    Server server = new Server();
                    server.Start(5000);
                    // Dòng này sẽ được in ra từ bên trong hàm Start() của class Server
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi TCP Server (5000): {ex.Message}");
                }
            });

            // ---------------------------------------------------------
            // 2. Chạy Webhook Server (Cổng 8888) - Dành cho SePay
            // ---------------------------------------------------------
            Task.Run(() =>
            {
                try
                {
                    WebhookServer webhookServer = new WebhookServer();
                    webhookServer.Start(8888);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi Webhook Server (8888): {ex.Message}");
                    Console.WriteLine("👉 Hãy chạy Visual Studio bằng quyền Admin (Run as Administrator)!");
                }
            });

            // ---------------------------------------------------------
            // 3. Thông báo hoàn tất
            // ---------------------------------------------------------
            Console.WriteLine("\n✅ Hệ thống đang chạy nền...");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("1. App Nhân viên kết nối tới: 127.0.0.1:5000");
            Console.WriteLine("2. SePay Webhook kết nối tới: 127.0.0.1:8888");
            Console.WriteLine("   (Cần chạy lệnh: ngrok http 8888)");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("🔴 Nhấn Enter để tắt Server...");

            // Giữ màn hình không tắt (Chỉ để 1 cái duy nhất ở cuối cùng)
            Console.ReadLine();
        }
    }
}