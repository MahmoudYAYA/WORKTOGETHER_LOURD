using System;
using System.Collections.Generic;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Users
{
    public class UserController
    {
        private readonly UserRepository _userRepo = new UserRepository();
        private readonly CommandeRepository _commandeRepo = new CommandeRepository();
        private readonly TicketSupportRepository _ticketRepo = new TicketSupportRepository();
        private readonly ReservationRepository _reservationRepo = new ReservationRepository();
        // Récupère tous les utilisateurs
        public List<User> GetAll()
        {
            return _userRepo.FindAll();
        }
        // Modifie un utilisateur
        public (bool succes, string message) Creer(string prenom, string nom, string email, string password, string role)
        {
            try
            {
                var existant = _userRepo.FindByEmail(email);
                if (existant != null)
                    return (false, "Cet email est déjà utilisé !");

                var user = new User
                {
                    Prenom = prenom,
                    Nom = nom,
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Roles = $"[\"{role}\"]",
                    Actif = 1,
                    IsVerified = 0,
                    DateCreation = DateTime.Now
                };
                _userRepo.Create(user);
                return (true, "Utilisateur créé !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        // Modifie un utilisateur
        public (bool succes, string message) Modifier(User user, string prenom, string nom, string email, string role, string password)
        {
            try
            {
                var existant = _userRepo.FindByEmail(email);
                if (existant != null && existant.Id != user.Id)
                    return (false, "Cet email est déjà utilisé !");


                user.Prenom = prenom;
                user.Nom = nom;
                user.Email = email;
                if (!string.IsNullOrEmpty(password))
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                user.Roles = $"[\"{role}\"]";
                _userRepo.Update(user);
                return (true, "Utilisateur modifié !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        // active ou désactive un utilisateur
        public (bool succes, string message) ToggleActif(int id)
        {
            try
            {
                _userRepo.ToggleActif(id);
                return (true, "Statut utilisateur modifié ");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        // Supprime un utilisateur
        public (bool succes, string message) Supprimer(int id)
        {
            try
            {
                // Vérifie commandes en cours
                var commandes = _commandeRepo.FindByClient(id)
                    .Where(c => c.StatutPaiement == "en_attente"
                             || c.StatutPaiement == "paye")
                    .ToList();
                if (commandes.Count > 0)
                    return (false, $"❌ {commandes.Count} commande(s) en cours !");

                // Vérifie tickets ouverts
                var tickets = _ticketRepo.FindByClient(id)
                    .Where(t => t.DateFermeture == null)
                    .ToList();
                if (tickets.Count > 0)
                    return (false, $"❌ {tickets.Count} ticket(s) ouvert(s) !");

                // Vérifie réservations actives
                var reservations = _reservationRepo.FindByClient(id)
                    .Where(r => r.DateFin >= DateTime.Now)
                    .ToList();
                if (reservations.Count > 0)
                    return (false, $"{reservations.Count} réservation active");

                // Supprime les relations restantes
                foreach (var t in _ticketRepo.FindByClient(id))
                    _ticketRepo.Delete(t.Id);
                foreach (var c in _commandeRepo.FindByClient(id))
                    _commandeRepo.Delete(c.Id);
                foreach (var r in _reservationRepo.FindByClient(id))
                    _reservationRepo.Delete(r.Id);

                _userRepo.Delete(id);
                return (true, "Utilisateur supprimé ");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        // Valide les champs du formulaire
        public List<string> Valider(
            string prenom, string nom, string email,
            object role, string password, bool isCreation)
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(prenom)) erreurs.Add("Le prénom est obligatoire");
            if (string.IsNullOrEmpty(nom)) erreurs.Add("Le nom est obligatoire");
            if (string.IsNullOrEmpty(email)) erreurs.Add("L'email est obligatoire");
            if (role == null) erreurs.Add("Veuillez choisir un rôle");

            if (isCreation && string.IsNullOrEmpty(password))
                erreurs.Add("Le mot de passe est obligatoire");

            return erreurs;
        }
    }
}