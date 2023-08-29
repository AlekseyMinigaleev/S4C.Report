using C4S.DB.Models;

namespace C4S.Services.Interfaces
{
    public interface IGetGameDataService
    {
        public Task GetAllGameDataAsync();
    }
}
