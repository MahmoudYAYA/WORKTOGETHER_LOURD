using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Commandes
{
    public partial class CommandePage : Page
    {
        // Repository pour les commandes
        private readonly CommandeRepository _repo = new CommandeRepository();

        // Commande sélectionnée dans la liste
        private Commande _commandeSelectionnee = null;

        public CommandePage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        // ── Charge toutes les commandes avec client et offre ──
        private void ChargerDonnees()
        {
            DgCommandes.ItemsSource = _repo.FindAllWithDetails();
            ViderDetail();
        }

        // ── Quand on clique sur une ligne → affiche le détail ──
        private void DgCommandes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _commandeSelectionnee = DgCommandes.SelectedItem as Commande;
            if (_commandeSelectionnee != null)
            {
                AfficherDetail(_commandeSelectionnee);
            }
        }

        // ── Bouton ANNULER COMMANDE ──
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            if (_commandeSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une commande !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Empêche l'annulation d'une commande déjà payée
            if (_commandeSelectionnee.StatutPaiement == "paye")
            {
                MessageBox.Show("Impossible d'annuler une commande déjà payée !",
                                "Annulation impossible",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Annuler la commande '{_commandeSelectionnee.NumeroCommande}' ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Met le statut à annulé
                _commandeSelectionnee.StatutPaiement = "annule";
                _repo.Update(_commandeSelectionnee);
                ChargerDonnees();

                MessageBox.Show("Commande annulée !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ── Affiche le détail de la commande sélectionnée ──
        private void AfficherDetail(Commande commande)
        {
            TxtNumero.Text = commande.NumeroCommande;
            TxtClient.Text = commande.Client?.Prenom + " " + commande.Client?.Nom;
            TxtOffre.Text = commande.Offre?.NomOffre;
            TxtTypePaiement.Text = commande.TypePaiement ?? "Non défini";
            TxtMontantHT.Text = commande.MontantTotal + " €";
            TxtTVA.Text = commande.MontantTva + " €";
            TxtStatut.Text = commande.StatutPaiement;
            TxtDateDebut.Text = commande.DateDebutService.ToString("dd/MM/yyyy");
            TxtDateFin.Text = commande.DateFinService.ToString("dd/MM/yyyy");

            if (commande.StripeCardLast4 != null)
                TxtCarte.Text = commande.StripeCardBrand + " **** " + commande.StripeCardLast4;
            else
                TxtCarte.Text = "Non renseignée";

            // ← Affiche les unités de la réservation
            if (commande.Reservation != null)
                DgUnites.ItemsSource = commande.Reservation.Unites;
            else
                DgUnites.ItemsSource = null;
        }

        // ── Vide le panneau de détail ──
        private void ViderDetail()
        {
            TxtNumero.Text = "";
            TxtClient.Text = "";
            TxtOffre.Text = "";
            TxtTypePaiement.Text = "";
            TxtMontantHT.Text = "";
            TxtTVA.Text = "";
            TxtStatut.Text = "";
            TxtDateDebut.Text = "";
            TxtDateFin.Text = "";
            TxtCarte.Text = "";
        }
    }
}