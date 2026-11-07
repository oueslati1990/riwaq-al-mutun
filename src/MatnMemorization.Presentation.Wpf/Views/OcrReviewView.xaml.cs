using System.Windows;
using System.Windows.Controls;
using MatnMemorization.Presentation.Wpf.ViewModels;

namespace MatnMemorization.Presentation.Wpf.Views;

public partial class OcrReviewView : UserControl
{
    public OcrReviewView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is OcrReviewViewModel vm)
            vm.LoadCommand.Execute(null);
    }
}
