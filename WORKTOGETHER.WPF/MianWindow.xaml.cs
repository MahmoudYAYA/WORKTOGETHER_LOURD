using System.Windows;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.WPF.Baies;
using WORKTOGETHER.WPF.Commandes;
using WORKTOGETHER.WPF.Dashboard;
using WORKTOGETHER.WPF.Interventions;
using WORKTOGETHER.WPF.Offres;
using WORKTOGETHER.WPF.Reservations;
using WORKTOGETHER.WPF.TicketSupports;
using WORKTOGETHER.WPF.Unites; 
using WORKTOGETHER.WPF.Users;
using WORKTOGETHER.WPF.Rapports;

namespace WORKTOGETHER.WPF
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            TxtUsername.Text = user.Prenom + " " + user.Nom;

            // ← Cache les menus admin si comptable
            if (user.Roles.Contains("ROLE_COMPTABLE"))
            {
                BtnUsers.Visibility = Visibility.Collapsed;
                BtnBaies.Visibility = Visibility.Collapsed;
                BtnInterventions.Visibility = Visibility.Collapsed;
                BtnOffres.Visibility = Visibility.Collapsed;
                BtnUnites.Visibility = Visibility.Collapsed;
            }

            MainFrame.Navigate(new DashboardPage());
        }
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void BtnRapports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RapportPage());
        }
        private void BtnOccupation_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OccupationPage());
        }
        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UserPage());
        }

        private void BtnBaies_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BaiePage());
        }

        private void BtnCommandes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CommandePage());
        }

        private void BtnTickets_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TicketPage());
        }

        private void BtnInterventions_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new InterventionPage());
        }

        private void BtnOffres_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OffrePage());
        }
        private void BtnUnites_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UnitePage());    
        }


        // une button pour les reservation 

        private void BtnReservations_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReservationPage());
        }
        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            // LoginWindow est dans le namespace principal
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }


    }
}