using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace GraphEditor
{
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();
            UpdateTitle();
            //Loaded += (x, y) => Keyboard.Focus(graphArea);      
            //_model.ModelChanged += UpdateTitle;
            //graphArea.MouseMove += GraphAreaOnMouseMove;
        }

        private void GraphAreaOnMouseMove(object sender, MouseEventArgs e)
        {
            //var p = e.GetPosition(graphArea);
            //Coordinates.Text = $"X: {p.X}, Y: {p.Y}";
            //Counter.Text =
            //    $"Elements: {graphArea.Children.Count}, Selected Elements: {( (GraphViewModel)graphArea.DataContext ).SelectedElementsCount}";
        }

        private void UpdateTitle()
        {
            var version = Assembly.GetExecutingAssembly().GetName();
            Title = $"{version.Name} - v.{version.Version.Major}.{version.Version.Minor}";
        }
    }
}