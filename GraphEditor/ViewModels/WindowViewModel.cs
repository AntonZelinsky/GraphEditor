using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Helper;
using GraphEditor.Models;
using GraphEditor.View;

namespace GraphEditor.ViewModels
{
    public class WindowViewModel : PropertyChangedBase
    {
        private static int _indexNewTab;
        public readonly CommandBindingCollection CommandBindings;
        private MDITabItem _selectedTab;

        public WindowViewModel()
        {
            CommandBindings = new CommandBindingCollection();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommand));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, LoadCommand));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveCommand, IsChangedCommand));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, ExitCommand));

            //var algorithmViewModel = new AlgorithmViewModel(graphViewModel);
            Tabs = new ObservableCollection<MDITabItem>();
            NewTab();
            NewTab();
            GenerateExample(Tabs[0]);
        }

        public MDITabItem SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<MDITabItem> Tabs { get; }

        private MDITabItem NewTab()
        {
            var model = new GraphModel();
            var graphViewModel = new GraphViewModel(model);

            var area = new GraphArea
            {
                DataContext = graphViewModel,
                Background = Brushes.WhiteSmoke,
                Name = "graphArea",
                Focusable = true
            };
            area.SubscribeEvents();
            var tab = new MDITabItem
            {
                Header = $"Tab {_indexNewTab}",
                Content = area,
                GraphViewModel = graphViewModel
            };
            _indexNewTab++;
            tab.CloseTab += TabOnCloseTab;
            Tabs.Add(tab);
            return tab;
        }

        private void GenerateExample(MDITabItem item)
        {
            var v1 = new Vertex(100, 100);
            var v2 = new Vertex(200, 200);
            var v3 = new Vertex(100, 200);
            var v = new List<Vertex> {v1, v2, v3};
            var es = new List<Edge> {new Edge(v1.Id, v2.Id), new Edge(v1.Id, v3.Id), new Edge(v2.Id, v3.Id)};

            item.GraphViewModel.CreateGraph(v, es);
        }

        private void TabOnCloseTab(object sender, RoutedEventArgs routedEventArgs)
        {
            Tabs.Remove(sender as MDITabItem);
        }

        #region Commands File menu

        private void NewCommand(object obj, ExecutedRoutedEventArgs e)
        {
            var tab = NewTab();
            SelectedTab = tab;
        }

        private void LoadCommand(object obj, ExecutedRoutedEventArgs e)
        {
            var model = FileOperation.Load();
            if (model == null)
                return;
            var tab = NewTab();
            SelectedTab = tab;
            SelectedTab.GraphViewModel.LoadFile(model);
        }

        private void SaveCommand(object obj, ExecutedRoutedEventArgs e)
        {
            var model = new GraphModelSerialization(SelectedTab.GraphViewModel.GetModel());
            FileOperation.Save(model);
            SelectedTab.GraphViewModel.SaveFile(model);
        }

        private void IsChangedCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectedTab?.GraphViewModel != null)
                e.CanExecute = SelectedTab.GraphViewModel.Changed;
        }

        private void ExitCommand(object obj, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion Commands File menu       
    }
}