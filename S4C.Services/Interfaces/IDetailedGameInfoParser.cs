using C4S.DB.Models;

namespace C4S.Services.Interfaces
{
    /*TODO: пока оставим так, в дальнейшем по идее надо сделать фабрику которая создает IDetailedGameInfoParser с проинициализированным url*/

    /// <summary>
    /// Перед вызовом методов, нужно проинициализировать Url, используя метод SetUrl
    /// </summary>
    public interface IDetailedGameInfoParser
    {
        public GameModel GetDetailedGameInfo();
        public Task ParseGameStatistic();

        public void SetUrl(string gameId);
    }
}
