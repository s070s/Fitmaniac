namespace Fitmaniac.MAUI.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    private bool _isBusy;
    private string? _errorMessage;

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
                OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsNotBusy => !IsBusy;
}
