namespace FinTrack;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Ignoramos o AppShell. 
        // Se o seu DatabaseService precisar ser passado, faremos isso via MainPage diretamente.
        return new Window(new MainPage(new FinTrack.Services.DatabaseService()));
    }
}