using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WORKTOGETHER.DATA.Entities;


namespace WORKTOGETHER.DATA.Repositories
{
    public class TicketSupportRepository : Repository<TicketSupport>
    {
        // Tickets ouverts (sans date fermeture)
        public List<TicketSupport> FindOuverts()
        {
            return table
                .Include(t => t.Client)
                .Where(t => t.DateFermeture == null)
                .ToList();
        }


        // Tickets d'un client
        public List<TicketSupport> FindByClient(int clientId)
        {
            return table
                .Include(t => t.Client)
                .Where(t => t.ClientId == clientId)
                .ToList();
        }

        public List<TicketSupport> FindAllWithDetails()
        {
            return table
                .Include(c => c.Client)
                .ToList();

        }

        public void Fermer(int id)
        {
            using var ctx = new WorktogetherContext();
            var ticket = ctx.TicketSupports.Find(id);
            if (ticket != null)
            {
                ticket.DateFermeture = DateTime.Now;
                ctx.SaveChanges();
            }
        }

    }
}
