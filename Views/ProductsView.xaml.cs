using MauiScrap.ViewModels;
using Microsoft.Maui.Controls;

namespace MauiScrap.Views;

public partial class ProductsView : ContentPage
{
    private ProductsViewModel _viewModel;
    public ProductsView(ProductsViewModel viewModel)
	{
		InitializeComponent();

        _viewModel = viewModel;
        this.BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.GetAllCommand.Execute(this);
    }
}