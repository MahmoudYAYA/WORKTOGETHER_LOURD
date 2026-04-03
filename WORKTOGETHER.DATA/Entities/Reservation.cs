using System;
using System.Collections.Generic;

namespace WORKTOGETHER.DATA.Entities;

public partial class Reservation
{
    public int Id { get; set; }

    public DateOnly DateDebut { get; set; }

    public DateOnly DateFin { get; set; }

    public double PrixTotal { get; set; }

    public int? ClientId { get; set; }

    public int OffreId { get; set; }

    public virtual User? Client { get; set; }

    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();

    public virtual Offre Offre { get; set; } = null!;

    public virtual ICollection<Unite> Unites { get; set; } = new List<Unite>();
}
