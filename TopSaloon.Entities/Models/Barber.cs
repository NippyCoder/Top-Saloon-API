using System;
using System.Collections.Generic;

namespace TopSaloon.Entities.Models
{
    public partial class Barber
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int? NumberOfCustomersHandled { get; set; }
        public int ShopId { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual List<BarberProfilePhoto> BarberProfilePhotos { get; set; }
        public virtual List<BarberQueue> BarberQueues { get; set; }
    }
}
