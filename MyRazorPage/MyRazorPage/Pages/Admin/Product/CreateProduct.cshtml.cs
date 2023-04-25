using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin.Product
{
    public class CreateProductModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public CreateProductModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }
        [BindProperty]
        public MyRazorPage.Models.ProductAdd product { get; set; }

        public List<MyRazorPage.Models.Category> categories { get; set; }

        public IActionResult OnGet()
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
            //

            categories = dBContext.Categories.ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            product.CategoryId = Convert.ToInt32(Request.Form["ddlCategory"]);
            if (!ModelState.IsValid)
            {
                categories = dBContext.Categories.ToList();
                return Page();

            }
            MyRazorPage.Models.Product newProduct = new Models.Product
            {
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                Discontinued = false,
                QuantityPerUnit = product.QuantityPerUnit,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
            };
            dBContext.Products.Add(newProduct);
            await dBContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("ReloadProduct", await dBContext.Products.OrderByDescending(o=>o.ProductId).ToListAsync());
            return RedirectToPage("/Admin/Product/AllProduct");

        }
    }
}
