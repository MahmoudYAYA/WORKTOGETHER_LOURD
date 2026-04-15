using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using WORKTOGETHER.DATA.Repositories;

namespace WORKTOGETHER.WPF.Rapports
{
    public partial class OccupationPage : Page
    {
        private readonly BaieRepository _baieRepo = new BaieRepository();

        public OccupationPage()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        private void ChargerDonnees()
        {
            var baies = _baieRepo.FindAllwithDetails();

            // ── Stats rapides ──
            int totalBaies = baies.Count;
            int totalUnites = baies.Sum(b => b.CapaciteTotale);
            int occupees = baies.Sum(b => b.NbUnitesOccupees);
            int disponibles = baies.Sum(b => b.NbUnitesDisponibles);
            double taux = totalUnites > 0 ? (double)occupees / totalUnites * 100 : 0;

            TxtTotalBaies.Text = totalBaies.ToString();
            TxtTotalUnites.Text = totalUnites.ToString();
            TxtOccupees.Text = occupees.ToString();
            TxtTaux.Text = $"{taux:F1}%";

            // ── Graphique barres par baie ──
            var valeursOccupees = new ChartValues<double>();
            var valeursDisponibles = new ChartValues<double>();
            var labels = new string[baies.Count];

            for (int i = 0; i < baies.Count; i++)
            {
                labels[i] = baies[i].NumeroBaie;
                valeursOccupees.Add(baies[i].NbUnitesOccupees);
                valeursDisponibles.Add(baies[i].NbUnitesDisponibles);
            }

            SeriesOccupees.Values = valeursOccupees;
            SeriesDisponibles.Values = valeursDisponibles;
            AxisBaies.Labels = labels;

            // ── Graphique camembert global ──
            PieOccupees.Values = new ChartValues<double> { occupees };
            PieDisponibles.Values = new ChartValues<double> { disponibles };
        }
    }
}