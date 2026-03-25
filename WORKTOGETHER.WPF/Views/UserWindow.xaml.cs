using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WORKTOGETHER.DATA.Repositories;
using System.Windows.Shapes;

namespace WORKTOGETHER.WPF.Views
{
    /// <summary>
    /// Logique d'interaction pour UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private readonly UserRepository _repository;

        public UserWindow()
        {
            InitializeComponent();
            _repository = new UserRepository();
            ChargerUsers();
        }

        private void ChargerUsers()
        {
            DgUtilisateurs.ItemsSource = _repository.FindAll();
        }
        private void BtnNouveau_Click(object sender, RoutedEventArgs e)
        {
            var fenetre = new NouvelleUserWindow();
            if (fenetre.ShowDialog() == true)
            {
                ChargerUsers();
            }
        }

        // methode pour surpprimer un utilisateur 
        public void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int userId = (int)btn.Tag;

            // confirmation 
            var result = MessageBox.Show("Voulez vous vraiment supprimer cet utilisateur ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) {
                _repository.Delete(userId);
                ChargerUsers();
                MessageBox.Show("Utilisateur supprimé ", "Succès",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // une methode pour acitver ou désactiver le utilisateur 
        public void BtnToggle_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int userId = (int)btn.Tag;

            var user = _repository.FindById(userId);

            // inverser les champs actif et inactif 
            user.Actif = user.Actif == 1 ? (sbyte)0 : (sbyte)1;
            _repository.Update(user);
            ChargerUsers();
            MessageBox.Show("Utilisateur modifier", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }

}