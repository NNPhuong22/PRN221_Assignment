using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using MyRazorPage.Models;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace MyRazorPage.Pages.Account
{
    public class ForgotModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public ForgotModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public string alert { get; set; }
        public string id { get; set; }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {

            string email = Request.Form["email"];

            HttpContext.Session.SetString("EmailReset", JsonSerializer.Serialize(email));

            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("phuongnnhe153074@fpt.edu.vn"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = "Reset pass";
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "<html>\r\n  <head>\r\n    <style>\r\n      .colored {\r\n        color: blue;\r\n      }\r\n      #body {\r\n        font-size: 14px;\r\n      }\r\n    </style>\r\n  </head>\r\n  <body>\r\n    <div id='body'>\r\n      <p>Hi "+email+",</p>\r\n      <p class='colored'>This is the link to reset your password, please access this <a href='http://localhost:5000/Account/ResetPass'>link</a> and reset your password" +
                ".</p>\r\n      <p></p>\r\n    </div>\r\n  </body>\r\n</html>\r\n" };

            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("phuongnnhe153074@fpt.edu.vn","1329nnp4968");
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
            }

            return RedirectToPage("/Account/ResetPass");
        }
    }
}
