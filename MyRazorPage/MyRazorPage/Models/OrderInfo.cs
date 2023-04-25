using System.ComponentModel.DataAnnotations;

namespace MyRazorPage.Models
{
    public class OrderInfo
    {
        [Required(ErrorMessage = "Ship city required!")]

        public string shipCity { get; set; }
        [Required(ErrorMessage = "Ship country required!")]

        public string shipCountry { get; set; }
        [Required(ErrorMessage = "Ship region required!")]

        public string shipRegion { get; set; }
        [Required(ErrorMessage ="Postal code required!")]

        public string postalCode { get; set; }
    }
}
