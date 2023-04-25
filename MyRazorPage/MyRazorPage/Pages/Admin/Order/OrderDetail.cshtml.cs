using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin.Order
{
    public class OrderDetailModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public OrderDetailModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public MyRazorPage.Models.Order order { get; set; }
        public List<MyRazorPage.Models.OrderDetail> orderDetail { get; set; }
        public IActionResult OnGet(int id)
        {
            // Authentication
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            if (HttpContext.Session.GetString("Custsession") == null)
            {
                return RedirectToPage("/ErrorPage");
            }
            else
            {
                var account = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(HttpContext.Session.GetString("Custsession"), option);
                if (account.Role == 2)
                {
                    return RedirectToPage("/ErrorPage");

                }
            }
            order = dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).ToList().SingleOrDefault(o => o.OrderId == id);
            orderDetail = dBContext.OrderDetails.Include(o => o.Product).ToList().Where(o => o.OrderId == id).ToList();
            return Page();
        }
    }
}
