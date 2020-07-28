using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class CustomerInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int? LastBarberId { get; set; } 
        public DateTime? LastVisitDate { get; set; }
        public int? TotalNumberOfVisits { get; set; }
    }

}
