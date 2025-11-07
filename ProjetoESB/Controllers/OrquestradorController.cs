using Microsoft.AspNetCore.Mvc;
using ProjetoESB.Infra.Orquestracoes;

namespace ProjetoESB.Api.Controllers
{
    [ApiController]
    [Route("api/orquestrador")]
    public class OrquestradorController : ControllerBase
    {
        private readonly OrquestradorService _service;

        public OrquestradorController(OrquestradorService service)
        {
            _service = service;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Executar(int id)
        {
            await _service.ExecutarOrquestracaoAsync(id);
            return Ok($"Orquestração {id} executada.");
        }
    }
}
