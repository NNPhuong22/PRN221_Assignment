using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.HubCustom;
using MyRazorPage.Models;
using OfficeOpenXml;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin.Order
{
    public class AllOrderModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public AllOrderModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;
        }
        [BindProperty]
        public int TotalPages { get; set; }
        [BindProperty]
        public int CurrentPage { get; set; }

        public List<MyRazorPage.Models.Order> orders { get; set; }
        public string exportOrder { get; set; }
        public DateTime FromDate { get; set; } 
        public DateTime ToDate { get; set; }

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

            exportOrder = JsonSerializer.Serialize(dBContext.Orders.ToList());
            HttpContext.Session.SetString("ExportOrder", exportOrder);


            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            orders = dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).OrderByDescending(o => o.OrderId).ToList().Skip(0).Take(10).ToList();
            return Page();
        }
        public IActionResult OnPost()
        {
            var from = Convert.ToDateTime(Request.Form["txtStartOrderDate"]);
            var to = Convert.ToDateTime(Request.Form["txtEndOrderDate"]);
            FromDate = from;
            ToDate = to;
            HttpContext.Session.Remove("ExportOrder");
            HttpContext.Session.SetString("ExportOrder", JsonSerializer.Serialize(dBContext.Orders.OrderByDescending(o => o.OrderId).Where(o => o.OrderDate >= from && o.OrderDate <= to).ToList()));
            var totalPages = 1;
            TotalPages = totalPages;
            CurrentPage = 1;
            orders = dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).OrderByDescending(o => o.OrderId).Where(o => o.OrderDate >= from && o.OrderDate <= to).ToList();


            return Page();
        }

        public IActionResult OnGetPaging(int id, int cateId)
        {

            CurrentPage = id;
            int skipProduct = (id - 1) * 10;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).ToList().Count(), 10));
            orders = dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).OrderByDescending(o => o.OrderId).ToList().Skip(skipProduct).Take(10).ToList();
            TotalPages = totalPages;


            return Page();
        }
        public IActionResult OnGetPagingNext(int id, int cateId)
        {
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).ToList().Count(), 10));
            CurrentPage = totalPages;
            orders = dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).OrderByDescending(o => o.OrderId).ToList().Skip(totalPages).Take(10).ToList();
            TotalPages = totalPages;


            return Page();
        }
        public IActionResult OnGetPagingPrevious(int id, int cateId)
        {
            CurrentPage = 1;
            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).ToList().Count(), 10));
            orders = dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).OrderByDescending(o => o.OrderId).ToList().Skip(0).Take(10).ToList();
            TotalPages = totalPages;

            return Page();
        }

        public async Task<IActionResult> OnGetCancelOrder(int id)
        {
            var orderCancel = dBContext.Orders.ToList().SingleOrDefault(o => o.OrderId == id);
            orderCancel.RequiredDate = null;
            dBContext.SaveChanges();

            var totalPages = (int)Math.Ceiling(Decimal.Divide(dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).ToList().Count(), 10));
            TotalPages = totalPages;
            CurrentPage = 1;
            orders = dBContext.Orders.Include(o => o.Employee).Include(o => o.Customer).OrderByDescending(o => o.OrderId).ToList().Skip(0).Take(10).ToList();
            await hubContext.Clients.All.SendAsync("ReloadPage");



            return Page();
        }
        public FileResult OnGetExport(string from, string to)
        {

            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            var account = JsonSerializer.Deserialize<List<MyRazorPage.Models.Order>>(HttpContext.Session.GetString("ExportOrder"), option);

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet =
                    workbook.Worksheets.Add("Orders");
                worksheet.Cell(2, 1).Value = "Order report";
                if (from !=null && to!=null)
                {
                    worksheet.Cell(3, 1).Value = "From: "+from;
                    worksheet.Cell(4, 1).Value = "To: "+to;

                }
                worksheet.Cell(5, 1).Value = "OrderId";
                worksheet.Cell(5, 2).Value = "OrderDate";
                worksheet.Cell(5, 3).Value = "RequiredDate";
                worksheet.Cell(5, 4).Value = "ShippedDate";
                worksheet.Cell(5, 5).Value = "Freight";
                worksheet.Cell(5, 6).Value = "ShipName";
                worksheet.Cell(5, 7).Value = "ShipAddress";

                IXLRange range = worksheet.Range(worksheet.Cell(1, 1).Address, worksheet.Cell(1, 7).Address);
                range.Style.Fill.SetBackgroundColor(XLColor.Almond);

                int index = 6;

                foreach (var item in account)
                {
                    index++;

                    worksheet.Cell(index, 1).Value = item.OrderId;
                    worksheet.Cell(index, 2).Value = item.OrderDate;
                    worksheet.Cell(index, 3).Value = item.RequiredDate;
                    worksheet.Cell(index, 4).Value = item.ShippedDate;
                    worksheet.Cell(index, 5).Value = item.Freight;
                    worksheet.Cell(index, 6).Value = item.ShipName;
                    worksheet.Cell(index, 7).Value = item.ShipAddress;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    var strDate = DateTime.Now.ToString("dd/MM/yyyy");
                    string fileName = string.Format($"OrdersReport_{strDate}.xlsx");
                    return File(content, contentType, fileName);

                }
            }
        }
    }
}
