using System.IO;
using MatnMemorization.Application.Contracts.UI;
using Microsoft.Win32;

namespace MatnMemorization.Presentation.Wpf.Services;

public sealed class FileDialogService : IFileDialogService
{
    public string? PickFile(string filter, string title = "")
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter,
            Title = title,
            CheckFileExists = true,
            CheckPathExists = true
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
