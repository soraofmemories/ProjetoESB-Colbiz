using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using System.Diagnostics;
using System.Text;
using ProjetoESB.Dominio.Conectores;

namespace ProjetoESB.Infra.Conectores
{
    public class ConectorRest : IConector
    {
        private readonly HttpClient _http;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly AsyncTimeoutPolicy _timeoutPolicy;

        private readonly ILogger<ConectorRest> _logger;

        public ConectorRest(HttpClient http, ILogger<ConectorRest> logger)
        {
            _http = http;
            _logger = logger;

            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            _timeoutPolicy = Policy.TimeoutAsync(15); // 15 segundos
        }

        public async Task<ConectorResultado> ExecutarAsync(ConectorRequisicao requisicao, CancellationToken ct = default)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var metodo = requisicao.Metodo ?? HttpMethod.Get;
                var request = new HttpRequestMessage(metodo, requisicao.Endpoint);

                // Headers
                if (requisicao.Headers != null)
                {
                    foreach (var h in requisicao.Headers)
                        request.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }

                // Correlation ID Header (padrão)
                if (!string.IsNullOrEmpty(requisicao.CorrelationId))
                    request.Headers.TryAddWithoutValidation("x-correlation-id", requisicao.CorrelationId);

                // Content
                if (!string.IsNullOrEmpty(requisicao.Body) && (requisicao.Metodo == HttpMethod.Post) || requisicao.Metodo == HttpMethod.Put || requisicao.Metodo == HttpMethod.Patch)
                
                    request.Content = new StringContent(requisicao.Body, Encoding.UTF8, "application/json");


                // Send
                var resp = await _timeoutPolicy.ExecuteAsync(
                    async token => await _retryPolicy.ExecuteAsync(ct2 => _http.SendAsync(request, ct2), token),
                    ct
                );
                var content = resp.Content != null ? await resp.Content.ReadAsStringAsync(ct) : null;

                sw.Stop();

                _logger.LogInformation("Conector REST: {Método} {Url} => {Status} (levou {ms}ms) CorrelationId={cid}",
                    requisicao.Metodo, requisicao.Endpoint, (int)resp.StatusCode, sw.ElapsedMilliseconds, requisicao.CorrelationId);

                return new ConectorResultado
                {
                    Successo = resp.IsSuccessStatusCode,
                    CorpoResposta = content,
                    StatusCode = (int)resp.StatusCode,
                    ErroTecnico = resp.IsSuccessStatusCode ? null : $"HTTP {(int)resp.StatusCode}: {resp.ReasonPhrase}",
                    DuracaoMs = sw.Elapsed.TotalMilliseconds
                    //IsTransientError = (int)resp.StatusCode >= 500 // exemplo simples: 5xx -> transitório
                };
            }
            catch (OperationCanceledException oce) when (ct.IsCancellationRequested)
            {
                _logger.LogWarning(oce, "Conector REST cancelado para {Url} (CorrelationId={cid}", requisicao.Endpoint, requisicao.CorrelationId);
                return new ConectorResultado { Successo = false, ErroTecnico = "Cancelado"};
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Conector REST erro ao chamar {Url} (CorrelationId={cid})", requisicao.Endpoint, requisicao.CorrelationId);
                // Exceptions de rede costumam ser transitórias
                return new ConectorResultado { Successo = true, ErroTecnico = ex.Message, DuracaoMs = sw.Elapsed.TotalMilliseconds };
            }
        }
    }
}
