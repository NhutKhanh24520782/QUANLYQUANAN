using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Models.Database;
using Models.Request;
using Models.Response;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantServer
{
    internal class Server
    {
        private TcpListener? listener;
        private bool isRunning = false;

        public void Start(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            isRunning = true;
            Console.WriteLine($"🚀 Server đang lắng nghe tại cổng {port}...");

            // ✅ SỬA: Async listener
            _ = Task.Run(async () => await ListenForClientsAsync());
        }

        public void Stop()
        {
            isRunning = false;
            listener?.Stop();
            Console.WriteLine("⛔ Server đã dừng.");
        }

        // ✅ SỬA: Async method
        private async Task ListenForClientsAsync()
        {
            while (isRunning)
            {
                try
                {
                    TcpClient client = await listener!.AcceptTcpClientAsync();
                    string endpoint = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";
                    Console.WriteLine($"🟢 Client kết nối: {endpoint}");

                    // ✅ Fire-and-forget async handling
                    _ = Task.Run(async () => await HandleClientAsync(client, endpoint));
                }
                catch (Exception ex) when (isRunning)
                {
                    Console.WriteLine($"⚠️ Lỗi accept client: {ex.Message}");
                }
            }
        }

        // ✅ SỬA: Async + using statements
        private async Task HandleClientAsync(TcpClient client, string endpoint)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                {
                    // ✅ SỬA: Đọc line thay vì Read()
                    string? jsonRequest = await reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(jsonRequest))
                    {
                        Console.WriteLine($"⚠️ {endpoint}: Request rỗng");
                        return;
                    }

                    Console.WriteLine($"📩 {endpoint}: {jsonRequest}");

                    // Parse request
                    JObject rawRequest = JObject.Parse(jsonRequest);
                    string type = rawRequest["Type"]?.ToString() ?? "";

                    // Process request
                    string jsonResponse = type switch
                    {
                        "Login" => await HandleLoginRequestAsync(rawRequest),
                        "Register" => await HandleRegisterRequestAsync(rawRequest),
                        "UpdatePassword" => await HandleUpdatePasswordRequestAsync(rawRequest),
                        "CheckEmail" => await HandleCheckEmailRequestAsync(rawRequest),
                        "GetEmployees" => await HandleGetEmployeesRequestAsync(rawRequest),
                        "AddEmployee" => await HandleAddEmployeeRequestAsync(rawRequest),
                        "UpdateEmployee" => await HandleUpdateEmployeeRequestAsync(rawRequest),
                        "DeleteEmployee" => await HandleDeleteEmployeeRequestAsync(rawRequest),
                        "ThongKeDoanhThu" => await HandleThongKeDoanhThuRequestAsync(rawRequest),
                        "XuatBaoCao" => await HandleXuatBaoCaoRequestAsync(rawRequest),
                        _ => HandleUnknownRequest()
                    };

                    // ✅ SỬA: Gửi line thay vì Write()
                    await writer.WriteLineAsync(jsonResponse);
                    Console.WriteLine($"📤 {endpoint}: Gửi phản hồi\n");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ {endpoint}: JSON không hợp lệ - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {endpoint}: Lỗi - {ex.Message}");
            }
        }

        // ✅ SỬA: Async methods
        private async Task<string> HandleLoginRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<LoginRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    var validation = request.Validate();
                    if (!validation.isValid)
                        return CreateErrorResponse(validation.error);

                    LoginResult result = DatabaseAccess.LoginUser(request.Username, request.Password);

                    var response = new LoginResponse
                    {
                        Success = result.Success,
                        Role = result.Role,
                        HoTen = result.HoTen,
                        Message = result.Message,
                        MaNguoiDung = result.MaNguoiDung,
                        Email = result.Email
                    };

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi đăng nhập: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleRegisterRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<RegisterRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    var validation = request.Validate();
                    if (!validation.isValid)
                        return CreateErrorResponse(validation.error);

                    RegisterResult result = DatabaseAccess.RegisterUser(
                        request.Username, request.Password, request.HoTen, request.Email, request.Role);

                    var response = new RegisterResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        MaNguoiDung = result.MaNguoiDung
                    };

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi đăng ký: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleUpdatePasswordRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<UpdatePasswordRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    RegisterResult result = DatabaseAccess.UpdatePassword(request.Email, request.NewPassword);

                    var response = new UpdatePasswordResponse
                    {
                        Success = result.Success,
                        Message = result.Message
                    };

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi đổi mật khẩu: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleCheckEmailRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<CheckEmailRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    EmailCheckResult result = DatabaseAccess.CheckEmailExists(request.Email);

                    var response = new CheckEmailResponse
                    {
                        Success = result.Success,
                        Exists = result.Exists,
                        Message = result.Message
                    };

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi kiểm tra email: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleGetEmployeesRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<GetEmployeesRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.GetEmployees(request.Keyword, request.VaiTro);

                    var response = new GetEmployeesResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        Employees = result.Employees
                    };

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi lấy danh sách nhân viên: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleAddEmployeeRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<AddEmployeeRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.AddEmployee(
                        request.TenDangNhap, request.MatKhau, request.HoTen,
                        request.Email, request.VaiTro, request.SDT, request.NgayVaoLam);

                    var response = new AddEmployeeResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        MaNguoiDung = result.MaNguoiDung
                    };

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi thêm nhân viên: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleUpdateEmployeeRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<UpdateEmployeeRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.UpdateEmployee(
                        request.MaNguoiDung, request.HoTen, request.Email,
                        request.VaiTro, request.SDT, request.TrangThai);

                    var response = new UpdateEmployeeResponse
                    {
                        Success = result.Success,
                        Message = result.Message
                    };

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi cập nhật nhân viên: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleDeleteEmployeeRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<DeleteEmployeeRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.DeleteEmployee(request.MaNguoiDung);

                    var response = new DeleteEmployeeResponse
                    {
                        Success = result.Success,
                        Message = result.Message
                    };

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi xóa nhân viên: {ex.Message}");
                }
            });
        }
        private async Task<string> HandleThongKeDoanhThuRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<ThongKeDoanhThuRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    var validation = request.Validate();
                    if (!validation.isValid)
                        return CreateErrorResponse(validation.error);

                    // Gọi database
                    var result = DatabaseAccess.GetDoanhThuFull(request.TuNgay, request.DenNgay);

                    var response = new ThongKeDoanhThuResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        TongDoanhThu = result.TongDoanhThu,
                        DoanhThuTheoBan = result.DoanhThuTheoBan
                    };

                    Console.WriteLine($"📊 Thống kê doanh thu: {request.TuNgay:dd/MM/yyyy} - {request.DenNgay:dd/MM/yyyy}");
                    Console.WriteLine($"   → Tổng doanh thu: {result.TongDoanhThu.tongDoanhThu:N0} VNĐ");
                    Console.WriteLine($"   → Số bàn: {result.DoanhThuTheoBan.Count}");

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi thống kê doanh thu: {ex.Message}");
                }
            });
        }
        private async Task<string> HandleXuatBaoCaoRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<XuatBaoCaoRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    // Gọi phương thức xuất báo cáo
                    var exportResult = DatabaseAccess.XuatBaoCaoExcel(
                        request.TuNgay, request.DenNgay, request.Data, request.TongDoanhThu);

                    // Tạo response
                    var response = new XuatBaoCaoResponse
                    {
                        Success = exportResult.success,
                        Message = exportResult.message,
                        FilePath = exportResult.filePath,
                        FileName = System.IO.Path.GetFileName(exportResult.filePath)
                    };

                    // Log kết quả
                    if (exportResult.success)
                    {
                        Console.WriteLine($"📄 Xuất báo cáo thành công: {exportResult.filePath}");
                    }

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi xuất báo cáo: {ex.Message}");
                }
            });
        }
        private string HandleUnknownRequest()
        {
            return CreateErrorResponse("Loại request không hợp lệ");
        }

        private string CreateErrorResponse(string message)
        {
            var errorResponse = new ErrorResponse
            {
                Message = message
            };
            return JsonConvert.SerializeObject(errorResponse);
        }
    }
}
