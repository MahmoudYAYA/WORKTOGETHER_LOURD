using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Offres
{
    public partial class OffrePage : Page
    {
        // Repositories
        private readonly OffreRepository _offreRepo = new OffreRepository();
        private readonly CommandeRepository _commandeRepo = new CommandeRepository();

        // Liste complète pour la recherche
        private List<Offre> _toutesLesOffres;

        // Offre sélectionnée (null = mode création)
        private Offre _offreSelectionnee = null;

        public OffrePage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        // ── Charge toutes les offres ──
        private void ChargerDonnees()
        {
            _toutesLesOffres = _offreRepo.FindAll();
            DgOffers.ItemsSource = _toutesLesOffres;
            ViderFormulaire();
        }

        // ── Filtre la liste selon la recherche ──
        private void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            var recherche = TxtRecherche.Text.ToLower();
            DgOffers.ItemsSource = _toutesLesOffres
                .Where(o => o.NomOffre.ToLower().Contains(recherche))
                .ToList();
        }

        // ── Quand on clique sur une ligne → remplit le formulaire ──
        private void DgOffers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _offreSelectionnee = DgOffers.SelectedItem as Offre;
            if (_offreSelectionnee != null)
            {
                RemplirFormulaire(_offreSelectionnee);
                TxtTitreFormulaire.Text = "MODIFIER OFFRE";
            }
        }

        // ── Bouton CRÉER ──
        private void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            _offreSelectionnee = null;
            DgOffers.SelectedItem = null;
            ViderFormulaire();
            TxtTitreFormulaire.Text = "NOUVELLE OFFRE";
            TxtNomOffre.Focus();
        }

        // ── Bouton MODIFIER ──
        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (_offreSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une offre !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            TxtTitreFormulaire.Text = "MODIFIER OFFRE";
            RemplirFormulaire(_offreSelectionnee);
        }

        // ── Bouton SUPPRIMER ──
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_offreSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une offre !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Vérifie si l'offre a des commandes
            var commandes = _commandeRepo.FindAll()
                .Where(c => c.OffreId == _offreSelectionnee.Id)
                .ToList();

            if (commandes.Count > 0)
            {
                MessageBox.Show(
                    $"Impossible de supprimer l'offre '{_offreSelectionnee.NomOffre}' !\n" +
                    $"{commandes.Count} commande(s) sont liées à cette offre.",
                    "Suppression impossible",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Supprimer l'offre '{_offreSelectionnee.NomOffre}' ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _offreRepo.Delete(_offreSelectionnee.Id);
                ChargerDonnees();
                MessageBox.Show("Offre supprimée !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ── Bouton ENREGISTRER → crée ou modifie ──
        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (!Valider()) return;

            if (_offreSelectionnee == null)
            {
                // ── MODE CRÉATION ──
                var offre = new Offre
                {
                    
                    NomOffre = TxtNomOffre.Text,
                    Description = TxtDescription.Text,
                    NombreUnites = int.Parse(TxtNombreUnites.Text),
                    PrixMensuel = decimal.Parse(TxtPrixMensuel.Text),
                    PrixAnnuelle = decimal.Parse(TxtPrixAnnuelle.Text),
                    ReductionAnnuelle = int.Parse(TxtReductionAnnuelle.Text)
                };
                _offreRepo.Create(offre);
                MessageBox.Show("Offre créée ", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // ── MODE MODIFICATION ──
                _offreSelectionnee.NomOffre = TxtNomOffre.Text;
                _offreSelectionnee.Description = TxtDescription.Text;
                _offreSelectionnee.NombreUnites = int.Parse(TxtNombreUnites.Text);
                _offreSelectionnee.PrixMensuel = decimal.Parse(TxtPrixMensuel.Text);
                _offreSelectionnee.PrixAnnuelle = decimal.Parse(TxtPrixAnnuelle.Text);
                _offreSelectionnee.ReductionAnnuelle = int.Parse(TxtReductionAnnuelle.Text);
                _offreRepo.Update(_offreSelectionnee);
                MessageBox.Show("Offre modifiée !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ChargerDonnees();
        }

        // ── Bouton ANNULER ──
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            _offreSelectionnee = null;
            DgOffers.SelectedItem = null;
            ViderFormulaire();
        }

        //Remplit le formulaire avec les données de l'offre
        private void RemplirFormulaire(Offre offre)
        {
            TxtNomOffre.Text = offre.NomOffre;
            TxtDescription.Text = offre.Description;
            TxtNombreUnites.Text = offre.NombreUnites.ToString();
            TxtPrixMensuel.Text = offre.PrixMensuel.ToString();
            TxtPrixAnnuelle.Text = offre.PrixAnnuelle.ToString();
            TxtReductionAnnuelle.Text = offre.ReductionAnnuelle.ToString();
        }

        // vide le formulair
        private void ViderFormulaire()
        {
            TxtNomOffre.Text = "";
            TxtDescription.Text = "";
            TxtNombreUnites.Text = "";
            TxtPrixMensuel.Text = "";
            TxtPrixAnnuelle.Text = "";
            TxtReductionAnnuelle.Text = "";
            TxtErreur.Visibility = Visibility.Collapsed;
            TxtTitreFormulaire.Text = "FORMULAIRE";
        }

        // ── Valide les champs obligatoires ──
        private bool Valider()
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(TxtNomOffre.Text))
                erreurs.Add("Le nom de l'offre est obligatoire");
            // Le nom doit etre unique
            else if (_toutesLesOffres.Any(o => o.NomOffre.Equals(TxtNomOffre.Text, StringComparison.OrdinalIgnoreCase)
                                              && o.Id != _offreSelectionnee?.Id))
                erreurs.Add("Une offre avec ce nom existe déjà");

            if (!int.TryParse(TxtNombreUnites.Text, out _))
                erreurs.Add("Le nombre d'unités doit être un entier");

            if (!decimal.TryParse(TxtPrixMensuel.Text, out _))
                erreurs.Add("Le prix mensuel doit être un nombre");

            if (!decimal.TryParse(TxtPrixAnnuelle.Text, out _))
                erreurs.Add("Le prix annuel doit être un nombre");

            if (!int.TryParse(TxtReductionAnnuelle.Text, out _))
                erreurs.Add("La réduction doit être un entier");

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