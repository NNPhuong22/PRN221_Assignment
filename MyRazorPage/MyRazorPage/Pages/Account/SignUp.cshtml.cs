using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyRazorPage.Models;
using System.Security.Cryptography;
using System.Text;

namespace MyRazorPage.Pages.Account
{
    public class SignUpModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public SignUpModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [BindProperty]
        public Models.Customer Customer { get; set; }
        [BindProperty]
        public Models.Account Account { get; set; }
        [BindProperty]
        public Models.ResetPass CheckPass { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";
            Account.Password = "abc";
            ModelState.ClearValidationState(nameof(Account));
            if (TryValidateModel(Account, nameof(Account)))
            {
                var acc = await dBContext.Accounts.SingleOrDefaultAsync(
                    a => a.Email == Account.Email
                    );
                if (acc != null)
                {
                    ViewData["msg"] = "This email exist!";
                    return Page();
                }
                else
                {
                    var cust = new Models.Customer()
                    {
                        CustomerId = RandomCustID(5),
                        CompanyName = Customer.CompanyName,
                        ContactName = Customer.ContactName,
                        ContactTitle = Customer.ContactTitle,
                        Address = Customer.Address
                    };
                    var newAcc = new Models.Account()
                    {
                        Email = Account.Email,
                        Password = EncryptString(key,CheckPass.Pass),
                        CustomerId = cust.CustomerId,
                        Role = 2
                    };
                    await dBContext.Customers.AddAsync(cust);
                    await dBContext.Accounts.AddAsync(newAcc);
                    dBContext.SaveChangesAsync();

                    return RedirectToPage("/Index");
                }
            }
            else
            {
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
