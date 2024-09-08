using MongoDB.Driver;
using PedidosApi.Models;

namespace PedidosApi.Services
{
    public class PedidoService
    {
        private readonly IMongoCollection<Pedido> _pedidosCollection;

        public PedidoService(IMongoClient client)
        {
            var database = client.GetDatabase("PedidosDb");
            _pedidosCollection = database.GetCollection<Pedido>("Pedidos");
        }

        public async Task<List<Pedido>> GetPedidosAsync()
        {
            return await _pedidosCollection.Find(pedido => true).ToListAsync();
        }

        public async Task<Pedido> GetPedidoAsync(int codigoPedido)
        {
            return await _pedidosCollection.Find(pedido => pedido.codigoPedido == codigoPedido).FirstOrDefaultAsync();
        }

        public async Task CreatePedidoAsync(Pedido pedido) =>
            await _pedidosCollection.InsertOneAsync(pedido);
    }
}