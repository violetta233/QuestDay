using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using QuestDay.Services;
using QuestDay.Models;
using QuestDay.Resources;
namespace QuestDay
{
    public partial class App : Application
    {
        private readonly IHabitService _habitService;

        public App(IHabitService habitService)
        {
            InitializeComponent();
            _habitService = habitService;

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
    }
}