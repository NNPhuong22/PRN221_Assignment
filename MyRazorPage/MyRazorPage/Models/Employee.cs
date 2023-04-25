using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyRazorPage.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Accounts = new HashSet<Account>();
            Orders = new HashSet<Order>();
        }

        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int? DepartmentId { get; set; }
        public string Title { get; set; }
        public string TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string Address { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
    public class EmployeeEdit
    {
        [Required(ErrorMessage = "LastName can not blank!")]
        [StringLength(maximumLength: 20, MinimumLength = 1, ErrorMessage = "Length must be between 1 to 20")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "FirstName can not blank!")]
        [StringLength(maximumLength: 10, MinimumLength = 1, ErrorMessage = "Length must be between 1 to 40")]

        public string FirstName { get; set; }
        [Required(ErrorMessage = "DepartmentId can not blank!")]

        public string Title { get; set; }
        [Required(ErrorMessage = "Title Of Courtesy can not blank!")]
        [StringLength(maximumLength: 25, MinimumLength = 1, ErrorMessage = "Length must be between 1 to 25")]

        public string TitleOfCourtesy { get; set; }
        [Required(ErrorMessage = "BirthDate can not blank!")]

        public DateTime? BirthDate { get; set; }
        [Required(ErrorMessage = "HireDate can not blank!")]

        public DateTime? HireDate { get; set; }
        [Required(ErrorMessage = "Address can not blank!")]
        [StringLength(maximumLength: 60, MinimumLength = 1, ErrorMessage = "Length must be between 1 to 60")]

        public string Address { get; set; }
    }
}
