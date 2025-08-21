using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class RotaRepository : IRotaRepository
    {
        private readonly string _caminhoArquivo = Path.Combine(AppContext.BaseDirectory, "data", "rotas.json");       

        public async Task<(List<Rota> caminho, int custoTotal)> ConsultarRotaMaisBarata(string origem, string destino)
        {
            if (!File.Exists(_caminhoArquivo))
                return (new List<Rota>(), -1);

            var json = await File.ReadAllTextAsync(_caminhoArquivo);
            var trechos = JsonSerializer.Deserialize<List<Rota>>(json) ?? new List<Rota>();
           
            var grafo = new Dictionary<string, List<(string destino, int custo)>>();

            foreach (var trecho in trechos)
            {
                if (!grafo.ContainsKey(trecho.origem))
                    grafo[trecho.origem] = new List<(string, int)>();

                grafo[trecho.origem].Add((trecho.destino, trecho.valor));
            }

            var distancias = new Dictionary<string, int>();
            var anteriores = new Dictionary<string, string>();
            var visitados = new HashSet<string>();
            var fila = new PriorityQueue<string, int>();

            foreach (var aeroporto in grafo.Keys)
                distancias[aeroporto] = int.MaxValue;

            distancias[origem] = 0;
            fila.Enqueue(origem, 0);

            while (fila.Count > 0)
            {
                string atual = fila.Dequeue();

                if (visitados.Contains(atual))
                    continue;

                visitados.Add(atual);

                if (!grafo.ContainsKey(atual))
                    continue;

                foreach (var (vizinho, custo) in grafo[atual])
                {
                    var novaDistancia = distancias[atual] + custo;

                    if (novaDistancia < distancias.GetValueOrDefault(vizinho, int.MaxValue))
                    {
                        distancias[vizinho] = novaDistancia;
                        anteriores[vizinho] = atual;
                        fila.Enqueue(vizinho, novaDistancia);
                    }
                }
            }

            if (!distancias.ContainsKey(destino) || distancias[destino] == int.MaxValue)
                return (new List<Rota>(), -1);
           
            var caminhoAeroportos = new List<string>();
            var aeroportoAtual = destino;

            while (aeroportoAtual != origem)
            {
                caminhoAeroportos.Insert(0, aeroportoAtual);
                aeroportoAtual = anteriores[aeroportoAtual];
            }

            caminhoAeroportos.Insert(0, origem);

            var caminhoFinal = new List<Rota>();
            int custoTotal = 0;

            for (int i = 0; i < caminhoAeroportos.Count - 1; i++)
            {
                var origemAtual = caminhoAeroportos[i];
                var destinoAtual = caminhoAeroportos[i + 1];

                var trecho = trechos.FirstOrDefault(t => t.origem == origemAtual && t.destino == destinoAtual);
                if (trecho != null)
                {
                    caminhoFinal.Add(trecho);
                    custoTotal += trecho.valor;
                }
            }

            return (caminhoFinal, custoTotal);
        }

        public async Task<List<Rota>> ObterTodasAsync()
        {
            if (!File.Exists(_caminhoArquivo))
                return new List<Rota>();

            var json = await File.ReadAllTextAsync(_caminhoArquivo);
            return JsonSerializer.Deserialize<List<Rota>>(json) ?? new List<Rota>();
        }

        public async Task<Rota?> ObterPorIdAsync(string id)
        {
            var rotas = await ObterTodasAsync();
            return rotas.FirstOrDefault(r => r.id == id);
        }

        public async Task AdicionarAsync(Rota rota)
        {
            var rotas = await ObterTodasAsync();
            rota.id = Guid.NewGuid().ToString();
            rotas.Add(rota);
            await SalvarAsync(rotas);
        }

        public async Task AtualizarAsync(Rota rota)
        {
            var rotas = await ObterTodasAsync();
            var index = rotas.FindIndex(r => r.id == rota.id);
            if (index >= 0)
            {
                rotas[index] = rota;
                await SalvarAsync(rotas);
            }
        }

        public async Task RemoverAsync(string id)
        {
            var rotas = await ObterTodasAsync();
            rotas.RemoveAll(r => r.id == id);
            await SalvarAsync(rotas);
        }

        private async Task SalvarAsync(List<Rota> rotas)
        {
            var json = JsonSerializer.Serialize(rotas, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_caminhoArquivo, json);
        }
    }
}
