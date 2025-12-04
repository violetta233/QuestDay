using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Logging;
using QuestDay.Converters;
using QuestDay.Models;
using QuestDay.Services;
using QuestDay.ViewModels;
using QuestDay.Views;

namespace QuestDay
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
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
            builder.Services.AddSingleton<IHabitService, HabitService>();
            builder.Services.AddSingleton<InverseBoolConverter>();

            builder.Services.AddTransient<HabitListViewModel>();
            builder.Services.AddTransient<AddHabitViewModel>();
            builder.Services.AddTransient<DaysViewModel>();

            builder.Services.AddTransient<ListPage>();
            builder.Services.AddTransient<AddPage>();
            builder.Services.AddSingleton<DaySelectedToColorConverter>();
            builder.Services.AddSingleton<DaySelectedToTextColorConverter>();

            return builder.Build();
        }
    }
}


