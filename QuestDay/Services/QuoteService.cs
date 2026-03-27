using QuestDay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestDay.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly List<Quote> _quotes = new List<Quote>
        {
            new Quote { Id = 1, Text = "Тихими шагами, уверенно к мечте" },
            new Quote { Id = 2, Text = "Пусть твоя вера в себя будет сильнее твоих страхов" },
            new Quote { Id = 3, Text = "Твоя энергия создает твою реальность" },
            new Quote { Id = 4, Text = "Мечты не работают, пока не работаешь ты" },
            new Quote { Id = 5, Text = "Чтобы дойти до цели, надо идти" },
            new Quote { Id = 6, Text = "Ты можешь всё, и дальше — больше" },
            new Quote { Id = 7, Text = "Нет легких путей, но есть легкое отношение к своему пути" },
            new Quote { Id = 8, Text = "Найди опору в самом себе, и ты станешь сильнее всех" }
        };

        public async Task<Quote> GetRandomQuoteAsync()
        {
            var random = new Random();
            int index = random.Next(_quotes.Count);
            return await Task.FromResult(_quotes[index]);
        }

        public async Task<List<Quote>> GetAllQuotesAsync()
        {
            return await Task.FromResult(_quotes);
        }
    }
}