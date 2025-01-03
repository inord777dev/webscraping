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

        private Product selectedProduct;

        public Product SelectedProduct 
        {  
            get 
            { 
                return selectedProduct; 
            } 
            set 
            {
                selectedProduct = value;
                OnPropertyChanged();
                RefreshCanExecutes();
            } 
        }

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
                execute: () =>
                {
                    if (SelectedProduct != null)
                    {
                        var parameters = new Dictionary<string, object>();
                        parameters.Add(nameof(Product), SelectedProduct);
                        AppShell.Current.GoToAsync(nameof(ProductView), parameters);
                    }
                },
                canExecute: () =>
                {
                    return SelectedProduct != null;
                });

            DeleteCommand = new Command(
                execute: async () =>
                {
                    if (SelectedProduct != null)
                    {
                        var result = await _productService.DeleteProduct(SelectedProduct);
                        if (result > 0)
                        {
                            GetAllCommand?.Execute(this);
                        }
                    }
                },
                canExecute: () =>
                {
                    return SelectedProduct != null;
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
                },
                canExecute: () =>
                {
                    return true;
                });
        }

        void RefreshCanExecutes()
        {
            (EditCommand as Command).ChangeCanExecute();
            (DeleteCommand as Command).ChangeCanExecute();
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
