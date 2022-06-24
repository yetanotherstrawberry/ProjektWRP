using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms.Integration;

namespace ProjektWRP
{

    public partial class Wyniki : Window
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

        public Wyniki(string wezel, Dictionary<string, Ulamek> wyniki)
        {

            InitializeComponent();

            Node poczatek = graph.AddNode(wezel);

            foreach (string koniec in wyniki.Keys)
                _ = graph.AddEdge(poczatek.Id, (wyniki[koniec]).ToString(), koniec);

            DataContext = this;

            host.Child = gViewer;
            okienko.Children.Add(host);

            gViewer.Graph = null;
            gViewer.Graph = graph;

        }


    }

}
