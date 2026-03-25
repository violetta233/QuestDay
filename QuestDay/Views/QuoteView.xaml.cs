using QuestDay.Services;

namespace QuestDay.Views
{
    public partial class QuoteView : ContentView
    {
        private readonly IQuoteService _quoteService = new QuoteService();

        public QuoteView()
        {
            InitializeComponent();
            ShowRandomQuote();
        }

        private async void ShowRandomQuote()
        {
            var quote = await _quoteService.GetRandomQuoteAsync();

            if (quote != null)
            {
                QuoteLabel.Text = quote.Text;
                QuoteContainer.IsVisible = true;
            }
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            QuoteContainer.IsVisible = false;
        }
    }
}