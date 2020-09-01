using System;
using System.Collections.Generic;
using System.Text;
namespace TopSaloon.DTOs.Models
{
    public class AddServiceDTO
    {
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public string AdminPath { get; set; }
        public string UserPath { get; set; }
        public int Time { get; set; }
        public float Price { get; set; }
    }
}