using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
   public  class EditAdminPassword
    {
        public int id { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
}
}
