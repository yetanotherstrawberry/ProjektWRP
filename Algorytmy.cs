using Microsoft.VisualBasic;
using System.Data;

namespace ProjektWRP
{

    public partial class KalkulatorMacierzy
    {

        private const string bladPodalgorytmu = "Algorytm podrzędny został zakończony zanim algorytm wyższego poziomu skończył pracę.";

        //public Macierz macierz { get; set; }

        // https://en.wikipedia.org/wiki/Gaussian_elimination#Pseudocode
        // https://apollo.astro.amu.edu.pl/PAD/pmwiki.php?n=Dybol.DydaktykaEliminacjaGaussa
        //private void EliminacjaGaussa(object obiekt, RoutedEventArgs argumenty)
        public static Macierz EliminacjaGaussa(Macierz macierz, object obiekt = null)
        {

            bool czyPokaz = false;// (bool)TrybPokazu.IsChecked;

            for (long i = 0, kol = 0; i < macierz.Wiersze() && kol < macierz.Kolumny(); i++, kol++)
            {

                while (kol < macierz.Kolumny() && macierz.Tablica()[i, kol].Licznik() == 0)
                {

                    bool czySameZera = true;

                    long j;
                    for (j = i; j < macierz.Wiersze(); j++)
                        if (macierz.Tablica()[j, kol].Licznik() != 0)
                        {
                            czySameZera = false;
                            break;
                        }

                    if (czySameZera) kol++;

                    if (j != i && i < macierz.Wiersze() && j < macierz.Wiersze())
                    {

                        if (czyPokaz)
                            if (Interaction.MsgBox("Zamień wiersze " + i.ToString() + " i " + j.ToString() + ".\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                                if (obiekt != null)
                                    return null;
                                else
                                    throw new ConstraintException(bladPodalgorytmu);

                        macierz = Macierz.MacierzZamianyWierszy(macierz, i, j) * macierz;
                        //OdswiezMacierz();

                    }

                }

                if (kol < macierz.Kolumny())
                {

                    if (czyPokaz && !new Ulamek(macierz.Tablica()[i, kol].Mianownik(), macierz.Tablica()[i, kol].Licznik()).Equals(new Ulamek(1, 1)))
                        if (Interaction.MsgBox("Pomnóż wiersz " + i.ToString() + " przez (" + new Ulamek(macierz.Tablica()[i, kol].Mianownik(), macierz.Tablica()[i, kol].Licznik()).ToString() + ").\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                            if (obiekt != null)
                                return null;
                            else
                                throw new ConstraintException(bladPodalgorytmu);

                    macierz = Macierz.MacierzMnozeniaWiersza(macierz, i, new Ulamek(macierz.Tablica()[i, kol].Mianownik(), macierz.Tablica()[i, kol].Licznik())) * macierz;
                    //OdswiezMacierz();

                    Ulamek przez_co_mnozyc = new Ulamek(macierz.Tablica()[i, kol].Mianownik(), macierz.Tablica()[i, kol].Licznik());

                    for (long j = i + 1; j < macierz.Wiersze(); j++)
                    {

                        Ulamek co_mnozyc = new Ulamek(macierz.Tablica()[j, kol].Licznik(), macierz.Tablica()[j, kol].Mianownik());
                        Ulamek mnoznik = co_mnozyc * przez_co_mnozyc * new Ulamek(-1, 1);

                        if (czyPokaz && !mnoznik.Equals(new Ulamek(0, 1)))
                            if (Interaction.MsgBox("Dodaj do wiersza " + j.ToString() + " wiersz " + i.ToString() + " pomnożony przez (" + mnoznik.ToString() + ").\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                                if (obiekt != null)
                                    return null;
                                else
                                    throw new ConstraintException(bladPodalgorytmu);

                        macierz = Macierz.MacierzDodawaniaWierszy(macierz, j, i, mnoznik) * macierz;
                        //OdswiezMacierz();

                    }

                }

            }

            for (long i = macierz.Wiersze() - 1; i > 0; i--)
            {

                long kolumna_bez_zera = 0;

                while (kolumna_bez_zera < macierz.Kolumny() && macierz.Tablica()[i, kolumna_bez_zera].Licznik() == 0) kolumna_bez_zera++;

                if (kolumna_bez_zera == macierz.Kolumny()) continue;

                for (long x = i - 1; x >= 0; x--)
                {

                    Ulamek skalar = (new Ulamek(macierz.Tablica()[x, kolumna_bez_zera].Licznik(), macierz.Tablica()[x, kolumna_bez_zera].Mianownik()) / new Ulamek(macierz.Tablica()[i, kolumna_bez_zera].Licznik(), macierz.Tablica()[i, kolumna_bez_zera].Mianownik())) * new Ulamek(-1, 1);

                    if (czyPokaz && !skalar.Equals(new Ulamek(0, 1)))
                        if (Interaction.MsgBox("Dodaj do wiersza " + x.ToString() + " wiersz " + i.ToString() + " pomnożony przez (" + skalar.ToString() + ").\nCzy chcesz kontynuować algorytm?", MsgBoxStyle.YesNo, "Eliminacja Gaussa") == MsgBoxResult.No)
                            if (obiekt != null)
                                return null;
                            else
                                throw new ConstraintException(bladPodalgorytmu);

                    macierz = Macierz.MacierzDodawaniaWierszy(macierz, x, i, skalar) * macierz;
                    //OdswiezMacierz();

                }

            }

            //OdswiezMacierz();
            if (czyPokaz) Interaction.MsgBox("Macierz jest w postaci całkowicie zredukowanej.", MsgBoxStyle.OkOnly, "Eliminacja Gaussa zakończona");

            return macierz;//

        }

    }

}
