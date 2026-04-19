using Microsoft.EntityFrameworkCore;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.DATA.Repositories
{
	public class UniteRepository : Repository<Unite>
	{
		// Unités disponibles
		public List<Unite> FindDisponibles()
		{
			return table
				.Include(u => u.Baie)
				.Where(u => u.Statut == "disponible")
				.ToList();
		}

		// Unités d'une baie
		public List<Unite> FindByBaie(int baieId)
		{
			return table
				.Include(u => u.Baie)
				.Where(u => u.BaieId == baieId)
				.ToList();
		}

		public List<Unite> FindAllWithDetails()
		{
			return table
				.Include(u => u.Baie)
				.Include(u => u.Reservation)
					.ThenInclude(u => u.Client)
				.ToList();
        }


		/// une mehtode modifier etate 
		public void ModifierEtat(int modifier)
		{

			if (modifier == 0)
				return;
		}
		
    }
}