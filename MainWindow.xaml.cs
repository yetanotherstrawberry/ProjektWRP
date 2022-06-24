using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;

namespace ProjektWRP
{

    public class Wezel
    {

        public uint Numer { get; set; }
        public uint? NrPochlaniacza { get; set; }
        public Node Wierzcholek { get; set; }
        public bool CzyKomputer => Wierzcholek.LabelText[0] == 'K';
        public bool? CzyZarazony { get; set; }

    }

    public class Krawedz
    {

        public Wezel Poczatek { get; set; }
        public Wezel Koniec { get; set; }
        public Ulamek Prawdopodobienstwo { get; set; }

    }

    public partial class MainWindow : Window
    {

        private readonly GViewer gViewer = new GViewer
        {
            NavigationVisible = false,
            UndoRedoButtonsVisible = false,
            LayoutEditingEnabled = false,
            SaveAsMsaglEnabled = false,
            SaveInVectorFormatEnabled = false,
            EdgeInsertButtonVisible = false,
            LayoutAlgorithmSettingsButtonVisible = false,
            PanButtonPressed = true,
        };
        private readonly WindowsFormsHost host = new WindowsFormsHost
        {
            Margin = new Thickness(2, 2, 2, 2),
        };
        private readonly Graph graph = new Graph();
        private readonly ICollection<Wezel> wezly = new List<Wezel>();
        private readonly ICollection<Krawedz> krawedzie = new List<Krawedz>();
        private uint lastPc = 1, lastSwitch = 1, pochlaniacz = 0, lastNode = 0;
        private Macierz matryca = null;

        public MainWindow()
        {

            InitializeComponent();
            DataContext = this;

            host.Child = gViewer;
            diagram.Content = host;

            Odswiez();

        }

        private void Odswiez()
        {

            gViewer.Graph = null;
            gViewer.Graph = graph;

        }

        private void Button_DodajKomputer(object sender, RoutedEventArgs e)
        {

            Node node = graph.AddNode("K" + lastPc++.ToString());
            node.Attr.Color = new Color(0, 200, 0);
            Odswiez();

            wezly.Add(new Wezel
            {
                CzyZarazony = false,
                Wierzcholek = node,
                Numer = lastNode++,
                NrPochlaniacza = pochlaniacz++,
            });

        }

        private void Button_DodajSwitch(object sender, RoutedEventArgs e)
        {

            Node node = graph.AddNode("S" + lastSwitch++.ToString());
            node.Attr.Color = new Color(0, 0, 200);
            Odswiez();

            wezly.Add(new Wezel
            {
                CzyZarazony = null,
                Wierzcholek = node,
                Numer = lastNode++,
                NrPochlaniacza = null,
            });

        }

        private void Licencje(object sender, RoutedEventArgs e)
        {

            MessageBox.Show(Properties.Resources.LICENSE, "Licencje pakietów trzecich i programu");

        }

        private void Button_DodajKrawedz(object sender, RoutedEventArgs e)
        {

            try
            {

                if (PolaczSkad.Text.Equals(PolaczDokad.Text))
                {
                    throw new ArgumentException("Nie można łączyć wierzchołka samego ze sobą.");
                }

                Ulamek prawdopodobienstwo = new Ulamek(PolaczSzansa.Text);
                if (prawdopodobienstwo.Licznik() <= 0 || prawdopodobienstwo.Mianownik() < 0 || prawdopodobienstwo.Licznik() / prawdopodobienstwo.Mianownik() > 1)
                {
                    throw new ArgumentException("Prawdopodobieństwo musi być w zakresie (0, 1].");
                }

                Wezel a = wezly.First(x => x.Wierzcholek.LabelText.Equals(PolaczSkad.Text));
                Wezel b = wezly.First(x => x.Wierzcholek.LabelText.Equals(PolaczDokad.Text));

                if (graph.Edges.Any(x => x.SourceNode == a.Wierzcholek && x.TargetNode == b.Wierzcholek))
                {
                    throw new ArgumentException("Wybrane wierzchołki są już połączone.");
                }

                if (a.CzyKomputer)
                {
                    throw new ArgumentException("Komputer musi być węzłem pochłaniającym.");
                }

                krawedzie.Add(new Krawedz
                {
                    Poczatek = a,
                    Koniec = b,
                    Prawdopodobienstwo = prawdopodobienstwo,
                });

                _ = graph.AddEdge(a.Wierzcholek.Id, prawdopodobienstwo.ToString(), b.Wierzcholek.Id);

                Odswiez();

            }
            catch (Exception wyjatek)
            {

                Blad(wyjatek.Message, "Nie można dodać krawędzi.");

            }

        }

