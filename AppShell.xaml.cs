using MauiScrap.Views;

namespace MauiScrap
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ProductView), typeof(ProductView));
        }
    }
}
