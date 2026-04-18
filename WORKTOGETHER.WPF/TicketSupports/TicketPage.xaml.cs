using System.Net;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.WPF.Tickets;

namespace WORKTOGETHER.WPF.TicketSupports
{
    public partial class TicketPage : Page
    {
        // ← Un seul controller !
        private readonly TicketController _controller = new TicketController();
        private TicketSupport _ticketSelectionne = null;

        public TicketPage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        // ── Charge les données ──
        private void ChargerDonnees()
        {
            DgTickets.ItemsSource = _controller.GetAll();
            CmbClient.ItemsSource = _controller.GetClients();
            ViderFormulaire();
        }

        // ── Sélection dans le tableau ──
        private void DgTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ticketSelectionne = DgTickets.SelectedItem as TicketSupport;
            if (_ticketSelectionne != null)
            {
                RemplirFormulaire(_ticketSelectionne);
                TxtTitreFormulaire.Text = "DÉTAIL TICKET";
            }
        }

        // ── Bouton CRÉER ──
        private void BtnCreer_Click(object sender, RoutedEventArgs e)
        {
            _ticketSelectionne = null;
            DgTickets.SelectedItem = null;
            ViderFormulaire();
            TxtTitreFormulaire.Text = "NOUVEAU TICKET";
            TxtSujet.Focus();
        }

        // ── Bouton FERMER ──
        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            if (_ticketSelectionne == null)
            {
                MessageBox.Show("Veuillez sélectionner un ticket !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ← Controller gère la logique
            var (succes, message) = _controller.Fermer(_ticketSelectionne.Id);
            MessageBox.Show(message, succes ? "Succès" : "Erreur",
                            MessageBoxButton.OK,
                            succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (succes) ChargerDonnees();
        }

        // ── Bouton SUPPRIMER ──
        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (_ticketSelectionne == null)
            {
                MessageBox.Show("Veuillez sélectionner un ticket !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Supprimer le ticket '{_ticketSelectionne.NumeroTicket}' ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            var (succes, message) = _controller.Supprimer(_ticketSelectionne.Id);
            MessageBox.Show(message, succes ? "Succès" : "Erreur",
                            MessageBoxButton.OK,
                            succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

            if (succes) ChargerDonnees();
        }

        // ── Bouton ENREGISTRER ──
        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            var selectedPriorite = CmbPriorite.SelectedItem as ComboBoxItem;
            var client = CmbClient.SelectedItem as User;

            // ← Validation dans le Controller
            var erreurs = _controller.Valider(TxtSujet.Text, selectedPriorite, client);
            if (erreurs.Count > 0)
            {
                TxtErreur.Text = string.Join("\n", erreurs);
                TxtErreur.Visibility = Visibility.Visible;
                return;
            }

            int priorite = int.Parse(selectedPriorite.Tag.ToString());

            try
            {
                (bool succes, string message) resultat;

                if (_ticketSelectionne == null)
                    // ← Création
                    resultat = _controller.Creer(
                        TxtSujet.Text, TxtDescription.Text,
                        priorite, client.Id);
                else
                    // ← Modification
                    resultat = _controller.Modifier(
                        _ticketSelectionne, TxtSujet.Text,
                        TxtDescription.Text, priorite, client.Id);

                MessageBox.Show(resultat.message,
                                resultat.succes ? "Succès" : "Erreur",
                                MessageBoxButton.OK,
                                resultat.succes ? MessageBoxImage.Information : MessageBoxImage.Warning);

                if (resultat.succes) ChargerDonnees();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}");
            }
        }

        // ── Bouton ANNULER ──
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            _ticketSelectionne = null;
            DgTickets.SelectedItem = null;
            ViderFormulaire();
        }

        // ── Remplit le formulaire ──
        private void RemplirFormulaire(TicketSupport ticket)
        {
            TxtSujet.Text = ticket.Sujet;
            TxtDescription.Text = ticket.Description;
            TxtTitreFormulaire.Text = "DÉTAIL TICKET";

            foreach (ComboBoxItem item in CmbPriorite.Items)
                if (item.Tag.ToString() == ticket.Priorite.ToString())
                { CmbPriorite.SelectedItem = item; break; }

            foreach (User u in CmbClient.Items)
                if (u.Id == ticket.ClientId)
                { CmbClient.SelectedItem = u; break; }
        }

        // ── Vide le formulaire ──
        private void ViderFormulaire()
        {
            TxtSujet.Text = "";
            TxtDescription.Text = "";
            CmbPriorite.SelectedIndex = -1;
            CmbClient.SelectedIndex = -1;
            TxtErreur.Visibility = Visibility.Collapsed;
            TxtTitreFormulaire.Text = "FORMULAIRE";
        }
    }
}