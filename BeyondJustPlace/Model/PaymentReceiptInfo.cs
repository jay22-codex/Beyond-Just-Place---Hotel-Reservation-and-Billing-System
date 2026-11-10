using System;

namespace Model
{
    public class PaymentReceiptInfo
    {
        public int PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal ChangeAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string ProcessedBy { get; set; }

        public string ReceiptNumber
        {
            get { return "BJP-" + PaymentId.ToString("D6"); }
        }
    }
}
