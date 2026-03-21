using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.DATA.Repositories
{
    public class InterventionRepository : Repository<Intervention>
    {
        // Interventions en cours
        public List<Intervention> FindEnCours()
        {
            return table
                .Include(i => i.Unite)
                .Where(i => i.Statut == "en_cours")
                .ToList();
        }

        // Interventions d'une unité
        public List<Intervention> FindByUnite(int uniteId)
        {
            return table
                .Include(i => i.Unite)
                .Where(i => i.UniteId == uniteId)
                .ToList();
        }
        public List<Intervention> FindAllWithDetails()
        {
            return table
                .Include(i => i.Unite)
                .ToList();



        }
    }
}
