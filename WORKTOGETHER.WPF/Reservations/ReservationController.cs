using System.Collections.Generic;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Reservations
{
    public class ReservationController
    {
        private readonly ReservationRepository _repo = new ReservationRepository();

        // ── Récupère toutes les réservations ──
        public List<Reservation> GetAll()
        {
            return _repo.FindAllWithDetails();
        }
    }
}