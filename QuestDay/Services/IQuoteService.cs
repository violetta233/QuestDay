using QuestDay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestDay.Services
{
    public interface IQuoteService
    {
        Task<Quote> GetRandomQuoteAsync();
        Task<List<Quote>> GetAllQuotesAsync();
    }
}