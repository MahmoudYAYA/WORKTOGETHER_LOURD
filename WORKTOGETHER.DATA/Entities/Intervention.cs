using System;
using System.Collections.Generic;

namespace WORKTOGETHER.DATA.Entities;

public partial class Intervention
{
    public int Id { get; set; }

    public string Titre { get; set; } = null!;

    public int Type { get; set; }

    public string? Description { get; set; }

    public DateTime? DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    public string TypeLabel => Type == 1 ? "Maintenance" : Type == 2 ? "Remplacement" : "Autre";
    public string StatutLabel => Statut == "en_cours" ? "🔧 En cours" : "✅ Terminée";
    public string Statut { get; set; } = null!;

    public int? UniteId { get; set; }

    public virtual Unite? Unite { get; set; }
}
