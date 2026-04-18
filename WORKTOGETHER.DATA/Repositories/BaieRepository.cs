using Microsoft.EntityFrameworkCore;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.DATA.Repositories
{
    public class BaieRepository : Repository<Baie>
    {
        /// <summary>
        /// Supprime une baie et toutes ses unités
        /// </summary>
        public override void Delete(int id)
        {
            var entity = table
                .Include(b => b.Unites)
                .FirstOrDefault(b => b.Id == id);

            if (entity == null)
                throw new Exception("Baie introuvable !");

            // Supprime les unités d'abord
            foreach (var unite in entity.Unites.ToList())
                context.Set<Unite>().Remove(unite);

            // Supprime la baie
            table.Remove(entity);
            context.SaveChanges();
        }

        /// <summary>
        /// Capacité disponible d'une baie
        /// </summary>
        public int AvailableCapacity(int baieId)
        {
            var baie = table
                .Include(b => b.Unites)
                .FirstOrDefault(b => b.Id == baieId);

            if (baie == null)
                throw new Exception("Baie introuvable !");

            return baie.CapaciteTotale - baie.Unites.Count;
        }

        
        /// <summary>
        /// Récupère toutes les baies avec leurs unités
        /// </summary>
        public List<Baie> FindAllwithDetails()
        {
            return table
                .Include(b => b.Unites)
                .ToList();
        }
    }
}