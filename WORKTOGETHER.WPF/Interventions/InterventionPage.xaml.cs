using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Interventions
{
    public partial class InterventionPage : Page
    {
        // Repositories nécessaires
        private readonly InterventionRepository _repo = new InterventionRepository();
        private readonly UniteRepository _uniteRepo = new UniteRepository();

        // Intervention sélectionnée dans la liste
        private Intervention _interventionSelectionnee = null;

        public InterventionPage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        // ── Charge les interventions et les unités ──
        private void ChargerDonnees()
        {
            // Charge les interventions avec leurs unités
            DgInterventions.ItemsSource = _repo.FindAllWithDetails();

            // Charge les unités dans le ComboBox
            CmbUnite.ItemsSource = _uniteRepo.FindAll();

            ViderFormulaire();
        }

        // ── Quand on sélectionne une ligne dans le tableau ──
        private void DgInterventions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _interventionSelectionnee = DgInterventions.SelectedItem as Intervention;
            if (_interventionSelectionnee != null)
            {
                RemplirFormulaire(_interventionSelectionnee);
            }
        }

        // ── Bouton CRÉER → vide le formulaire ──
        private void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            _interventionSelectionnee = null;
            ViderFormulaire();
            TxtTitreFormulaire.Text = "NOUVELLE INTERVENTION";
            TxtTitre.Focus();
        }

        // ── Bouton TERMINER → met le statut à terminée ──
        private void BtnTerminer_Click(object sender, RoutedEventArgs e)
        {
            if (_interventionSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une intervention !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_interventionSelectionnee.Statut == "terminee")
            {
                MessageBox.Show("Cette intervention est déjà terminée !", "Information",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Termine l'intervention via méthode spécifique du repo
            _repo.Terminer(_interventionSelectionnee.Id);
            ChargerDonnees();

            MessageBox.Show("Intervention terminée !", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ── Bouton SUPPRIMER ──
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_interventionSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une intervention !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Empêche la suppression si en cours
            if (_interventionSelectionnee.Statut == "en_cours")
            {
                MessageBox.Show("Impossible de supprimer une intervention en cours !\nTerminez-la d'abord.",
                                "Suppression impossible",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Supprimer l'intervention '{_interventionSelectionnee.Titre}' ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _repo.Delete(_interventionSelectionnee.Id);
                ChargerDonnees();
            }
        }

        // ── Bouton ENREGISTRER → crée une nouvelle intervention ──
        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (!Valider()) return;

            var selectedType = CmbType.SelectedItem as ComboBoxItem;
            int type = int.Parse(selectedType.Tag.ToString());
            var unite = CmbUnite.SelectedItem as Unite;

            if (_interventionSelectionnee == null)
            {
                // ← Pas de sélection → CRÉER
                var intervention = new Intervention
                {
                    Titre = TxtTitre.Text,
                    Type = type,
                    Description = TxtDescription.Text,
                    Statut = "en_cours",
                    DateDebut = DpDateDebut.SelectedDate ?? DateTime.Now,
                    UniteId = unite.Id
                };
                _repo.Create(intervention);
                MessageBox.Show("Intervention créée !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // ← Sélection existante → MODIFIER
                _interventionSelectionnee.Titre = TxtTitre.Text;
                _interventionSelectionnee.Type = type;
                _interventionSelectionnee.Description = TxtDescription.Text;
                _interventionSelectionnee.DateDebut = DpDateDebut.SelectedDate ?? DateTime.Now;
                _interventionSelectionnee.UniteId = unite.Id;
                _repo.Update(_interventionSelectionnee);
                MessageBox.Show("Intervention modifiée !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ChargerDonnees();
        }

        // ── Bouton ANNULER → vide le formulaire ──
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            ViderFormulaire();
            _interventionSelectionnee = null;
        }

        // ── Remplit le formulaire avec les données de l'intervention ──
        private void RemplirFormulaire(Intervention intervention)
        {
            TxtTitre.Text = intervention.Titre;
            TxtDescription.Text = intervention.Description;
            DpDateDebut.SelectedDate = intervention.DateDebut;
            TxtTitreFormulaire.Text = "DÉTAIL INTERVENTION";

            // Sélectionne le bon type dans le ComboBox
            foreach (ComboBoxItem item in CmbType.Items)
            {
                if (item.Tag.ToString() == intervention.Type.ToString())
                {
                    CmbType.SelectedItem = item;
                    break;
                }
            }

            // Sélectionne la bonne unité dans le ComboBox
            foreach (Unite u in CmbUnite.Items)
            {
                if (u.Id == intervention.UniteId)
                {
                    CmbUnite.SelectedItem = u;
                    break;
                }
            }
        }

        // ── Vide tous les champs du formulaire ──
        private void ViderFormulaire()
        {
            TxtTitre.Text = "";
            TxtDescription.Text = "";
            DpDateDebut.SelectedDate = null;
            CmbType.SelectedIndex = -1;
            CmbUnite.SelectedIndex = -1;
            TxtErreur.Visibility = Visibility.Collapsed;
            TxtTitreFormulaire.Text = "FORMULAIRE";
        }

        // ── Valide les champs obligatoires ──
        private bool Valider()
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(TxtTitre.Text)) erreurs.Add("Le titre est obligatoire");
            if (CmbType.SelectedItem == null) erreurs.Add("Veuillez choisir un type");
            if (CmbUnite.SelectedItem == null) erreurs.Add("Veuillez choisir une unité");

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