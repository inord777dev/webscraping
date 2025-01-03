using MauiScrap.Services;
using MauiScrap.ViewModels;
using MauiScrap.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WpfScrap.Services;

namespace MauiScrap
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Services

            //builder.Services.AddDbContext<DataContext>();

            builder.Services.AddDbContext<DataContext>(options => {
                var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                //var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                System.IO.Directory.CreateDirectory(path);
                options.UseSqlite($"Filename={Path.Combine(path, "kufar.db")}");

            }, ServiceLifetime.Singleton);
            builder.Services.AddSingleton<IProductService, ProductService>();

            // Views 
            builder.Services.AddSingleton<ProductsView>();
            builder.Services.AddTransient<ProductView>();

            // View Models
            builder.Services.AddSingleton<ProductsViewModel>();
            builder.Services.AddTransient<ProductViewModel>();

            return builder.Build();
        }
    }
}
