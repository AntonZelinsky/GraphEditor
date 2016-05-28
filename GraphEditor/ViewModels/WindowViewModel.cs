using System.Windows;
using System.Windows.Input;
using GraphEditor.Helper;
using GraphEditor.Models;

namespace GraphEditor.ViewModels
{
    public class WindowViewModel
    {
        private readonly GraphViewModel _graphViewModel;
        public readonly CommandBindingCollection CommandBindings;

        public WindowViewModel(GraphViewModel graphViewModel)
        {
            _graphViewModel = graphViewModel;

            CommandBindings = new CommandBindingCollection();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommand));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, LoadCommand));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveCommand, IsChangedCommand));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, ExitCommand));
        }

        #region Commands File menu

        private void NewCommand(object obj, ExecutedRoutedEventArgs e)
        {
            if (_graphViewModel.Changed)
            {
                var result = MessageBox.Show("Save changed?", "Save?", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    SaveCommand(null, null);
                if (result == MessageBoxResult.Cancel)
                    return;
            }
            _graphViewModel.NewFile();
            ((Shell) obj).StopAlgorithm.Command.Execute(null);
        }

        private void LoadCommand(object obj, ExecutedRoutedEventArgs e)
        {
            var model = FileOperation.Load();
            if (model == null)
                return;
            _graphViewModel.LoadFile(model);
            ((Shell) obj).StopAlgorithm.Command.Execute(null);
        }

        private void SaveCommand(object obj, ExecutedRoutedEventArgs e)
        {
            var model = new GraphModelSerialization(_graphViewModel.GetModel());
            FileOperation.Save(model);
            _graphViewModel.SaveFile(model);
        }

        private void IsChangedCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _graphViewModel.Changed;
        }

        private void ExitCommand(object obj, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion Commands File menu       
    }
}