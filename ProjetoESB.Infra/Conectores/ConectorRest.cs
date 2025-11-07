using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoESB.Infra.Conectores
{
    public class ConectorRest : IConector
    {
        private readonly HttpClient _http;
        private readonly ILogger<ConectorRest> _logger;

        public ConectorRest(HttpClient http, ILogger<ConectorRest> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<ConectorResultado> ExecutarAsync(ConectorRequisicao requisicao, CancellationToken token = default)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                using var req = new HttpRequestMessage(requisicao.Metodo, requisicao.Endpoint);

                // Headers
                if (requisicao.Headers != null)
                {
                    foreach (var h in requisicao.Headers)
                        req.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }

                // Correlation ID Header (padrão)
                if (!string.IsNullOrEmpty(requisicao.CorrelationId))
                    req.Headers.TryAddWithoutValidation("x-correlation-id", requisicao.CorrelationId);

                // Content
                if (!string.IsNullOrEmpty(requisicao.Body) && (requisicao.Metodo == HttpMethod.Post) || requisicao.Metodo == HttpMethod.Put || requisicao.Metodo == HttpMethod.Patch)
                
                    req.Content = new StringContent(requisicao.Body, Encoding.UTF8, "application/json");
                

                // Send
                var resp = await _http.SendAsync(req, token);
                var content = resp.Content != null ? await resp.Content.ReadAsStringAsync(token) : null;

                sw.Stop();
                _logger.LogInformation("Conector REST: {Método} {Url} => {Status} (levou {ms}ms) CorrelationId={cid}",
                    requisicao.Metodo, requisicao.Endpoint, (int)resp.StatusCode, sw.ElapsedMilliseconds, requisicao.CorrelationId);

                return new ConectorResultado
                {
                    Successo = resp.IsSuccessStatusCode,
                    CorpoResposta = content,
                    StatusCode = (int)resp.StatusCode,
                    ErroTecnico = resp.IsSuccessStatusCode ? null : $"HTTP {(int)resp.StatusCode}: {resp.ReasonPhrase}",
                    IsTransientError = (int)resp.StatusCode >= 500 // exemplo simples: 5xx -> transitório
                };
            }
            catch (OperationCanceledException oce) when (token.IsCancellationRequested)
            {
                _logger.LogWarning(oce, "Conector REST cancelado para {Url} (CorrelationId={cid}", requisicao.Endpoint, requisicao.CorrelationId);
                return new ConectorResultado { Successo = false, ErroTecnico = "Cancelado", IsTransientError = true };
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Conector REST erro ao chamar {Url} (CorrelationId={cid})", requisicao.Endpoint, requisicao.CorrelationId);
                // Exceptions de rede costumam ser transitórias
                return new ConectorResultado { Successo = true, ErroTecnico = ex.Message, IsTransientError = true };
            }
        }
    }
}
