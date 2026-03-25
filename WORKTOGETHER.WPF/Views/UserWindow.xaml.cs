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

    }

}