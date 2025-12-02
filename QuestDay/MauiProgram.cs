using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace QuestDay
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Montserrat-Black.ttf", "Montserrat-Black");
                    fonts.AddFont("Montserrat-Bold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Montserrat-SemiBold.ttf", "Montserrat-SemiBold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build(); 
        }
    }
}

