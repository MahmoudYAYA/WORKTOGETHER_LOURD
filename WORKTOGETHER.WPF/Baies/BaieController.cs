using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Baies
{
    public class BaieController
    {
        /// <summary>
        /// La logique métier pour gérer les baies et leurs unités.
        /// </summary>
        private readonly BaieRepository _baieRepo = new BaieRepository();
        private readonly UniteRepository _uniteRepo = new UniteRepository();

        // ── Récupère toutes les baies avec leurs unités ──
        public List<Baie> GetAll()
        {
            return _baieRepo.FindAllwithDetails();
        }

        // ── Crée une baie + génère les unités automatiquement ──
        public (bool succes, string message) Creer(string numeroBaie, int capacite)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(numeroBaie))
                    return (false, "Le numéro de baie est obligatoire !");

                if (capacite <= 0)
                    return (false, "La capacité doit être supérieure à 0 !");

                // Crée la baie
                var baie = new Baie
                {
                    NumeroBaie = numeroBaie,
                    CapaciteTotale = capacite
                };
                _baieRepo.Create(baie);

                // Génère les unités automatiquement
                for (int i = 1; i <= capacite; i++)
                {
                    var unite = new Unite
                    {
                        NumeroUnite = "U" + i.ToString().PadLeft(2, '0'),
                        NomUnite = "Unité " + i,
                        Etat = "OK",
                        Statut = "disponible",
                        BaieId = baie.Id
                    };
                    _uniteRepo.Create(unite);
                }

                return (true, $"Baie {numeroBaie} créée avec {capacite} unités !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        // ── Supprime une baie ──
        public (bool succes, string message) Supprimer(int id)
        {
            try
            {
                // Récupère la baie avec ses unités
                var baie = _baieRepo.FindAllwithDetails()
                    .FirstOrDefault(b => b.Id == id);

                if (baie == null)
                    return (false, "Baie introuvable !");

                // ← Vérifie les unités occupées
                if (baie.NbUnitesOccupees > 0)
                    return (false, $"❌ {baie.NbUnitesOccupees} unité(s) sont occupées !");

                // ← Supprime (BaieRepository.Delete supprime les unités aussi)
                _baieRepo.Delete(id);
                return (true, "Baie supprimée avec succès !");
            }
            catch (Exception ex)
            {
                return (false, $"Erreur : {ex.Message}");
            }
        }

        // ── Valide les champs ──
        public List<string> Valider(string numeroBaie, string capacite)
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(numeroBaie))
                erreurs.Add("Le numéro de baie est obligatoire");

            if (!int.TryParse(capacite, out int cap) || cap <= 0)
                erreurs.Add("La capacité doit être un nombre entier positif");

            return erreurs;
        }
    }
}