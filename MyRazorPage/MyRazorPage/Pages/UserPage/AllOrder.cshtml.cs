using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.UserPage
{
    public class AllOrderModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public AllOrderModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
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
                return RedirectToPage("/Account/SignIn");
            }

            var account = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(HttpContext.Session.GetString("Custsession"), option);
            Customer = dBContext.Customers.Include(o => o.Orders).ThenInclude(a => a.OrderDetails).ThenInclude(p => p.Product).SingleOrDefault(i => i.CustomerId == account.CustomerId);
            return Page();
        }

        public async Task<IActionResult> OnGetCancelOrder(int id)
        {
            var orderCancel = dBContext.Orders.ToList().SingleOrDefault(o => o.OrderId == id);
            orderCancel.RequiredDate = null;
            dBContext.SaveChanges();

            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            var account = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(HttpContext.Session.GetString("Custsession"), option);

            Customer = await dBContext.Customers.Include(o => o.Orders).ThenInclude(a => a.OrderDetails).ThenInclude(p => p.Product).SingleOrDefaultAsync(i => i.CustomerId == account.CustomerId);
            await hubContext.Clients.All.SendAsync("ReloadPage");

            return Page();
        }

    }
}
