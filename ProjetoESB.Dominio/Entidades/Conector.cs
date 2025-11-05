using System;
using System.Collections.Generic;

namespace ProjetoESB.Dominio.Entidades;

public partial class Conector
{
    public int ConectorId { get; set; }

    public int? IntegracaoId { get; set; }

    public string? Tipo { get; set; }

    public string? Endpoint { get; set; }

    public string? Metodo { get; set; }

    public virtual Integracao? Integracao { get; set; }

    public virtual ICollection<PassoOrquestracao> PassosOrquestracaos { get; set; } = new List<PassoOrquestracao>();
}
