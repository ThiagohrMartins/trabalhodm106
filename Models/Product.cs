﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TraballhoDM106.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        public string description { get; set; }
        public string color { get; set; }
        [Required]
        public string model { get; set; }
        [Required]
        public string code { get; set; }
        public decimal price { get; set; }
    }
}