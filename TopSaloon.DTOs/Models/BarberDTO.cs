using System;
using System.Collections.Generic;
using System.Text;

namespace TopSaloon.DTOs.Models
{
    public class BarberDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int? NumberOfCustomersHandled { get; set; }
        public int ShopId { get; set; }
        public virtual ShopDTO Shop { get; set; }
        public virtual List<BarberProfilePhotoDTO> BarberProfilePhotos { get; set; }
        public virtual List<BarberQueueDTO> BarberQueues { get; set; }
    }
}
