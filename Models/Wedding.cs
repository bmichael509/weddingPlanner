using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key] // the below prop is the primary key, [Key] is not needed if named with pattern: ModelNameId
        public int WeddingId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MinLength(2, ErrorMessage = "Must be longer than 2 characters")]
        [Display(Name = "Wedder One")]
        public string One { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [MinLength(2, ErrorMessage = "Must be longer than 2 characters")]
        [Display(Name = "Wedder Two")]
        public string Two { get; set; }
        [Required(ErrorMessage = "is required")]
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? Date { get; set; }
        [Required(ErrorMessage = "is required")]
        [MinLength(2, ErrorMessage = "Must be longer than 2 characters")]
        [Display(Name = "Wedding Address")]
        public string Address { get; set; }
        public int UserId { get; set; }
        public User WeddingPlanner { get; set; }
        public List<Guest> GuestList { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
