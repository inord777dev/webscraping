using MauiScrap.Models;
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

    private async void SwipeItem_Edit(object sender, EventArgs e)
    {
        var item = sender as SwipeItem;
        if (item == null)
            return;

        _viewModel.EditCommand.Execute(item.BindingContext);
    }

    private async void SwipeItem_Delete(object sender, EventArgs e)
    {
        var item = sender as SwipeItem;
        if (item == null)
            return;

        bool result = await DisplayAlert("Action", "Do you want delete item?", "Yes", "No");
        if (result)
        {
            _viewModel.DeleteCommand.Execute(item.BindingContext);
        }
    }

    private void collectionView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        _viewModel.EditCommand.Execute(e.Item);
    }
}