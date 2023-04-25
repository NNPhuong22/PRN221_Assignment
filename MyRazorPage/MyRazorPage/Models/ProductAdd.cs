using System.ComponentModel.DataAnnotations;

namespace MyRazorPage.Models
{
    public class ProductAdd
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Product name is required!")]
        [StringLength(maximumLength: 25, MinimumLength = 1, ErrorMessage = "Length must be between 10 to 25")]

        public string ProductName { get; set; }

        public int? CategoryId { get; set; }
        [Required(ErrorMessage = "Quantity per unit is required!")]
        public string QuantityPerUnit { get; set; }
        [Required(ErrorMessage = "Unit price is required!")]

        public decimal? UnitPrice { get; set; }
        [Required(ErrorMessage = "Unit in stock is required!")]

        public short? UnitsInStock { get; set; }
    }
}
