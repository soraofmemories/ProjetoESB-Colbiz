using System;
using System.Collections.Generic;

namespace ProjetoESB.Dominio.Entidades;

public partial class LogExecucao
{
    public int LogId { get; set; }
    public int? IntegracaoId { get; set; }
    public DateTime? DataHora { get; set; }
    public string? Status { get; set; }
    public string? Mensagem { get; set; }
    public string? Detalhes { get; set; }
    public string? Endpoint { get; set; }
    public int? CodigoHttp { get; set; }
    public double? DuracaoMs { get; set; }
}
