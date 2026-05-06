using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace QuestDay.Views
{
    public partial class BeautyPopup : ContentView
    {
        private TaskCompletionSource<bool> _tcs;

        public BeautyPopup()
        {
            InitializeComponent();

            ConfirmBtn.Clicked += OnConfirm;
            CancelBtn.Clicked += OnCancel;
        }

        private async void OnConfirm(object sender, EventArgs e)
        {
            ContentLayout.IsVisible = false;
            LoadingLayout.IsVisible = true;

            await Task.Delay(100);

            _tcs?.TrySetResult(true);
            await HideAsync();
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            _tcs?.TrySetResult(false);
            await HideAsync();
        }

        public async Task<bool> ShowAsync(string title, string message, string confirmText = "Да", string cancelText = "Нет", string icon = "✓")
        {
            ContentLayout.IsVisible = true;
            LoadingLayout.IsVisible = false;

            TitleLabel.Text = title;
            MessageLabel.Text = message;
            ConfirmBtn.Text = confirmText;
            CancelBtn.Text = cancelText;
            IconLabel.Text = icon;

            // Цвет иконки
            if (icon == "✓" || icon == "✅")
                IconLabel.TextColor = Color.FromArgb("#A7DFAF");
            else if (icon == "❌" || icon == "✕")
                IconLabel.TextColor = Color.FromArgb("#FF5252");
            else if (icon == "📅")
                IconLabel.TextColor = Color.FromArgb("#F68063");
            else if (icon == "⚠️")
                IconLabel.TextColor = Color.FromArgb("#F39C12");
            else
                IconLabel.TextColor = Color.FromArgb("#A7DFAF");

            if (string.IsNullOrEmpty(cancelText))
            {
                CancelBtn.IsVisible = false;
            }
            else
            {
                CancelBtn.IsVisible = true;
            }

            _tcs = new TaskCompletionSource<bool>();

            this.IsVisible = true;

            this.Scale = 0.85;
            this.Opacity = 0;
            await this.ScaleTo(1, 200, Easing.SpringOut);
            await this.FadeTo(1, 150);

            return await _tcs.Task;
        }

        private async Task HideAsync()
        {
            await this.FadeTo(0, 150);
            await this.ScaleTo(0.9, 100);
            this.IsVisible = false;

            ContentLayout.IsVisible = true;
            LoadingLayout.IsVisible = false;
        }
    }
}