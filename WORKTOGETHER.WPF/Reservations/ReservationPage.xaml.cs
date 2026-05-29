using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Reservations
{
    public partial class ReservationPage : Page
    {
        private readonly ReservationController _controller = new ReservationController();
        private Reservation _reservationSelectionnee = null;

        public ReservationPage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        private void ChargerDonnees()
        {
            DgReservations.ItemsSource = _controller.GetAll();
            ViderDetail();
        }

        private void DgReservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _reservationSelectionnee = DgReservations.SelectedItem as Reservation;
            if (_reservationSelectionnee != null)
                AfficherDetail(_reservationSelectionnee);
        }

        private void AfficherDetail(Reservation reservation)
        {
            TxtClient.Text = reservation.Client?.Prenom + " " + reservation.Client?.Nom;
            TxtOffre.Text = reservation.Offre?.NomOffre;
            TxtDateDebut.Text = reservation.DateDebut.ToString("dd/MM/yyyy");
            TxtDateFin.Text = reservation.DateFin.ToString("dd/MM/yyyy");
            TxtPrixTotal.Text = reservation.PrixTotal + " €";
            TxtNbUnites.Text = $"{reservation.Unites.Count} unité(s)";
            DgUnites.ItemsSource = reservation.Unites;
        }

      
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