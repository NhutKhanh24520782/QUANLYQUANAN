using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantServer
{
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Route("api/[controller]")]
    [ApiController]
    public class SePayController : ControllerBase
    {
        [HttpPost("webhook")]
        public IActionResult ReceiveWebhook([FromBody] SePayWebhookModel data)
        {
            // 1. (Bảo mật) Kiểm tra API Token từ Header xem có đúng là SePay gửi không
            // Vì bạn học InfoSec, cái này quan trọng để chống giả mạo request.
            // string sepayToken = Request.Headers["Authorization"];
            // if (sepayToken != "Bearer TOKEN_CUA_BAN") return Unauthorized();

            // 2. Log dữ liệu ra để debug
            Console.WriteLine($"[SePay Webhook] Nhận được: {data.TransferAmount} VND - Nội dung: {data.TransferContent}");

            // 3. Xử lý nghiệp vụ (Kết nối Database, Update trạng thái đơn hàng)
            // VD: Tìm đơn hàng có mã trùng với data.TransferContent và set Status = Paid
            // ProcessOrder(data.TransferContent, data.TransferAmount);

            // 4. Trả về success để SePay biết bạn đã nhận được tin
            return Ok(new { success = true, message = "Đã nhận tiền" });
        }
    }
}
