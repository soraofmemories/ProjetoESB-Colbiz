
using ProjetoESB.Infra.Conectores;
using System.Threading;
using System.Threading.Tasks;

public interface IConector
{
    Task<ConectorResultado> ExecutarAsync(ConectorRequisicao requisicao, CancellationToken token = default);
    //Task<bool> TestarConexaoAsync();
    //Task<string> GerarDescricaoTecnicaAsync();
}