using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyRazorPage.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class ResetPassModel : PageModel
    {
        private readonly PRN221DBContext dBContext;

        public ResetPassModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [BindProperty]
        public ResetPass ResetPass { get; set; }
        public string alert { get; set; }
        public void OnGet()
        {
            alert = "Email send successfully, check your email to reset password";
            ResetPass = new ResetPass();
        }

        public IActionResult OnPost()
        {
            var key = "b14ca5898a4e4133bbce2ea2315a1916";

            if (!ModelState.IsValid)
            {
                return Page();
            }
            var option = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
                var account = JsonSerializer.Deserialize<string>(HttpContext.Session.GetString("EmailReset"), option);
            var changePass = dBContext.Accounts.SingleOrDefault(o => o.Email == account);
            changePass.Password = EncryptString(key, ResetPass.Pass);
            dBContext.SaveChanges();

            return RedirectToPage("/Account/SignIn");
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
