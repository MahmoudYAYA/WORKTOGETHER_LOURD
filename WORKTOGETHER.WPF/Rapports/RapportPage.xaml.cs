using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Rapports
{
    // Classe pour les données du rapport
    public class LigneRapport
    {
        public string NomOffre { get; set; }
        public int NbReservations { get; set; }
        public decimal ChiffreAffaires { get; set; }
        public decimal Tva => ChiffreAffaires * 0.2m;
        public decimal ChiffreAffairesTTC => ChiffreAffaires + Tva;
    }

    public partial class RapportPage : Page
    {
        private readonly CommandeRepository _commandeRepo = new CommandeRepository();

        public RapportPage()
        {
            InitializeComponent();
            InitialiserAnnees();
            // Sélectionne le mois et l'année actuels
            CmbMois.SelectedIndex = DateTime.Now.Month - 1;
        }

        // ── Initialise la liste des années ──
        private void InitialiserAnnees()
        {
            int anneeActuelle = DateTime.Now.Year;
            for (int i = anneeActuelle; i >= anneeActuelle - 3; i--)
            {
                var item = new ComboBoxItem
                {
                    Content = i.ToString(),
                    Tag = i
                };
                CmbAnnee.Items.Add(item);
            }
            CmbAnnee.SelectedIndex = 0;
        }

        // ── Bouton GÉNÉRER → calcule le rapport ──
        private void BtnGenerer_Click(object sender, RoutedEventArgs e)
        {
            if (CmbMois.SelectedItem == null || CmbAnnee.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner un mois et une année !",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Récupère le mois et l'année sélectionnés
            int mois = int.Parse(((ComboBoxItem)CmbMois.SelectedItem).Tag.ToString());
            int annee = int.Parse(((ComboBoxItem)CmbAnnee.SelectedItem).Tag.ToString());

            // Récupère les commandes du mois sélectionné
            var commandes = _commandeRepo.FindAllWithDetails()
                .Where(c => c.DateCommande.Month == mois
                         && c.DateCommande.Year == annee
                         && c.StatutPaiement == "paye")
                .ToList();

            // Groupe par offre
            var rapport = commandes
                .GroupBy(c => c.Offre?.NomOffre ?? "Inconnue")
                .Select(g => new LigneRapport
                {
                    NomOffre = g.Key,
                    NbReservations = g.Count(),
                    ChiffreAffaires = g.Sum(c => c.MontantTotal)
                })
                .OrderByDescending(l => l.ChiffreAffaires)
                .ToList();

            // Affiche dans le DataGrid
            DgRapport.ItemsSource = rapport;

            // Calcule les totaux
            int totalReservations = rapport.Sum(l => l.NbReservations);
            decimal totalCA = rapport.Sum(l => l.ChiffreAffaires);
            decimal totalTTC = rapport.Sum(l => l.ChiffreAffairesTTC);

            TxtTotalReservations.Text = $"{totalReservations} réservation(s)";
            TxtTotalCA.Text = $"CA HT : {totalCA:N2} €";
            TxtTotalTTC.Text = $"CA TTC : {totalTTC:N2} €";
        }
    }
}