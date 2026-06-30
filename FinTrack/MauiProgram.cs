using Microsoft.Extensions.Logging;
using Microcharts.Maui; // Importação da biblioteca gráfica

namespace FinTrack;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Correção aqui: O método correto é CreateBuilder e não CreateMauiAppBuilder
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMicrocharts() // Registra o componente de gráfico para o Android usar
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}