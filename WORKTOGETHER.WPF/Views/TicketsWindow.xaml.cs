using System.Windows;
using System.Windows.Controls;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Views
{
    public partial class TicketsWindow : Window
    {
        private readonly TicketSupportRepository _repository = new TicketSupportRepository();

        public TicketsWindow()
        {
            InitializeComponent();
            ChargerTickets();
        }

        private void ChargerTickets()
        {
            DgTickets.ItemsSource = _repository.FindAllWithDetails();
        }

        private void BtnFermer_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int ticketId = (int)btn.Tag;

            var ticket = _repository.FindById(ticketId);

            // Vérifie que le ticket n'est pas déjà fermé
            if (ticket.DateFermeture != null)
            {
                MessageBox.Show("Ce ticket est déjà fermé !", "Information",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Ferme le ticket
            ticket.DateFermeture = DateTime.Now;
            _repository.Update(ticket);

            // Recharge la liste
            ChargerTickets();

            MessageBox.Show("Ticket fermé avec succès !", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}