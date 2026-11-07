namespace MatnMemorization.Application.Contracts.UI;

public interface IFileDialogService
{
    string? PickFile(string filter, string title = "");
}
