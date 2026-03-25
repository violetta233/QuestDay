using System;
using System.Threading.Tasks;
using QuestDay.Models;

namespace QuestDay.Services
{
    public interface IHouseStateService
    {
        Task<HouseState> GetCurrentStateAsync();
        Task UpdateStateAsync();
        Task CleanHouseAsync();
        Task AddDirtAsync(int amount);

        event EventHandler<string>? BackgroundChanged;
        event EventHandler<int>? DirtLevelChanged;
    }
}