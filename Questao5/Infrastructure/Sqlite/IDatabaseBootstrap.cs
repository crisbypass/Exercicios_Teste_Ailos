namespace Questao5.Infrastructure.Sqlite
{
    public interface IDatabaseBootstrap
    {
        Task SetupAsync();
    }
}