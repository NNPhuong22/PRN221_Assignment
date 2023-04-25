using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin.EmployeesControl
{
    public class CreateEmployeeModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public CreateEmployeeModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }
        [BindProperty]
        public EmployeeEdit EmployeeEdit { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
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
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Employee employee = new Employee();

            employee.Address = EmployeeEdit.Address;
            employee.BirthDate = EmployeeEdit.BirthDate;
            employee.FirstName = EmployeeEdit.FirstName;
            employee.HireDate = EmployeeEdit.HireDate;
            employee.LastName = EmployeeEdit.LastName;
            employee.Title = EmployeeEdit.Title;
            employee.TitleOfCourtesy = EmployeeEdit.TitleOfCourtesy;
            await dBContext.Employees.AddAsync(employee);
            dBContext.SaveChanges();

            await hubContext.Clients.All.SendAsync("ReloadPage");
            return RedirectToPage("/Admin/EmployeesControl/AllEmployee");
        }
    }
}
