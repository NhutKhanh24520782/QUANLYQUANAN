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
                        "GetBills" => await HandleGetBillsRequestAsync(rawRequest),
                        "GetMenu" => await HandleGetMenuAsync(rawRequest),
                        "SearchMenu" => await HandleSearchMenuAsync(rawRequest),
                        "AddMenu" => await HandleAddMenuAsync(rawRequest),
                        "UpdateMenu" => await HandleUpdateMenuAsync(rawRequest),
                        "DeleteMenu" => await HandleDeleteMenuAsync(rawRequest),
                        "UpdateMenuStatus" => await HandleUpdateMenuStatusRequestAsync(rawRequest), // ✅ THÊM DÒNG NÀY


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
        private async Task<string> HandleGetBillsRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<GetBillRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.GetBills();

                    var response = new GetBillResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        Bills = result.Bills,
                    };

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi lấy danh sách hóa đơn: {ex.Message}");
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
        private async Task<string> HandleGetMenuAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = DatabaseAccess.GetMenu();
                    var response = new GetMenuResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        Items = result.Items
                    };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi lấy menu: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleSearchMenuAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<SearchMenuRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var result = DatabaseAccess.SearchMenu(request.Keyword);
                    var response = new GetMenuResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        Items = result.Items
                    };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi tìm menu: {ex.Message}");
                }
            });
        }
        private async Task<string> HandleAddMenuAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<AddMenuRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    // ✅ THÊM TRANGTHAI
                    var result = DatabaseAccess.AddMenu(request.TenMon, request.Gia, request.MoTa, request.MaLoaiMon, request.TrangThai);
                    var response = new AddMenuResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        MaMon = result.MaMon
                    };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi thêm món: {ex.Message}");
                }
            });
        }
        private async Task<string> HandleUpdateMenuAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<UpdateMenuRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    // ✅ THÊM TRANGTHAI VÀO LỆNH GỌI
                    var result = DatabaseAccess.UpdateMenu(
                        request.MaMon, request.TenMon, request.Gia, request.MoTa, request.MaLoaiMon, request.TrangThai);

                    var response = new UpdateMenuResponse
                    {
                        Success = result.Success,
                        Message = result.Message
                    };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi cập nhật món: {ex.Message}");
                }
            });
        }
        private async Task<string> HandleDeleteMenuAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<DeleteMenuRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.DeleteMenu(request.MaMon);
                    var response = new DeleteMenuResponse
                    {
                        Success = result.Success,
                        Message = result.Message
                    };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi xóa món: {ex.Message}");
                }
            });
        }
        // ✅ THÊM HANDLER MỚI
        private async Task<string> HandleUpdateMenuStatusRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<UpdateMenuStatusRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request không hợp lệ");

                    // Gọi database cập nhật status
                    var result = DatabaseAccess.UpdateMenuStatus(request.MaMon, request.TrangThai);

                    var response = new UpdateMenuResponse // Dùng chung response
                    {
                        Success = result.Success,
                        Message = result.Message
                    };

                    Console.WriteLine($"🔄 Cập nhật trạng thái món: {request.MaMon} -> {request.TrangThai}");

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi cập nhật trạng thái món: {ex.Message}");
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
        //===========TABLES===========
        private async Task<string> HandleAddTableRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // 1. Giải mã JSON từ request nhận được
                    var request = rawRequest.ToObject<AddTableRequest>();
                    if (request == null)
                        return CreateErrorResponse("Request 'AddTable' không hợp lệ");

                    // 2. Tạo đối tượng BanAn (Class lồng trong class Database)
                    // Cấu trúc: Namespace.OuterClass.InnerClass
                    Models.Database.BanAn banMoi = new Models.Database.BanAn
                    {
                        MaBan = request.MaBan,
                        TenBan = request.TenBan,
                        TrangThai = request.TrangThai
                    };

                    // 3. Gọi DatabaseAccess để thêm vào SQL
                    // (Hàm AddBanToSQL bên kia cũng phải nhận tham số là Models.Database.BanAn)
                    bool result = DatabaseAccess.AddBanToSQL(banMoi);

                    // 4. Tạo response trả về cho Client
                    var response = new AddTableResponse
                    {
                        Success = result,
                        Message = result ? "Thêm bàn thành công" : "Lỗi: ID bàn có thể đã tồn tại"
                    };

                    // 5. Đóng gói thành JSON gửi đi
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    // Bắt lỗi nếu có sự cố bất ngờ
                    return CreateErrorResponse($"Lỗi hệ thống khi thêm bàn: {ex.Message}");
                }
            });
        }
    }
}
