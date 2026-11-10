using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BusinessLogic
{
    public class EmailJsReceiptService
    {
        private const string SendEndpoint =
            "https://api.emailjs.com/api/v1.0/email/send";

        public bool IsConfigured
        {
            get
            {
                return IsRealValue(GetSetting("EmailJsServiceId")) &&
                       IsRealValue(GetSetting("EmailJsTemplateId")) &&
                       IsRealValue(GetSetting("EmailJsPublicKey"));
            }
        }

        public async Task SendReceiptAsync(
            string toEmail,
            string guestName,
            string receiptNumber,
            string receiptHtml)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                throw new InvalidOperationException(
                    "The guest does not have an email address saved.");
            }

            string serviceId = GetSetting("EmailJsServiceId");
            string templateId = GetSetting("EmailJsTemplateId");
            string publicKey = GetSetting("EmailJsPublicKey");

            if (!IsRealValue(serviceId))
                throw new InvalidOperationException(
                    "EmailJsServiceId is missing from App.config.");

            if (!IsRealValue(templateId))
                throw new InvalidOperationException(
                    "EmailJsTemplateId is missing from App.config.");

            if (!IsRealValue(publicKey))
                throw new InvalidOperationException(
                    "EmailJsPublicKey is missing from App.config.");

            // Explicit TLS 1.2 support for .NET Framework desktop clients.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dictionary<string, object> templateParams =
                new Dictionary<string, object>();

            templateParams["to_email"] = toEmail.Trim();
            templateParams["guest_name"] =
                string.IsNullOrWhiteSpace(guestName)
                    ? "Guest"
                    : guestName.Trim();
            templateParams["receipt_number"] =
                string.IsNullOrWhiteSpace(receiptNumber)
                    ? "Receipt"
                    : receiptNumber.Trim();
            templateParams["hotel_name"] =
                GetSetting("HotelName", "Beyond Just Place");
            templateParams["receipt_html"] = receiptHtml ?? "";

            Dictionary<string, object> payload =
                new Dictionary<string, object>();

            payload["service_id"] = serviceId;
            payload["template_id"] = templateId;
            payload["user_id"] = publicKey;
            payload["template_params"] = templateParams;

            string privateKey = GetSetting("EmailJsPrivateKey");
            if (IsRealValue(privateKey))
                payload["accessToken"] = privateKey;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(payload);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                using (StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    HttpResponseMessage response;

                    try
                    {
                        response = await client.PostAsync(
                            SendEndpoint, content);
                    }
                    catch (TaskCanceledException)
                    {
                        throw new InvalidOperationException(
                            "EmailJS request timed out. Check your internet connection.");
                    }
                    catch (HttpRequestException ex)
                    {
                        throw new InvalidOperationException(
                            "Could not connect to EmailJS.\n" + ex.Message,
                            ex);
                    }

                    string responseBody =
                        await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException(
                            "EmailJS rejected the email.\n\n" +
                            "HTTP Status: " +
                            ((int)response.StatusCode).ToString() +
                            "\nResponse: " + responseBody +
                            "\nRecipient: " + toEmail);
                    }
                }
            }
        }

        private static string GetSetting(
            string key,
            string fallback = "")
        {
            string value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value)
                ? fallback
                : value.Trim();
        }

        private static bool IsRealValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string upper = value.Trim().ToUpperInvariant();
            return !upper.StartsWith("YOUR_") &&
                   !upper.Contains("CHANGE_ME");
        }
    }
}
