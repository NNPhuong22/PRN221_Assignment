using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace MyRazorPage.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage ="Product name is required!")]
        [StringLength(maximumLength: 25, MinimumLength = 10, ErrorMessage = "Length must be between 10 to 25")]

        public string ProductName { get; set; }

        public int? CategoryId { get; set; }
        [Required(ErrorMessage ="Quantity per unit is required!")]
        public string QuantityPerUnit { get; set; }
        [Required(ErrorMessage = "Unit price is required!")]

        public decimal? UnitPrice { get; set; }
        [Required(ErrorMessage = "Unit in stock is required!")]

        public short? UnitsInStock { get; set; }

        public short? UnitsOnOrder { get; set; }

        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
