using Application.Interfaces;
using Domain.Entities;


namespace Application.Services
{
    public class RotaService
    {
        private readonly IRotaRepository _repository;

        public RotaService(IRotaRepository repository)
        {
            _repository = repository;
        }

        public Task<List<Rota>> ListarRotasAsync() => _repository.ObterTodasAsync();

        public Task<Rota?> BuscarPorIdAsync(string id) => _repository.ObterPorIdAsync(id);

        public Task CriarRotaAsync(Rota rota) => _repository.AdicionarAsync(rota);

        public Task AtualizarRotaAsync(Rota rota) => _repository.AtualizarAsync(rota);

        public Task RemoverRotaAsync(string id) => _repository.RemoverAsync(id);

        public Task<(List<Rota> caminho, int custoTotal)> ConsultarRotaMaisBarata(string origem, string destino) => _repository.ConsultarRotaMaisBarata(origem,destino);
    }

}
