using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;

namespace WORKTOGETHER.WPF.Unites
{
    public partial class UnitePage : Page
    {
        private readonly UniteController _controller = new UniteController();
        private Unite _uniteSelectionnee = null;

        public UnitePage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        private void ChargerDonnees()
        {
            DgUnites.ItemsSource = _controller.GetAll();
        }

        private void DgUnites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _uniteSelectionnee = DgUnites.SelectedItem as Unite;
            if (_uniteSelectionnee != null)
                AfficherDetail(_uniteSelectionnee);
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (_uniteSelectionnee == null)
            {
                MessageBox.Show("Veuillez sélectionner une unité ", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

          

            var selectedEtat = CmbEtat.SelectedItem as ComboBoxItem;
            if (selectedEtat == null)
            {
                TxtErreur.Text = "Veuillez choisir un état !";
                TxtErreur.Visibility = Visibility.Visible;
                return;
            }

            var ancienEtat = CmbEtat.SelectedItem as ComboBoxItem;
            if (ancienEtat == selectedEtat)
            {
                TxtErreur.Text = "Veuillez changer l'état de unité d'avant ";
                TxtErreur.Visibility = Visibility.Visible;
                return;
            }
            var (succes, message) = _controller.Modifier(
                _uniteSelectionnee,
                selectedEtat.Tag.ToString());

            MessageBox.Show(message, succes ? "Succès" : "Erreur",
                            MessageBoxButton.OK,
                            succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (succes) ChargerDonnees();
        }

        private void AfficherDetail(Unite unite)
        {
            TxtNumero.Text = unite.NumeroUnite;
            TxtNom.Text = unite.NomUnite;
            TxtBaie.Text = unite.Baie?.NumeroBaie ?? "Non définie";
            TxtStatut.Text = unite.Statut;
            TxtClient.Text = unite.Reservation?.Client?.Prenom + " " +
                              unite.Reservation?.Client?.Nom ?? "Disponible";

            foreach (ComboBoxItem item in CmbEtat.Items)
                if (item.Tag.ToString() == unite.Etat)
                { CmbEtat.SelectedItem = item; break; }
        }
    }
}