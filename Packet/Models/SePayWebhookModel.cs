using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SePayWebhookModel
    {
        public long Id { get; set; }
        public string Gateway { get; set; } // Ví dụ: MBBank, VCB
        public DateTime TransactionDate { get; set; }
        public string AccountNumber { get; set; }
        public string SubAccount { get; set; }
        public decimal TransferAmount { get; set; } // Số tiền
        public string TransferContent { get; set; } // Nội dung CK (Mã đơn hàng)
        public string ReferenceCode { get; set; }
        public string Description { get; set; }
    }
}
