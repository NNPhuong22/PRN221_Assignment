using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using System.Net.WebSockets;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin.EmployeesControl
{
    public class AllEmployeeModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public AllEmployeeModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }
        [BindProperty]
        public int TotalPages { get; set; }
        [BindProperty]
        public int CurrentPage { get; set; }
        public List<MyRazorPage.Models.Employee> products { get; set; }
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
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Employees.OrderByDescending(o => o.EmployeeId).ToList().Skip(0).Take(10).ToList();
            return Page();
        }

        public IActionResult OnPostSearching()
        {
            string a = Request.Form["txtSearch"];
                //paging
                var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.Where(o => o.FirstName.Trim().ToLower().Contains(a.Trim().ToLower()) || o.LastName.Trim().ToLower().Contains(a.Trim().ToLower())).ToList().Count(), 10));
                TotalPages = totalPages1;
                CurrentPage = 1;
                products = dBContext.Employees.Where(o => o.FirstName.Trim().ToLower().Contains(a.Trim().ToLower()) || o.LastName.Trim().ToLower().Contains(a.Trim().ToLower())).OrderByDescending(o => o.EmployeeId).ToList().Skip(0).Take(10).ToList();
                search = a;

                return Page();
        }
        public IActionResult OnGetPaging(int id, string searching)
        {
            CurrentPage = id;
            int skipProduct = (id - 1) * 10;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.ToList().Count(), 10));
            products = dBContext.Employees.ToList();
            TotalPages = totalPages;
            if (searching != null)
            {
                search = searching;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.Where(o => o.FirstName.Trim().ToLower().Contains(searching.Trim().ToLower()) || o.LastName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                products = products.Where(o => o.FirstName.Trim().ToLower().Contains(searching.Trim().ToLower()) || o.LastName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList();
            }
            products = products.OrderByDescending(o => o.EmployeeId).Skip(skipProduct).Take(10).ToList();
            return Page();
        }
        public IActionResult OnGetPagingNext(int id, int filter, string searching)
        {
            CurrentPage = 1;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.ToList().Count(), 10));
            products = dBContext.Employees.ToList();
            TotalPages = totalPages;
            if (searching != null)
            {
                search = searching;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.Where(o => o.FirstName.Trim().ToLower().Contains(searching.Trim().ToLower()) || o.LastName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                products = products.Where(o => o.FirstName.Trim().ToLower().Contains(searching.Trim().ToLower()) || o.LastName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList();
            }
            products = products.OrderByDescending(o => o.EmployeeId).Skip(0).Take(10).ToList();
            return Page();
        }
        public IActionResult OnGetPagingPrevious(int id, string searching)
        {
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.ToList().Count(), 10));
            products = dBContext.Employees.ToList();
            int skipProduct = (totalPages - 1) * 10;
            CurrentPage = totalPages;
            TotalPages = totalPages;
            if (searching != null)
            {
                search = searching;
                TotalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.Where(o => o.FirstName.Trim().ToLower().Contains(searching.Trim().ToLower()) || o.LastName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList().Count(), 10));
                products = products.Where(o => o.FirstName.Trim().ToLower().Contains(searching.Trim().ToLower()) || o.LastName.Trim().ToLower().Contains(searching.Trim().ToLower())).ToList();
            }
            products = products.OrderByDescending(o => o.EmployeeId).Skip(skipProduct).Take(10).ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostDeleteEmployee(int id)
        {
            if (dBContext.Orders.ToList().Where(o => o.EmployeeId == id).Count() > 0)
            {
                var totalPages1 = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.ToList().Count(), 10));
                TotalPages = totalPages1;
                CurrentPage = 1;
                products = await dBContext.Employees.OrderByDescending(o => o.EmployeeId).Skip(0).Take(10).ToListAsync();
                alert = "This account has paticipated in order, you can not delete!";
                return Page();
            }
            var productDelete = await dBContext.Accounts.SingleOrDefaultAsync(o => o.EmployeeId == id);
            dBContext.Accounts.Remove(productDelete);
            dBContext.SaveChanges();

            var employeeDelete = await dBContext.Employees.SingleOrDefaultAsync(o => o.EmployeeId == id);
            dBContext.Employees.Remove(employeeDelete);
            dBContext.SaveChanges();


            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Employees.ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            products = dBContext.Employees.OrderByDescending(o => o.EmployeeId).ToList().Skip(0).Take(10).ToList();

            alert = "Delete successful!";


            await hubContext.Clients.All.SendAsync("ReloadPage");
            return Page();
        }
    }
}
