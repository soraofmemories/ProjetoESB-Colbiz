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
    public class LogExecucaoRepositorio : ILogExecucaoRepositorio
    {
        private readonly ESBContext _context;

        public LogExecucaoRepositorio(ESBContext context)
        {
            _context = context;
        }

        public async Task GravarLogAsync(LogExecucao log)
        {
            log.DataHora = DateTime.Now;
            _context.LogsExecucao.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LogExecucao>> ListarLogsAsync(int integracaoId)
        {
            return await _context.LogsExecucao
                .Where(l => l.IntegracaoId == integracaoId)
                .OrderByDescending(l => l.DataHora)
                .ToListAsync();
        }
    }
}
