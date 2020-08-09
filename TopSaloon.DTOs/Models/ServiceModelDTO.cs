using System;
using System.Collections.Generic;
using System.Text;
namespace TopSaloon.DTOs.Models
{
    public class ServiceModelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Time { get; set; }
        public float Price { get; set; }
    }
}