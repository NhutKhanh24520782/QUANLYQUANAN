using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;

namespace RestaurantServer
{
    internal class Server
    {
        private TcpListener? listener;

        // 🔹 Hàm khởi động server
        public void Start(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"🚀 Server đang lắng nghe tại cổng {port}...");

            // Lắng nghe client trong luồng riêng
            Thread thread = new Thread(ListenForClients);
            thread.Start();
        }

        // 🔹 Chờ client kết nối
        private void ListenForClients()
        {
            while (true)
            {
                TcpClient client = listener!.AcceptTcpClient();
                Console.WriteLine("🟢 Client mới đã kết nối!");

                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        // 🔹 Xử lý client gửi dữ liệu
        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                string jsonRequest = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("📩 Nhận từ client: " + jsonRequest);

                dynamic request = JsonConvert.DeserializeObject(jsonRequest);
                string type = request.Type;
                string response = "";

                if (type == "Login")
                {
                    string username = request.Username;
                    string password = request.Password;
                    bool success = DatabaseAccess.LoginUser(username, password);

                    response = JsonConvert.SerializeObject(new
                    {
                        Type = "LoginResponse",
                        Success = success
                    });
                }
                else if (type == "Register")
                {
                    string username = request.Username;
                    string password = request.Password;
                    string fullName = request.FullName;
                    string email = request.Email;
                    string role = request.Role;

                    bool success = DatabaseAccess.RegisterUser(username, password, fullName, email, role);

                    response = JsonConvert.SerializeObject(new
                    {
                        Type = "RegisterResponse",
                        Success = success
                    });
                }
                else if (type == "UpdatePassword")
                {
                    string email = request.Email;
                    string newPass = request.NewPassword;
                    bool success = DatabaseAccess.UpdatePassword(email, newPass);

                    response = JsonConvert.SerializeObject(new
                    {
                        Type = "UpdatePasswordResponse",
                        Success = success
                    });
                }
                else if (type == "CheckEmail")
                {
                    string email = request.Email;
                    bool exists = DatabaseAccess.CheckEmailExists(email); // trả về true/false
                    response = JsonConvert.SerializeObject(new
                    {
                        Type = "CheckEmailResponse",
                        Exists = exists
                    });
                }


                // Gửi phản hồi lại client
                byte[] data = Encoding.UTF8.GetBytes(response);
                stream.Write(data, 0, data.Length);

                stream.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Lỗi xử lý client: " + ex.Message);
            }
        }
    }
}