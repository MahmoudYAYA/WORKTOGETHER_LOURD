using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Reservations
{
    public partial class ReservationPage : Page
    {
        // Repositories
        private readonly ReservationRepository _reservationRepo = new ReservationRepository();
        private readonly UniteRepository _uniteRepo = new UniteRepository();

        // Réservation sélectionnée
        private Reservation _reservationSelectionnee = null;

        public ReservationPage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        // ── Charge toutes les réservations ──
        private void ChargerDonnees()
        {
            // ← Utilise FindAllWithDetails() au lieu de FindByClient(0)
            DgReservations.ItemsSource = _reservationRepo.FindAllWithDetails();
            ViderDetail();
        }

        // ── Quand on clique sur une ligne → affiche le détail ──
        private void DgReservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _reservationSelectionnee = DgReservations.SelectedItem as Reservation;
            if (_reservationSelectionnee != null)
            {
                AfficherDetail(_reservationSelectionnee);
            }
        }

        // ── Bouton ANNULER RÉSERVATION ──
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            if (_reservationSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une réservation !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Annuler la réservation de {_reservationSelectionnee.Client?.Nom} ?\n" +
                $"Les unités seront libérées.",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Libère toutes les unités
                var unites = _uniteRepo.FindAll()
                    .Where(u => u.ReservationId == _reservationSelectionnee.Id)
                    .ToList();

                foreach (var unite in unites)
                {
                    unite.Statut = "disponible";
                    unite.ReservationId = null;
                    _uniteRepo.Update(unite);
                }

                // Supprime la réservation
                _reservationRepo.Delete(_reservationSelectionnee.Id);
                ChargerDonnees();

                MessageBox.Show("Réservation annulée ! Les unités ont été libérées.", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ── Affiche le détail de la réservation ──
        private void AfficherDetail(Reservation reservation)
        {
            TxtClient.Text = reservation.Client?.Prenom + " " + reservation.Client?.Nom;
            TxtOffre.Text = reservation.Offre?.NomOffre;
            TxtDateDebut.Text = reservation.DateDebut.ToString("dd/MM/yyyy");
            TxtDateFin.Text = reservation.DateFin.ToString("dd/MM/yyyy");
            TxtPrixTotal.Text = reservation.PrixTotal + " €";

            // Charge les unités de cette réservation
            var unites = _uniteRepo.FindAll()
                .Where(u => u.ReservationId == reservation.Id)
                .ToList();

            TxtNbUnites.Text = $" {unites.Count} unité(s) attribuée(s)";
            DgUnites.ItemsSource = unites;
        }

        // ── Vide le panneau détail ──
        private void ViderDetail()
        {
            TxtClient.Text = "";
            TxtOffre.Text = "";
            TxtDateDebut.Text = "";
            TxtDateFin.Text = "";
            TxtPrixTotal.Text = "";
            TxtNbUnites.Text = "";
            DgUnites.ItemsSource = null;
        }
    }
}