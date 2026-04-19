using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WORKTOGETHER.DATA.Repositories;
using WORKTOGETHER.DATA.Entities;
using Microsoft.Extensions.Primitives;

namespace WORKTOGETHER.WPF.Unites
{
    public class UniteController
    {
        private readonly UniteRepository _uniteRepo = new UniteRepository();

        public List<Unite> GetAll()
        {
            return _uniteRepo.FindAllWithDetails();
        }

        // mOdification d'un état d'une unité
        public (bool succes, string message) Modifier (Unite unite, string nouvelleEtat)
        {
            try
            {
                unite.Etat = nouvelleEtat;
                _uniteRepo.Update(unite);
                return (true, "état de unité modifié");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            } 
        }
    }
}
