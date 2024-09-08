using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;
using PedidosApi.Models;
using System.Net.Http.Json;

namespace PedidosApi.Tests
{
    public class PedidosIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly MongoDbRunner _mongoDbRunner;
        private readonly IMongoDatabase _database;

        public PedidosIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _mongoDbRunner = MongoDbRunner.Start();
            var mongoClient = new MongoClient(_mongoDbRunner.ConnectionString);
            _database = mongoClient.GetDatabase("PedidosDb");

            var testFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IMongoClient>(mongoClient);
                });
            });

            _client = testFactory.CreateClient();
        }

        public void Dispose()
        {
            _mongoDbRunner.Dispose();
        }

        [Fact]
        public async Task GetPedidosByCliente_ReturnsPedidos_ForGivenClienteCode()
        {
            var pedido = new Pedido
            {
                codigoPedido = 1,
                codigoCliente = 123,
                itens =
                [
                    new ItemPedido { produto = "Produto1", quantidade = 2, preco = 10 },
                    new ItemPedido { produto = "Produto2", quantidade = 1, preco = 5 }
                ]
            };
            var pedidosCollection = _database.GetCollection<Pedido>("Pedidos");
            await pedidosCollection.InsertOneAsync(pedido);

            var response = await _client.GetAsync("/pedidos/123");
            response.EnsureSuccessStatusCode();

            var pedidos = await response.Content.ReadFromJsonAsync<List<Pedido>>();

            Assert.NotNull(pedidos);
            Assert.NotEmpty(pedidos);
            Assert.All(pedidos, p => Assert.Equal(123, p.codigoCliente));
        }

        [Fact]
        public async Task GetValorTotalPedido_ReturnsValorTotal_ForGivenPedidoCode()
        {
            var pedido = new Pedido
            {
                codigoPedido = 1,
                codigoCliente = 123,
                itens =
                [
                    new ItemPedido { produto = "Produto1", quantidade = 2, preco = 10 },
                    new ItemPedido { produto = "Produto2", quantidade = 1, preco = 5 }
                ]
            };
            var pedidosCollection = _database.GetCollection<Pedido>("Pedidos");
            await pedidosCollection.InsertOneAsync(pedido);

            var response = await _client.GetAsync("/pedidos/valorTotal/1");
            response.EnsureSuccessStatusCode();

            var valorTotal = await response.Content.ReadFromJsonAsync<decimal>();

            Assert.Equal(25.0m, valorTotal); // 2 * 10 + 1 * 5
        }

        [Fact]
        public async Task GetQuantidadePedidosPorCliente_ReturnsQuantity_ForGivenClienteCode()
        {
            var pedido1 = new Pedido
            {
                codigoPedido = 1,
                codigoCliente = 123,
                itens =
                [
                    new ItemPedido { produto = "Produto1", quantidade = 2, preco = 10 },
                    new ItemPedido { produto = "Produto2", quantidade = 1, preco = 5 }
                ]
            };

            var pedido2 = new Pedido
            {
                codigoPedido = 2,
                codigoCliente = 123,
                itens =
                [
                    new ItemPedido { produto = "Produto3", quantidade = 3, preco = 7 },
                ]
            };

            var pedidosCollection = _database.GetCollection<Pedido>("Pedidos");
            await pedidosCollection.InsertManyAsync([pedido1, pedido2]);

            var response = await _client.GetAsync("/pedidos/quantidadePorCliente/123");
            response.EnsureSuccessStatusCode();

            var quantidade = await response.Content.ReadFromJsonAsync<int>();

            Assert.Equal(2, quantidade);
        }
    }
}