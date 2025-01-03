using MauiScrap.Models;
using MauiScrap.ViewModels;
using MauiScrap.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfScrap.Services;

namespace MauiScrap.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<int> AddProduct(Product product);
        Task<int> DeleteProduct(Product product);
        Task<int> UpdateProduct(Product product);
        Task<int> Load();
    }
}
