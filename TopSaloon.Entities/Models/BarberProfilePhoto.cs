using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TopSaloon.Entities.Models
{
    public partial class BarberProfilePhoto
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int BarberId { get; set; }
        public virtual Barber Barber { get; set; }
    }
}