        private string[][] JaggedArray(long wysokosc, long szerokosc)
        {

            string[][] ret = new string[wysokosc][];

            for (int i = 0; i < wysokosc; i++)
            {
                ret[i] = new string[szerokosc];
            }

            return ret;

        }

        private void UtworzMacierz()
        {

            string[][] a = JaggedArray(lastNode, lastNode + (lastPc - 1));

            for (int i = 0; i < a.Length; i++)
            {

                for (int j = 0; j < a[i].Length; j++)
                {

                    a[i][j] = "0/1";

                }

            }

            foreach (Krawedz krawedz in krawedzie)
            {

                a[krawedz.Poczatek.Numer][krawedz.Koniec.Numer] = krawedz.Prawdopodobienstwo.ToString();

            }

            foreach (Wezel wezel in wezly)
            {

                if (wezel.CzyKomputer)
                {

                    a[wezel.Numer][wezel.Numer] = "1/1";
                    a[wezel.Numer][lastNode + wezel.NrPochlaniacza.Value] = "1/1";

                }
                else
                {

                    a[wezel.Numer][wezel.Numer] = "-1/1";

                }

            }

            matryca = new Macierz(a);

        }

        private void Button_MacierzPrzed(object sender, RoutedEventArgs e)
        {

            try
            {

                UtworzMacierz();
                new MacierzOkno(matryca, czyZredukowana: false).Show();

            }
            catch (Exception wyjatek)
            {

                Blad(wyjatek.Message);

            }

        }

        private void Button_MacierzPo(object sender, RoutedEventArgs e)
        {
            Button_MacierzPo(true);
        }

        private void Button_MacierzPo(bool pokazMacierz)
        {

            try
            {

                UtworzMacierz();

                matryca = KalkulatorMacierzy.EliminacjaGaussa(matryca);

                if (pokazMacierz)
                {
                    new MacierzOkno(matryca, czyZredukowana: true).Show();
                }

            }
            catch (Exception wyjatek)
            {

                Blad(wyjatek.Message);

            }

        }

        public string GetRandom(IDictionary<string, double> zbior, Random rand, bool gubiacyWezel = false)
        {

            IDictionary<string, double> temp = new Dictionary<string, double>();

            foreach (KeyValuePair<string, double> element in zbior)
            {
                temp.Add(element);
            }

            if (zbior.Values.Sum() > 1)
            {
                throw new ArgumentException("Prawdopodobieństwo sumuje się do więcej niż 1.");
            }

            if (gubiacyWezel)
            {
                temp.Add("WEZEL", 1 - zbior.Values.Sum());
            }

            double maks = temp.Values.Sum();

            double losowa = rand.NextDouble() * maks;

            double suma = 0;
            foreach (string element in temp.Keys)
            {

                if (losowa <= (suma += temp[element]))
                {
                    return element;
                }

            }

            return null;

        }

