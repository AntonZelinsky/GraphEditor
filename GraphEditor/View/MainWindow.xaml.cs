using System.Reflection;
using System.Windows;
using System.Windows.Input;
using GraphEditor.ViewModels;

namespace GraphEditor
{
    public partial class MainWindow : Window
    {
        private readonly GraphViewModel _model;

        public MainWindow(GraphViewModel model)
        {
            InitializeComponent();
            Loaded += (x, y) => Keyboard.Focus(graphArea);
            _model = model;
            _model.ModelChanged += UpdateTitle;
            UpdateTitle();
            graphArea.MouseMove += GraphAreaOnMouseMove;
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