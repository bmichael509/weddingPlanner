using System;
using System.ComponentModel.DataAnnotations;
namespace WeddingPlanner.Models
{
    public class Login
    {
        [Display(Name = "Login")]
        public string LoginEmail { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}