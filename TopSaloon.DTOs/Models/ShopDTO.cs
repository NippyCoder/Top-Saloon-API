using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class ShopDTO
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public virtual List<AdministratorDTO> Administrators { get; set; }
        public virtual List<BarberDTO> Barbers { get; set; }
    }
}
