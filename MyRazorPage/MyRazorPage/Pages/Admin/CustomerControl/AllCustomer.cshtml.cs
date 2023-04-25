using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin.CustomerControl
{
    public class AllCustomerModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public AllCustomerModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }
        [BindProperty]
        public int TotalPages { get; set; }
        [BindProperty]
        public int CurrentPage { get; set; }
        public List<MyRazorPage.Models.Customer> products { get; set; }
        public string filterSelected { get; set; }
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


            //paging
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Customers.OrderByDescending(o => o.CreateDate).ToList().Skip(0).Take(10).ToList();
            return Page();
        }

        public IActionResult OnPostSearching(string filter)
        {
            string a = Request.Form["txtSearch"];
            if (filter == null || filter == "0")
            {
                filterSelected = filter;
                //paging
                var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).Where(o => o.ProductName.Trim().ToLower().Contains(a.Trim().ToLower())).ToList().Count(), 10));
                TotalPages = totalPages1;
                CurrentPage = 1;
                products = dBContext.Customers.Where(o => o.ContactName.Trim().ToLower().Contains(a.Trim().ToLower())).OrderByDescending(o => o.CreateDate).ToList().Skip(0).Take(10).ToList();
                search = a;

                return Page();
            }

            filterSelected = filter;
            search = a;
            //paging
            if (filter == "Active")
            {
                var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.ContactName.Trim().ToLower().Contains(a.Trim().ToLower())).Where(o => o.Address != "1").ToList().Count(), 10));
                TotalPages = totalPages;
                CurrentPage = 1;
                products = dBContext.Customers.Where(o => o.ContactName.Trim().ToLower().Contains(a.Trim().ToLower())).OrderByDescending(o => o.CreateDate).Where(o => o.Address != "1").ToList().Skip(0).Take(10).ToList();
            }
            else if (filter == "Deactive")
            {
                var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.ContactName.Trim().ToLower().Contains(a.Trim().ToLower())).Where(o => o.Address == "1").ToList().Count(), 10));
                TotalPages = totalPages;
                CurrentPage = 1;
                products = dBContext.Customers.Where(o => o.ContactName.Trim().ToLower().Contains(a.Trim().ToLower())).OrderByDescending(o => o.CreateDate).Where(o => o.Address == "1").ToList().Skip(0).Take(10).ToList();

            }
            return Page();
        }
        public IActionResult OnPostFiltering(string searching)
        {
            string a = Request.Form["ddlCategory"];
            if (searching == null)
            {
                if (a == "Active")
                {
                    filterSelected = a;
                    //paging
                    var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address != "1").ToList().Count(), 10));
                    TotalPages = totalPages1;
                    CurrentPage = 1;
                    products = dBContext.Customers.Where(o => o.Address != "1").OrderByDescending(o => o.CreateDate).ToList().Skip(0).Take(10).ToList();
                    return Page();
                }
                else if (a == "Deactive")
                {
                    filterSelected = a;
                    //paging
                    var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address == "1").ToList().Count(), 10));
                    TotalPages = totalPages1;
                    CurrentPage = 1;
                    products = dBContext.Customers.Where(o => o.Address == "1").OrderByDescending(o => o.CreateDate).ToList().Skip(0).Take(10).ToList();
                    return Page();
                }

            }
            if (a == "Active")
            {
                search = searching;
                filterSelected = a;
                //paging
                var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address != "1").Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                TotalPages = totalPages;
                CurrentPage = 1;
                products = dBContext.Customers.Where(o => o.Address != "1").Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).OrderByDescending(o => o.CreateDate).ToList().Skip(0).Take(10).ToList();
                return Page();
            }
            else if (a == "Deactive")
            {
                search = searching;
                filterSelected = a;
                //paging
                var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address == "1").Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                TotalPages = totalPages;
                CurrentPage = 1;
                products = dBContext.Customers.Where(o => o.Address == "1").Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).OrderByDescending(o => o.CreateDate).ToList().Skip(0).Take(10).ToList();
                return Page();
            }
            return Page();
        }
        public IActionResult OnGetPaging(int id, string filter, string searching)
        {
            CurrentPage = id;
            int skipProduct = (id - 1) * 10;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.ToList().Count(), 10));
            products = dBContext.Customers.ToList();
            TotalPages = totalPages;
            if (filter != "0")
            {
                if (filter == "Active")
                {
                    filterSelected = filter;
                    products = products.Where(o => o.Address != "1").ToList();
                    TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address != "1").ToList().Count(), 10));
                }
                else if (filter == "Deactive")
                {
                    filterSelected = filter;
                    products = products.Where(o => o.Address == "1").ToList();
                    TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address == "1").ToList().Count(), 10));
                }

            }
            if (searching != null)
            {
                search = searching;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                products = products.Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList();
            }
            products = products.OrderByDescending(o => o.CreateDate).Skip(skipProduct).Take(10).ToList();
            return Page();
        }
        public IActionResult OnGetPagingNext(int id, string filter, string searching)
        {
            CurrentPage = 1;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.ToList().Count(), 10));
            products = dBContext.Customers.ToList();
            TotalPages = totalPages;
            if (filter != "0")
            {
                if (filter == "Active")
                {
                    filterSelected = filter;
                    products = products.Where(o => o.Address != "1").ToList();
                    TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address != "1").ToList().Count(), 10));
                }
                else if (filter == "Deactive")
                {
                    filterSelected = filter;
                    products = products.Where(o => o.Address == "1").ToList();
                    TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address == "1").ToList().Count(), 10));
                }
            }
            if (searching != null)
            {
                search = searching;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                products = products.Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList();
            }
            products = products.OrderByDescending(o => o.CreateDate).Skip(0).Take(10).ToList();
            return Page();
        }
        public IActionResult OnGetPagingPrevious(int id, string filter, string searching)
        {
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Products.Include(o => o.Category).ToList().Count(), 10));
            products = dBContext.Customers.ToList();
            int skipProduct = (totalPages - 1) * 10;
            CurrentPage = totalPages;
            TotalPages = totalPages;
            if (filter != "0")
            {
                if (filter == "Active")
                {
                    filterSelected = filter;
                    products = products.Where(o => o.Address != "1").ToList();
                    TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address != "1").ToList().Count(), 10));
                }
                else if (filter == "Deactive")
                {
                    filterSelected = filter;
                    products = products.Where(o => o.Address == "1").ToList();
                    TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.Address == "1").ToList().Count(), 10));
                }
            }
            if (searching != null)
            {
                search = searching;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Customers.Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                products = products.Where(o => o.ContactName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList();
            }
            products = products.OrderByDescending(o => o.CreateDate).Skip(skipProduct).Take(10).ToList();
            return Page();
        }
        public async Task<IActionResult> OnGetActiveCustomer(string id)
        {
            var listCustomer = await dBContext.Customers.SingleOrDefaultAsync(o => o.CustomerId == id);
            listCustomer.Address = "Ha Noi";
            dBContext.SaveChanges();

            await hubContext.Clients.All.SendAsync("ReloadPage");
            return RedirectToPage("/Admin/CustomerControl/AllCustomer");

        }

        public async Task<IActionResult> OnGetDeactiveCustomer(string id)
        {
            var listCustomer = await dBContext.Customers.SingleOrDefaultAsync(o => o.CustomerId == id);
            listCustomer.Address = "1";
            dBContext.SaveChanges();


            await hubContext.Clients.All.SendAsync("ReloadPage");
            return RedirectToPage("/Admin/CustomerControl/AllCustomer");
        }

    }
}