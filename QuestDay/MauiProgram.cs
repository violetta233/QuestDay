using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using QuestDay.Converters;
using QuestDay.Models;
using QuestDay.Services;
using QuestDay.Converters;
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
                .UseLocalNotification() // Регистрация сервиса для локальных уведомлений
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
            builder.Services.AddSingleton<AvatarAppearanceService>();

            // Регистрация конвертеров
            builder.Services.AddSingleton<InverseBoolConverter>();
            builder.Services.AddSingleton<DaySelectedToColorConverter>();
            builder.Services.AddSingleton<DaySelectedToTextColorConverter>();
            builder.Services.AddSingleton<EnabledToColorConverter>();
            builder.Services.AddSingleton<StringNotNullOrEmptyConverter>();
            builder.Services.AddSingleton<HabitIsActiveToBackgroundColorConverter>();
            builder.Services.AddSingleton<HabitIsActiveToTextStyleConverter>();

            // Регистрация ViewModels
            builder.Services.AddTransient<HabitListViewModel>();
            builder.Services.AddTransient<AddHabitViewModel>();
            builder.Services.AddTransient<DaysViewModel>();

            // Регистрация страниц
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<ListPage>();
            builder.Services.AddTransient<AddPage>();
            builder.Services.AddTransient<SettingPage>();
            builder.Services.AddTransient<userPage>();

            return builder.Build();
        }
    }
}
