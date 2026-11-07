using System.Windows;
using System.Windows.Controls;
using MatnMemorization.Presentation.Wpf.ViewModels;

namespace MatnMemorization.Presentation.Wpf.Views;

public partial class StatisticsView : UserControl
{
    public StatisticsView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is StatisticsViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
