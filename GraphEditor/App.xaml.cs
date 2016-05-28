using System.Collections.Generic;
using System.Windows;
using GraphEditor.Models;
using GraphEditor.ViewModels;

namespace GraphEditor
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var model = new GraphModel();
            var graphViewModel = new GraphViewModel(model);
            var windowViewModel = new WindowViewModel(graphViewModel);
            var algorithmViewModel = new AlgorithmViewModel(graphViewModel);
            var view = new Shell(graphViewModel);
            view.CommandBindings.AddRange(windowViewModel.CommandBindings);
            view.CommandBindings.AddRange(algorithmViewModel.CommandBindings);
            view.graphArea.DataContext = graphViewModel;
            view.graphArea.SubscribeEvents();

            var v1 = new Vertex(100, 100);
            var v2 = new Vertex(200, 200);
            var v3 = new Vertex(100, 200);
            var v = new List<Vertex> {v1, v2, v3};
            var es = new List<Edge> {new Edge(v1.Id, v2.Id), new Edge(v1.Id, v3.Id), new Edge(v2.Id, v3.Id)};

            graphViewModel.CreateGraph(v, es);
            Current.MainWindow = view;
            view.Show();
        }
    }
}