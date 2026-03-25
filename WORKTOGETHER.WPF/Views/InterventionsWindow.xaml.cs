using System;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Views
{
    public partial class InterventionsWindow : Window
    {
        private readonly InterventionRepository _repository = new InterventionRepository();

        public InterventionsWindow()
        {
            InitializeComponent();
            ChargerInterventions();
        }

        private void ChargerInterventions()
        {
            DgInterventions.ItemsSource = _repository.FindAllWithDetails();
        }

        private void BtnTerminer_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int interventionId = (int)btn.Tag;

            // ← Crée un nouveau repository à chaque fois
            var repo = new InterventionRepository();
            var intervention = repo.FindById(interventionId);

            if (intervention.Statut == "terminee")
            {
                MessageBox.Show("Cette intervention est déjà terminée !", "Information",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            intervention.Statut = "terminee";
            intervention.DateFin = DateTime.Now;
            repo.Update(intervention);

            ChargerInterventions();

            MessageBox.Show("Intervention terminée avec succès !", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnNouvelle_Click(object sender, RoutedEventArgs e)
        {
            var fenetre = new NouvelleInterventionWindow();
            // ShowDialog → attend que la fenêtre se ferme
            if (fenetre.ShowDialog() == true)
            {
                // Recharge la liste si intervention créée
                ChargerInterventions();
            }
        }
    }
}