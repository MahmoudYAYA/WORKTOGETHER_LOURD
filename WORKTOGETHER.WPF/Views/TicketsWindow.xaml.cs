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
    /// Logique d'interaction pour TicketsWindow.xaml
    /// </summary>
    public partial class TicketsWindow : Window
    {
        private TicketSupportRepository _repository;

        public TicketsWindow()
        {

            InitializeComponent();
            _repository = new TicketSupportRepository();
            ChargerTicketSupport();
        }

        private void ChargerTicketSupport()
        {
            DgTickets.ItemsSource = _repository.FindAllWithDetails();
        }
    }
}
