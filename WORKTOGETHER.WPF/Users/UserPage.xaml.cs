using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WORKTOGETHER.DATA.Entities;
namespace WORKTOGETHER.WPF.Users

{
    public partial class UserPage : Page
    {
        private readonly UserController _controller = new UserController();
        private List<User> _tousLesUsers;
        private User _userSelectionne = null;
        private readonly User _currentUser = Session.CurrentUser;

        public UserPage()
        {
            InitializeComponent();
            ChargerUsers();
            AdapterAffichage();
        }

        private void AdapterAffichage()
        {
            if (!_currentUser.Roles.Contains("ROLE_COMPTABLE")) return;

            PanelBoutons.Visibility = Visibility.Collapsed;
            PanelFormulaire.Visibility = Visibility.Collapsed;
            GridContent.ColumnDefinitions[1].Width = new GridLength(0);
        }

        // ── Charge les données ──
        private void ChargerUsers()
        {
            try
            {
                _tousLesUsers = _controller.GetAll();
                DgUsers.ItemsSource = _tousLesUsers;
                ViderFormulaire();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de connexion : {ex.Message}", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Filtre la recherche ──
        private void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            var recherche = TxtRecherche.Text.ToLower();
            DgUsers.ItemsSource = _tousLesUsers
                .Where(u => u.Nom.ToLower().Contains(recherche)
                         || u.Prenom.ToLower().Contains(recherche)
                         || u.Email.ToLower().Contains(recherche))
                .ToList();
        }

        // Sélection dans le tableau 
        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _userSelectionne = DgUsers.SelectedItem as User;
            if (_userSelectionne != null)
            {
                RemplirFormulaire(_userSelectionne);
                TxtTitreFormulaire.Text = "MODIFIER UTILISATEUR";
            }
        }

        // ── Bouton CRÉER ──
        private void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            _userSelectionne = null;
            DgUsers.SelectedItem = null;
            ViderFormulaire();
            TxtTitreFormulaire.Text = "NOUVEL UTILISATEUR";
            TxtPrenom.Focus();
        }

        // ── Bouton MODIFIER ──
        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (_userSelectionne == null)
            {
                MessageBox.Show("Veuillez sélectionner un utilisateur !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            TxtTitreFormulaire.Text = "MODIFIER UTILISATEUR";
            RemplirFormulaire(_userSelectionne);
        }

        
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_userSelectionne == null)
            {
                MessageBox.Show("Veuillez sélectionner un utilisateur !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Supprimer {_userSelectionne.Prenom} {_userSelectionne.Nom} ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            
            var (succes, message) = _controller.Supprimer(_userSelectionne.Id);
            MessageBox.Show(message, succes ? "Succès" : "Erreur",
                            MessageBoxButton.OK,
                            succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (succes) ChargerUsers();
        }

        //  Bouton  
        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            var selectedRole = CmbRole.SelectedItem as ComboBoxItem;
            bool isCreation = _userSelectionne == null;

            // Validation dans le Controller
            var erreurs = _controller.Valider(
                TxtPrenom.Text, TxtNom.Text, TxtEmail.Text,
                selectedRole, TxtPassword.Password, isCreation);

            if (erreurs.Count > 0)
            {
                TxtErreur.Text = string.Join("\n", erreurs);
                TxtErreur.Visibility = Visibility.Visible;
                return;
            }

            string role = selectedRole.Tag.ToString();

            try
            {
                (bool succes, string message) resultat;

                if (isCreation)
                    resultat = _controller.Creer(
                        TxtPrenom.Text, TxtNom.Text,
                        TxtEmail.Text, TxtPassword.Password, role);
                else
                    resultat = _controller.Modifier(
                        _userSelectionne, TxtPrenom.Text, TxtNom.Text,
                        TxtEmail.Text, role, TxtPassword.Password);

                MessageBox.Show(resultat.message,
                                resultat.succes ? "Succès" : "Erreur",
                                MessageBoxButton.OK,
                                resultat.succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

                if (resultat.succes) ChargerUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        //Bouton ANNULER 
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            _userSelectionne = null;
            DgUsers.SelectedItem = null;
            ViderFormulaire();
        }

        //  Remplit le formulaire 
        private void RemplirFormulaire(User user)
        {
            TxtPrenom.Text = user.Prenom;
            TxtNom.Text = user.Nom;
            TxtEmail.Text = user.Email;
            TxtPassword.Clear();

            foreach (ComboBoxItem item in CmbRole.Items)
                if (user.Roles.Contains(item.Tag.ToString()))
                { CmbRole.SelectedItem = item; break; }
        }

        // Vide le formulaire
        private void ViderFormulaire()
        {
            TxtPrenom.Text = "";
            TxtNom.Text = "";
            TxtEmail.Text = "";
            TxtPassword.Clear();
            CmbRole.SelectedIndex = -1;
            TxtErreur.Visibility = Visibility.Collapsed;
            TxtTitreFormulaire.Text = "FORMULAIRE";
        }
    }
}