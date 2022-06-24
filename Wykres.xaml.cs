using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProjektWRP
{

    public partial class Wykres : Window
    {

        public Wykres(IDictionary<long, long> zakazenia)
        {

            InitializeComponent();
            DataContext = this;

            Chart wykres = new Chart();
            wykres.BeginInit();

            ChartArea przestrzen = new ChartArea();
            przestrzen.AxisX.IsMarginVisible = false;
            przestrzen.AxisX.MajorGrid.Enabled = false;
            przestrzen.AxisY.MajorGrid.Enabled = false;
            przestrzen.AxisX.Interval = 1;
            przestrzen.AxisY.Interval = 1;
            wykres.ChartAreas.Add(przestrzen);
            wykres.Dock = System.Windows.Forms.DockStyle.Fill;
            wykres.TabIndex = 0;

            wykres.EndInit();
            wykres.Series.Clear();
            Series seria = new Series("Liczba")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
            };

            wykres.Series.Add(seria);

            foreach (long iteracja in zakazenia.Keys)
            {

                _ = seria.Points.AddXY(iteracja, zakazenia[iteracja]);

            }

            wykres.Invalidate();

            _ = wykres.Titles.Add("Osiągnięte komputery względem iteracji");

            okienko.Child = wykres;

        }

    }

}
