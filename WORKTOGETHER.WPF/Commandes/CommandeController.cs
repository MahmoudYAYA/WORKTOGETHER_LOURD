using System;
using System.Collections.Generic;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Commandes
{
    public class CommandeController
    {
        // Seulement les repos nécessaires aux commandes
        private readonly CommandeRepository _repo = new CommandeRepository();

        // Récupère toutes les commandes avec détails 
        public List<Commande> GetAll()
        {
            return _repo.FindAllWithDetails();
        }

        // Annule une commande 
        public (bool succes, string message) Annuler(int id)
        {
            try
            {
                var commande = _repo.FindById(id);

                if (commande.StatutPaiement == "annule")
                    return (false, "Cette commande est déjà annulée !");

                if (commande.StatutPaiement == "paye")
                    return (false, "Impossible d'annuler une commande déjà payée !");

                _repo.Annuler(id);
                return (true, "Commande annulée !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }
    }
}