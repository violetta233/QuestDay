using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using QuestDay.Services;
using QuestDay.Models;
using QuestDay.Resources;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
namespace QuestDay
{
    public partial class App : Application
    {
        private readonly IHabitService _habitService;

        public App(IHabitService habitService)
        {
            InitializeComponent();
            _habitService = habitService;
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationTapped;

            MainPage = new AppShell();
            Task.Run(async () =>
            {
                await _habitService.InitializeAsync();
            }).Wait();

        }

        protected override async void OnStart()
        {
            base.OnStart();
            await _habitService.InitializeAsync();
        }

        private void OnNotificationTapped(NotificationActionEventArgs e)
        {
            if (e.IsTapped)
            {
                // Логика перехода на нужную страницу на основе e.Request.ReturningData
                // Shell.Current.GoToAsync("///DetailsPage");
            }
        }        
    }
}