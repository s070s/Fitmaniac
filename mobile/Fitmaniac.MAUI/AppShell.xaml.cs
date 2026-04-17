namespace Fitmaniac.MAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("workoutdetail", typeof(Pages.WorkoutDetailPage));
    }
}
