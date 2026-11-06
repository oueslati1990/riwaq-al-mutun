using System.IO;
using System.Windows;
using MatnMemorization.Application.Contracts.Navigation;
using MatnMemorization.Infrastructure.Persistence;
using MatnMemorization.Presentation.Wpf.Services.Navigation;
using MatnMemorization.Presentation.Wpf.ViewModels;
using MatnMemorization.Presentation.Wpf.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MatnMemorization.Presentation.Wpf;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            })
            .UseSerilog((ctx, services, cfg) =>
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var logDir = Path.Combine(appData, "MatnMemorization", "logs");
                Directory.CreateDirectory(logDir);

                cfg.MinimumLevel.Information()
                   .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                   .WriteTo.File(
                       Path.Combine(logDir, "app-.log"),
                       rollingInterval: RollingInterval.Day,
                       retainedFileCountLimit: 7);
            })
            .ConfigureServices((ctx, services) =>
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var dataDir = Path.Combine(appData, "MatnMemorization", "data");
                Directory.CreateDirectory(dataDir);

                var dbFileName = ctx.Configuration["Database:FileName"] ?? "matn.db";
                var connectionString = $"Data Source={Path.Combine(dataDir, dbFileName)}";

                services.AddDbContext<AppDbContext>(o => o.UseSqlite(connectionString));
                RegisterViewModels(services);
                RegisterViews(services);
                RegisterNavigation(services);
            })
            .Build();

        await _host.StartAsync();

        using (var scope = _host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        var mainWindow = new MainWindow
        {
            DataContext = _host.Services.GetRequiredService<MainViewModel>()
        };
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        base.OnExit(e);
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<LibraryViewModel>();
        services.AddTransient<ImportViewModel>();
        services.AddTransient<OcrReviewViewModel>();
        services.AddTransient<MatnDetailViewModel>();
        services.AddTransient<MemorizationViewModel>();
        services.AddTransient<ReviewViewModel>();
        services.AddTransient<StatisticsViewModel>();
        services.AddTransient<SettingsViewModel>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddTransient<DashboardView>();
        services.AddTransient<LibraryView>();
        services.AddTransient<ImportView>();
        services.AddTransient<OcrReviewView>();
        services.AddTransient<MatnDetailView>();
        services.AddTransient<MemorizationView>();
        services.AddTransient<ReviewView>();
        services.AddTransient<StatisticsView>();
        services.AddTransient<SettingsView>();
    }

    private static void RegisterNavigation(IServiceCollection services)
    {
        services.AddSingleton<NavigationService>();
        services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<NavigationService>());
    }
}
