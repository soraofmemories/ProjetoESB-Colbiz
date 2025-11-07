using Microsoft.AspNetCore.Mvc;
using ProjetoESB.Core.Services;

namespace ProjetoESB.Api.Controllers
{
    [ApiController]
    [Route("api/orquestrador")]
    public class OrquestradorController : ControllerBase
    {
        private readonly OrquestradorService _orquestrador;

        public OrquestradorController(OrquestradorService orquestrador)
        {
            _orquestrador = orquestrador;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Executar(int id)
        {
            await _orquestrador.ExecutarOrquestracaoAsync(id);
            return Ok(new { mensagem = $"Orquestração {id} executada." });
        }
    }
}
