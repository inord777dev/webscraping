using MauiScrap.Models;
using MauiScrap.Services;
using MauiScrap.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiScrap.ViewModels
{
    public partial class ProductsViewModel : INotifyPropertyChanged
    {
        private readonly IProductService _productService;
        public ObservableCollection<Product>? Products { get; set; } = new ObservableCollection<Product>();
        public ICommand AddCommand { private set; get; }
        public ICommand EditCommand { private set; get; }
        public ICommand DeleteCommand { private set; get; }
        public ICommand GetAllCommand { private set; get; }
        public ICommand LoadCommand { private set; get; }

        public ProductsViewModel(IProductService productService) 
        {
            _productService = productService;

            AddCommand = new Command(
                execute: () =>
                {
                    AppShell.Current.GoToAsync(nameof(ProductView));
                },
                canExecute: () =>
                {
                    return true;
                });

            EditCommand = new Command(
                execute: (object product) =>
                {
                    if (product != null)
                    {
                        var parameters = new Dictionary<string, object>
                        {
                            { nameof(Product), product }
                        };
                        AppShell.Current.GoToAsync(nameof(ProductView), parameters);
                    }
                },
                canExecute: (object product) =>
                {
                    return product != null;
                });

            DeleteCommand = new Command(
                execute: async (object product) =>
                {
                    if (product != null)
                    {
                        var result = await _productService.DeleteProduct((Product)product);
                        if (result > 0)
                        {
                            GetAllCommand?.Execute(this);
                        }
                    }
                },
                canExecute: (object product) =>
                {
                    return product != null;
                });

            GetAllCommand = new Command(
                execute: async () =>
                {
                    var products = await _productService.GetProductsAsync();
                    Products.Clear();
                    foreach(var product in products)
                    {
                        Products.Add(product);
                    }
                    OnPropertyChanged("Products");
                },
                canExecute: () =>
                {
                    return true;
                });

            LoadCommand = new Command(
                execute: async () =>
                {
                    var result = await _productService.Load();
                    if (result > -1)
                    {
                        Application.Current.MainPage.DisplayAlert("Loading", $"Loading complete. Updated {result}.", "Ok");
                        GetAllCommand?.Execute(this);
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Warning", "Error loading.", "Ok");
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
