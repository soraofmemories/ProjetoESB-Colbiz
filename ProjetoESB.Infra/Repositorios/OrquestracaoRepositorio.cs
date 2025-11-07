using Microsoft.EntityFrameworkCore;
using ProjetoESB.Dominio.Entidades;
using ProjetoESB.Dominio.Repositorios;
using ProjetoESB.Infra.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoESB.Infra.Repositorios
{
    public class OrquestracaoRepositorio : IOrquestracaoRepositorio
    {
        private readonly ESBContext _ctx;
        public OrquestracaoRepositorio(ESBContext ctx) => _ctx = ctx;

        public async Task<Orquestracao?> ObterOrquestracaoAsync(int id)
        {
            return await _ctx.Orquestracoes
                .Include(o => o.PassosOrquestracao)
                .ThenInclude(p => p.Conector)
                .FirstOrDefaultAsync(o => o.OrquestracaoId == id);
        }
    }
}
