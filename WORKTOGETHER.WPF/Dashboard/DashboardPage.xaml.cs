using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Repositories;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.WPF.Dashboard
{
    public partial class DashboardPage : Page
    {
        private readonly UserRepository _userRepo = new UserRepository();
        private readonly BaieRepository _baieRepo = new BaieRepository();
        private readonly CommandeRepository _commandeRepo = new CommandeRepository();
        private readonly TicketSupportRepository _ticketRepo = new TicketSupportRepository();
        private readonly ReservationRepository _reservationRepo = new ReservationRepository();
        private readonly User _currentUser = Session.CurrentUser;

        public DashboardPage()
        {
            InitializeComponent();
            ChargerStatistiques();
            AdapterAffichage();
        }

        private void ChargerStatistiques()
        {
            TxtNbClients.Text = _userRepo.FindAll()
                .Count(u => u.Roles.Contains("ROLE_CLIENT")).ToString();

            TxtNbBaies.Text = _baieRepo.FindAll().Count.ToString();

            TxtNbCommandes.Text = _commandeRepo.FindAll()
                .Count(c => c.StatutPaiement == "paye").ToString();

            TxtNbTickets.Text = _ticketRepo.FindAll()
                .Count(t => t.DateFermeture == null).ToString();

            DgDernieresCommandes.ItemsSource = _commandeRepo
                .FindAllWithDetails()
                .OrderByDescending(c => c.DateCommande)
                .Take(5)
                .ToList();

            DgTicketsOuverts.ItemsSource = _ticketRepo
                .FindAllWithDetails()
                .Where(t => t.DateFermeture == null)
                .Take(5)
                .ToList();
        }

        private void AdapterAffichage()
        {
            if (!_currentUser.Roles.Contains("ROLE_COMPTABLE")) return;

            // Carte 2 : Baies → Réservations
            TxtCard2Label.Text = "Réservations";
            TxtNbBaies.Text = _reservationRepo.FindAll().Count.ToString();

            // Cache les cartes Commandes et Tickets
            BorderCard3.Visibility = Visibility.Collapsed;
            BorderCard4.Visibility = Visibility.Collapsed;
            GridStats.ColumnDefinitions[2].Width = new GridLength(0);
            GridStats.ColumnDefinitions[3].Width = new GridLength(0);

            // Cache la table commandes, étend la section droite
            SectionCommandes.Visibility = Visibility.Collapsed;
            GridTables.ColumnDefinitions[0].Width = new GridLength(0);

            // Remplace tickets par réservations
            TxtTitreSection2.Text = "Dernières réservations";
            DgTicketsOuverts.Visibility = Visibility.Collapsed;
            DgDernieresReservations.Visibility = Visibility.Visible;
            DgDernieresReservations.ItemsSource = _reservationRepo
                .FindAllWithDetails()
                .OrderByDescending(r => r.DateDebut)
                .Take(5)
                .ToList();
        }
    }
}
