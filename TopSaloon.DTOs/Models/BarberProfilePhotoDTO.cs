using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class BarberProfilePhotoDTO
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int BarberId { get; set; }
        public virtual BarberDTO Barber { get; set; }
    }
}
