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
            // valider les champs obligatiore 
            if (string.IsNullOrEmpty(TxtTitre.Text))
            {
                MessageBox.Show("Le Titre est obligatoire", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbType.SelectedItem == null)
            {
                MessageBox.Show("Erreu", "veuillez choisir un type", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbUnite.SelectedItem == null)
            {
                MessageBox.Show("Erreu", "veuillez choisir un type",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Vérifier si l'unité à déjà à une intervention en cours 
            var unite = CmbUnite.SelectedItem as Unite;
            var interventionEnCours = _interventionRepo.FindByUnite(unite.Id)
                .FirstOrDefault(i  => i.Statut == "en_cours");

            if (interventionEnCours != null)
            {
                MessageBox.Show($"L'unite {unite.NomUnite} a déjà une intervention en cours \n" + $"Titre : {interventionEnCours.Titre}\n"
                    + $"Terminez la avant de en créer une nouvelle. ",
                    "Intervention en cours ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // récupérer le type 
            var selectedType = CmbType.SelectedItem as System.Windows.Controls.ComboBoxItem;
            var type = int.Parse(selectedType.Tag.ToString());

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
            MessageBox.Show("Intervention créée avec succès", "Succès",
                MessageBoxButton.OK, MessageBoxImage.Warning );

            this.DialogResult = true;
            this.Close();
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}