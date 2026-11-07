using System.Windows;
using System.Windows.Controls;
using MatnMemorization.Presentation.Wpf.ViewModels;

namespace MatnMemorization.Presentation.Wpf.Views;

public partial class ReviewView : UserControl
{
    public ReviewView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ReviewViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
