using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin.EmployeesControl
{
    public class EditEmployeeModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public EditEmployeeModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }
        [BindProperty]
        public EmployeeEdit EmployeeEdit { get; set; }
        public int emId { get; set; }   

        public void OnGet(int id)
        {
            Employee employee = dBContext.Employees.SingleOrDefault(o => o.EmployeeId == id);
            EmployeeEdit = new EmployeeEdit
            {
                Address = employee.Address,
                BirthDate = employee.BirthDate,
                FirstName = employee.FirstName,
                HireDate = employee.HireDate,
                LastName = employee.LastName,
                Title = employee.Title,
                TitleOfCourtesy = employee.TitleOfCourtesy,
            };
            emId = id;
        }
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                int EmEditId1 = Convert.ToInt32(Request.Form["emId"]);
                emId = EmEditId1;
                return Page();
            }
            int EmEditId = Convert.ToInt32(Request.Form["emId"]);
            var employee = await dBContext.Employees.SingleOrDefaultAsync(o => o.EmployeeId == EmEditId);

            employee.Address = EmployeeEdit.Address;
            employee.BirthDate = EmployeeEdit.BirthDate;
            employee.FirstName = EmployeeEdit.FirstName;
            employee.HireDate = EmployeeEdit.HireDate;
            employee.LastName = EmployeeEdit.LastName;
            employee.Title = EmployeeEdit.Title;
            employee.TitleOfCourtesy = EmployeeEdit.TitleOfCourtesy;
            dBContext.SaveChanges();

            await hubContext.Clients.All.SendAsync("ReloadPage");
            return RedirectToPage("/Admin/EmployeesControl/AllEmployee");
        }
    }
}
