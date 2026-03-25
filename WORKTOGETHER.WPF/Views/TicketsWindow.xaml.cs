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

            // ← Nouveau repository à chaque fois
            var repo = new TicketSupportRepository();
            var ticket = repo.FindById(ticketId);

            if (ticket.DateFermeture != null)
            {
                MessageBox.Show("Ce ticket est déjà fermé !", "Information",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ticket.DateFermeture = DateTime.Now;
            repo.Update(ticket);

            ChargerTickets();

            MessageBox.Show("Ticket fermé avec succès !", "Succès",
                            MessageBoxButton.OK, MessageBoxImage.Information);
            ChargerTickets();
        }
    }
}