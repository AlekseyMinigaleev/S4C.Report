using S4C.YandexGateway.DeveloperPageGateway.Models;

namespace S4C.YandexGateway.DeveloperPageGateway
{
    public interface IDeveloperPageGetaway
    {
        public Task<int[]> GetGameIdsAsync(
            CancellationToken cancellationToken = default);

        public Task<GameInfo[]> GetGameInfoAsync(
            int[] gameIds,
            CancellationToken cancellationToken = default);
    }
}