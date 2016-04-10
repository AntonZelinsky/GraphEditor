using System.Windows;
using GraphEditor.Models;
using GraphEditor.ViewModels;   
﻿using System.Collections.Generic;

namespace GraphEditor
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);  
            var view = new MainWindow();
            var model = new GraphModel();
            var viewModel = new GraphViewModel(model);   
            view.CommandBindings.AddRange(viewModel.CommandBindings);
            view.graphArea.DataContext = viewModel;
            view.graphArea.SubscribeEvents();

            var v1 = new Vertex(100, 100);
            var v2 = new Vertex(200, 200);
            var v3 = new Vertex(100, 200);
            var v = new List<Vertex> {v1, v2, v3};
            var es = new List<Edge> { new Edge(v1.Id, v2.Id), new Edge(v1.Id, v3.Id), new Edge(v2.Id, v3.Id) };

            viewModel.CreateGraph(v, es);
            Application.Current.MainWindow = view;
            view.Show();
        }
    }
}
