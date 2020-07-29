using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class AdminLoginModel
    {
        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }
    }
}
