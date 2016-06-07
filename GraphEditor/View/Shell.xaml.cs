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
        }
                
        private void UpdateTitle()
        {
            var version = Assembly.GetExecutingAssembly().GetName();
            Title = $"{version.Name} - v.{version.Version.Major}.{version.Version.Minor}";
        }
    }
}