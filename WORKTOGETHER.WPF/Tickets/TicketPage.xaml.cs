using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Entities;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Tickets
{
    public partial class TicketPage : Page
    {
        // Repositories nécessaires
        private readonly TicketSupportRepository _repo = new TicketSupportRepository();
        private readonly UserRepository _userRepo = new UserRepository();

        // Ticket sélectionné (null = mode création)
        private TicketSupport _ticketSelectionne = null;

        public TicketPage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        // ── Charge les tickets et les clients ──
        private void ChargerDonnees()
        {
            // Charge les tickets avec leurs clients
            DgTickets.ItemsSource = _repo.FindAllWithDetails();

            // Charge seulement les clients dans le ComboBox
            CmbClient.ItemsSource = _userRepo.FindAll()
                .FindAll(u => u.Roles.Contains("ROLE_CLIENT"));

            ViderFormulaire();
        }

        // ── Quand on clique sur une ligne ──
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

        // ── Bouton FERMER → met la date de fermeture ──
        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            if (_ticketSelectionne == null)
            {
                MessageBox.Show("Veuillez sélectionner un ticket !", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Vérifie que le ticket n'est pas déjà fermé
            if (_ticketSelectionne.DateFermeture != null)
            {
                MessageBox.Show("Ce ticket est déjà fermé !", "Information",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Ferme le ticket via méthode spécifique du repo
            _repo.Fermer(_ticketSelectionne.Id);
            ChargerDonnees();

            MessageBox.Show("Ticket fermé avec succès !", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);
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

            // Empêche la suppression d'un ticket ouvert
            if (_ticketSelectionne.DateFermeture == null)
            {
                MessageBox.Show("Impossible de supprimer un ticket ouvert !\nFermez-le d'abord.",
                                "Suppression impossible",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Supprimer le ticket '{_ticketSelectionne.NumeroTicket}' ?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _repo.Delete(_ticketSelectionne.Id);
                ChargerDonnees();
            }
        }

        // ── Bouton ENREGISTRER → crée ou modifie ──
        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (!Valider()) return;

            var selectedPriorite = CmbPriorite.SelectedItem as ComboBoxItem;
            int priorite = int.Parse(selectedPriorite.Tag.ToString());
            var client = CmbClient.SelectedItem as User;

            if (_ticketSelectionne == null)
            {
                // ── MODE CRÉATION ──
                var ticket = new TicketSupport
                {
                    NumeroTicket = "TKT-" + DateTime.Now.Ticks,
                    Sujet = TxtSujet.Text,
                    Description = TxtDescription.Text,
                    Priorite = priorite,
                    DateCreation = DateTime.Now,
                    ClientId = client.Id
                };
                _repo.Create(ticket);
                MessageBox.Show("Ticket créé !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // ── MODE MODIFICATION ──
                _ticketSelectionne.Sujet = TxtSujet.Text;
                _ticketSelectionne.Description = TxtDescription.Text;
                _ticketSelectionne.Priorite = priorite;
                _ticketSelectionne.ClientId = client.Id;
                _repo.Update(_ticketSelectionne);
                MessageBox.Show("Ticket modifié !", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ChargerDonnees();
        }

        // ── Bouton ANNULER ──
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            _ticketSelectionne = null;
            DgTickets.SelectedItem = null;
            ViderFormulaire();
        }

        // ── Remplit le formulaire avec les données du ticket ──
        private void RemplirFormulaire(TicketSupport ticket)
        {
            TxtSujet.Text = ticket.Sujet;
            TxtDescription.Text = ticket.Description;

            // Sélectionne la bonne priorité
            foreach (ComboBoxItem item in CmbPriorite.Items)
            {
                if (item.Tag.ToString() == ticket.Priorite.ToString())
                {
                    CmbPriorite.SelectedItem = item;
                    break;
                }
            }

            // Sélectionne le bon client
            foreach (User u in CmbClient.Items)
            {
                if (u.Id == ticket.ClientId)
                {
                    CmbClient.SelectedItem = u;
                    break;
                }
            }
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

        // ── Valide les champs obligatoires ──
        private bool Valider()
        {
            var erreurs = new List<string>();

            if (string.IsNullOrEmpty(TxtSujet.Text)) erreurs.Add("Le sujet est obligatoire");
            if (CmbPriorite.SelectedItem == null) erreurs.Add("Veuillez choisir une priorité");
            if (CmbClient.SelectedItem == null) erreurs.Add("Veuillez choisir un client");

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