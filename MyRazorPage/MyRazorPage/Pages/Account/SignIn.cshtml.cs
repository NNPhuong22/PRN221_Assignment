using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyRazorPage.Models;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class SignInModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public SignInModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [BindProperty]
        public Models.Account Account { get; set; }
        public void OnGet()
        {


        }

        public IActionResult OnGetLogOut()
        {
            if (HttpContext.Session.GetString("Custsession") != null)
            {
                HttpContext.Session.Remove("CartList");
                HttpContext.Session.Remove("Custsession");
                HttpContext.Session.Remove("CusInfo");
            }
            return RedirectToPage("/Index");

        }
        public IActionResult OnPost()
        {
            if (Account.Email == null || Account.Password == null|| Account.Email == "" || Account.Password == "")
            {
                return Page();
            }
            var key = "b14ca5898a4e4133bbce2ea2315a1916";

            var usernameAcc = dBContext.Accounts.SingleOrDefault(o => o.Email.Trim().ToLower().Equals(Account.Email.Trim().ToLower()));
            var acc = dBContext.Accounts.SingleOrDefault(o => o.Email.Trim().ToLower().Equals(Account.Email.Trim().ToLower())
        && o.Password.Trim().ToLower().Equals(EncryptString(key,Account.Password.Trim().ToLower())));
            if (usernameAcc != null && acc == null)
            {
                ViewData["msg"] = "Wrong username or password!";
                return Page();
            }
            if (acc != null)
            {
                var customer1 = dBContext.Customers.SingleOrDefault(o => o.CustomerId == acc.CustomerId);
                if (customer1.Address == "1" && acc.Role == 2)
                {
                    ViewData["msg"] = "Your account has been deactive, contact admin for futher assistance!";
                    return Page();

                }
                // admin
                if (acc.Role == 1)
                {
                    MyRazorPage.Models.Account a = new Models.Account{
                        CustomerId = acc.CustomerId,
                        Role = acc.Role,
                        Email = acc.Email,
                        EmployeeId = acc.EmployeeId,
                        AccountId = acc.AccountId,
                    };
                    HttpContext.Session.SetString("Custsession", JsonSerializer.Serialize(a));
                    return RedirectToPage("/Admin/Dashboard");
                }
                // user
                else
                {
                    MyRazorPage.Models.Account  newCus = new Models.Account
                    {
                        CustomerId = acc.CustomerId,
                        Role = acc.Role,
                        Email = acc.Email,
                        EmployeeId = acc.EmployeeId,
                        AccountId = acc.AccountId
                    };
                    HttpContext.Session.SetString("Custsession", JsonSerializer.Serialize(newCus));
                    var customer = dBContext.Customers.SingleOrDefault(o => o.CustomerId == acc.CustomerId);
                    Customer a = new Customer
                    {
                        ContactName = customer.ContactName
                    };
                    HttpContext.Session.SetString("CusInfo", JsonSerializer.Serialize(a));
                    return RedirectToPage("/Index");
                }

            }
            else
            {
                ViewData["msg"] = "This email doesn't register yet! ";
                return Page();
            }

        }
        public static string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
