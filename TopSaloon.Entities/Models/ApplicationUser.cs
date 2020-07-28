using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.Entities.Models
{
    public class ApplicationUser  : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
