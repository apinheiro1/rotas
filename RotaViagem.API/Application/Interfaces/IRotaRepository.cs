using Domain.Entities;

namespace Application.Interfaces
{
    public interface IRotaRepository
    {
        Task<List<Rota>> ObterTodasAsync();
        Task<Rota?> ObterPorIdAsync(string id);
        Task AdicionarAsync(Rota rota);
        Task AtualizarAsync(Rota rota);
        Task RemoverAsync(string id);
        Task<(List<Rota> caminho, int custoTotal)> ConsultarRotaMaisBarata(string origem, string destino);
    }

}
