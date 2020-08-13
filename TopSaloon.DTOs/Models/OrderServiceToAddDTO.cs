using System;
using System.Collections.Generic;
using System.Text;
using TopSaloon.DTOs.Models;

namespace TopSaloon.DTOs.Models
{
    public class OrderServiceToAddDTO
    {
        //ServiceID
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public float? Price { get; set; }
        public int? Time { get; set; }
        public bool? IsConfirmed { get; set; }
    }
}
