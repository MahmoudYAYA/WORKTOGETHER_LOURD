using System.Windows;
using WORKTOGETHER.DATA.Entities;
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
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            new UserWindow().Show();
        }

        private void BtnUnites_Click(object sender, RoutedEventArgs e)
        {
            new UnitesWindow().Show();
        }

        private void BtnBaies_Click(object sender, RoutedEventArgs e)
        {
            new BaieWindow().ShowDialog();
        }

        private void BtnCommandes_Click(object sender, RoutedEventArgs e)
        {
            new CommandeWindow().ShowDialog();
        }

        private void BtnTickets_Click(object sender, RoutedEventArgs e)
        {
            new TicketsWindow().ShowDialog();
        }

        private void BtnInterventions_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }
    }
}