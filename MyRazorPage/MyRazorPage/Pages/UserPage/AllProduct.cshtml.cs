using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.UserPage
{
    public class AllProductModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public AllProductModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public List<Models.Product> products { get; set; }
        public List<Models.Category> categories { get; set; }
        [BindProperty]
        public int TotalPages { get; set; }
        [BindProperty]
        public int CurrentPage { get; set; }
        public string filterSelection { get; set; }

        public void OnGet()
        {
            categories = dBContext.Categories.ToList();

            //paging
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.ToList().Count(), 12));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.ToList().Skip(0).Take(12).ToList();
        }
        public IActionResult OnPost()
        {
            var filter = Request.Form["filter"];
            filterSelection = filter;
            //asc
            if (filter.ToString() == "1")
            {
                categories = dBContext.Categories.ToList();

                //paging
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.OrderBy(o => o.UnitPrice).ToList().Count(), 12));
                CurrentPage = 1;
                products = dBContext.Products.OrderBy(o => o.UnitPrice).ToList().Skip(0).Take(12).ToList();
                return Page();
            }
            //desc
            else if (filter.ToString() == "2")
            {
                categories = dBContext.Categories.ToList();

                //paging
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.OrderByDescending(o => o.UnitPrice).ToList().Count(), 12));
                CurrentPage = 1;
                products = dBContext.Products.OrderByDescending(o => o.UnitPrice).ToList().Skip(0).Take(12).ToList();
                return Page();
            }
            categories = dBContext.Categories.ToList();

            //paging
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.ToList().Count(), 12));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.ToList().Skip(0).Take(12).ToList();
            return Page();
        }
        public IActionResult OnGetPaging(int id,string filter)
        {
            categories = dBContext.Categories.ToList();

            filterSelection = filter;
            if (filter!= "")
            {
                if (filter.ToString() == "1")
                {
                    categories = dBContext.Categories.ToList();

                    //paging
                    CurrentPage = id;
                    int skipProduct1 = (id - 1) * 12;
                    var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.ToList().Count(), 12));
                    products = dBContext.Products.OrderBy(o => o.UnitPrice).ToList().Skip(skipProduct1).Take(12).ToList();
                    TotalPages = totalPages1;
                    return Page();
                }
                //desc
                else if (filter.ToString() == "2")
                {
                    categories = dBContext.Categories.ToList();

                    //paging
                    CurrentPage = id;
                    int skipProduct1 = (id - 1) * 12;
                    var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.ToList().Count(), 12));
                    products = dBContext.Products.OrderByDescending(o => o.UnitPrice).ToList().Skip(skipProduct1).Take(12).ToList();
                    TotalPages = totalPages1;
                    return Page();
                }
            }

            CurrentPage = id;
            int skipProduct = (id - 1) * 12;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.ToList().Count(), 12));
            products = dBContext.Products.ToList().Skip(skipProduct).Take(12).ToList();
            TotalPages = totalPages;
            return Page();
        }
        public IActionResult OnGetPagingNext(int id)
        {
            categories = dBContext.Categories.ToList();

            CurrentPage = id + 1;
            int skipProduct = (id) * 12;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.ToList().Count(), 12));
            products = dBContext.Products.ToList().Skip(skipProduct).Take(12).ToList();
            TotalPages = totalPages;
            return Page();
        }
        public IActionResult OnGetPagingPrevious(int id)
        {
            categories = dBContext.Categories.ToList();

            CurrentPage = id - 1;
            int skipProduct = (id - 2) * 12;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.ToList().Count(), 12));
            products = dBContext.Products.ToList().Skip(skipProduct).Take(12).ToList();
            TotalPages = totalPages;
            return Page();
        }
        public IActionResult OnGetAddProduct(int id)
        {
            if (HttpContext.Session.GetString("Custsession") == null)
            {
                return RedirectToPage("/Account/SignIn");
            }
            categories = dBContext.Categories.ToList();
            //paging
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.ToList().Count(), 12));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.ToList().Skip(0).Take(12).ToList();

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
                if (check !=null)
                {
                    check.Quantity+=1;
                    check.Price += addedProduct.UnitPrice;
                }
                //cart chưa có => add mới 
                else
                {
                    cart.Add(new CartSession { ProductId = id,Price=addedProduct.UnitPrice,Quantity=1, ProductName = addedProduct.ProductName });
                }
                HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(cart));

            }
            else
            {
                List<Models.CartSession> newCart = new List<Models.CartSession>();
                newCart.Add(new CartSession { ProductId= id,Price=addedProduct.UnitPrice,Quantity=1, ProductName=addedProduct.ProductName});
                HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(newCart));
            }

            return Page();
        }
    }
}
