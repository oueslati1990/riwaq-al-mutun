using System.IO;
using System.Windows;
using MatnMemorization.Application.Contracts.Navigation;
using MatnMemorization.Application.Contracts.Ocr;
using MatnMemorization.Application.Contracts.Pdf;
using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Application.Contracts.UI;
using MatnMemorization.Application.UseCases.Import;
using MatnMemorization.Application.UseCases.Matns;
using MatnMemorization.Application.UseCases.Sections;
using MatnMemorization.Application.UseCases.Review;
using MatnMemorization.Application.UseCases.Sessions;
using MatnMemorization.Application.UseCases.Statistics;
using MatnMemorization.Application.UseCases.TextUnits;
using MatnMemorization.Infrastructure.Ocr;
using MatnMemorization.Infrastructure.Pdf;
using MatnMemorization.Infrastructure.Persistence;
using MatnMemorization.Infrastructure.Persistence.Repositories;
using MatnMemorization.Presentation.Wpf.Services;
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
                RegisterRepositories(services);
                RegisterOcrServices(services);
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

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IMatnRepository, MatnRepository>();
        services.AddScoped<IMatnSectionRepository, MatnSectionRepository>();
        services.AddScoped<ITextUnitRepository, TextUnitRepository>();
        services.AddScoped<IMemorizationProgressRepository, MemorizationProgressRepository>();
        services.AddScoped<CreateMatnHandler>();
        services.AddScoped<UpdateMatnHandler>();
        services.AddScoped<SoftDeleteMatnHandler>();
        services.AddScoped<GetMatnListHandler>();
        services.AddScoped<GetMatnDetailHandler>();
        services.AddScoped<AddSectionHandler>();
        services.AddScoped<AddTextUnitHandler>();
        services.AddScoped<ImportMatnFromOcrHandler>();
        services.AddScoped<StartSessionHandler>();
        services.AddScoped<RecordAttemptHandler>();
        services.AddScoped<GetDueUnitsHandler>();
        services.AddScoped<IStatisticsRepository, StatisticsRepository>();
        services.AddScoped<GetStatisticsHandler>();
    }

    private static void RegisterOcrServices(IServiceCollection services)
    {
        services.Configure<TesseractOptions>(opts =>
        {
            opts.TessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
            opts.Language = "ara";
        });
        services.AddSingleton<IOcrService, TesseractOcrService>();
        services.AddSingleton<IPdfService, DocnetPdfService>();
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<OcrSessionStore>();
    }

    private static void RegisterNavigation(IServiceCollection services)
    {
        services.AddSingleton<MatnSelectionService>();
        services.AddSingleton<NavigationService>();
        services.AddSingleton<INavigationService>(sp => sp.GetRequiredService<NavigationService>());
    }
}
