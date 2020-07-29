using System;
using System.Collections.Generic;
using System.Text;
using TopSalon.DTOs.Models;

namespace TopSaloon.DTOs.Models
{
    public class OrderServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float? Price { get; set; }
        public int? Time { get; set; }
        public bool? IsConfirmed { get; set; }
        public int OrderId { get; set; }
        public virtual OrderDTO Order { get; set; }
    }
}
