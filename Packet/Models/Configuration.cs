using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configuration
{
    public class VNPayConfig
    {
        public string VnpUrl { get; set; } = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public string VnpReturnUrl { get; set; } = "http://localhost:5000/payment/vnpay-callback";
        public string VnpTmnCode { get; set; } = "YOUR_TMN_CODE";
        public string VnpHashSecret { get; set; } = "YOUR_HASH_SECRET";
        public string VnpVersion { get; set; } = "2.1.0";
        public string VnpCommand { get; set; } = "pay";
        public string VnpCurrCode { get; set; } = "VND";
        public string VnpLocale { get; set; } = "vn";
        public string VnpOrderType { get; set; } = "billpayment";
    }
}