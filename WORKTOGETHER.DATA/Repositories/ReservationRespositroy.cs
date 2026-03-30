using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.DATA.Repositories
{
    public class ReservationRepository : Repository<Reservation>
    {
        /// <summary>
        /// methode pour trouver toutes les réservations d'un client donné, en incluant les données de l'offre associée à chaque réservation et les unités associées à chaque réservation.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public List<Reservation> FindByClient(int clientId)
        {
            return table
                .Include(r => r.Offre)
                .Include(r => r.Unites)
                .Where(r => r.ClientId == clientId)
                .ToList();
        }

        // une methode pour charger tous les reservation
        public List<Reservation> FindAllWithDetails()
        {
            using var ctx = new WorktogetherContext();
            return ctx.Reservations
                .Include(r => r.Client)
                .Include(r => r.Offre)
                .ToList();
        }
    }
}
