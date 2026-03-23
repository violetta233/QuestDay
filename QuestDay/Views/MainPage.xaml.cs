namespace QuestDay.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = App.AvatarAppearance;
        }
    }
}
