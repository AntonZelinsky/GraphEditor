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

        private void Rename_OkClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Rename => rename.Text;
    }
}
