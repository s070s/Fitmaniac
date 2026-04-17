namespace Fitmaniac.MAUI.Pages;

public partial class ProgressPage : ContentPage
{
    private readonly ProgressViewModel _vm;

    public ProgressPage(ProgressViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.LoadCommand.Execute(null);
    }
}
