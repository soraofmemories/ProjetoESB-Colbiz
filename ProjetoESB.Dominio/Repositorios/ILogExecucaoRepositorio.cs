using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetoESB.Dominio.Entidades;


namespace ProjetoESB.Dominio.Repositorios
{
    public interface ILogExecucaoRepositorio
    {
        Task GravarLogAsync(LogExecucao log);
        Task<IEnumerable<LogExecucao>> ListarLogsAsync(int integracaoId);
    }
}
