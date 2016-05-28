using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphEditor.View;
using GraphEditor.ViewModels;

namespace GraphEditor
{
    public partial class Shell : Window
    {
        private readonly GraphViewModel _model;

        public Shell(GraphViewModel model)
        {
            InitializeComponent();
            AddHandler(MDITabItem.CloseTabEvent, new RoutedEventHandler(CloseTab));
            Loaded += (x, y) => Keyboard.Focus(graphArea);
            _model = model;
            _model.ModelChanged += UpdateTitle;
            UpdateTitle();
            graphArea.MouseMove += GraphAreaOnMouseMove;
        }

        private void CloseTab(object source, RoutedEventArgs args)
        {
            var tabItem = args.Source as TabItem;
            if (tabItem != null)
            {
                var tabControl = tabItem.Parent as TabControl;
                if (tabControl != null)
                    tabControl.Items.Remove(tabItem);
            }
        }

        private void GraphAreaOnMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(graphArea);
            Coordinates.Text = $"X: {p.X}, Y: {p.Y}";
            Counter.Text =
                $"Elements: {graphArea.Children.Count}, Selected Elements: {((GraphViewModel) graphArea.DataContext).SelectedElementsCount}";
        }

        private void UpdateTitle()
        {
            var version = Assembly.GetExecutingAssembly().GetName();
            var ch = _model.Changed ? "*" : "";
            Title = $"{version.Name} - v.{version.Version.Major}.{version.Version.Minor}  {_model.FileName}{ch}";
        }
    }
}