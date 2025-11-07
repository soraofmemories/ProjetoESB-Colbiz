using ProjetoESB.Infra.Conectores;

namespace ProjetoESB.Dominio.Conectores;
public interface IConector
{
    Task<ConectorResultado> ExecutarAsync(ConectorRequisicao requisicao, CancellationToken token = default);
    //Task<bool> TestarConexaoAsync();
    //Task<string> GerarDescricaoTecnicaAsync();
}