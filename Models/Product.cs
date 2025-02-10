using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DateTime? Created { get; set; }

        [NotMapped]
        public string? PriceDelta
        {
            get
            {
                if (PriceChanges.Count == 0) 
                {
                    return string.Empty;
                }

                string? value = string.Empty;
                try
                {
                    decimal currectPrice = Convert.ToDecimal(Price);
                    decimal lastPrice = Convert.ToDecimal(PriceChanges.LastOrDefault());
                    value = string.Format("{0:0.##}", lastPrice - currectPrice);
                }
                catch
                {

                }
                return value;
            }
        }

        public List<PriceChanges> PriceChanges { get; set; } = new();
    }
}
