using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using TopSalon.Entities.Models;

namespace TopSalon.DTOs.Models
{
    public class CustomerDetailsModelDTO
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public int LastBarberId { get; set; }
        public DateTime dateVisitDate { get; set; }
        public int TotalNumberOfVisit { get; set; }
        public virtual List<OrderDTO> orders { get; set; }
    }
}

