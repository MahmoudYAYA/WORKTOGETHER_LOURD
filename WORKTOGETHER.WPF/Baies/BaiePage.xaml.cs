using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Baies
{
    public partial class BaiePage : Page
    {
        // Repositories nécessaires
        private readonly BaieRepository _baieRepo = new BaieRepository();
        private readonly UniteRepository _uniteRepo = new UniteRepository();

        // Baie sélectionnée dans la liste (null = mode création)
        private Baie _baieSelectionnee = null;

        public BaiePage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        // ── Charge toutes les baies avec leurs unités ──
        private void ChargerDonnees()
        {
            DgBaies.ItemsSource = _baieRepo.FindAllwithDetails();
            ViderFormulaire();
        }

        // ── Quand on clique sur une ligne ──
        private void DgBaies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _baieSelectionnee = DgBaies.SelectedItem as Baie;
            if (_baieSelectionnee != null)
            {
                RemplirFormulaire(_baieSelectionnee);
                TxtTitreFormulaire.Text = "MODIFIER BAIE";
            }
        }

        // ── Bouton CRÉER → vide le formulaire ──
        private void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            _baieSelectionnee = null;
            DgBaies.SelectedItem = null;
            ViderFormulaire();
            TxtTitreFormulaire.Text = "NOUVELLE BAIE";
            TxtNumeroBaie.Focus();
        }

        // ── Bouton SUPPRIMER ──
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_baieSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une baie !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ← Vérifie si des unités sont occupées
            if (_baieSelectionnee.NbUnitesOccupees > 0)
            {
                MessageBox.Show(
                    $"Impossible de supprimer la baie {_baieSelectionnee.NumeroBaie} !\n" +
                    $"{_baieSelectionnee.NbUnitesOccupees} unité(s) sont occupées par des clients.",
                    "Suppression impossible",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ← Vérifie si des unités ont des interventions en cours
            var unites = _uniteRepo.FindByBaie(_baieSelectionnee.Id);
            var unitesAvecIntervention = unites
                .Where(u => u.Interventions.Any(i => i.Statut == "en_cours"))
                .ToList();

            if (unitesAvecIntervention.Count > 0)
            {
                MessageBox.Show(
                    $"Impossible de supprimer la baie {_baieSelectionnee.NumeroBaie} !\n" +
                    $"{unitesAvecIntervention.Count} unité(s) ont des interventions en cours.",
                    "Suppression impossible",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ← Confirmation finale
            var result = MessageBox.Show(
                $"Supprimer la baie '{_baieSelectionnee.NumeroBaie}' et ses {_baieSelectionnee.CapaciteTotale} unités ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Supprime d'abord toutes les unités
                foreach (var unite in unites)
                    _uniteRepo.Delete(unite.Id);

                // Supprime la baie
                _baieRepo.Delete(_baieSelectionnee.Id);
                ChargerDonnees();

                MessageBox.Show("Baie supprimée avec succès !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ── Bouton ENREGISTRER → crée ou modifie ──
        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (!Valider()) return;

            if (_baieSelectionnee == null)
            {
                // ── MODE CRÉATION ──
                var baie = new Baie
                {
                    NumeroBaie = TxtNumeroBaie.Text,
                    CapaciteTotale = int.Parse(TxtCapacite.Text)
                };

                _baieRepo.Create(baie);

                // ← Crée automatiquement 42 unités pour cette baie
                for (int i = 1; i <= baie.CapaciteTotale; i++)
                {
                    var unite = new Unite
                    {
                        NumeroUnite = "U" + i.ToString().PadLeft(2, '0'), // U01, U02...
                        NomUnite = "Unité " + i,
                        Etat = "OK",
                        Statut = "disponible",
                        BaieId = baie.Id
                    };
                    _uniteRepo.Create(unite);
                }

                MessageBox.Show(
                    $"Baie {baie.NumeroBaie} créée avec {baie.CapaciteTotale} unités !",
                    "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // ── MODE MODIFICATION ──
                _baieSelectionnee.NumeroBaie = TxtNumeroBaie.Text;
                _baieSelectionnee.CapaciteTotale = int.Parse(TxtCapacite.Text);
                _baieRepo.Update(_baieSelectionnee);

                MessageBox.Show("Baie modifiée !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ChargerDonnees();
        }

        // ── Bouton ANNULER ──
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            _baieSelectionnee = null;
            DgBaies.SelectedItem = null;
            ViderFormulaire();
        }

        // ── Remplit le formulaire avec les données de la baie ──
        private void RemplirFormulaire(Baie baie)
        {
            TxtNumeroBaie.Text = baie.NumeroBaie;
            TxtCapacite.Text = baie.CapaciteTotale.ToString();
        }

        // ── Vide le formulaire ──
        private void ViderFormulaire()
        {
            TxtNumeroBaie.Text = "";
            TxtCapacite.Text = "";
            TxtErreur.Visibility = Visibility.Collapsed;
            TxtTitreFormulaire.Text = "FORMULAIRE";
        }

        // ── Valide les champs obligatoires ──
        private bool Valider()
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(TxtNumeroBaie.Text))
                erreurs.Add("Le numéro de baie est obligatoire");

            if (string.IsNullOrEmpty(TxtCapacite.Text))
                erreurs.Add("La capacité est obligatoire");

            // Vérifie que la capacité est bien un nombre
            if (!int.TryParse(TxtCapacite.Text, out _))
                erreurs.Add("La capacité doit être un nombre entier");

            if (erreurs.Count > 0)
            {
                TxtErreur.Text = string.Join("\n", erreurs);
                TxtErreur.Visibility = Visibility.Visible;
                return false;
            }

            TxtErreur.Visibility = Visibility.Collapsed;
            return true;
        }
    }
}