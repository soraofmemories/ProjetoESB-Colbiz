using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjetoESB.Dominio.Repositorios;
using ProjetoESB.Infra.Conectores;
using ProjetoESB.Dominio.Entidades;

namespace ProjetoESB.Infra.Orquestracoes
{
    public class OrquestradorService
    {
        private readonly IConectorFactory _factory;
        private readonly IIntegracaoRepositorio _integracaoRepositorio;
        private readonly IOrquestracaoRepositorio _orquestracaoRepositorio;
        private readonly ILogExecucaoRepositorio _logRepositorio;
        private readonly ILogger<OrquestradorService> _logger;

        public OrquestradorService(
            IConectorFactory factory,
            IIntegracaoRepositorio integracaoRepositorio,
            IOrquestracaoRepositorio orquestracaoRepositorio,
            ILogExecucaoRepositorio logRepositorio,
            ILogger<OrquestradorService> logger)
        {
            _factory = factory;
            _integracaoRepositorio = integracaoRepositorio;
            _orquestracaoRepositorio = orquestracaoRepositorio;
            _logRepositorio = logRepositorio;
            _logger = logger;
        }

        public async Task ExecutarOrquestracaoAsync(int orquestracaoId, CancellationToken ct = default)
        {
            _logger.LogInformation("Iniciando orquestração {id}", orquestracaoId);

            var orq = await _orquestracaoRepositorio.ObterOrquestracaoAsync(orquestracaoId);
            if (orq == null)
            {
                _logger.LogWarning("Orquestração {id} não encontrada.", orquestracaoId);
                return;
            }

            var passos = orq.PassosOrquestracao?.OrderBy(p => p.Ordem).ToList();
            if (passos == null || passos.Count == 0)
            {
                _logger.LogWarning("Nenhum passo encontrado para a orquestração {id}.", orquestracaoId);
                return;
            }

            foreach (var passo in passos)
            {
                var conector = passo.Conector;
                _logger.LogInformation("Executando passo {passoId} -> Conector {conId} ({tipo})",
                    passo.PassoId, conector.ConectorId, conector.Tipo);

                var impl = _factory.Resolve(conector.Tipo ?? "HTTP");

                var requisicao = new ConectorRequisicao
                {
                    IntegracaoId = conector.IntegracaoId ?? 0,
                    ConectorId = conector.ConectorId,
                    Endpoint = conector.Endpoint ?? "",
                    Metodo = new HttpMethod(conector.Metodo ?? "GET"),
                    CorrelationId = Guid.NewGuid().ToString()
                };

                var resultado = await impl.ExecutarAsync(requisicao, ct);

                await _logRepositorio.GravarLogAsync(new LogExecucao
                {
                    IntegracaoId = conector.IntegracaoId ?? 0,
                    DataHora = DateTime.Now,
                    Status = resultado.Successo ? "Sucesso" : "Erro",
                    Mensagem = resultado.Successo
                        ? $"Conector {conector.ConectorId} executado com sucesso"
                        : resultado.ErroTecnico ?? "Erro desconhecido"
                });

                if (!resultado.Successo && !resultado.IsTransientError)
                {
                    _logger.LogError("Falha não transitória em {conector}. Abortando execução.", conector.ConectorId);
                    break;
                }
            }

            _logger.LogInformation("Orquestração {id} finalizada.", orquestracaoId);
        }
    }
}
