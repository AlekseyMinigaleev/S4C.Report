using S4C.YandexGateway.DeveloperPageGateway.Models;

namespace S4C.YandexGateway.DeveloperPageGateway
{
    public interface IDeveloperPageGetaway
    {
        public Task<int[]> GetGameIdsAsync();

        public Task<GameInfo[]> GetGameInfoAsync(int[] gameIds);
    }
}
