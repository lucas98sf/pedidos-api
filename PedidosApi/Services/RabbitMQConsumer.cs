using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using PedidosApi.Models;

namespace PedidosApi.Services
{
    public class RabbitMQConsumer(PedidoService pedidoService, ILogger<RabbitMQConsumer> logger)
    {
        private readonly PedidoService _pedidoService = pedidoService;
        private readonly ILogger<RabbitMQConsumer> _logger = logger;
        private readonly CancellationTokenSource _cts = new();

        public void Start()
        {
            Task.Run(() => ConsumeMessages(_cts.Token));
        }

        private void ConsumeMessages(CancellationToken token)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                _logger.LogInformation("Declarando a fila...");
                channel.QueueDeclare(queue: "pedidos", durable: false, exclusive: false, autoDelete: false, arguments: null);

                _logger.LogInformation("Fila declarada. Iniciando consumidor...");
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var pedido = JsonConvert.DeserializeObject<Pedido>(message);

                    _logger.LogInformation("Mensagem recebida do RabbitMQ: {Message}", message);

                    if (pedido == null)
                    {
                        _logger.LogWarning("Mensagem inválida recebida do RabbitMQ: {Message}", message);
                        return;
                    }

                    try
                    {
                        await _pedidoService.CreatePedidoAsync(pedido);
                        _logger.LogInformation("Pedido processado com sucesso com Código: {Codigo}", pedido.codigoPedido);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ocorrido ao processar o pedido com Código: {Codigo}", pedido.codigoPedido);
                    }
                };

                channel.BasicConsume(queue: "pedidos", autoAck: true, consumer: consumer);
                _logger.LogInformation("Consumidor iniciado e ouvindo a fila: pedidos");

                // Mantém o consumidor rodando
                while (!token.IsCancellationRequested)
                {
                    Task.Delay(1000, token).Wait(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ocorrido ao iniciar o consumidor do RabbitMQ.");
            }
        }
    }
}