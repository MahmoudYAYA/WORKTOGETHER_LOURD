using System;
using System.Collections.Generic;
using System.Linq;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Tickets
{
    public class TicketController
    {
       
        // les répos nécessaires 
        private readonly TicketSupportRepository _repo = new TicketSupportRepository();
        private readonly UserRepository _userRepo = new UserRepository();

       

        /// <summary>
        /// Récupère tous les tickets avec leurs clients
        /// Utilisé dans TicketPage pour remplir le DataGrid
        /// </summary>
        public List<TicketSupport> GetAll()
        {
            return _repo.FindAllWithDetails();
        }

        /// <summary>
        /// Récupère seulement les clients pour le ComboBox du formulaire
        /// </summary>
        public List<User> GetClients()
        {
            
            return _userRepo.FindByRole("ROLE_CLIENT");
        }

    
        /// <summary>
        /// Crée un nouveau ticket
        /// 
        /// </summary>
        public (bool succes, string message) Creer(
            string sujet, string description, int priorite, int clientId)
        {
            try
            {
                var ticket = new TicketSupport
                {
                    // Génère un numéro unique avec la date
                    NumeroTicket = "TKT-" + DateTime.Now.Ticks,
                    Sujet = sujet,
                    Description = description,
                    Priorite = priorite,
                    DateCreation = DateTime.Now,
                  
                    ClientId = clientId
                };

                _repo.Create(ticket);
                return (true, "Ticket créé avec succès !");
            }
            catch (Exception ex)
            {
                // Si erreur BDD → on retourne false avec le message
                return (false, $"Erreur : {ex.Message}");
            }
        }


        /// <summary>
        /// Modifie un ticket existant
        /// </summary>
        public (bool succes, string message) Modifier(
            TicketSupport ticket, string sujet, string description,
            int priorite, int clientId)
        {
            try
            {
                // Vérifie qu'on ne modifie pas un ticket fermé
                if (ticket.DateFermeture != null)
                    return (false, "Impossible de modifier un ticket fermé !");

                ticket.Sujet = sujet;
                ticket.Description = description;
                ticket.Priorite = priorite;
                ticket.ClientId = clientId;

                _repo.Update(ticket);
                return (true, "Ticket modifié !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        /// <summary>
        /// Ferme un ticket en ajoutant la date de fermeture
        /// Utilisé quand on clique FERMER
        /// </summary>
        public (bool succes, string message) Fermer(int id)
        {
            try
            {
                var ticket = _repo.FindById(id);

                // Vérifie que le ticket n'est pas déjà fermé
                if (ticket.DateFermeture != null)
                    return (false, "Ce ticket est déjà fermé !");

                // Ferme le ticket via méthode spécifique du repo
                //utilise un nouveau contexte pour éviter les conflits EF
                _repo.Fermer(id);
                return (true, "Ticket fermé avec succès !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }



        /// <summary>
        /// Supprime un ticket seulement s'il est fermé
        /// Utilisé quand on clique SUPPRIMER
        /// </summary>
        public (bool succes, string message) Supprimer(int id)
        {
            try
            {
                var ticket = _repo.FindById(id);

                // Vérifie que le ticket est bien fermé avant de supprimer
                if (ticket.DateFermeture == null)
                    return (false, "Fermez le ticket avant de le supprimer !");

                _repo.Delete(id);
                return (true, "Ticket supprimé !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        /// <summary>
        /// Valide les champs du formulaire
        /// </summary>
        public List<string> Valider(string sujet, object priorite, object client)
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(sujet))
                erreurs.Add("Le sujet est obligatoire");

            if (priorite == null)
                erreurs.Add("Veuillez choisir une priorité");

            if (client == null)
                erreurs.Add("Veuillez choisir un client");

            return erreurs;
           
        }
    }
}