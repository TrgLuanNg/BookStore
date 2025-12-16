using System.Net;
using System.Net.Mail;

namespace BookStore.Services
{
    public class EmailService
    {
        public void SendOrderConfirmation(string toEmail, string orderId, double total, string customerName)
        {
            var fromAddress = new MailAddress("ngluan0307@gmail.com", "TheLibrary BookStore");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "rbdq elqi dtfo fliy"; // Vào Google Account > Security > App Passwords để lấy

            string subject = $"Xác nhận đơn hàng #{orderId}";
            string body = $@"
                <h2>Cảm ơn {customerName} đã đặt hàng tại TheLibrary!</h2>
                <p>Mã đơn hàng: <strong>{orderId}</strong></p>
                <p>Tổng tiền: <strong>{total.ToString("#,##0")} đ</strong></p>
                <p>Chúng tôi sẽ sớm liên hệ để giao hàng.</p>
                <br>
                <p>Trân trọng,</p>
                <p>TheLibrary Team</p>
            ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}