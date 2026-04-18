using System;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.WPF.Interventions
{
    public partial class InterventionPage : Page
    {
        // Repositories
        private readonly InterventionController _controller = new InterventionController();
        private Intervention _interventionSelectionnee = null;

        public InterventionPage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        /// <summary>
        /// Methode pour charger les données depuis le Controller et afficher dans le DataGrid
        /// </summary>
        private void ChargerDonnees()
        {
            DgInterventions.ItemsSource = _controller.GetAll();
            CmbUnite.ItemsSource = _controller.GetUnites();
            ViderFormulaire();
        }

     /// <summary>
     /// Selection une intervetion 
     /// </summary>
     /// <param name="sender"></param>
     /// <param name="e"></param>
        private void DgInterventions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _interventionSelectionnee = DgInterventions.SelectedItem as Intervention;
            if (_interventionSelectionnee != null)
                RemplirFormulaire(_interventionSelectionnee);
        }

        /// <summary>
        /// Methode de création d'une intervention : vide le formulaire et désélectionne la ligne du DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            _interventionSelectionnee = null;
            DgInterventions.SelectedItem = null;
            ViderFormulaire();
            TxtTitreFormulaire.Text = "NOUVELLE INTERVENTION";
            TxtTitre.Focus();
        }

        /// <summary>
        /// Methode pour terminer une intervention : vérifie qu'une intervention est sélectionnée,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTerminer_Click(object sender, RoutedEventArgs e)
        {
            if (_interventionSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une intervention !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ← Toute la logique est dans le Controller !
            var (succes, message) = _controller.Terminer(_interventionSelectionnee.Id);
            MessageBox.Show(message, succes ? "Succès" : "Erreur",
                            MessageBoxButton.OK,
                            succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (succes) ChargerDonnees();
        }

        /// <summary>
        /// Methode de suppression d'une intervention : vérifie qu'une intervention est sélectionnée, demande confirmation, puis délègue la suppression au Controller
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_interventionSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une intervention !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Supprimer '{_interventionSelectionnee.Titre}' ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            var (succes, message) = _controller.Supprimer(_interventionSelectionnee.Id);
            MessageBox.Show(message, succes ? "Succès" : "Erreur",
                            MessageBoxButton.OK,
                            succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (succes) ChargerDonnees();
        }

        /// <summary>
        /// Buton ENREGISTRER : vérifie les champs du formulaire, affiche les erreurs s'il y en a, puis délègue la création ou modification au Controller selon le mode 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            var selectedType = CmbType.SelectedItem as ComboBoxItem;
            var unite = CmbUnite.SelectedItem as Unite;

            // Validation dans le Controller
            var erreurs = _controller.Valider(TxtTitre.Text, selectedType, unite);
            if (erreurs.Count > 0)
            {
                TxtErreur.Text = string.Join("\n", erreurs);
                TxtErreur.Visibility = Visibility.Visible;
                return;
            }

            int type = int.Parse(selectedType.Tag.ToString());
            var dateDebut = DpDateDebut.SelectedDate ?? DateTime.Now;

            (bool succes, string message) resultat;

            if (_interventionSelectionnee == null)
                // Création dans le Controller
                resultat = _controller.Creer(TxtTitre.Text, type,
                                             TxtDescription.Text, dateDebut, unite.Id);
            else
                // Modification dans le Controller
                resultat = _controller.Modifier(_interventionSelectionnee, TxtTitre.Text,
                                                type, TxtDescription.Text, dateDebut, unite.Id);

            MessageBox.Show(resultat.message,
                            resultat.succes ? "Succès" : "Erreur",
                            MessageBoxButton.OK,
                            resultat.succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (resultat.succes) ChargerDonnees();
        }

        /// <summary>
        /// metodes pour annuler la sélection d'une intervention : null la variable d'instance, désélectionne la ligne du DataGrid et vide le formulaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            _interventionSelectionnee = null;
            DgInterventions.SelectedItem = null;
            ViderFormulaire();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intervention"></param>
        private void RemplirFormulaire(Intervention intervention)
        {
            TxtTitre.Text = intervention.Titre;
            TxtDescription.Text = intervention.Description;
            DpDateDebut.SelectedDate = intervention.DateDebut;
            TxtTitreFormulaire.Text = "DÉTAIL INTERVENTION";

            foreach (ComboBoxItem item in CmbType.Items)
                if (item.Tag.ToString() == intervention.Type.ToString())
                { CmbType.SelectedItem = item; break; }

            foreach (Unite u in CmbUnite.Items)
                if (u.Id == intervention.UniteId)
                { CmbUnite.SelectedItem = u; break; }
        }

        // ── Vide le formulaire ──
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
    }
}