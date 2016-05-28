using System.Windows;
using GraphEditor.ViewModels;

namespace GraphEditor
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var windowViewModel = new WindowViewModel();

            var view = new Shell
            {
                DataContext = windowViewModel
            };
            view.CommandBindings.AddRange(windowViewModel.CommandBindings);

            Current.MainWindow = view;
            view.Show();
        }
    }
}