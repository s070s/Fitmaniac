namespace Fitmaniac.MAUI.ViewModels;

public partial class LoginViewModel(IAuthService authService) : BaseViewModel
{
    private string _email = string.Empty;
    private string _password = string.Empty;

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
    private async Task LoginAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            var result = await authService.LoginAsync(new LoginRequestDto(Email, Password));
            if (result is not null)
                await Shell.Current.GoToAsync("//home");
            else
                ErrorMessage = "Invalid credentials.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task GoToRegisterAsync() => Shell.Current.GoToAsync("//register");
}
