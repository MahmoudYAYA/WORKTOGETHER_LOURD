using System;
using System.Windows;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Views
{
    public partial class NouvelleInterventionWindow : Window
    {
        private readonly InterventionRepository _interventionRepo = new InterventionRepository();
        private readonly UniteRepository _uniteRepo = new UniteRepository();

        public NouvelleInterventionWindow()
        {
            InitializeComponent();
            ChargerUnites();
        }

        private void ChargerUnites()
        {
            // Charge les unités dans le ComboBox
            CmbUnite.ItemsSource = _uniteRepo.FindAll();
        }

        private void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrEmpty(TxtTitre.Text))
            {
                MessageBox.Show("Le titre est obligatoire !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbType.SelectedItem == null)
            {
                MessageBox.Show("Veuillez choisir un type !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbUnite.SelectedItem == null)
            {
                MessageBox.Show("Veuillez choisir une unité !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Récupère le type depuis le Tag du ComboBoxItem
            var selectedType = CmbType.SelectedItem as System.Windows.Controls.ComboBoxItem;
            int type = int.Parse(selectedType.Tag.ToString());

            // Récupère l'unité sélectionnée
            var unite = CmbUnite.SelectedItem as Unite;

            // Crée l'intervention
            var intervention = new Intervention
            {
                Titre = TxtTitre.Text,
                Type = type,
                Description = TxtDescription.Text,
                Statut = "en_cours",
                DateDebut = DpDateDebut.SelectedDate ?? DateTime.Now,
                UniteId = unite.Id
            };

            _interventionRepo.Create(intervention);

            MessageBox.Show("Intervention créée avec succès !", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);

            // Ferme la fenêtre
            this.DialogResult = true;
            this.Close();
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}