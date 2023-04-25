using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyRazorPage.Models
{
    public partial class Account
    {
        public int AccountId { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage ="Email is required!")]
        public string Email { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public int? Role { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
    }
    public class ResetPass
    {
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = false, ErrorMessage = "Please enter a valid password")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }
        [Compare(otherProperty: "Pass", ErrorMessage = "New & confirm password does not match")]
        [DataType(DataType.Password)]
        public string rePass { get; set; }
    }
    public class ResetAccount
    {
        public string Id { get; set; }
        public string email { get; set; }
    }
}
