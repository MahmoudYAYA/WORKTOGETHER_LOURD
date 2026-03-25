using System;
using System.Windows;
using WORKTOGETHER.DATA.Entities;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Views
{
    public partial class NouvelleUserWindow : Window
    {
        private readonly UserRepository _userRepo = new UserRepository();


        
        public NouvelleUserWindow()
        {
            InitializeComponent();
            
        }

        // une methode de validation 
        private bool Valider()
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(TxtNom.Text)) erreurs.Add("Le nom est obligatiore");
            if (string.IsNullOrEmpty(TxtPrenom.Text)) erreurs.Add("Le prénom est obligatiore");
            if (string.IsNullOrEmpty(TxtEmail.Text)) erreurs.Add("L'email est obligatiore");
            if (string.IsNullOrEmpty(Password.Password)) erreurs.Add("Le mot de passe est obligatiore");
            if (CmbType.SelectedItem == null) erreurs.Add("Veuillez choisir un role");

            if (erreurs.Count > 0)
            {
                MessageBox.Show(string.Join("\n", erreurs), "Erreurs de validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;

            }
            return true;

        }
        public void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            if (!Valider()) return;
            var selectedRole = CmbType.SelectedItem as ComboBoxItem;
            string role = selectedRole.Tag.ToString();

            var user = new User
            {
                Nom = TxtNom.Text,
                Prenom = TxtPrenom.Text,
                Email = TxtEmail.Text,
                Password = BCrypt.Net.BCrypt.HashPassword(Password.Password),
                Roles = $"[\"{role}\"]",
                Actif = 1,
                IsVerified = 0,
                DateCreation = DateTime.Now,
            };

            _userRepo.Create(user);

            MessageBox.Show("Utilisateur créé avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}