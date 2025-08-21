using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence;


using Xunit;

namespace Tests
{
    [TestClass]
    public sealed class Test1
    {

        [Fact]
        public async Task Deve_Adicionar_Rota_Corretamente()
        {
            // Arrange
            var repo = new RotaRepository(); // Idealmente, use um mock ou stub
            var service = new RotaService(repo);

            var rotas = new[]
            {
            new Rota { origem = "GRU", destino = "BRC", valor = 10 },
            new Rota { origem = "BRC", destino = "SCL", valor = 5 },
            new Rota { origem = "SCL", destino = "ORL", valor = 20 },
            new Rota { origem = "ORL", destino = "CDG", valor = 1 },
            new Rota { origem = "GRU", destino = "CDG", valor = 100 }
        };

            foreach (var rota in rotas)
            {
                await service.CriarRotaAsync(rota);
            }

            // Act
            var (caminho, custoTotal) = await service.ConsultarRotaMaisBarata("GRU", "CDG");

            var caminhoEsperado = new[] { "GRU", "BRC", "SCL", "ORL", "CDG" };
            var caminhoCalculado = caminho.Select(r => r.origem).ToList();
            caminhoCalculado.Add(caminho.Last().destino);
            Xunit.Assert.Equal(caminhoCalculado, caminhoEsperado);
            
        }


        [Fact]
        public async Task Deve_retornar_valor_40()
        {
            // Arrange
            var repo = new RotaRepository(); // Idealmente, use um mock ou stub
            var service = new RotaService(repo);
            // Act
            var (caminho, custoTotal) = await service.ConsultarRotaMaisBarata("GRU", "CDG");

            var caminhoEsperado = 40;
            var caminhoCalculado = caminho.FirstOrDefault().valor;           
            Xunit.Assert.Equal(caminhoCalculado, caminhoEsperado);
        }


    }
}
