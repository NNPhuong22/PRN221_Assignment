using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.UserPage
{
    public class CategoryProductModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public CategoryProductModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public List<Models.Product> products { get; set; }
        public List<Models.Category> categories { get; set; }
        [BindProperty]
        public int TotalPages { get; set; }
        [BindProperty]
        public int CurrentPage { get; set; }
        public int CateId { get; set; } 
        public string filter { get; set; }

        public void OnGet(int id)
        {
            categories = dBContext.Categories.ToList();
            filter = "Sort by";
            //paging
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Where(o=>o.CategoryId==id).ToList().Count(), 12));
            CateId = id;
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.Where(o => o.CategoryId == id).ToList().Skip(0).Take(12).ToList();
        }
        public IActionResult OnPost(int cateId, string filterSelected)
        {

            //asc
            if (filterSelected == "Sort by")
            {
                categories = dBContext.Categories.ToList();
                filter = "ASC";

                //paging
                CateId = cateId;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Count(), 12));
                CurrentPage = 1;
                products = dBContext.Products.Where(o => o.CategoryId == cateId).OrderBy(o=>o.UnitPrice).ToList().Skip(0).Take(12).ToList();
                return Page();
            }
            //desc
            else if (filterSelected == "ASC")
            {
                categories = dBContext.Categories.ToList();
                filter = "DESC";

                //paging
                CateId = cateId;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Count(), 12));
                CurrentPage = 1;
                products = dBContext.Products.Where(o => o.CategoryId == cateId).OrderByDescending(o => o.UnitPrice).ToList().Skip(0).Take(12).ToList();
                return Page();
            }
            categories = dBContext.Categories.ToList();
            filter = "Sort by";
            //paging
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Count(), 12));
            CateId = cateId;
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Skip(0).Take(12).ToList();
            return Page();
        }
        public IActionResult OnGetPaging(int id,int cateId)
        {
            categories = dBContext.Categories.ToList();

            CateId = cateId;
            CurrentPage = id;
            int skipProduct = (id - 1) * 12;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Count(), 12));
            products = dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Skip(skipProduct).Take(12).ToList();
            TotalPages = totalPages;
            return Page();
        }
        public IActionResult OnGetPagingNext(int id, int cateId)
        {
            categories = dBContext.Categories.ToList();
            CateId = cateId;

            CurrentPage = id + 1;
            int skipProduct = (id) * 12;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Count(), 12));
            products = dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Skip(skipProduct).Take(12).ToList();
            TotalPages = totalPages;
            return Page();
        }
        public IActionResult OnGetPagingPrevious(int id, int cateId)
        {
            categories = dBContext.Categories.ToList();
            CateId = cateId;

            CurrentPage = id - 1;
            int skipProduct = (id - 2) * 12;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Count(), 12));
            products = dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Skip(skipProduct).Take(12).ToList();
            TotalPages = totalPages;
            return Page();
        }
        public IActionResult OnGetAddProduct(int id, int cateId)
        {
            filter = "Sort by";

            CateId = cateId;

            categories = dBContext.Categories.ToList();
            //paging



            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Count(), 12));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.Where(o => o.CategoryId == cateId).ToList().Skip(0).Take(12).ToList();

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

            return Page();
        }
    }
}
