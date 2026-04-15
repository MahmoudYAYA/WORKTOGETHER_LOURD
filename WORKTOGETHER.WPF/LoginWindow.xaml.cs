using System.Windows;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var userRepo = new UserRepository();
            var user = userRepo.FindByEmail(TxtEmail.Text);

            // Vérifie que l'utilisateur existe et est actif
            if (user == null || user.Actif == 0)
            {
                TxtErreur.Text = "Utilisateur introuvable ou inactif !";
                TxtErreur.Visibility = Visibility.Visible;
                return;
            }

            // Vérifie que c'est bien un admin
            if (!user.Roles.Contains("ROLE_ADMIN") && !user.Roles.Contains("ROLE_COMPTABLE"))
            {
                TxtErreur.Text = "Accès refusé ! Seuls les administrateurs et comptables peuvent se connecter.";
                TxtErreur.Visibility = Visibility.Visible;
                return;
            }

            // Connexion réussie
            MainWindow main = new MainWindow(user);
            main.Show();
            this.Close();
        }
    }
}