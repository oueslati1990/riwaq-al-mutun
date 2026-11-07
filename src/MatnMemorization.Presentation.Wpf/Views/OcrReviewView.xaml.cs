using System.Windows;
using System.Windows.Controls;
using MatnMemorization.Application.Models;
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

    // Intercept focus on the text editor: if the selected page is Approved, ask for confirmation
    private void TextEditor_GotFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is OcrReviewViewModel vm &&
            vm.SelectedPage?.Status == OcrPageStatus.Approved)
        {
            vm.RequestEditPageCommand.Execute(vm.SelectedPage);
            // Move focus away so the user can't type until they confirm
            ((TextBox)sender).MoveFocus(new System.Windows.Input.TraversalRequest(
                System.Windows.Input.FocusNavigationDirection.Previous));
        }
    }
}
