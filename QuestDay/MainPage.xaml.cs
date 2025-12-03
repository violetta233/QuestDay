using System.Diagnostics;
namespace QuestDay
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = e.NewTextValue;
            Debug.WriteLine($"Введенный текст: {text}");
        }
    }
}

