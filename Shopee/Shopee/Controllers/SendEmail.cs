using SendGrid.Helpers.Mail;
using SendGrid;
using static System.Net.WebRequestMethods;
using Shopee.Models;

namespace Shopee.Controllers
{
    public class SendEmail
    {
        public void SendOrderDetails(OrderModel order, CartModel cart)
        {
            string apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            Console.WriteLine(apiKey);
            if (apiKey != null)
            {
                string email = cart.User.UserEmail;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("warmouleas@gmail.com", "Shopee - Order");
                var subject = "Your Order got placed";
                var to = new EmailAddress(email, "User");
                var plainTextContent = "";
                var htmlContent = $"<h2>Your order details</h2>" +
                    $"<p>Name: {cart.User.UserName}</p><br />" +
                    $"<p>Address: {cart.User.UserAddress}</p><br />" +
                    $"<p>Item name: {cart.Inventory.ItemName}</p><br />" +
                    $"<p>Item Quantity: {cart.Quantity}</p><br />" +
                    $"<h3>Total: ${cart.Quantity*Convert.ToInt32(cart.Inventory.ItemPrice)}</h3><br />" +
                    $"<hr />";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                DateTime sendDateTime = DateTime.Now;
                msg.SendAt = new DateTimeOffset(sendDateTime).ToUnixTimeSeconds();
                var response = client.SendEmailAsync(msg).Result;
            }
        }
    }
}
