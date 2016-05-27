using System.Windows;

namespace GraphEditor.View
{
    public partial class RenameDialog : Window
    {
        public RenameDialog(string name = "")
        {
            InitializeComponent();
            rename.Text = name;
            rename.Focus();
            rename.SelectAll();
        }

        public string Rename => rename.Text;

        private void Rename_OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}