namespace C4S.Services.Interfaces
{
    public interface IGameDataService
    {
        public Task GetAllGameDataAsync(
            CancellationToken cancellationToken = default);
    }
}