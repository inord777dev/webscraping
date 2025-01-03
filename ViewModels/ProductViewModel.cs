using MauiScrap.Models;
using MauiScrap.Services;
using MauiScrap.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiScrap.ViewModels
{
    [QueryProperty(nameof(Product), "Product")]
    public class ProductViewModel : INotifyPropertyChanged
    {
        private Product product = new Product();
        public Product Product
        {
            get => product;
            set
            {
                product = value;
                OnPropertyChanged();
            }
        } 

        private readonly IProductService _productService;

        public ICommand SaveCommand { private set; get; }

        public ProductViewModel(IProductService productService)
        {
            this._productService = productService;

            SaveCommand = new Command(
                execute: async () =>
                {
                    int result = 0;
                    if (this.Product.Id == 0)
                    {
                        result = await _productService.AddProduct(this.Product);
                    }
                    else
                    {
                        result = await _productService.UpdateProduct(this.Product);
                    }
                    if (result > 0)
                    {
                        AppShell.Current.SendBackButtonPressed();
                    }
                },
                canExecute: () =>
                {
                    return true;
                });
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}
