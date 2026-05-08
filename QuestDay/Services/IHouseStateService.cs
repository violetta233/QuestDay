using System;
using System.Threading.Tasks;
using QuestDay.Models;

namespace QuestDay.Services
{
    public interface IHouseStateService
    {
        event EventHandler<string>? BackgroundChanged;
        event EventHandler<string>? UserPageBackgroundChanged;
        event EventHandler<int>? DirtLevelChanged;
        event EventHandler<bool>? RabbitDirtyStateChanged;

        Task UpdateStateAsync();
        Task<HouseState> GetCurrentStateAsync();
        Task CleanHouseAsync();
        Task AddDirtAsync(int amount);
        Task ResetIncompletionStartTime();
    }
}