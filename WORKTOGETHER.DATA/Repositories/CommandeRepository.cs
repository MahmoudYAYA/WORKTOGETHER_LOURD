using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.DATA.Repositories
{
    public class CommandeRepository : Repository<Commande>
    {
        /// Trouver toutes les commandes d'un client
        public List<Commande> FindByClient(int clientId)
        {
            return table
                .Include(c => c.Client)
                .Include(c => c.Offre)
                .Where(c => c.ClientId == clientId)
                .ToList();
        }

        /// Commandes en attente d'un client
        public List<Commande> FindEnAttente(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentException("ClientId invalide");

            return table
                .Include(c => c.Client)
                .Include(c => c.Offre)
                .Where(c => c.ClientId == clientId
                         && c.StatutPaiement == "en_attente")
                .ToList();
        }

        /// Commandes payées
        public List<Commande> FindPayee()
        {
            return table
                .Include(c => c.Client)
                .Include(c => c.Offre)
                .Where(c => c.StatutPaiement == "paye")
                .ToList();
        }

        // Afficher tous les commandes
        public List<Commande> FindAllWithDetails()
        {
            return table
                .Include(c => c.Client)
                .Include(c => c.Offre)
                .ToList();



        }
    }
}