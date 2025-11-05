using System;
using System.Collections.Generic;

namespace ProjetoESB.Dominio.Entidades;

public partial class PassoOrquestracao
{
    public int PassoId { get; set; }

    public int? OrquestracaoId { get; set; }

    public int? Ordem { get; set; }

    public int? ConectorId { get; set; }

    public virtual Conector? Conector { get; set; }

    public virtual Orquestracao? Orquestracao { get; set; }
}
