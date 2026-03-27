using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Users
{
    public partial class UserPage : Page
    {
        private readonly UserRepository _repo = new UserRepository();
        private List<User> _tousLesUsers;
        // Utilisateur sélectionné dans la liste (null = mode création)
        private User _userSelectionne = null;

        public UserPage()
        {
            InitializeComponent();
            ChargerUsers();
        }

        // ── Charge tous les utilisateurs depuis la BDD ──
        private void ChargerUsers()
        {
            _tousLesUsers = _repo.FindAll();
            DgUsers.ItemsSource = _tousLesUsers;
            ViderFormulaire();
        }

        // ── Filtre la liste selon la recherche ──
        private void TxtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            var recherche = TxtRecherche.Text.ToLower();
            DgUsers.ItemsSource = _tousLesUsers
                .Where(u => u.Nom.ToLower().Contains(recherche)
                         || u.Prenom.ToLower().Contains(recherche)
                         || u.Email.ToLower().Contains(recherche))
                .ToList();
        }

        // ── Quand on clique sur une ligne → remplit le formulaire ──
        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _userSelectionne = DgUsers.SelectedItem as User;
            if (_userSelectionne != null)
            {
                RemplirFormulaire(_userSelectionne);
                TxtTitreFormulaire.Text = "MODIFIER UTILISATEUR";
            }
        }

        // ── Bouton CRÉER → vide le formulaire et désélectionne ──
        private void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            // null = mode création
            _userSelectionne = null;
            DgUsers.SelectedItem = null;
            ViderFormulaire();
            TxtTitreFormulaire.Text = "NOUVEL UTILISATEUR";
            TxtPrenom.Focus();
        }

        // ── Bouton MODIFIER → vérifie qu'un user est sélectionné ──
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

        // ── Bouton SUPPRIMER → confirmation puis suppression ──
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

            if (result == MessageBoxResult.Yes)
            {
                _repo.Delete(_userSelectionne.Id);
                ChargerUsers();
            }
        }

        // ── Bouton ENREGISTRER → crée ou modifie selon _userSelectionne ──
        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (!Valider()) return;

            var selectedRole = CmbRole.SelectedItem as ComboBoxItem;

            if (_userSelectionne == null)
            {
                // ── MODE CRÉATION ──
                var user = new User
                {
                    Prenom = TxtPrenom.Text,
                    Nom = TxtNom.Text,
                    Email = TxtEmail.Text,
                    Password = BCrypt.Net.BCrypt.HashPassword(TxtPassword.Password),
                    Roles = $"[\"{selectedRole.Tag}\"]",
                    Actif = 1,
                    IsVerified = 0,
                    DateCreation = System.DateTime.Now
                };
                _repo.Create(user);
                MessageBox.Show("Utilisateur créé !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // ── MODE MODIFICATION ──
                _userSelectionne.Prenom = TxtPrenom.Text;
                _userSelectionne.Nom = TxtNom.Text;
                _userSelectionne.Email = TxtEmail.Text;
                _userSelectionne.Roles = $"[\"{selectedRole.Tag}\"]";

                // Change le mot de passe seulement si renseigné
                if (!string.IsNullOrEmpty(TxtPassword.Password))
                    _userSelectionne.Password = BCrypt.Net.BCrypt.HashPassword(TxtPassword.Password);

                _repo.Update(_userSelectionne);
                MessageBox.Show("Utilisateur modifié !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ChargerUsers();
        }

        // ── Bouton ANNULER → vide le formulaire ──
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            _userSelectionne = null;
            DgUsers.SelectedItem = null;
            ViderFormulaire();
        }

        // ── Remplit le formulaire avec les données du user ──
        private void RemplirFormulaire(User user)
        {
            TxtPrenom.Text = user.Prenom;
            TxtNom.Text = user.Nom;
            TxtEmail.Text = user.Email;
            TxtPassword.Clear(); // Ne jamais afficher le mot de passe hashé

            // Sélectionne le bon rôle dans le ComboBox
            foreach (ComboBoxItem item in CmbRole.Items)
            {
                if (user.Roles.Contains(item.Tag.ToString()))
                {
                    CmbRole.SelectedItem = item;
                    break;
                }
            }
        }

        // ── Vide tous les champs du formulaire ──
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

        // ── Valide les champs obligatoires ──
        private bool Valider()
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(TxtPrenom.Text)) erreurs.Add("Le prénom est obligatoire");
            if (string.IsNullOrEmpty(TxtNom.Text)) erreurs.Add("Le nom est obligatoire");
            if (string.IsNullOrEmpty(TxtEmail.Text)) erreurs.Add("L'email est obligatoire");

            // Mot de passe obligatoire seulement en création
            if (_userSelectionne == null && string.IsNullOrEmpty(TxtPassword.Password))
                erreurs.Add("Le mot de passe est obligatoire");

            if (CmbRole.SelectedItem == null) erreurs.Add("Veuillez choisir un rôle");

            if (erreurs.Count > 0)
            {
                TxtErreur.Text = string.Join("\n", erreurs);
                TxtErreur.Visibility = Visibility.Visible;
                return false;
            }

            TxtErreur.Visibility = Visibility.Collapsed;
            return true;
        }
    }
}