using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiScrap.Models
{
    public class PriceChanges
    {
        public int Id { get; set; }
        public string? Price { get; set; }
        public string? Updated { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
