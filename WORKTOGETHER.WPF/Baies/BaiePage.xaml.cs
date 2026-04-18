using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.WPF.Baies
{
    public partial class BaiePage : Page
    {
        /// <summary>
        ///  un Controller pour centraliser la logique métier 
        /// </summary>
        private readonly BaieController _controller = new BaieController();
        private Baie _baieSelectionnee = null;

        
        public BaiePage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        /// <summary>
        /// Methode pour charger les données depuis le Controller et afficher dans le DataGrid
        /// </summary>
        private void ChargerDonnees()
        {
            DgBaies.ItemsSource = _controller.GetAll();
            ViderFormulaire();
        }

        /// <summary>
        /// Méthode appelée quand on sélectionne une ligne dans le DataGrid pour remplir le formulaire avec les données de la baie sélectionnée
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgBaies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _baieSelectionnee = DgBaies.SelectedItem as Baie;
            if (_baieSelectionnee != null)
            {
                RemplirFormulaire(_baieSelectionnee);
                TxtTitreFormulaire.Text = "MODIFIER BAIE";
            }
        }
        /// <summary>
        /// Methode de création d'une baie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            _baieSelectionnee = null;
            DgBaies.SelectedItem = null;
            ViderFormulaire();
            TxtTitreFormulaire.Text = "NOUVELLE BAIE";
            TxtNumeroBaie.Focus();
        }

        /// <summary>
        /// Methode de supprission 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_baieSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une baie !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Supprimer la baie '{_baieSelectionnee.NumeroBaie}' ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            // ← Controller gère toute la logique
            var (succes, message) = _controller.Supprimer(_baieSelectionnee.Id);
            MessageBox.Show(message, succes ? "Succès" : "Erreur",
                            MessageBoxButton.OK,
                            succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (succes) ChargerDonnees();
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // ← Validation dans le Controller
            var erreurs = _controller.Valider(TxtNumeroBaie.Text, TxtCapacite.Text);
            if (erreurs.Count > 0)
            {
                TxtErreur.Text = string.Join("\n", erreurs);
                TxtErreur.Visibility = Visibility.Visible;
                return;
            }

            int capacite = int.Parse(TxtCapacite.Text);

            // ← Création dans le Controller
            var (succes, message) = _controller.Creer(TxtNumeroBaie.Text, capacite);
            MessageBox.Show(message, succes ? "Succès" : "Erreur",
                            MessageBoxButton.OK,
                            succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (succes) ChargerDonnees();
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            _baieSelectionnee = null;
            DgBaies.SelectedItem = null;
            ViderFormulaire();
        }

        private void RemplirFormulaire(Baie baie)
        {
            TxtNumeroBaie.Text = baie.NumeroBaie;
            TxtCapacite.Text = baie.CapaciteTotale.ToString();
        }

        private void ViderFormulaire()
        {
            TxtNumeroBaie.Text = "";
            TxtCapacite.Text = "";
            TxtErreur.Visibility = Visibility.Collapsed;
            TxtTitreFormulaire.Text = "FORMULAIRE";
        }
    }
}