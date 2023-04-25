using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin.Product
{
    public class UpdateProductModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public UpdateProductModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }
        [BindProperty]
        public MyRazorPage.Models.ProductAdd product { get; set; }
        public List<MyRazorPage.Models.Category> categories { get; set; }
        public int ReorderLevel { get; set; }
        public int UnitOnOrder { get; set; }
        public int proId { get; set; }

        [BindProperty]

        public bool Discontinue { get; set; }

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
            //

            proId = id;
            Discontinue = false;
            product = new MyRazorPage.Models.ProductAdd();
            var editPro = dBContext.Products.ToList().SingleOrDefault(o => o.ProductId == id);
            product.ProductName = editPro.ProductName;
            product.CategoryId = editPro.CategoryId;
            product.QuantityPerUnit = editPro.ProductName;
            product.UnitPrice = editPro.UnitPrice;
            product.UnitsInStock = editPro.UnitsInStock;
            Discontinue = editPro.Discontinued;


            //reorder & unit on order level
            ReorderLevel = 0;
            UnitOnOrder = 0;

            foreach (var item in dBContext.OrderDetails.ToList())
            {
                if (item.ProductId == id)
                {
                    ReorderLevel++;
                    UnitOnOrder += item.Quantity;
                }
            }


            categories = dBContext.Categories.ToList();
            return Page();
        }
        public async Task<IActionResult> OnPost(int id)
        {
            if (!ModelState.IsValid)
            {
                proId = id;
                categories = await dBContext.Categories.ToListAsync();
                return Page();

            }
            MyRazorPage.Models.Product edited = dBContext.Products.ToList().SingleOrDefault(o => o.ProductId == id);

            var check = Request.Form["chkDiscontinued"];

            if (check.ToString()=="")
            {
                edited.Discontinued = false;
            }
            else
            {
                edited.Discontinued = true;
            }

            edited.CategoryId = Convert.ToInt32(Request.Form["ddlCategory"]);
            edited.ProductName = product.ProductName;
            edited.UnitPrice = product.UnitPrice;
            edited.QuantityPerUnit = product.QuantityPerUnit;
            edited.UnitsInStock = product.UnitsInStock;
            dBContext.SaveChanges();

            await hubContext.Clients.All.SendAsync("ReloadPage");
            return RedirectToPage("/Admin/Product/AllProduct");

        }
    }
}
