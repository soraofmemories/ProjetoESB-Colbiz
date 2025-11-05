using System;
using System.Collections.Generic;

namespace ProjetoESB.Dominio.Entidades;

public partial class Integracao
{
    public int IntegracaoId { get; set; }

    public string? Nome { get; set; }

    public string? Descricao { get; set; }

    public bool? Ativo { get; set; }

    public virtual ICollection<Conector> Conectores { get; set; } = new List<Conector>();
}
