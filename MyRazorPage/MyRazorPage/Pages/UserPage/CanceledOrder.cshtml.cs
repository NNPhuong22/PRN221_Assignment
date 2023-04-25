using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Security.Principal;
using System.Text.Json;

namespace MyRazorPage.Pages.UserPage
{
    public class CanceledOrderModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public CanceledOrderModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Customer Customer { get; set; }

        public IActionResult OnGet()
        {
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            if (HttpContext.Session.GetString("Custsession") == null)
            {
                return RedirectToPage("ErrorPage");
            }
            var account = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(HttpContext.Session.GetString("Custsession"), option);
            Customer = dBContext.Customers.Include(o => o.Orders).ThenInclude(a => a.OrderDetails).ThenInclude(p => p.Product).SingleOrDefault(i => i.CustomerId == account.CustomerId);
            return Page();
        }
    }
}
