using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using MyRazorPage.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace MyRazorPage.Pages.UserPage
{
    public class CartModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public CartModel(PRN221DBContext _dBContext)
        {
            dBContext = _dBContext;
        }
        public List<Models.CartSession> cart { get; set; }
        public decimal? TotalMoney { get; set; }
        [BindProperty]
        public Customer customer1 { get; set; }
        [BindProperty]

        public DateTime requiredDateOrder { get; set; }
        public Customer existedCustomer { get; set; }
        public void OnGet()
        {

            var option = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    WriteIndented = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
            TotalMoney = 0;
            // fill existed customer field
            if (HttpContext.Session.GetString("Custsession") != null)
            {
                var account = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(HttpContext.Session.GetString("Custsession"), option);
                existedCustomer = dBContext.Customers.SingleOrDefault(o => o.CustomerId == account.CustomerId);
            }
            if (HttpContext.Session.GetString("CartList") != null)
            {
                
                cart = JsonSerializer.Deserialize<List<MyRazorPage.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
            }
        }
        public IActionResult OnGetMinusProduct(int id)
        {
            TotalMoney = 0;
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            cart = JsonSerializer.Deserialize<List<MyRazorPage.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);

            var minusPro = cart.SingleOrDefault(o => o.ProductId == id);
            if (minusPro.Quantity == 1)
            {
                cart.Remove(minusPro);
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
            }
            else
            {
                minusPro.Quantity -= 1;
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
            }

            HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(cart));
            return Page();
        }
        public IActionResult OnGetPlusProduct(int id)
        {
            TotalMoney = 0;
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            cart = JsonSerializer.Deserialize<List<MyRazorPage.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);

            var plusPro = cart.SingleOrDefault(o => o.ProductId == id);
            var productInStock = dBContext.Products.SingleOrDefault(o => o.ProductId == id);
            if (plusPro.Quantity == productInStock.UnitsInStock)
            {
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
                return Page();
            }
            else
            {
                plusPro.Quantity += 1;
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }
            }

            HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(cart));

            return Page();
        }
        public IActionResult OnGetRemoveProduct(int id)
        {
            TotalMoney = 0;
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            cart = JsonSerializer.Deserialize<List<MyRazorPage.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);

            var removePro = cart.SingleOrDefault(o => o.ProductId == id);
            cart.Remove(removePro);
            foreach (var item in cart)
            {
                TotalMoney += (item.Price * item.Quantity);
            }
            HttpContext.Session.SetString("CartList", JsonSerializer.Serialize(cart));

            return Page();
        }
        public IActionResult OnPost(decimal total)
        {
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            // guess 
            if (HttpContext.Session.GetString("Custsession") == null)
            {
                
                if (!ModelState.IsValid)
                {
                    
                    return Page();

                }
                if (customer1.ContactName!=null)
                {
                    customer1.CustomerId = RandomCustID(5);
                customer1.CreateDate = DateTime.Now;
                    //add Customer
                    dBContext.Customers.Add(customer1);
                    dBContext.SaveChanges();

                    //add order
                    Order newOrder = new Order
                    {
                        CustomerId = customer1.CustomerId,
                        EmployeeId =4,
                        OrderDate = DateTime.Now,
                        RequiredDate = requiredDateOrder,
                        ShipName = customer1.ContactName,
                        ShipAddress = customer1.Address,
                        ShippedDate = requiredDateOrder,
                        Freight = total
                    };
                    dBContext.Orders.Add(newOrder);
                    dBContext.SaveChanges();

                    //add order detail
                    var ordersList1 = dBContext.Orders; 
                    var orderLatestGuest = ordersList1.OrderBy(o=>o.OrderId).LastOrDefault();
                    cart = JsonSerializer.Deserialize<List<MyRazorPage.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);
                    foreach (var order in cart)
                    {
                        dBContext.Add(new OrderDetail
                        {
                            OrderId = orderLatestGuest.OrderId,
                            ProductId = order.ProductId,
                            UnitPrice = (decimal)order.Price,
                            Quantity = (short)order.Quantity,
                            Discount = 0
                        });
                    }
                    dBContext.SaveChanges();
                    HttpContext.Session.Remove("CartList");

                    //PDF
                    HttpContext.Session.SetString("Invoice", JsonSerializer.Serialize(cart));

                    return RedirectToPage("/UserPage/Invoice");
                }
                // if field input invalid return cart and reload cart product
                cart = JsonSerializer.Deserialize<List<MyRazorPage.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);
                foreach (var item in cart)
                {
                    TotalMoney += (item.Price * item.Quantity);
                }

                return Page();
            }

            // Account customer

            //lấy thông tin khách hàng
            var account = JsonSerializer.Deserialize<MyRazorPage.Models.Account>(HttpContext.Session.GetString("Custsession"), option);
            var customer = dBContext.Customers.SingleOrDefault(o => o.CustomerId == account.CustomerId);
            // add order
            Order orderAdd = new Order
            {
                CustomerId = customer.CustomerId,
                EmployeeId = 4,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now,
                ShipName = customer.ContactName,
                ShipAddress = customer.Address,
                ShippedDate = requiredDateOrder,
                Freight = total

            };
            dBContext.Orders.Add(orderAdd);
            dBContext.SaveChanges();

            //add order detail
            var ordersList = dBContext.Orders;
            var orderLatest = ordersList.OrderBy(o => o.OrderId).LastOrDefault();
            cart = JsonSerializer.Deserialize<List<MyRazorPage.Models.CartSession>>(HttpContext.Session.GetString("CartList"), option);
            foreach (var order in cart)
            {
                dBContext.Add(new OrderDetail
                {
                    OrderId = orderLatest.OrderId,
                    ProductId = order.ProductId,
                    UnitPrice = (decimal)order.Price,
                    Quantity = (short)order.Quantity,
                    Discount = 0
                });
            }


            dBContext.SaveChanges();
            HttpContext.Session.Remove("CartList");

            //PDF
            HttpContext.Session.SetString("Invoice", JsonSerializer.Serialize(cart));


            return RedirectToPage("/Index");
        }

        private string RandomCustID(int length)
        {
            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }

    }
}
