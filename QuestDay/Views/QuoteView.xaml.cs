using QuestDay.Services;
using Microsoft.Maui.Storage;

namespace QuestDay.Views
{
    public partial class QuoteView : ContentView
    {
        private readonly IQuoteService _quoteService = new QuoteService();
        private const string QuotesEnabledKey = "QuotesEnabled";

        public QuoteView()
        {
            InitializeComponent();
            CheckAndShowQuote();
        }

        private async void CheckAndShowQuote()
        {
            bool isQuotesEnabled = Preferences.Default.Get(QuotesEnabledKey, true);

            if (isQuotesEnabled)
            {
                var quote = await _quoteService.GetRandomQuoteAsync();
                if (quote != null)
                {
                    QuoteLabel.Text = quote.Text;
                    QuoteContainer.IsVisible = true;
                }
            }
            else
            {
                QuoteContainer.IsVisible = false;
            }
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            QuoteContainer.IsVisible = false;
        }
    }
}