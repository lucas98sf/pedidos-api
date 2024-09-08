using Microsoft.AspNetCore.Mvc;
using PedidosApi.Services;

namespace PedidosApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PedidosController(PedidoService pedidoService, ILogger<PedidosController> logger) : ControllerBase
    {
        private readonly PedidoService _pedidoService = pedidoService;
        private readonly ILogger<PedidosController> _logger = logger;

        [HttpGet("{codigoCliente}")]
        public async Task<IActionResult> GetPedidosByCliente(int codigoCliente)
        {
            _logger.LogInformation("Buscando pedidos para o cliente com código: {CodigoCliente}", codigoCliente);

            try
            {
                var pedidos = await _pedidoService.GetPedidosAsync();
                var pedidosCliente = pedidos.Where(p => p.codigoCliente == codigoCliente).ToList();

                _logger.LogInformation("Obtidos com sucesso {Count} pedidos para o cliente com código: {CodigoCliente}", pedidosCliente.Count, codigoCliente);
                return Ok(pedidosCliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ocorrido ao buscar pedidos para o cliente com código: {CodigoCliente}", codigoCliente);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("valorTotal/{codigoPedido}")]
        public async Task<IActionResult> GetValorTotalPedido(int codigoPedido)
        {
            _logger.LogInformation("Buscando valor total para o pedido com código: {CodigoPedido}", codigoPedido);

            try
            {
                var pedido = await _pedidoService.GetPedidoAsync(codigoPedido);
                if (pedido == null)
                {
                    _logger.LogWarning("Pedido com código: {CodigoPedido} não encontrado", codigoPedido);
                    return NotFound();
                }

                _logger.LogInformation("Valor total calculado para o pedido com código: {CodigoPedido} é {ValorTotal}", codigoPedido, pedido.valorTotal);
                return Ok(pedido.valorTotal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ocorrido ao buscar valor total para o pedido com código: {CodigoPedido}", codigoPedido);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("quantidadePorCliente/{codigoCliente}")]
        public async Task<IActionResult> GetQuantidadePedidosPorCliente(int codigoCliente)
        {
            _logger.LogInformation("Buscando quantidade de pedidos para o cliente com código: {CodigoCliente}", codigoCliente);

            try
            {
                var pedidos = await _pedidoService.GetPedidosAsync();
                var quantidade = pedidos.Count(p => p.codigoCliente == codigoCliente);

                _logger.LogInformation("Quantidade de pedidos calculada para o cliente com código: {CodigoCliente} é {Quantidade}", codigoCliente, quantidade);
                return Ok(quantidade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ocorrido ao buscar quantidade de pedidos para o cliente com código: {CodigoCliente}", codigoCliente);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}