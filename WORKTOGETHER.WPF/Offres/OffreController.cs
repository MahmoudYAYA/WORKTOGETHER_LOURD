using System;
using System.Collections.Generic;
using System.Linq;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Offres
{
    public class OffreController
    {
       
        private readonly OffreRepository _offreRepo = new OffreRepository();
        private readonly CommandeRepository _commandeRepo = new CommandeRepository();
        private readonly ReservationRepository _reservationRepo = new ReservationRepository();

    
        /// <summary>
        /// Récupère toutes les offres triées par prix
        /// Appelé dans OffrePage.ChargerDonnees()
        /// </summary>
        public List<Offre> GetAll()
        {
            return _offreRepo.FindAllOrderByPrix();
        }

        /// <summary>
        /// Crée une nouvelle offre
        /// </summary>
        public (bool succes, string message) Creer(
            string nomOffre,      
            string description, 
            int nombreUnites,     
            decimal prixMensuel,  
            decimal prixAnnuel,   
            int reductionAnnuelle 
        )
        {
            try
            {
                var offre = new Offre
                {
                    NomOffre = nomOffre,
                    Description = description,
                    NombreUnites = nombreUnites,
                    PrixMensuel = prixMensuel,
                    PrixAnnuelle = prixAnnuel,
                    ReductionAnnuelle = reductionAnnuelle
                };

                _offreRepo.Create(offre);
                return (true, $"Offre '{nomOffre}' créée avec succès ");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        
        /// <summary>
        /// Modifie une offre existante
        /// </summary>
        public (bool succes, string message) Modifier(
            Offre offre,          // l'offre à modifier récupérée depuis le DataGrid
            string nomOffre,
            string description,
            int nombreUnites,
            decimal prixMensuel,
            decimal prixAnnuel,
            int reductionAnnuelle
        )
        {
            try
            {
                //Metter à jour les propriétés de l'offre
                offre.NomOffre = nomOffre;
                offre.Description = description;
                offre.NombreUnites = nombreUnites;
                offre.PrixMensuel = prixMensuel;
                offre.PrixAnnuelle = prixAnnuel;
                offre.ReductionAnnuelle = reductionAnnuelle;

                _offreRepo.Update(offre);
                return (true, "Offre modifiée ");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

       
        /// <summary>
        /// Supprime une offre seulement si elle n'a pas de commandes ou réservations
        /// </summary>
        public (bool succes, string message) Supprimer(int id)
        {
            try
            {
                // ← Vérifie les commandes liées à cette offre
                var commandes = _commandeRepo.FindAll()
                    .Where(c => c.OffreId == id
                             && (c.StatutPaiement == "paye"
                             || c.StatutPaiement == "en_attente"))
                    .ToList();

                // Si des commandes existent bloque la suppression
                if (commandes.Count > 0)
                    return (false,
                        $"Impossible ! {commandes.Count} commande liée à cette offre ");

                // Vérifie les réservations liées à cette offre
                var reservations = _reservationRepo.FindAll()
                    .Where(r => r.OffreId == id)
                    .ToList();

                if (reservations.Count > 0)
                    return (false,
                        $"Impossible  {reservations.Count} réservation liée à cette offre ");

                _offreRepo.Delete(id);
                return (true, "Offre supprimée ");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

       

        /// <summary>
        /// Valide les champs du formulaire
        /// Retourne une liste d'erreurs (vide = tout est OK)
        /// Appelé dans OffrePage.BtnEnregistrer_Click() avant de créer/modifier
        /// </summary>
        public List<string> Valider(
            string nomOffre,
            string nombreUnites,
            string prixMensuel,
            string prixAnnuel)
        {
            var erreurs = new List<string>();

            // Vérifie que le nom n'est pas vide
            if (string.IsNullOrEmpty(nomOffre))
                erreurs.Add("Le nom de l'offre est obligatoire");

            //  Vérifie que nombreUnites est bien 42 ni plus 
            // out int n    stocke le résultat dans n
            if (!int.TryParse(nombreUnites, out int n) || n <= 0)
                erreurs.Add("Le nombre d'unités doit être un entier positif");

            // Vérifie que prixMensuel est bien un nombre décimal positif
            if (!decimal.TryParse(prixMensuel, out decimal pm) || pm <= 0)
                erreurs.Add("Le prix mensuel doit être un nombre positif");

            if (!decimal.TryParse(prixAnnuel, out decimal pa) || pa <= 0)
                erreurs.Add("Le prix annuel doit être un nombre positif");

            return erreurs;
            
        }
    }
}