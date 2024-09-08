using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PedidosApi.Models
{
    public class Pedido
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        public int codigoPedido { get; set; }
        public int codigoCliente { get; set; }
        public required List<ItemPedido> itens { get; set; }

        public decimal valorTotal => itens.Sum(i => i.quantidade * i.preco);
    }

    public class ItemPedido
    {
        public required string produto { get; set; }
        public int quantidade { get; set; }
        public decimal preco { get; set; }
    }
}