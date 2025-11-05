using Microsoft.EntityFrameworkCore;
using ProjetoESB.Dominio.Entidades;
using ProjetoESB.Dominio.Repositorios; // Para implementar a interface
using ProjetoESB.Infra.Contexts; // Para usar o ESBContext


namespace ProjetoESB.Infra.Repositorios
{
    // A classe Implementa a Interface e Herda de IIntegracaoRepositorio
    public class IntegracaoRepositorio : IIntegracaoRepositorio
    {
        // Precisamos do ESBContext para acessar o banco, injetado via construtor
        private readonly ESBContext _context;

        // Injeção de Dependência (quem fornece o Context é o .NET)
        public IntegracaoRepositorio(ESBContext context)
        {
            _context = context;
        }

        // Implementação do Método GetAllAsync (usando EF Core)
        public async Task<IEnumerable<Integracao>> GetAllAsync()
        {
            return await _context.Integracoes.ToListAsync();
        }

        // Implementação do Método GetByIdAsync (usando EF Core)
        public async Task<Integracao?> GetByIdAsync(int id)
        {
            return await _context.Integracoes.FirstOrDefaultAsync(i => i.IntegracaoId == id);
        }

        // Implementação do Método AddAsync
        public async Task AddAsync(Integracao integracao)
        {
            _context.Integracoes.Add(integracao);
            await _context.SaveChangesAsync(); // Commit no banco
        }
    }
}
