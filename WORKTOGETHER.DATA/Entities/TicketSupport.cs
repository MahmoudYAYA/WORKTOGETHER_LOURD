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

    // C'est la date de fermeture que je consome dans le formulaires 
    public string StatutLabel => DateFermeture == null ? " Ouvert" : " Fermé";

    public DateTime? DateFermeture { get; set; }

    public int? ClientId { get; set; }

    public virtual User? Client { get; set; }
}
