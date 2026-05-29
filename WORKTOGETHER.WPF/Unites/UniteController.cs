using System;
using System.Collections.Generic;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Unites
{
    public class UniteController
    {
        private readonly UniteRepository _repo = new UniteRepository();

        // ── Récupère toutes les unités avec détails ──
        public List<Unite> GetAll()
        {
            return _repo.FindAllWithDetails();
        }

        // ── Modifie l'état d'une unité ──
        public (bool succes, string message) ModifierEtat(Unite unite, string nouvelEtat)
        {
            try
            {
                unite.Etat = nouvelEtat;
                _repo.Update(unite);
                return (true, "État modifié !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }
    }
}