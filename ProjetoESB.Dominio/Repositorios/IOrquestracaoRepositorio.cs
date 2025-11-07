using ProjetoESB.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoESB.Dominio.Repositorios
{
    public interface IOrquestracaoRepositorio
    {
        Task<Orquestracao?> ObterOrquestracaoAsync(int id);
    }
}
