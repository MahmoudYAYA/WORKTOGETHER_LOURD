using System;
using System.Collections.Generic;

namespace WORKTOGETHER.DATA.Entities;

public partial class Commande
{
    public int Id { get; set; }

    public string NumeroCommande { get; set; } = null!;

    public DateTime DateCommande { get; set; }

    public decimal MontantTotal { get; set; }

    public decimal MontantTva { get; set; }

    public DateOnly DateDebutService { get; set; }

    public DateOnly DateFinService { get; set; }

    public string? StripePaymentId { get; set; }

    public string? StripeCardLast4 { get; set; }

    public string? StripeCardBrand { get; set; }

    public string? StatutPaiement { get; set; }

    public string? TypePaiement { get; set; }

    public int? ClientId { get; set; }

    public int? OffreId { get; set; }

    public int? ReservationId { get; set; }

    public virtual User? Client { get; set; }

    public virtual Offre? Offre { get; set; }

    public virtual Reservation? Reservation { get; set; }
}
