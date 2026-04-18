using Microsoft.EntityFrameworkCore;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.DATA.Repositories
{
    public class OffreRepository : Repository<Offre>
    {
        /// <summary>
        /// Récupère toutes les offres triées par prix mensuel
        /// Utilisé dans OffrePage pour afficher la liste
        /// </summary>
        public List<Offre> FindAllOrderByPrix()
        {
            return table
                .OrderBy(o => o.PrixMensuel) // tri croissant par prix
                .ToList();
        }

        /// <summary>
        /// Récupère toutes les offres avec leurs commandes liées
        /// Utilisé dans les rapports pour voir le CA par offre
        /// </summary>
        public List<Offre> FindAllWithDetails()
        {
            return table
                .Include(o => o.Commandes) // charge les commandes de chaque offre
                .OrderBy(o => o.PrixMensuel)
                .ToList();
        }
    }
}