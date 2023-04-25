using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Text.Json;

namespace MyRazorPage.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public DashboardModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public int Jan { get; set; }
        public int Feb { get; set; }
        public int Mar { get; set; }
        public int Apr { get; set; }
        public int May { get; set; }
        public int Jun { get; set; }
        public int Jul { get; set; }
        public int Aug { get; set; }
        public int Sep { get; set; }
        public int Oct { get; set; }
        public int Nov { get; set; }

        public int Dec { get; set; }



        public decimal? weeklySale { get; set; }
        public decimal? totalSale { get; set; }
        public int totalCustomer { get; set; }
        public int totalGuest { get; set; }
        public int newGuest { get; set; }   
        public List<YearFilter> listYear { get; set; }
        public class YearFilter
        {
            public int id { get; set; }
            public int value { get; set; }
        }
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
            listYear = new List<YearFilter>();
            //list year
            foreach (var item in dBContext.Orders)
            {
                DateTime a =(DateTime)item.OrderDate;
                listYear.Add(new YearFilter
                {
                    id = a.Year,
                    value = a.Year
                });
            }
            listYear.DistinctBy(o=>o.id).ToList();

            //weekly sale 
            DateTime input = DateTime.Now;
            int delta = DayOfWeek.Monday - input.DayOfWeek;
            DateTime currentMonday = input.AddDays(delta);

            var monday = currentMonday;
            var sunday = currentMonday.AddDays(6);

            List<MyRazorPage.Models.Order> listOrder = dBContext.Orders.ToList()
                .Where(o => o.OrderDate >= monday && o.OrderDate <= sunday).ToList();

            weeklySale = 0;
            foreach (var item in listOrder)
            {
                if (item.RequiredDate != null&& item.ShippedDate<= DateTime.Now)
                {
                    weeklySale += item.Freight;
                }
            }

            // Total order
            totalSale = 0;
            List<MyRazorPage.Models.Order> orders = dBContext.Orders.ToList();
            foreach (var item in orders)
            {
                if (item.RequiredDate != null && item.ShippedDate <= DateTime.Now)
                {
                    totalSale += item.Freight;
                }
            }

            // total customer
            totalCustomer = 0;
            List<MyRazorPage.Models.Account> accounts = dBContext.Accounts.ToList();
            foreach (var item in accounts)
            {
                if (item.Role == 2)
                {
                    totalCustomer++;
                }
            }

            // total guest
            int totalCustomerInOrder = dBContext.Customers.ToList().Count();

            totalGuest = totalCustomerInOrder - totalCustomer;


            // order month
            Jan = 0;
            Feb = 0;
            Mar = 0;
            Apr = 0;
            May = 0;
            Jun = 0;
            Jul = 0;
            Aug = 0;
            Sep = 0;
            Oct = 0;
            Nov = 0;
            Dec = 0;

            List<MyRazorPage.Models.Order> ordersJan = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 1
                && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersJan)
            {
                Jan += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersFeb = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 2
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersFeb)
            {
                Feb += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersMarch = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 3
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersMarch)
            {
                Mar += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersApr = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 4
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersApr)
            {
                Apr += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersMay = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 5
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersMay)
            {
                May += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersJun = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 6
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersJun)
            {
                Jun += Convert.ToInt32(item.Freight);
            }
            List<MyRazorPage.Models.Order> ordersJul = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 7
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersJul)
            {
                Jul += Convert.ToInt32(item.Freight);
            }
            List<MyRazorPage.Models.Order> ordersAug = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 8
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersAug)
            {
                Aug += Convert.ToInt32(item.Freight);
            }
            List<MyRazorPage.Models.Order> ordersSep = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 9
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersSep)
            {
                Sep += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersOct = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 10
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersOct)
            {
                Oct += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersNov = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 11
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersNov)
            {
                Nov += Convert.ToInt32(item.Freight);
            }
            List<MyRazorPage.Models.Order> ordersDec = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 12
            && o.OrderDate.GetValueOrDefault().Year == DateTime.Now.Year).ToList();
            foreach (var item in ordersDec)
            {
                Dec += Convert.ToInt32(item.Freight);
            }


            // new customer 

            var listCust = dBContext.Customers.ToList().Where(o => o.CreateDate.GetValueOrDefault().Month == DateTime.Now.Month
            && o.CreateDate.GetValueOrDefault().Year == DateTime.Now.Year);
            newGuest = listCust.Count();

            return Page();
        }
        public IActionResult OnGetFiltering(int id)
        {
            int year = id;
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
            listYear = new List<YearFilter>();
            //list year
            foreach (var item in dBContext.Orders)
            {
                DateTime a = (DateTime)item.OrderDate;
                listYear.Add(new YearFilter
                {
                    id = a.Year,
                    value = a.Year
                });
            }
            listYear.Select(o => o.id).Distinct();
            //weekly sale 
            DateTime input = DateTime.Now;
            int delta = DayOfWeek.Monday - input.DayOfWeek;
            DateTime currentMonday = input.AddDays(delta);

            var monday = currentMonday;
            var sunday = currentMonday.AddDays(6);

            List<MyRazorPage.Models.Order> listOrder = dBContext.Orders.ToList()
                .Where(o => o.OrderDate >= monday && o.OrderDate <= sunday).ToList();

            weeklySale = 0;
            foreach (var item in listOrder)
            {
                if (item.RequiredDate != null && item.ShippedDate <= DateTime.Now)
                {
                    weeklySale += item.Freight;
                }
            }

            // Total order
            totalSale = 0;
            List<MyRazorPage.Models.Order> orders = dBContext.Orders.ToList();
            foreach (var item in orders)
            {
                if (item.RequiredDate != null && item.ShippedDate <= DateTime.Now)
                {
                    totalSale += item.Freight;
                }
            }

            // total customer
            totalCustomer = 0;
            List<MyRazorPage.Models.Account> accounts = dBContext.Accounts.ToList();
            foreach (var item in accounts)
            {
                if (item.Role == 2)
                {
                    totalCustomer++;
                }
            }

            // total guest
            int totalCustomerInOrder = dBContext.Customers.ToList().Count();

            totalGuest = totalCustomerInOrder - totalCustomer;


            // order month
            Jan = 0;
            Feb = 0;
            Mar = 0;
            Apr = 0;
            May = 0;
            Jun = 0;
            Jul = 0;
            Aug = 0;
            Sep = 0;
            Oct = 0;
            Nov = 0;
            Dec = 0;

            List<MyRazorPage.Models.Order> ordersJan = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 1
                && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersJan)
            {
                Jan += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersFeb = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 2
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersFeb)
            {
                Feb += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersMarch = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 3
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersMarch)
            {
                Mar += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersApr = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 4
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersApr)
            {
                Apr += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersMay = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 5
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersMay)
            {
                May += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersJun = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 6
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersJun)
            {
                Jun += Convert.ToInt32(item.Freight);
            }
            List<MyRazorPage.Models.Order> ordersJul = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 7
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersJul)
            {
                Jul += Convert.ToInt32(item.Freight);
            }
            List<MyRazorPage.Models.Order> ordersAug = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 8
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersAug)
            {
                Aug += Convert.ToInt32(item.Freight);
            }
            List<MyRazorPage.Models.Order> ordersSep = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 9
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersSep)
            {
                Sep += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersOct = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 10
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersOct)
            {
                Oct += Convert.ToInt32(item.Freight);
            }

            List<MyRazorPage.Models.Order> ordersNov = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 11
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersNov)
            {
                Nov += Convert.ToInt32(item.Freight);
            }
            List<MyRazorPage.Models.Order> ordersDec = dBContext.Orders.ToList().Where(o => o.OrderDate.GetValueOrDefault().Month == 12
            && o.OrderDate.GetValueOrDefault().Year == year).ToList();
            foreach (var item in ordersDec)
            {
                Dec += Convert.ToInt32(item.Freight);
            }


            // new customer 

            var listCust = dBContext.Customers.ToList().Where(o => o.CreateDate.GetValueOrDefault().Month == DateTime.Now.Month
            && o.CreateDate.GetValueOrDefault().Year == DateTime.Now.Year);
            newGuest = listCust.Count();

            return Page();
        }

    }
}
