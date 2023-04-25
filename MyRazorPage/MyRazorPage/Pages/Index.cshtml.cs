using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Reflection;
using System.Text.Json;

namespace MyRazorPage.Pages
{
    public class IndexModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public IndexModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public List<Models.OrderDetail> hotProduct { get; set; }
        public List<Models.Product> newProduct { get; set; }
        public List<Models.Category> categories { get; set; }

        public void OnGet()
        {
            var a = dBContext.OrderDetails.Include(o=>o.Product).OrderByDescending(a=>a.Quantity).ToList().DistinctBy(o => o.ProductId).ToList();
            hotProduct = a;
            newProduct = dBContext.Products.OrderByDescending(o => o.ProductId).Take(4).ToList();
            categories = dBContext.Categories.ToList();
        }
        public IActionResult OnGetAddProduct(int id)
        {
            
                categories = dBContext.Categories.ToList();

            var addedProduct = dBContext.Products.SingleOrDefault(o => o.ProductId == id);
            if (HttpContext.Session.GetString("CartList") != null)
            {
                var option = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    WriteIndented = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                List<Models.CartSession> cart = JsonSerializer.Deserialize<List<MyRazorPage.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);

                var check = cart.SingleOrDefault(o => o.ProductId == id);

                //nếu cart đã có sẵn sản phẩm => thêm quantity
                if (check != null)
                {
                    check.Quantity += 1;
                    check.Price += addedProduct.UnitPrice;
                }
                //cart chưa có => add mới 
                else
                {
                    cart.Add(new CartSession { ProductId = id, Price = addedProduct.UnitPrice, Quantity = 1, ProductName = addedProduct.ProductName });
                }
                HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(cart));

            }
            else
            {
                List<Models.CartSession> newCart = new List<Models.CartSession>();
                newCart.Add(new CartSession { ProductId = id, Price = addedProduct.UnitPrice, Quantity = 1, ProductName = addedProduct.ProductName });
                HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(newCart));
            }
            var a = dBContext.OrderDetails.Include(o => o.Product).OrderByDescending(a => a.Quantity).ToList().DistinctBy(o => o.ProductId).ToList();
            hotProduct = a;
            newProduct = dBContext.Products.OrderByDescending(o => o.ProductId).Take(4).ToList();
            return Page();
        }
    }
}
