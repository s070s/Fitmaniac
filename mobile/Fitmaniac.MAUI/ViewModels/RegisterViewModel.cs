namespace Fitmaniac.MAUI.ViewModels;

public partial class RegisterViewModel(IAuthService authService) : BaseViewModel
{
    private string _username = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            var dto = new RegisterRequestDto(Username, Email, Password, "Client");
            var result = await authService.RegisterAsync(dto);
            if (result is not null)
                await Shell.Current.GoToAsync("//home");
            else
                ErrorMessage = "Registration failed.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task GoToLoginAsync() => Shell.Current.GoToAsync("//login");
}
