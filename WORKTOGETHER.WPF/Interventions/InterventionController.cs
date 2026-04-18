using System;
using System.Collections.Generic;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Interventions
{
    public class InterventionController
    {
        // ── Repositories ──
        private readonly InterventionRepository _repo = new InterventionRepository();
        private readonly UniteRepository _uniteRepo = new UniteRepository();

        // ── Récupère toutes les interventions avec détails ──
        public List<Intervention> GetAll()
        {
            return _repo.FindAllWithDetails();
        }

        // ── Récupère toutes les unités ──
        public List<Unite> GetUnites()
        {
            return _uniteRepo.FindAll();
        }

       
        // ── Modifie une intervention ──
        public (bool succes, string message) Modifier(
            Intervention intervention, string titre, int type,
            string description, DateTime dateDebut, int uniteId)
        {
            intervention.Titre = titre;
            intervention.Type = type;
            intervention.Description = description;
            intervention.DateDebut = dateDebut;
            intervention.UniteId = uniteId;

            _repo.Update(intervention);
            return (true, "Intervention modifiée !");
        }

        // ── Crée une intervention + change statut unité ──
        public (bool succes, string message) Creer(
            string titre, int type, string description,
            DateTime dateDebut, int uniteId)
        {
            try
            {
                var unite = _uniteRepo.FindById(uniteId);

                // ← Vérifie que l'unité n'a pas déjà une intervention en cours
                var interventionEnCours = _repo.FindByUnite(uniteId)
                    .Find(i => i.Statut == "en_cours");

                if (interventionEnCours != null)
                    return (false, "Cette unité a déjà une intervention en cours !");

                // ← Crée l'intervention
                var intervention = new Intervention
                {
                    Titre = titre,
                    Type = type,
                    Description = description,
                    Statut = "en_cours",
                    DateDebut = dateDebut,
                    UniteId = uniteId
                };
                _repo.Create(intervention);

                // ← Change le statut de l'unité
                unite.Etat = "incident";  // ou "maintenance" selon le type
                _uniteRepo.Update(unite);

                return (true, "Intervention créée !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        // ── Termine une intervention + remet l'unité disponible ──
        public (bool succes, string message) Terminer(int id)
        {
            try
            {
                var intervention = _repo.FindById(id);

                if (intervention.Statut == "terminee")
                    return (false, "Cette intervention est déjà terminée !");

                // ← Le repo gère tout maintenant
                _repo.Terminer(id);
                return (true, "Intervention terminée et unité remise en état OK !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        // ── Supprime une intervention ──
        public (bool succes, string message) Supprimer(int id)
        {
            var intervention = _repo.FindById(id);

            if (intervention == null)
                return (false, "Intervention introuvable !");

            if (intervention.Statut == "en_cours")
                return (false, "Impossible de supprimer une intervention en cours !\nTerminez-la d'abord.");

            _repo.Delete(id);
            return (true, "Intervention supprimée !");
        }

        // ── Valide les champs ──
        public List<string> Valider(string titre, object type, object unite)
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(titre)) erreurs.Add("Le titre est obligatoire");
            if (type == null) erreurs.Add("Veuillez choisir un type");
            if (unite == null) erreurs.Add("Veuillez choisir une unité");

            return erreurs;
        }
    }
}