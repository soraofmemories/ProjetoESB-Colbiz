namespace ProjetoESB.Dominio.Conectores;
public class ConectorResultado
{
    public bool Successo { get; set; }
    public string? CorpoResposta { get; set; }
    public int StatusCode { get; set; }
    public string? ErroTecnico { get; set; }
    public bool IsTransientError => StatusCode is >= 500 and < 600;// para orquestrador decidir retry
    public double? DuracaoMs { get; set; }

}