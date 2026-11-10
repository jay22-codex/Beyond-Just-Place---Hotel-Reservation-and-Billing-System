using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Model;

namespace BusinessLogic
{
    public class HotelReceiptService
    {
        public string BuildReceiptHtml(
            BillingInfo billing,
            PaymentReceiptInfo payment)
        {
            if (billing == null) throw new ArgumentNullException("billing");
            if (payment == null) throw new ArgumentNullException("payment");

            string hotelName = GetSetting("HotelName", "Beyond Just Place");
            string hotelAddress = GetSetting("HotelAddress", "Hotel address not configured");
            string hotelPhone = GetSetting("HotelPhone", "Not configured");
            string hotelEmail = GetSetting("HotelEmail", "Not configured");
            string hotelTin = GetSetting("HotelTIN", "Not configured");

            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html><html><head><meta charset='utf-8'>");
            html.Append("<title>").Append(E(payment.ReceiptNumber)).Append("</title>");
            html.Append("<style>");
            html.Append("body{font-family:Arial,sans-serif;color:#222;background:#fff;margin:0;padding:24px;}");
            html.Append(".receipt{max-width:850px;margin:0 auto;border:1px solid #aaa;padding:28px;}");
            html.Append("h1{margin:0 0 6px;font-size:26px;} h2{font-size:18px;margin:24px 0 8px;}");
            html.Append(".muted{color:#555;font-size:13px;} .top{display:flex;justify-content:space-between;gap:30px;}");
            html.Append("table{width:100%;border-collapse:collapse;margin-top:8px;} th,td{padding:9px;border-bottom:1px solid #ddd;text-align:left;}");
            html.Append("th{background:#f1f1f1;} .right{text-align:right;} .total{font-size:18px;font-weight:bold;}");
            html.Append(".status{font-weight:bold;} .footer{margin-top:28px;padding-top:16px;border-top:1px solid #bbb;font-size:12px;color:#555;}");
            html.Append("@media print{body{padding:0}.receipt{border:0;max-width:none}.no-print{display:none}}");
            html.Append("</style></head><body><div class='receipt'>");

            html.Append("<div class='top'><div>");
            html.Append("<h1>").Append(E(hotelName)).Append("</h1>");
            html.Append("<div class='muted'>").Append(E(hotelAddress)).Append("</div>");
            html.Append("<div class='muted'>Phone: ").Append(E(hotelPhone)).Append("</div>");
            html.Append("<div class='muted'>Email: ").Append(E(hotelEmail)).Append("</div>");
            html.Append("<div class='muted'>TIN: ").Append(E(hotelTin)).Append("</div>");
            html.Append("</div><div>");
            html.Append("<strong>CHECK-OUT RECEIPT</strong><br>");
            html.Append("Receipt No.: ").Append(E(payment.ReceiptNumber)).Append("<br>");
            html.Append("Payment Date: ").Append(E(payment.PaymentDate.ToString("MMMM dd, yyyy hh:mm tt"))).Append("<br>");
            html.Append("Reservation ID: ").Append(billing.ReservationId).Append("</div></div>");

            html.Append("<h2>Guest / Stay Information</h2><table>");
            Row(html, "Guest Name", billing.GuestName, "Room", billing.RoomNumber);
            Row(html, "Contact Number", billing.GuestContactNumber, "Room Type", billing.RoomType);
            Row(html, "Email", billing.GuestEmail, "Check-In", DateValue(billing.CheckInDate));
            Row(html, "Address", billing.GuestAddress, "Check-Out", DateValue(billing.CheckOutDate));
            Row(html, "ID Type", billing.GuestIdType, "Nights", billing.StayDays.ToString());
            Row(html, "Guests", billing.NumberOfGuests.ToString(), "Adults / Children", billing.Adults + " / " + billing.Children);
            if (!string.IsNullOrWhiteSpace(billing.SpecialRequest))
                Row(html, "Special Request", billing.SpecialRequest, "Status", "Checked Out");
            html.Append("</table>");

            html.Append("<h2>Charges</h2><table><tr><th>Description</th><th class='right'>Rate</th><th class='right'>Qty</th><th class='right'>Amount</th></tr>");
            html.Append("<tr><td>Room Charges</td><td class='right'>PHP ").Append(M(billing.RoomRate)).Append("</td><td class='right'>").Append(billing.StayDays).Append("</td><td class='right'>PHP ").Append(M(billing.RoomCharges)).Append("</td></tr>");
            html.Append("<tr><td>Other Charges</td><td class='right'>PHP ").Append(M(billing.OtherCharges)).Append("</td><td class='right'>").Append(billing.OtherCharges > 0m ? "1" : "0").Append("</td><td class='right'>PHP ").Append(M(billing.OtherCharges)).Append("</td></tr>");
            html.Append("<tr class='total'><td colspan='3'>TOTAL BILL</td><td class='right'>PHP ").Append(M(billing.TotalBill)).Append("</td></tr></table>");

            html.Append("<h2>Payment</h2><table>");
            Row(html, "Payment Method", payment.PaymentMethod, "Payment Status", payment.PaymentStatus);
            Row(html, "Amount Due", "PHP " + M(payment.PaymentAmount), "Amount Received", "PHP " + M(payment.AmountReceived));
            Row(html, "Change", "PHP " + M(payment.ChangeAmount), "Processed By", payment.ProcessedBy);
            html.Append("</table>");

            html.Append("<div class='footer'>Thank you for staying with ").Append(E(hotelName)).Append(".<br>");
            html.Append("This is a system-generated hotel payment and check-out receipt.</div>");
            html.Append("</div></body></html>");
            return html.ToString();
        }

        public string SaveReceiptFile(
            BillingInfo billing,
            PaymentReceiptInfo payment)
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "BeyondJustPlace",
                "Receipts");

            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder,
                "Receipt_" + payment.ReceiptNumber + ".html");

            File.WriteAllText(path, BuildReceiptHtml(billing, payment), Encoding.UTF8);
            return path;
        }

        public void OpenReceiptForPrinting(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new FileNotFoundException("Receipt file was not found.", path);

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        private static void Row(StringBuilder html,
            string leftLabel, string leftValue,
            string rightLabel, string rightValue)
        {
            html.Append("<tr><td><strong>").Append(E(leftLabel)).Append("</strong></td><td>")
                .Append(E(leftValue)).Append("</td><td><strong>").Append(E(rightLabel))
                .Append("</strong></td><td>").Append(E(rightValue)).Append("</td></tr>");
        }

        private static string E(string value)
        {
            return WebUtility.HtmlEncode(value ?? "");
        }

        private static string M(decimal value)
        {
            return value.ToString("N2", CultureInfo.CurrentCulture);
        }

        private static string DateValue(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("MMMM dd, yyyy") : "";
        }

        private static string GetSetting(string key, string fallback)
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }
    }
}
