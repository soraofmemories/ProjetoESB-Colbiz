public class ConectorResultado
{
    public bool Successo { get; set; }
    public string? CorpoResposta { get; set; }
    public int StatusCode { get; set; }
    public string? ErroTecnico { get; set; }
    public bool IsTransientError { get; set; } = false; // para orquestrador decidir retry
}