using System;
using System.Collections.Generic;

namespace WORKTOGETHER.DATA.Entities;

public partial class Unite
{
    public int Id { get; set; }

    public string NumeroUnite { get; set; } = null!;

    public string NomUnite { get; set; } = null!;

    public string Etat { get; set; } = null!;

    public string Statut { get; set; } = null!;

    public int BaieId { get; set; }

    public int? ReservationId { get; set; }

    public string? NomPersonnalise { get; set; }

    public string? TypeUnite { get; set; }

    public string? Couleur { get; set; }

    public virtual Baie Baie { get; set; } = null!;

    public virtual ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();

    public virtual Reservation? Reservation { get; set; }
}
