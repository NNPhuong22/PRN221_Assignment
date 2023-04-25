using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using OfficeOpenXml;
using System.Text.Json;
using static NuGet.Packaging.PackagingConstants;

namespace MyRazorPage.Pages.Admin.Product
{
    public class AllProductModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public AllProductModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }
        [BindProperty]
        public int TotalPages { get; set; }
        [BindProperty]
        public int CurrentPage { get; set; }
        public List<MyRazorPage.Models.Product> products { get; set; }
        public List<MyRazorPage.Models.Category> categories { get; set; }
        public int filterSelected { get; set; }
        public string search { get; set; }
        public string alert { get; set; }


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
            categories = dBContext.Categories.ToList();


            //paging
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.Include(o => o.Category).OrderByDescending(o => o.ProductId).ToList().Skip(0).Take(10).ToList();
            return Page();
        }

        public IActionResult OnPostSearching(int filter)
        {
            string a = Request.Form["txtSearch"];
            if (filter == null || filter == 0)
            {
                filterSelected = filter;
                categories = dBContext.Categories.ToList();
                //paging
                var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.CategoryId == filter).Where(o => o.ProductName.Trim().ToLower().Contains(a.Trim().ToLower())).ToList().Count(), 10));
                TotalPages = totalPages1;
                CurrentPage = 1;
                products = dBContext.Products.Include(o => o.Category).Where(o => o.ProductName.Trim().ToLower().Contains(a.Trim().ToLower())).OrderByDescending(o => o.ProductId).ToList().Skip(0).Take(10).ToList();
                search = a;

                return Page();
            }

            filterSelected = filter;
            categories = dBContext.Categories.ToList();
            search = a;
            //paging
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.ProductName.Trim().ToLower().Contains(a.Trim().ToLower())).ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.Include(o => o.Category).Where(o => o.ProductName.Trim().ToLower().Contains(a.Trim().ToLower())).OrderByDescending(o => o.ProductId).ToList().Skip(0).Take(10).ToList();
            return Page();
        }
        public IActionResult OnPostFiltering(string searching)
        {
            int a = Convert.ToInt32(Request.Form["ddlCategory"]);
            if (searching == null)
            {
                filterSelected = a;
                categories = dBContext.Categories.ToList();
                //paging
                var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.CategoryId == a).ToList().Count(), 10));
                TotalPages = totalPages1;
                CurrentPage = 1;
                products = dBContext.Products.Include(o => o.Category).Where(o => o.CategoryId == a).OrderByDescending(o => o.ProductId).ToList().Skip(0).Take(10).ToList();
                return Page();
            }
            search = searching;
            filterSelected = a;
            categories = dBContext.Categories.ToList();
            //paging
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.CategoryId == a).Where(o => o.ProductName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.Include(o => o.Category).Where(o => o.CategoryId == a).Where(o => o.ProductName.Trim().ToLower().Contains(searching.Trim().ToLower())).OrderByDescending(o => o.ProductId).ToList().Skip(0).Take(10).ToList();
            return Page();
        }
        public IActionResult OnGetPaging(int id, int filter, string searching)
        {
            categories = dBContext.Categories.ToList();

            CurrentPage = id;
            int skipProduct = (id - 1) * 10;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).ToList().Count(), 10));
            products = dBContext.Products.Include(o => o.Category).ToList();
            TotalPages = totalPages;
            if (filter != 0)
            {
                filterSelected = filter;
                products = products.Where(o => o.CategoryId == filter).ToList();
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.CategoryId == filter).ToList().Count(), 10));
            }
            if (searching != null)
            {
                search = searching;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.ProductName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                products = products.Where(o => o.ProductName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList();
            }
            products = products.OrderByDescending(o => o.ProductId).Skip(skipProduct).Take(10).ToList();
            return Page();
        }
        public IActionResult OnGetPagingNext(int id, int filter, string searching)
        {
            categories = dBContext.Categories.ToList();

            CurrentPage = 1;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).ToList().Count(), 10));
            products = dBContext.Products.Include(o => o.Category).ToList();
            TotalPages = totalPages;
            if (filter != 0)
            {
                filterSelected = filter;
                products = products.Where(o => o.CategoryId == filter).ToList();
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.CategoryId == filter).ToList().Count(), 10));
            }
            if (searching != null)
            {
                search = searching;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.ProductName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                products = products.Where(o => o.ProductName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList();
            }
            products = products.OrderByDescending(o => o.ProductId).Skip(0).Take(10).ToList();
            return Page();
        }
        public IActionResult OnGetPagingPrevious(int id, int filter, string searching)
        {
            categories = dBContext.Categories.ToList();

            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).ToList().Count(), 10));
            products = dBContext.Products.Include(o => o.Category).ToList();
            int skipProduct = (totalPages - 1) * 10;
            CurrentPage = totalPages;
            TotalPages = totalPages;
            if (filter != 0)
            {
                filterSelected = filter;
                products = products.Where(o => o.CategoryId == filter).ToList();
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.CategoryId == filter).ToList().Count(), 10));
            }
            if (searching != null)
            {
                search = searching;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.ProductName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                products = products.Where(o => o.ProductName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList();
            }
            products = products.OrderByDescending(o => o.ProductId).Skip(skipProduct).Take(10).ToList();
            return Page();
        }
        public async Task<IActionResult> OnGetDeleteProduct(int id)
        {
            if (dBContext.OrderDetails.ToList().Where(o => o.ProductId == id).Count() > 0)
            {
                categories = await dBContext.Categories.ToListAsync();

                var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).ToList().Count(), 10));
                TotalPages = totalPages1;
                CurrentPage = 1;
                products = dBContext.Products.Include(o => o.Category).OrderByDescending(o => o.ProductId).ToList().Skip(0).Take(10).ToList();
                alert = "This product is already used, you can not delete!";
                return Page();
            }
            var productDelete = await dBContext.Products.SingleOrDefaultAsync(o => o.ProductId == id);
            dBContext.Products.Remove(productDelete);
            dBContext.SaveChanges();

            categories = await dBContext.Categories.ToListAsync();

            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.Include(o => o.Category).OrderByDescending(o => o.ProductId).ToList().Skip(0).Take(10).ToList();


            await hubContext.Clients.All.SendAsync("ReloadPage");
            return Page();
        }
        public IActionResult OnGetExport()
        {
            List<MyRazorPage.Models.Product> products = dBContext.Products.ToList();
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                    workbook.Worksheets.Add("Products");
                worksheet.Cell(2, 1).Value = "Products report";

                worksheet.Cell(5, 1).Value = "Product Id";
                worksheet.Cell(5, 2).Value = "Product Name";
                worksheet.Cell(5, 3).Value = "CategoryId";
                worksheet.Cell(5, 4).Value = "Quantity Per Unit";
                worksheet.Cell(5, 5).Value = "Unit Price";
                worksheet.Cell(5, 6).Value = "Units In Stock";
                worksheet.Cell(5, 7).Value = "Units On Order";

                IXLRange range = worksheet.Range(worksheet.Cell(1, 1).Address, worksheet.Cell(1, 7).Address);
                range.Style.Fill.SetBackgroundColor(XLColor.Almond);

                int index = 6;

                foreach (var item in products)
                {
                    index++;

                    worksheet.Cell(index, 1).Value = item.ProductId;
                    worksheet.Cell(index, 2).Value = item.ProductName;
                    worksheet.Cell(index, 3).Value = item.CategoryId;
                    worksheet.Cell(index, 4).Value = item.QuantityPerUnit;
                    worksheet.Cell(index, 5).Value = item.UnitPrice;
                    worksheet.Cell(index, 6).Value = item.UnitsInStock;
                    worksheet.Cell(index, 7).Value = item.UnitsOnOrder;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var strDate = DateTime.Now.ToString("dd/MM/yyyy");
                    string fileName = string.Format($"ProductsReport_{strDate}.xlsx");
                    return File(content, contentType, fileName);

                }
            }
        }

        public IActionResult OnPostImport(IFormFile file)
        {
            //lisence 
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        for (int row = 7; row <= rowcount; row++)
                        {
                            dBContext.Products.Add(new Models.Product
                            {
                                ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                CategoryId = Convert.ToInt32(worksheet.Cells[row, 3].Value.ToString().Trim()),
                                QuantityPerUnit = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                UnitPrice = Convert.ToDecimal(worksheet.Cells[row, 5].Value.ToString().Trim()),
                                UnitsInStock = Convert.ToInt16(worksheet.Cells[row, 6].Value.ToString().Trim()),
                                UnitsOnOrder = Convert.ToInt16(worksheet.Cells[row, 7].Value.ToString().Trim()),
                                Discontinued = false,
                            });
                            dBContext.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception)
            {
                categories = dBContext.Categories.ToList();
                var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).ToList().Count(), 10));
                TotalPages = totalPages1;
                CurrentPage = 1;
                products = dBContext.Products.Include(o => o.Category).OrderByDescending(o => o.ProductId).ToList().Skip(0).Take(10).ToList();
                alert = "File import is invalid, check again!";

                return Page();
            }

            categories = dBContext.Categories.ToList();
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Products.Include(o => o.Category).OrderByDescending(o => o.ProductId).ToList().Skip(0).Take(10).ToList();

            return Page();
        }
    }
}
