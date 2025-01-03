using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScrap.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? Address { get; set; }
        public string? Name { get; set; }
        public string? Price { get; set; }
        public string? Updated { get; set; }
        public bool IsFavorites { get; set; }

        public List<PriceChanges> PriceChanges { get; set; } = new();
    }
}
