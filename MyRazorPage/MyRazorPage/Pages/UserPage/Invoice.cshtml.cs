using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Options;
using MimeKit;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.UserPage
{
    public class InvoiceModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public InvoiceModel(PRN221DBContext _dBContext)
        {
            dBContext = _dBContext;
        }
        public List<Models.CartSession> cart { get; set; }
        public Models.Order order { get; set; }
        public Models.Customer customer { get; set; }
        public decimal? TotalMoney { get; set; }
        public void OnGet()
        {
            var option = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    WriteIndented = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
            if (HttpContext.Session.GetString("Invoice") != null)
            {
                
                cart = JsonSerializer.Deserialize<List<MyRazorPage.Models.CartSession>>(HttpContext.Session.GetString("Invoice"), option);
                TotalMoney = 0;

                order = dBContext.Orders.OrderBy(o => o.OrderId).LastOrDefault();
                     
                customer = dBContext.Customers.OrderBy(o => o.CustomerId).LastOrDefault();

                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }

            }
            var account = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(HttpContext.Session.GetString("Custsession"), option);

            MimeMessage emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("phuongnnhe153074@fpt.edu.vn"));
            emailToSend.To.Add(MailboxAddress.Parse(account.Email));
            emailToSend.Subject = "Invoice";
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "<html>\r\n  <head>\r\n    <style>\r\n      .colored {\r\n        color: blue;\r\n      }\r\n      #body {\r\n        font-size: 14px;\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div id='body'>\r\n      <p>Hi " + customer.ContactName + ",</p>\r\n      <p class='colored'>This is the link to reset your invoice, please access this <a href='http://localhost:5000/UserPage/Invoice'>link</a> and download the invoice" +
                ".</p>\r\n      <p></p>\r\n    </div>\r\n  </body>\r\n</html>\r\n"
            };
            //using (var emailClient = new SmtpClient())
            //{
            //    emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            //    emailClient.Authenticate("phuongnnhe153074@fpt.edu.vn", "1329nnp4968");
            //    emailClient.Send(emailToSend);
            //    emailClient.Disconnect(true);
            //}



        }
    }
}
