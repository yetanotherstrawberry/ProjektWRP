using System.Windows;
using System.Windows.Controls;

namespace ProjektWRP
{

    public partial class MacierzOkno : Window
    {

        public MacierzOkno(Macierz macierz, bool? czyZredukowana = null)
        {

            InitializeComponent();

            okienko.Children.Clear();
            _ = okienko.Children.Add(new DataGrid
            {
                ColumnWidth = DataGridLength.SizeToCells,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HeadersVisibility = DataGridHeadersVisibility.None,
                IsEnabled = false,
                CanUserAddRows = false,
                ItemsSource = macierz.DataTable().DefaultView,
                FontSize = 20
            });

            if (czyZredukowana != null)
            {

                if (czyZredukowana.Value)
                {

                    Title = "Zredukowana macierz";

                }
                else
                {

                    Title = "Macierz przed redukcją";

                }

            }

        }


    }

}
