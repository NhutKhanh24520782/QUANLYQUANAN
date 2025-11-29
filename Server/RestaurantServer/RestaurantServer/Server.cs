using Azure.Core;
using Models.Database;
using Models.Request;
using Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            _ = Task.Run(async () => await ListenForClientsAsync());
        }

        public void Stop()
        {
            isRunning = false;
            listener?.Stop();
            Console.WriteLine("⛔ Server đã dừng.");
        }

        private async Task ListenForClientsAsync()
        {
            while (isRunning)
            {
                try
                {
                    TcpClient client = await listener!.AcceptTcpClientAsync();
                    string endpoint = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";
                    Console.WriteLine($"🟢 Client kết nối: {endpoint}");

                    _ = Task.Run(async () => await HandleClientAsync(client, endpoint));
                }
                catch (Exception ex) when (isRunning)
                {
                    Console.WriteLine($"⚠️ Lỗi accept client: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, string endpoint)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                {
                    string? jsonRequest = await reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(jsonRequest))
                    {
                        Console.WriteLine($"⚠️ {endpoint}: Request rỗng");
                        return;
                    }

                    Console.WriteLine($"📩 {endpoint}: {jsonRequest}");

                    JObject rawRequest = JObject.Parse(jsonRequest);
                    string type = rawRequest["Type"]?.ToString() ?? "";

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
                        "UpdateMenuStatus" => await HandleUpdateMenuStatusRequestAsync(rawRequest),
                        "GetTables" => await HandleGetTablesRequestAsync(rawRequest),
                        "SearchTables" => await HandleSearchTablesRequestAsync(rawRequest),
                        "AddTable" => await HandleAddTableRequestAsync(rawRequest),
                        "UpdateTable" => await HandleUpdateTableRequestAsync(rawRequest),
                        "DeleteTable" => await HandleDeleteTableRequestAsync(rawRequest),
                        "GetPendingPayments" => await HandleGetPendingPaymentsRequestAsync(rawRequest),
                        "ProcessPayment" => await HandleProcessPaymentRequestAsync(rawRequest),
                        "GetCategories" => await HandleGetCategoriesRequestAsync(rawRequest),
                        "GetMenuByCategory" => await HandleGetMenuByCategoryRequestAsync(rawRequest),
                        "GetMon" => await HandleGetMonRequestAsync(rawRequest),
                        "CreateOrder" => await HandleCreateOrderRequestAsync(rawRequest),
                        _ => HandleUnknownRequest()
                    };

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

        private async Task<string> HandleLoginRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<LoginRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var validation = request.Validate();
                    if (!validation.isValid) return CreateErrorResponse(validation.error);

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
                catch (Exception ex) { return CreateErrorResponse($"Lỗi đăng nhập: {ex.Message}"); }
            });
        }

        private async Task<string> HandleRegisterRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<RegisterRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var validation = request.Validate();
                    if (!validation.isValid) return CreateErrorResponse(validation.error);

                    RegisterResult result = DatabaseAccess.RegisterUser(request.Username, request.Password, request.HoTen, request.Email, request.Role);
                    var response = new RegisterResponse { Success = result.Success, Message = result.Message, MaNguoiDung = result.MaNguoiDung };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi đăng ký: {ex.Message}"); }
            });
        }

        private async Task<string> HandleUpdatePasswordRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<UpdatePasswordRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    RegisterResult result = DatabaseAccess.UpdatePassword(request.Email, request.NewPassword);
                    var response = new UpdatePasswordResponse { Success = result.Success, Message = result.Message };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi đổi mật khẩu: {ex.Message}"); }
            });
        }

        private async Task<string> HandleCheckEmailRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<CheckEmailRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    EmailCheckResult result = DatabaseAccess.CheckEmailExists(request.Email);
                    var response = new CheckEmailResponse { Success = result.Success, Exists = result.Exists, Message = result.Message };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi kiểm tra email: {ex.Message}"); }
            });
        }

        private async Task<string> HandleGetEmployeesRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<GetEmployeesRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var result = DatabaseAccess.GetEmployees(request.Keyword, request.VaiTro);
                    var response = new GetEmployeesResponse { Success = result.Success, Message = result.Message, Employees = result.Employees };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi lấy danh sách nhân viên: {ex.Message}"); }
            });
        }

        private async Task<string> HandleAddEmployeeRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<AddEmployeeRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var result = DatabaseAccess.AddEmployee(request.TenDangNhap, request.MatKhau, request.HoTen, request.Email, request.VaiTro, request.SDT, request.NgayVaoLam);
                    var response = new AddEmployeeResponse { Success = result.Success, Message = result.Message, MaNguoiDung = result.MaNguoiDung };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi thêm nhân viên: {ex.Message}"); }
            });
        }

        private async Task<string> HandleUpdateEmployeeRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<UpdateEmployeeRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var result = DatabaseAccess.UpdateEmployee(request.MaNguoiDung, request.HoTen, request.Email, request.VaiTro, request.SDT, request.TrangThai);
                    var response = new UpdateEmployeeResponse { Success = result.Success, Message = result.Message };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi cập nhật nhân viên: {ex.Message}"); }
            });
        }

        private async Task<string> HandleDeleteEmployeeRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<DeleteEmployeeRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var result = DatabaseAccess.DeleteEmployee(request.MaNguoiDung);
                    var response = new DeleteEmployeeResponse { Success = result.Success, Message = result.Message };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi xóa nhân viên: {ex.Message}"); }
            });
        }

        private async Task<string> HandleThongKeDoanhThuRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<ThongKeDoanhThuRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var validation = request.Validate();
                    if (!validation.isValid) return CreateErrorResponse(validation.error);

                    var result = DatabaseAccess.GetDoanhThuFull(request.TuNgay, request.DenNgay);
                    var response = new ThongKeDoanhThuResponse { Success = result.Success, Message = result.Message, TongDoanhThu = result.TongDoanhThu, DoanhThuTheoBan = result.DoanhThuTheoBan };

                    Console.WriteLine($"📊 Thống kê doanh thu: {request.TuNgay:dd/MM/yyyy} - {request.DenNgay:dd/MM/yyyy}");
                    Console.WriteLine($"   → Tổng doanh thu: {result.TongDoanhThu.tongDoanhThu:N0} VNĐ");
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi thống kê doanh thu: {ex.Message}"); }
            });
        }

        private async Task<string> HandleGetBillsRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<GetBillRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var result = DatabaseAccess.GetBills();
                    var response = new GetBillResponse { Success = result.Success, Message = result.Message, Bills = result.Bills };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi lấy danh sách hóa đơn: {ex.Message}"); }
            });
        }

        private async Task<string> HandleXuatBaoCaoRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<XuatBaoCaoRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var exportResult = DatabaseAccess.XuatBaoCaoExcel(request.TuNgay, request.DenNgay, request.Data, request.TongDoanhThu);
                    var response = new XuatBaoCaoResponse { Success = exportResult.success, Message = exportResult.message, FilePath = exportResult.filePath, FileName = System.IO.Path.GetFileName(exportResult.filePath) };
                    if (exportResult.success) Console.WriteLine($"📄 Xuất báo cáo thành công: {exportResult.filePath}");
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi xuất báo cáo: {ex.Message}"); }
            });
        }

        private async Task<string> HandleGetMenuAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = DatabaseAccess.GetMenu();
                    var response = new GetMenuResponse { Success = result.Success, Message = result.Message, Items = result.Items };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi lấy menu: {ex.Message}"); }
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
                    var response = new GetMenuResponse { Success = result.Success, Message = result.Message, Items = result.Items };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi tìm menu: {ex.Message}"); }
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
                    var result = DatabaseAccess.AddMenu(request.TenMon, request.Gia, request.MoTa, request.MaLoaiMon, request.TrangThai);
                    var response = new AddMenuResponse { Success = result.Success, Message = result.Message, MaMon = result.MaMon };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi thêm món: {ex.Message}"); }
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
                    var result = DatabaseAccess.UpdateMenu(request.MaMon, request.TenMon, request.Gia, request.MoTa, request.MaLoaiMon, request.TrangThai);
                    var response = new UpdateMenuResponse { Success = result.Success, Message = result.Message };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi cập nhật món: {ex.Message}"); }
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
                    var response = new DeleteMenuResponse { Success = result.Success, Message = result.Message };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi xóa món: {ex.Message}"); }
            });
        }

        private async Task<string> HandleUpdateMenuStatusRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<UpdateMenuStatusRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var result = DatabaseAccess.UpdateMenuStatus(request.MaMon, request.TrangThai);
                    var response = new UpdateMenuResponse { Success = result.Success, Message = result.Message };
                    Console.WriteLine($"🔄 Cập nhật trạng thái món: {request.MaMon} -> {request.TrangThai}");
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi cập nhật trạng thái món: {ex.Message}"); }
            });
        }
        // ====================== TABLE HANDLERS ======================

        private async Task<string> HandleGetTablesRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = DatabaseAccess.GetTables();
                    var response = new GetTablesResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        ListBan = result.Tables
                    };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi lấy danh sách bàn: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleSearchTablesRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<SearchTablesRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.SearchTables(request.Keyword);
                    var response = new GetTablesResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        ListBan = result.Tables
                    };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi tìm kiếm bàn: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleAddTableRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<AddTableRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.AddTable(
                        request.TenBan,
                        request.SoChoNgoi,
                        request.TrangThai,
                        request.MaNhanVien
                    );

                    var response = new AddTableResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        MaBan = result.MaBanAn
                    };

                    if (result.Success)
                        Console.WriteLine($"✅ Thêm bàn thành công: {request.TenBan} (ID: {result.MaBanAn})");

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi thêm bàn: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleUpdateTableRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<UpdateTableRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.UpdateTable(
                        request.MaBanAn,
                        request.TenBan,
                        request.SoChoNgoi,
                        request.TrangThai,
                        request.MaNhanVien
                    );

                    var response = new UpdateTableResponse
                    {
                        Success = result.Success,
                        Message = result.Message
                    };

                    if (result.Success)
                        Console.WriteLine($"✅ Cập nhật bàn thành công: {request.TenBan} (ID: {request.MaBanAn})");

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi cập nhật bàn: {ex.Message}");
                }
            });
        }

        private async Task<string> HandleDeleteTableRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<DeleteTableRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.DeleteTable(request.MaBanAn);

                    var response = new DeleteTableResponse
                    {
                        Success = result.Success,
                        Message = result.Message
                    };

                    if (result.Success)
                        Console.WriteLine($"✅ Xóa bàn thành công: ID {request.MaBanAn}");

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi xóa bàn: {ex.Message}");
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
        //----------------THANH TOÁN---------------------------
        // ==================== LẤY DANH SÁCH CHỜ THANH TOÁN ====================
        private async Task<string> HandleGetPendingPaymentsRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<GetPendingPaymentsRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.GetPendingPayments(request.MaNhanVien);

                    var response = new GetPendingPaymentsResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        PendingPayments = result.PendingPayments
                    };

                    if (result.Success)
                        Console.WriteLine($"✅ Lấy danh sách chờ thanh toán: {result.PendingPayments.Count} hóa đơn");

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi lấy danh sách thanh toán: {ex.Message}");
                }
            });
        }

        // ==================== XỬ LÝ THANH TOÁN TỔNG QUÁT ====================
        private async Task<string> HandleProcessPaymentRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<ProcessPaymentRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    // Validate request
                    var validation = request.Validate();
                    if (!validation.isValid) return CreateErrorResponse(validation.error);

                    BaseResponse response;

                    if (request.PhuongThucThanhToan == "TienMat")
                    {
                        var result = DatabaseAccess.ProcessCashPayment(
                            request.MaHD,
                            request.SoTienNhan,
                            request.MaNhanVien
                        );

                        response = new ProcessPaymentResponse
                        {
                            Success = result.Success,
                            Message = result.Message,
                            MaGiaoDich = string.IsNullOrEmpty(result.MaGiaoDich) ? 0 : int.Parse(result.MaGiaoDich),
                            NgayThanhToan = result.NgayThanhToan,
                            PhuongThucThanhToan = "TienMat",
                            SoTienThanhToan = request.SoTienThanhToan,
                            SoTienThua = result.SoTienThua
                        };

                        if (result.Success)
                            Console.WriteLine($"✅ Thanh toán tiền mặt: HD{request.MaHD}, Tiền thừa: {result.SoTienThua:N0} VNĐ");
                    }
                    else if (request.PhuongThucThanhToan == "ChuyenKhoan")
                    {
                        var result = DatabaseAccess.ProcessTransferPayment(
                            request.MaHD,
                            request.MaNhanVien
                        );

                        response = new ProcessPaymentResponse
                        {
                            Success = result.Success,
                            Message = result.Message,
                            MaGiaoDich = string.IsNullOrEmpty(result.TransactionNo) ? 0 : int.Parse(result.TransactionNo),
                            NgayThanhToan = result.NgayThanhToan,
                            PhuongThucThanhToan = "ChuyenKhoan",
                            SoTienThanhToan = request.SoTienThanhToan,
                            SoTienThua = 0
                        };

                        if (result.Success)
                            Console.WriteLine($"✅ Thanh toán chuyển khoản: HD{request.MaHD}, Mã GD: {result.TransactionNo}");
                    }
                    else
                    {
                        return CreateErrorResponse("Phương thức thanh toán không được hỗ trợ");
                    }

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi xử lý thanh toán: {ex.Message}");
                }
            });
        }
        private async Task<string> HandleGetMonRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<GetMonRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");
                    var result = DatabaseAccess.GetMon();
                    var response = new GetMonResponse { 
                        Success = result.Success,
                        Message = result.Message,
                        MaMon=result.MaMon,
                        OrderMons = result.OrderMons };
                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex) { return CreateErrorResponse($"Lỗi lấy danh sách món: {ex.Message}"); }
            });
        }
        private async Task<string> HandleGetCategoriesRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = DatabaseAccess.GetCategories();
                    var response = new GetCategoriesResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        Categories = result.Categories
                    };

                    if (result.Success)
                        Console.WriteLine($"✅ Lấy danh sách loại món: {result.Categories?.Count ?? 0} loại");

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi lấy danh sách loại món: {ex.Message}");
                }
            });
        }

        // 🔥 THÊM: Handler lấy món ăn theo loại
        private async Task<string> HandleGetMenuByCategoryRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<GetMenuByCategoryRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.GetMenuByCategory(request.MaLoaiMon);
                    var response = new GetMenuResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        Items = result.Items
                    };

                    if (result.Success)
                    {
                        string categoryInfo = request.MaLoaiMon == 0 ? "tất cả các món" : $"loại {request.MaLoaiMon}";
                        Console.WriteLine($"✅ Lấy menu theo {categoryInfo}: {result.Items?.Count ?? 0} món");
                    }

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi lấy menu theo loại: {ex.Message}");
                }
            });
        }
        private async Task<string> HandleCreateOrderRequestAsync(JObject rawRequest)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var request = rawRequest.ToObject<CreateOrderRequest>();
                    if (request == null) return CreateErrorResponse("Request không hợp lệ");

                    var result = DatabaseAccess.CreateOrder(
                        request.MaBanAn,
                        request.MaNhanVien,
                        request.TongTien,
                        request.ChiTietOrder
                    );

                    var response = new CreateOrderResponse
                    {
                        Success = result.Success,
                        Message = result.Message,
                        MaHoaDon = result.MaHoaDon
                    };

                    if (result.Success)
                        Console.WriteLine($"✅ Tạo order thành công: HD{result.MaHoaDon} cho bàn {request.MaBanAn}");

                    return JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    return CreateErrorResponse($"Lỗi tạo order: {ex.Message}");
                }
            });
        }
    }
}