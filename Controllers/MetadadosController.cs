using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoESB.Infra.Contexts;
using ProjetoESB.Dominio.Repositorios;  // Importa a Interface

namespace ProjetoESB.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetadadosController : ControllerBase
    {
        // Antes era assim
        // private readonly ESBContext _context;

        // Agora Injetarmos a Interface do Repositório
        private readonly IIntegracaoRepositorio _integracaoRepositorio;

        // O Framework injeta o ESBContext configurado no Program.cs (antes era assim)
        //public MetadadosController(ESBContext context)
        //{
        //    _context = context;
        //}

        // Na verdade o DI Container injeta a implementação do Repositório!
        public MetadadosController(IIntegracaoRepositorio integracaoRepositorio)
        {
            _integracaoRepositorio = integracaoRepositorio;
        }

        [HttpGet("integracoes")]
        public async Task<IActionResult> GetIntegracoes()
        {
            // Tenta ler dados da tabela Integracoes (Metadados do ESB)
            // Lembre-se que _context.Integracoes é um DbSet<Integracao>

            // O Controller usa apenas a REGRA de Domínio

            // var integracoes = await _context.Integracoes.ToListAsync();
            var integracoes = await _integracaoRepositorio.GetAllAsync();

            if (integracoes == null || !integracoes.Any())
            {
                // Retorna 404 se a tabela estiver vazia (o que é normal em um banco recém-criado)
                return NotFound("Nenhuma integração de metadados encontrada. A conexão está OK, mas a tabela está vazia.");
            }

            return Ok(integracoes);
        }
    }
}
