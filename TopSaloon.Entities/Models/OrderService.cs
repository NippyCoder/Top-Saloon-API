using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class OrderService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float? Price { get; set; }
        public int? Time { get; set; }
        public bool? IsConfirmed { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}
