using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using MyRazorPage.Models;
using SignalRLab.Models;

namespace MyRazorPage.Pages.UserPage
{
    public class SignalrProductModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<SignalrServer> _signalRHub;

        public SignalrProductModel(PRN221DBContext dBContext, IHubContext<SignalrServer> signalRHub)
        {
            this.dBContext = dBContext;
            _signalRHub = signalRHub;
        }
        public void OnGet()
        {
        }
        public List<Models.Product> GetProducts()
        {
            return dBContext.Products.ToList();
        }
    }
}
