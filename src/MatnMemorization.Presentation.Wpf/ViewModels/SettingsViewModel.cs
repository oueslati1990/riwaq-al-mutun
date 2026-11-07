using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Contracts.UI;
using MatnMemorization.Application.UseCases.Backup;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly CreateBackupHandler _createBackup;
    private readonly RestoreBackupHandler _restoreBackup;
    private readonly IFileDialogService _fileDialog;

    private static readonly string BackupFilter =
        "قاعدة بيانات SQLite (*.db)|*.db|جميع الملفات (*.*)|*.*";

    // Backup state
    [ObservableProperty] private bool _isBackingUp;
    [ObservableProperty] private string? _backupMessage;
    [ObservableProperty] private bool _backupSuccess;

    // Restore state
    [ObservableProperty] private bool _isRestoreConfirmOpen;
    [ObservableProperty] private string? _pendingRestorePath;
    [ObservableProperty] private bool _isRestoring;
    [ObservableProperty] private string? _restoreMessage;
    [ObservableProperty] private bool _restoreSuccess;

    public SettingsViewModel(
        CreateBackupHandler createBackup,
        RestoreBackupHandler restoreBackup,
        IFileDialogService fileDialog)
    {
        _createBackup = createBackup;
        _restoreBackup = restoreBackup;
        _fileDialog = fileDialog;
    }

    [RelayCommand]
    private async Task BackupAsync(CancellationToken ct = default)
    {
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var dest = _fileDialog.SaveFile(
            BackupFilter,
            defaultFileName: $"riwaq-backup-{today}.db",
            title: "اختر مكان حفظ النسخة الاحتياطية");

        if (dest is null) return;

        IsBackingUp = true;
        BackupMessage = null;
        BackupSuccess = false;
        try
        {
            await _createBackup.HandleAsync(new CreateBackupCommand(dest), ct);
            BackupMessage = "✓ تم إنشاء النسخة الاحتياطية بنجاح.";
            BackupSuccess = true;
        }
        catch (Exception ex)
        {
            BackupMessage = $"خطأ: {ex.Message}";
            BackupSuccess = false;
        }
        finally
        {
            IsBackingUp = false;
        }
    }

    [RelayCommand]
    private void OpenRestoreConfirm()
    {
        var src = _fileDialog.PickFile(
            BackupFilter,
            title: "اختر ملف النسخة الاحتياطية للاستعادة");

        if (src is null) return;

        PendingRestorePath = src;
        RestoreMessage = null;
        IsRestoreConfirmOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmRestoreAsync(CancellationToken ct = default)
    {
        if (PendingRestorePath is null) return;

        IsRestoreConfirmOpen = false;
        IsRestoring = true;
        RestoreMessage = null;
        RestoreSuccess = false;
        try
        {
            await _restoreBackup.HandleAsync(new RestoreBackupCommand(PendingRestorePath), ct);
            RestoreSuccess = true;
            RestoreMessage = "✓ تمت الاستعادة. سيُعاد تشغيل التطبيق الآن...";
            await Task.Delay(1500, ct);
            RestartApplication();
        }
        catch (Exception ex)
        {
            RestoreMessage = $"خطأ: {ex.Message}";
            RestoreSuccess = false;
        }
        finally
        {
            IsRestoring = false;
        }
    }

    [RelayCommand]
    private void CancelRestore()
    {
        IsRestoreConfirmOpen = false;
        PendingRestorePath = null;
    }

    private static void RestartApplication()
    {
        var exe = Environment.ProcessPath;
        if (exe is not null)
            Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true });
        System.Windows.Application.Current.Shutdown();
    }
}
