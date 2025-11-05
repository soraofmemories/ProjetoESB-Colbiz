using System;
using System.Collections.Generic;

namespace ProjetoESB.Dominio.Entidades;

public partial class Orquestracao
{
    public int OrquestracaoId { get; set; }

    public string? Nome { get; set; }

    public string? Descricao { get; set; }

    public virtual ICollection<PassoOrquestracao> PassoOrquestracao { get; set; } = new List<PassoOrquestracao>();
}
