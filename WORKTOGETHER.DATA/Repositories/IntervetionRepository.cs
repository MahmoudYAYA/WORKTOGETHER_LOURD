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

        public void Terminer(int id)
        {
            using var ctx = new WorktogetherContext();

            var intervention = ctx.Interventions
                .Include(i => i.Unite) // ← charge l'unité en même temps
                .FirstOrDefault(i => i.Id == id);

            if (intervention == null)
                throw new Exception("Intervention introuvable !");

            // ← Termine l'intervention
            intervention.Statut = "terminee";
            intervention.DateFin = DateTime.Now;

            // ← Change l'état de l'unité dans le MÊME contexte
            if (intervention.Unite != null)
                intervention.Unite.Etat = "OK";

            ctx.SaveChanges(); // ← Sauvegarde tout en une seule fois
        }
    }
}
