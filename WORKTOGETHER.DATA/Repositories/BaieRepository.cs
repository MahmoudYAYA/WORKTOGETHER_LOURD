using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WORKTOGETHER.DATA.Entities;


namespace WORKTOGETHER.DATA.Repositories
{
    public class BaieRepository : Repository<Baie>
    {
        // Une methode pour recoupérer tous les bais 
        //public List<Baie> GetAllBaies()
        //{
        //    return table
        //        .Inlude(b => b.Unites) // Inclure les unités associées à chaque baie
        //        .ToList();
        //}
        /// <summary>
        /// Une methode pour recoupérer la cpapacite  d'un baie en fonction de son id
        /// </summary>
        /// <param name="baieId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int AvailableCapacity(int baieId)
        {
            var baie = table.Find(baieId);
            if (baie == null)
                throw new Exception("Baie not found");
            int usedCapacity = baie.Unites.Count();
            return baie.CapaciteTotale - usedCapacity;
        }

        /// <summary>
        /// Une methode pour aujouter une unite dans une baie  
        /// </summary>
        /// <param name="baieId"></param>
        /// <param name="unite"></param>
        /// <exception cref="Exception"></exception>
        public void AddUniteToBaie(int baieId, Unite unite)
        {
            var baie = table.Find(baieId);
            if (baie == null)
                throw new Exception("Baie not found");
            if (AvailableCapacity(baieId) <= 0)
                throw new Exception("No available capacity in this baie");
            //baie.Unites.Add(unite);

            unite.BaieId = baieId;
            context.Update(unite);
            context.SaveChanges();

        }

        public List<Baie> FindAllwithDetails()
        {
            return table
                .Include(u => u.Unites)
                .ToList();
        }
        // 

    }
}
    
