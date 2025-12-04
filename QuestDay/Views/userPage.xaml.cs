using QuestDay.Views;
using System.Diagnostics;

namespace QuestDay.Views
{
    public partial class userPage : ContentPage
    {
        public userPage()
        {
            InitializeComponent();
        }

        private void OnHatButtonClicked(object sender, EventArgs e)
        {
            // Например: Navigation.PushAsync(new HatSelectionPage());
            Debug.WriteLine("Hat button clicked");
        }

        private void OnTopButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Top button clicked");
        }

        private void OnBottomButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Bottom button clicked");
        }
    }
}