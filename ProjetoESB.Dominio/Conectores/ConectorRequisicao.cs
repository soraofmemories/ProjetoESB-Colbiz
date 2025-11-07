using System.Collections.Generic;
using System.Net.Http;

namespace ProjetoESB.Infra.Conectores
{
    public class ConectorRequisicao
    {
        public int IntegracaoId { get; set; }           // id da integração / orquestração
        public int ConectorId { get; set; }             // id do conector sendo usado
        public string Endpoint { get; set; }            // url completa
        public HttpMethod Metodo { get; set; }          // HttpMethod.Get/Post/...
        public string Body { get; set; }             // corpo (json/xml)
        public IDictionary<string, string>? Headers { get; set; } // headers opcionais
        public string? CorrelationId { get; set; }      // id de correlação para logs/idempotência
        public int MaxRetries { get; set; } = 3;
        public IDictionary<string, object>? Extra { get; set; } // espaço para extensões
    }
}