using System;
using System.Collections.Generic;

namespace WORKTOGETHER.DATA.Entities;

public partial class Baie
{
    public int Id { get; set; }

    public string NumeroBaie { get; set; } = null!;

    public int CapaciteTotale { get; set; }

    public int NbUnitesOccupees => Unites.Count(u => u.Statut == "occupe");
    public int NbUnitesDisponibles => Unites.Count(u => u.Statut == "disponible");

    public virtual ICollection<Unite> Unites { get; set; } = new List<Unite>();
}
