using System.Windows;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.WPF.Baies;
using WORKTOGETHER.WPF.Commandes;
using WORKTOGETHER.WPF.Interventions;
using WORKTOGETHER.WPF.Tickets;
using WORKTOGETHER.WPF.Users;
using WORKTOGETHER.WPF.Views;

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

            // ← Affiche le dashboard par défaut
            MainFrame.Navigate(new DashboardPage());
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
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

        private void BtnUnites_Click(object sender, RoutedEventArgs e)
        {
            // On fera UnitePage après
        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}