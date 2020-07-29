using System;
using System.Collections.Generic;

namespace TopSaloon.DTOs.Models
{
    public partial class OrderTimeServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float? Price { get; set; }
        public int? Time { get; set; }
        public bool? IsConfirmed { get; set; }
        public int OrderId { get; set; }
    }
}
