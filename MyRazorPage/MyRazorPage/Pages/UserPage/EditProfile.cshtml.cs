using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.UserPage
{
    public class EditProfileModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public EditProfileModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Models.Account Account { get; set; }
        [BindProperty]
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
            Account = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(HttpContext.Session.GetString("Custsession"), option);
            Customer = dBContext.Customers.SingleOrDefault(o => o.CustomerId == Account.CustomerId);
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var a = await dBContext.Customers.SingleOrDefaultAsync(o=>o.CustomerId == Customer.CustomerId);
                a.ContactTitle = Customer.ContactTitle;
                a.ContactName = Customer.ContactName;
                a.Address = Customer.Address;
                a.CompanyName = Customer.CompanyName;
                await dBContext.SaveChangesAsync();
                return RedirectToPage("/UserPage/Profile");
            }
            else
            {
                return Page();
            }
        }

    }
}
