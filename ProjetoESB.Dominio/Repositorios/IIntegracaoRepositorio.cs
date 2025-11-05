using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjetoESB.Dominio.Entidades;


namespace ProjetoESB.Dominio.Repositorios
{
    // Define os métodos que o Repositório de Integrações DEVE implementar
    public interface IIntegracaoRepositorio
    {
        // 1. Método para listar todas as integrações
        Task<IEnumerable<Integracao>> GetAllAsync();

        // 2. Método para buscar uma integração por ID
        Task<Integracao?> GetByIdAsync(int id);

        // 3. Método para adicionar uma nova integração
        Task AddAsync(Integracao integracao);

        // Adicionaremos os métodos Update e Delete futuramente
    }
}
