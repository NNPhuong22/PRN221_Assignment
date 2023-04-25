using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.UserPage
{
    public class ProfileModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public ProfileModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public Models.Account Account { get; set; }
        [BindProperty]
        public Customer Customer { get; set; }
        public string isEdited { get; set; }
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
                return RedirectToPage("/ErrorPage");
            }
            Account = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(HttpContext.Session.GetString("Custsession"), option);
            Customer = dBContext.Customers.SingleOrDefault(o => o.CustomerId == Account.CustomerId);
            return Page();
        }
        public IActionResult OnGetEditPress()
        {
            isEdited = "abc";
            return Page();
        }
        public IActionResult OnPostEditProfile()
        {
            var a = Customer;

            return Page();
        }
    }
}
