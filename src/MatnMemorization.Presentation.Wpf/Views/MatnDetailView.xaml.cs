using System.Windows;
using System.Windows.Controls;
using MatnMemorization.Presentation.Wpf.ViewModels;

namespace MatnMemorization.Presentation.Wpf.Views;

public partial class MatnDetailView : UserControl
{
    public MatnDetailView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MatnDetailViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
