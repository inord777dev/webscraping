using MauiScrap.ViewModels;

namespace MauiScrap.Views;

public partial class ProductView : ContentPage
{
    private ProductViewModel _viewModel;
    public ProductView(ProductViewModel viewModel)
	{
		InitializeComponent();

        _viewModel = viewModel;
        this.BindingContext = viewModel;
	}
}