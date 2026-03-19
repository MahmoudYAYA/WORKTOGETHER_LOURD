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
using System.Windows.Shapes;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Views
{
    /// <summary>
    /// Logique d'interaction pour Baie.xaml
    /// </summary>
    public partial class BaieWindow : Window
    {
        private readonly BaieRepository _repository;
        public BaieWindow()
        {
            InitializeComponent();
            _repository = new BaieRepository();
            ChargerBaies();
        }

        private void ChargerBaies()
        {
            DgBaies.ItemsSource = _repository.FindAllwithDetails();
        }

    }
}
