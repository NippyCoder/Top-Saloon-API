using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class Shop
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public virtual List<Administrator> Administrators { get; set; }
        public virtual List<Barber> Barbers { get; set; }
    }
}
