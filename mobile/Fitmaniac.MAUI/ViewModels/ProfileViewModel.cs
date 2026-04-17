using Fitmaniac.Shared.DTOs.Users;

namespace Fitmaniac.MAUI.ViewModels;

public partial class ProfileViewModel(IAuthService authService) : BaseViewModel
{
    [RelayCommand]
    private async Task LogoutAsync()
    {
        await authService.LogoutAsync();
        await Shell.Current.GoToAsync("//login");
    }
}
