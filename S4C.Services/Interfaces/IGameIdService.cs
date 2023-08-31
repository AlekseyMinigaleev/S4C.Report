namespace C4S.Services.Interfaces
{
    public interface IGameIdService
    {
        public Task GetAllGameIdAsync(
            CancellationToken cancellationToken = default);
    }
}