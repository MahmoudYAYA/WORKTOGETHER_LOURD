using System;
using System.Collections.Generic;

namespace WORKTOGETHER.DATA.Entities;

public partial class TicketSupport
{
    public int Id { get; set; }

    public string NumeroTicket { get; set; } = null!;

    public string Sujet { get; set; } = null!;

    public string? Description { get; set; }

    public int? Priorite { get; set; }

    public DateTime DateCreation { get; set; }

    public DateTime? DateFermeture { get; set; }

    public string StatutLabel => DateFermeture == null ? "🔴 Ouvert" : "✅ Fermé";
    public int? ClientId { get; set; }

    public virtual User? Client { get; set; }
}