        private async void Button_Wirus(object sender, RoutedEventArgs e)
        {

            try
            {

                GuzikWirus.IsEnabled = false;

                if (!wezly.Any(x => x.Wierzcholek.LabelText.Equals(WezelStartowy.Text)))
                {
                    throw new ArgumentException("Musisz określić wierzchołek początkowy.");
                }

                Dictionary<string, Ulamek> wyniki = Prawdopodobienstwo(WezelStartowy.Text);

                if (wyniki.Count == 0)
                {
                    throw new ArgumentException("Nieprawidłowy wierzchołek początkowy lub brak jakichkolwiek ścieżek.");
                }

                Dictionary<string, double> do_petli = new Dictionary<string, double>();

                foreach (string wezel in wyniki.Keys)
                {

                    do_petli.Add(wezel, (double)wyniki[wezel].Licznik() / wyniki[wezel].Mianownik());

                }

                Random rand = new Random();

                foreach (Wezel wezel in wezly.Where(x => x.CzyKomputer))
                {

                    wezel.Wierzcholek.Attr.Color = new Color(0, 200, 0);
                    wezel.CzyZarazony = false;

                }

                Odswiez();

                Dictionary<long, long> wykres = new Dictionary<long, long>();
                long iteracja = 1;

                if (wyniki.Values.Any(x => x.Licznik() == 0))
                {
                    throw new InvalidOperationException("Co najmniej jeden węzeł jest nieosiągalny.");
                }

                while (wezly.Where(x => x.CzyKomputer).Any(x => !x.CzyZarazony.Value))
                {

                    string losowy = GetRandom(do_petli, rand, Puste.IsChecked.Value);

                    if (losowy.Equals("WEZEL"))
                    {
                        iteracja++;
                        continue;
                    }

                    Wezel wylosowany = wezly.Single(x => x.Wierzcholek.LabelText.Equals(losowy));

                    wylosowany.CzyZarazony = true;

                    if (Progres.IsChecked.Value)
                    {
                        wylosowany.Wierzcholek.Attr.Color = new Color(200, 0, 0);
                        Odswiez();
                    }

                    wykres.Add(iteracja, wezly.Where(x => x.CzyKomputer).Count(x => x.CzyZarazony.Value));

                    if (Progres.IsChecked.Value)
                    {
                        await Task.Delay(int.Parse(CzasCzekania.Text));
                    }

                    iteracja++;

                }

                new Wykres(wykres).Show();
                GuzikWirus.IsEnabled = true;

            }
            catch (Exception wyjatek)
            {

                Blad(wyjatek.Message);

            }
            finally
            {

                GuzikWirus.IsEnabled = true;

            }

        }

        private void Progres_Click(object sender, RoutedEventArgs e)
        {

            CzasCzekania.IsEnabled = Progres.IsChecked.Value;

        }

        private void Button_Prawdopodobienstwo(object sender, RoutedEventArgs e)
        {
            new Wyniki(WezelStartowy.Text, Prawdopodobienstwo(WezelStartowy.Text)).Show();
        }

        private Dictionary<string, Ulamek> Prawdopodobienstwo(string startowy)
        {

            try
            {

                Button_MacierzPo(false);

                Dictionary<string, Ulamek> wyniki = new Dictionary<string, Ulamek>();

                Wezel wezel = wezly.First(x => x.Wierzcholek.LabelText.Equals(startowy));

                if (wezel.CzyKomputer)
                {
                    throw new ArgumentException("Węzłem startowym może być tylko przełącznik.");
                }

                for (uint i = lastNode; i < matryca.Tablica().GetLength(1); i++)
                {

                    Wezel pochlaniajacy = wezly.Single(x => x.NrPochlaniacza == i - lastNode);
                    Ulamek prawdopodobienstwo = matryca.Tablica()[wezel.Numer, i];
                    wyniki.Add(pochlaniajacy.Wierzcholek.LabelText, prawdopodobienstwo);

                }

                return wyniki;

            }
            catch (Exception wyjatek)
            {

                Blad(wyjatek.Message);

            }

            return new Dictionary<string, Ulamek>();

        }

        private void Blad(string tresc, string tytul = "Wystąpił błąd.")
        {

            _ = MessageBox.Show(this, tresc, tytul, MessageBoxButton.OK, MessageBoxImage.Error);

        }

    }

}
