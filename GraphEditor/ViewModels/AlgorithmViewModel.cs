using System.Collections.Generic;
using System.Windows.Input;
using GraphEditor.Algorithms;
using GraphEditor.Algorithms.Search;

namespace GraphEditor.ViewModels
{
    public class AlgorithmViewModel
    {
        private readonly GraphViewModel _graphViewModel;
        public readonly CommandBindingCollection CommandBindings;
        private IAlgorithm _algorithm;
        private LinkedListNode<HistoryItemAlgorithm> _historyItem;

        public AlgorithmViewModel(GraphViewModel graphViewModel)
        {
            _graphViewModel = graphViewModel;

            CommandBindings = new CommandBindingCollection
            {
                new CommandBinding(StartCommand, Start, CanStart),
                new CommandBinding(StopCommand, Stop, CanStop),
                new CommandBinding(ClearCommand, Clear, CanClear),
                new CommandBinding(FromCommand, From, CanFrom),
                new CommandBinding(ToCommand, To, CanTo),
                new CommandBinding(StepForwardCommand, StepForward, CanStepForward),
                new CommandBinding(SkipForwardCommand, SkipForvard, CanSkipForward),
                new CommandBinding(StepBackCommand, StepBack, CanStepBack),
                new CommandBinding(SkipBackCommand, SkipBack, CanSkipBack),
                new CommandBinding(DFSCommandDirected, DFSAlgorithmDirected),
                new CommandBinding(DFSCommandUndirected, DFSAlgorithmUndirected),
                new CommandBinding(BFSCommand, BFSAlgorithm)
            };
        }

        #region Commands

        #region Manage Command

        public static readonly ICommand StartCommand = new RoutedUICommand();

        private void Start(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Clear(null, null);
            _algorithm.Compute();

            _historyItem = _algorithm.History.First;
            _graphViewModel.ChangeColor(_historyItem.Value.Id, _historyItem.Value.Color);
        }

        private void CanStart(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            if (_algorithm != null && _algorithm.SourceId != null & _algorithm.TargetId != null &&
                _algorithm.TargetId != _algorithm.SourceId)
                canExecuteRoutedEventArgs.CanExecute = true;
        }

        public static readonly ICommand StopCommand = new RoutedUICommand();

        private void Stop(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Clear(null, null);
            _algorithm = null;
        }

        private void CanStop(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            if (_algorithm != null)
                canExecuteRoutedEventArgs.CanExecute = true;
        }

        public static readonly ICommand ClearCommand = new RoutedCommand();

        private void Clear(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            _graphViewModel.ResetColor();
            _algorithm.Clear(sender != null);
        }

        private void CanClear(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            if (_algorithm != null)
                canExecuteRoutedEventArgs.CanExecute = true;
        }

        #region Select from to

        public static readonly ICommand FromCommand = new RoutedUICommand();

        private void From(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            _graphViewModel.SelectedElement += OnSelectedFromElement;
        }

        private void CanFrom(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            if (_algorithm != null)
                canExecuteRoutedEventArgs.CanExecute = true;
        }

        private void OnSelectedFromElement(int id)
        {
            Clear(null, null);
            _algorithm.SourceId = id;
            _graphViewModel.SelectedElement -= OnSelectedFromElement;
        }

        public static readonly ICommand ToCommand = new RoutedUICommand();

        private void To(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            _graphViewModel.SelectedElement += OnSelectedToElement;
        }

        private void CanTo(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            if (_algorithm != null)
                canExecuteRoutedEventArgs.CanExecute = true;
        }

        private void OnSelectedToElement(int id)
        {
            Clear(null, null);
            _algorithm.TargetId = id;
            _graphViewModel.SelectedElement -= OnSelectedToElement;
        }

        #endregion Select from to

        #region Forward

        public static readonly ICommand StepForwardCommand = new RoutedUICommand();

        private void StepForward(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            _historyItem = _historyItem.Next;
            var oldColor = _graphViewModel.ChangeColor(_historyItem.Value.Id, _historyItem.Value.Color);
            var copyHistoryItem = _historyItem.Value;
            copyHistoryItem.OldColor = oldColor;
            _historyItem.Value = copyHistoryItem;
        }

        private void CanStepForward(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            if (_algorithm != null && _algorithm.SourceId != null && _algorithm.TargetId != null)
                if (_historyItem?.Next != null)
                    canExecuteRoutedEventArgs.CanExecute = true;
        }

        public static readonly ICommand SkipForwardCommand = new RoutedCommand();

        private void SkipForvard(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            while (_historyItem?.Next != null)
            {
                StepForward(sender, executedRoutedEventArgs);
            }
        }

        private void CanSkipForward(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            CanStepForward(sender, canExecuteRoutedEventArgs);
        }

        #endregion Forward

        #region Back

        public static readonly ICommand StepBackCommand = new RoutedCommand();

        private void StepBack(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (_historyItem.Value.OldColor != null)
            {
                _graphViewModel.ChangeColor(_historyItem.Value.Id, _historyItem.Value.OldColor.Value);
                _historyItem = _historyItem.Previous;
            }
        }

        private void CanStepBack(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            if (_algorithm != null && _algorithm.SourceId != null && _algorithm.TargetId != null)
                if (_historyItem?.Previous != null)
                    canExecuteRoutedEventArgs.CanExecute = true;
        }

        public static readonly ICommand SkipBackCommand = new RoutedCommand();

        private void SkipBack(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            while (_historyItem?.Previous != null)
            {
                StepBack(sender, executedRoutedEventArgs);
            }
        }

        private void CanSkipBack(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            CanStepBack(sender, canExecuteRoutedEventArgs);
        }

        #endregion Back

        #endregion Manage Command

        #region Search algorithms

        public static readonly ICommand DFSCommandDirected = new RoutedCommand();

        private void DFSAlgorithmDirected(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            _algorithm = new DepthFirstSearchDirectedAlgorithm(_graphViewModel.GetModel());
        }

        public static readonly ICommand DFSCommandUndirected = new RoutedCommand();

        private void DFSAlgorithmUndirected(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            _algorithm = new DepthFirstSearchUndirectedAlgorithm(_graphViewModel.GetModel());
        }

        public static readonly ICommand BFSCommand = new RoutedCommand();

        private void BFSAlgorithm(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            _algorithm = new BreadthFirstSearchAlgorithm(_graphViewModel.GetModel());
        }

        #endregion Search algorithms

        #endregion Commands
    }
}