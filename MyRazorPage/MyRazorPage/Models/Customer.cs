using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyRazorPage.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Accounts = new HashSet<Account>();
            Orders = new HashSet<Order>();
        }

        public string CustomerId { get; set; }
        [Required(ErrorMessage ="Company name can not blank!")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Contact name can not blank!")]

        public string ContactName { get; set; }
        [Required(ErrorMessage = "Contact title can not blank!")]

        public string ContactTitle { get; set; }
        [Required(ErrorMessage = "Address can not blank!")]

        public string Address { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
