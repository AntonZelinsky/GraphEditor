using System;             
using System.Linq;                   
using System.Windows;   
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;  
using System.Collections.Generic;
using GraphEditor.Models; 
using GraphEditor.Controls; 
using GraphEditor.ViewModels;             
using GraphEditor.Controls.Interfaces;

namespace GraphEditor.View
{
    public class GraphArea : Canvas
    {
        private bool _multiSelection;
        private readonly List<IUiElement> _multiSelectedElements = new List<IUiElement>();       
        private Point _startPointClick;
        private Rectangle _selectionRectangle;    
        private IUiElement _targetUiElement;
        private bool _changePosition;

        private GraphViewModel _graphViewModel;
        private readonly Dictionary<int, IUiElement> _elementsGraph;
                                      
        public GraphArea()
        {
            _elementsGraph = new Dictionary<int, IUiElement>();  
        }

        public void SubscribeEvents()
        {
            _graphViewModel = DataContext as GraphViewModel;
            if (_graphViewModel != null)
            {
                _graphViewModel.AddedVertex += OnCreateVertex;
                _graphViewModel.AddedEdge += OnCreateEdge;
                _graphViewModel.RemovedElement += OnRemoveElement;
                _graphViewModel.SelectedElement += OnSelectedElement;  
                _graphViewModel.UnselectedElement += OnUnselectedElement;        
                _graphViewModel.UpdateLabel += OnLabelUpdate;
            }
        }

        #region Override Events  

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            _startPointClick = e.GetPosition(this);
            var element = GetElement(_startPointClick);
            if (element == null)
            {
                if (e.ClickCount == 2)
                {
                    _graphViewModel.AddVertex(_startPointClick);
                    return;                 
                }

                if (!Keyboard.IsKeyDown(Key.RightCtrl) && !Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    _graphViewModel.UnselectElements();
                }
            }
            // Select element
            else
            {           if(!_graphViewModel.SelectedElements.Contains(element.Id))
                AddSelectedElement(element, false);
                _targetUiElement = element;  
            }
                                                                                         
            CaptureMouse();
            base.OnMouseLeftButtonDown(e);
        }


        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            _startPointClick = e.GetPosition(this);      
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);   

            // Derawing edge
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if(_createdEdge == null & Math.Abs(( _startPointClick - mousePosition ).X) > 3 &&
                    Math.Abs(( _startPointClick - mousePosition ).Y) > 3)
                {
                    var element = GetElement(_startPointClick);

                    // Draw edge
                    if (element is VertexControl)
                    {
                        CreateEdgeControl((VertexControl)element);
                    }
                }
                if (_createdEdge != null)
                {
                    CreatingEdgeControl(mousePosition);
                }                      
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_targetUiElement == null & !_multiSelection & 
                    Math.Abs((_startPointClick - mousePosition).X) > 3 &
                    Math.Abs((_startPointClick - mousePosition).Y) > 3)
                {
                    CreateRectangleSelection(_startPointClick);  
                    _multiSelection = true;
                }
                // Multiselection              
                if (_multiSelection)
                {
                    MoveRectangleSelection(mousePosition);
                }
                // Drugging
                else
                {
                    if (_graphViewModel.SelectedElementsCount > 0)
                    {
                        var vectorPosition = _startPointClick - mousePosition;
                        if(vectorPosition.Length == 0)
                            return;
                        _startPointClick = mousePosition;
                        _changePosition = true;
                                                                     
                        foreach (int idElement in _graphViewModel.SelectedElements)
                        {
                            var element = _elementsGraph[idElement];        
                            if (!( element is IVertexElement ))
                                continue;

                            var currentPosition = ( (IVertexElement)element ).GetPosition();
                            ( (IVertexElement)element ).SetPosition(currentPosition.X - vectorPosition.X, currentPosition.Y - vectorPosition.Y);
                        }
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            Point mousePosition = e.GetPosition(this);

            if (_multiSelection)
            {
                CompleteRectangleSelection(mousePosition);
                _multiSelectedElements.ForEach(el => AddSelectedElement(el, true));
                _multiSelectedElements.Clear();
                _multiSelection = false;
            }
            else
            {
                if (_graphViewModel.SelectedElementsCount > 0 & _changePosition)   
                {
                    _changePosition = false;
                    _graphViewModel.ChangePosition(mousePosition);     
                }   
            }
            _targetUiElement = null;   
            ReleaseMouseCapture();
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
                                                        
            var element = GetElement(e.GetPosition(this));

            // Complete drawed edge
            if (_createdEdge != null)
            {
                if (element is IVertexElement)
                    CreatedEdgeControl((VertexControl)element);
                else
                    UnreleasedEdgeControl();
            }
            else
            {
                if(element == null)
                    return;
                ContextMenu cm = new ContextMenu();
                var rename = new MenuItem { Header = "Rename", };
                rename.Click += (sender, args) => RenameOnClick(element);
                cm.Items.Add(rename);
                cm.IsOpen = true;
            }

            _createdEdge = null;
            _targetUiElement = null;   
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {                        
            base.OnKeyDown(e);  
                     
            if (e.Key == Key.Delete)
            {                                                
                _graphViewModel.RemoveSelectedElements();   
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.A)
                {
                    _graphViewModel.SelectAll();
                    _elementsGraph.Values.ToList().ForEach(el => el.IsSelected = true);
                }
            }
        }

        #endregion

        #region Events

        #region Selection event

        private void OnSelectedElement(int id)
        {     
            _elementsGraph[id].IsSelected = true;          
        }

        private void OnUnselectedElement(int id)
        {
            _elementsGraph[id].IsSelected = false;
        }

        #endregion Selection event     

        #region Ui events

        private void OnCreateVertex(IElement el)
        {
            var v = el as Vertex;
            var vertex = new VertexControl(this, v.Id, v.Position)
            {
                DataContext = v
            };
            _elementsGraph.Add(v.Id, vertex);           
        }

        private void OnRemoveElement(int id)
        {
            if(!_elementsGraph.ContainsKey(id))
                return;
            _elementsGraph[id].Destruction();
            _elementsGraph.Remove(id);
        }
         
        private void OnLabelUpdate(int id, string name)
        {
            var uiElement = _elementsGraph[id];
            if (!string.IsNullOrEmpty(name))
            {
                if (!uiElement.IsLabel)
                {
                    var label = new LabelElement(this, name);
                    label.Attach(uiElement);
                    label.UpdatePosition();     
                }
                else
                {
                    uiElement.LabelName = name;
                }
            }
            else
                uiElement.DetachLabel();
        }

        private void RenameOnClick(IUiElement sender)
        {
            string oldName = string.Empty;
            if (sender.IsLabel)
                oldName = sender.LabelName;
            var dialog = new RenameDialog(oldName);
            if (dialog.ShowDialog() == true)
            {
                _graphViewModel.UpdeteElementLabel(sender.Id, dialog.Rename);
            }
        }

        #endregion Ui events

        #endregion Events


        #region Add

        private void AddSelectedElement(IUiElement uiElement, bool multi)
        {
            var ctrl = Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl);

            _graphViewModel.AddSelectedElement(uiElement.Id, ctrl, multi);  
        }

        #endregion

        #region Creating edge control

        private EdgeControl _createdEdge;

        private void CreateEdgeControl(VertexControl from)
        {
            var edgeControl = new EdgeControl(this, from);
            _createdEdge = edgeControl;
        }

        private void CreatingEdgeControl(Point to)
        {
            if (_createdEdge.To != null)
                throw new Exception();
            _createdEdge.SetToPoint(to); 
        }

        private void CreatedEdgeControl(VertexControl to)
        {
            if (_createdEdge.From == to)
            {
                UnreleasedEdgeControl();
                return;
            }
            _createdEdge.SetTo(to);
            _createdEdge.From.AddEdge(_createdEdge);
            _createdEdge.To.AddEdge(_createdEdge);
            _graphViewModel.AddEdge(_createdEdge.From.Id, _createdEdge.To.Id);
        }

        private void UnreleasedEdgeControl()
        {
            this.Children.Remove(_createdEdge);  
            _createdEdge = null;
        }

        private void OnCreateEdge(IElement el)
        {
            if (_createdEdge != null) // Created edge on canvas
            {
                _createdEdge.DataContext = el;  
                _elementsGraph.Add(el.Id, _createdEdge);
            }
            else
            {
                var e = el as Edge;
                var edge = new EdgeControl(this, (VertexControl)_elementsGraph[e.FromId],
                    (VertexControl)_elementsGraph[e.ToId])
                {
                    DataContext = e
                };
                _elementsGraph.Add(e.Id, edge);
            }
        }

        #endregion

        #region Rectangle Selection

        private void CreateRectangleSelection(Point mousePosition)
        {
            _selectionRectangle = new Rectangle
            {
                Width = 0,
                Height = 0,
                Fill = Brushes.LightCyan,
                Stroke = Brushes.Orange,
                StrokeThickness = 2,
                Opacity = 0.3
            };

            this.Children.Add(_selectionRectangle);
            Canvas.SetZIndex(_selectionRectangle, 200);
            Canvas.SetLeft(_selectionRectangle, mousePosition.X);
            Canvas.SetTop(_selectionRectangle, mousePosition.Y);
        }

        private void MoveRectangleSelection(Point mousePosition)
        {
            // защита от случайных движений мыши              
            if (Math.Max(Math.Abs(_startPointClick.X - mousePosition.X), Math.Abs(_startPointClick.Y - mousePosition.Y)) < 5)
                return;
            var x = Math.Min(mousePosition.X, _startPointClick.X);
            var y = Math.Min(mousePosition.Y, _startPointClick.Y);

            var w = Math.Max(mousePosition.X, _startPointClick.X) - x;
            var h = Math.Max(mousePosition.Y, _startPointClick.Y) - y;

            _selectionRectangle.Width = w;
            _selectionRectangle.Height = h;

            Canvas.SetLeft(_selectionRectangle, x);
            Canvas.SetTop(_selectionRectangle, y);
        }

        private void CompleteRectangleSelection(Point mousePosition)
        {
            var geometry = new RectangleGeometry(new Rect(_startPointClick, mousePosition));
            GetElements(geometry);
            this.Children.Remove(_selectionRectangle);
            _selectionRectangle = null;
        }

        #endregion Rectangle Selection

        #region Attached dependency property position

        protected static readonly DependencyProperty XProperty =
            DependencyProperty.RegisterAttached("X", typeof(double), typeof(GraphArea),
                                                 new FrameworkPropertyMetadata(double.NaN,
                                                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                    FrameworkPropertyMetadataOptions.AffectsArrange |
                                                    FrameworkPropertyMetadataOptions.AffectsRender |
                                                    FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                                                    FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, x_changed));

        private static void x_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(LeftProperty, e.NewValue);
        }

        public static double GetX(DependencyObject obj)
        {
            return (double)obj.GetValue(XProperty);
        }

        public static void SetX(DependencyObject obj, double value, bool alsoSetFinal = true)
        {
            obj.SetValue(XProperty, value);
        }

        private static readonly DependencyProperty YProperty =
           DependencyProperty.RegisterAttached("Y", typeof(double), typeof(GraphArea),
                                                 new FrameworkPropertyMetadata(double.NaN,
                                                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                    FrameworkPropertyMetadataOptions.AffectsArrange |
                                                    FrameworkPropertyMetadataOptions.AffectsRender |
                                                    FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                                                    FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                                                    , y_changed));

        private static void y_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(TopProperty, e.NewValue);
        }
        public static double GetY(DependencyObject obj)
        {
            return (double)obj.GetValue(YProperty);
        }

        public static void SetY(DependencyObject obj, double value)
        {
            obj.SetValue(YProperty, value);
        }

        #endregion

        #region HitTest

        private IUiElement GetElement(Point p)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, p);
            return hitResult?.VisualHit as IUiElement;    
        }

        private IVertexElement GetVertexElement(Point p)
        {
            _targetUiElement = null;
            HitTestResultCallback callback = HitTestClickCallback;
            VisualTreeHelper.HitTest(this, null, callback, new PointHitTestParameters(p));
            return _targetUiElement as IVertexElement;
        }

        private HitTestResultBehavior HitTestClickCallback(HitTestResult result)
        {
            var element = result.VisualHit as VertexControl;
            if (element != null)
                _targetUiElement = element;
            return HitTestResultBehavior.Stop;
        }

        private List<IUiElement> GetElements(Geometry region)
        {
            VisualTreeHelper.HitTest(this, null, HitTestRectangleCallback, new GeometryHitTestParameters(region));

            return _multiSelectedElements;
        }

        private HitTestResultBehavior HitTestRectangleCallback(HitTestResult result)
        {
            var geometryResult = (GeometryHitTestResult)result;
            var element = result.VisualHit as IUiElement;
            if (element != null &&
                geometryResult.IntersectionDetail == IntersectionDetail.FullyInside)
            {
                element.IsSelected = true;
                if (!_multiSelectedElements.Contains(element))
                    _multiSelectedElements.Add(element);
            }
            return HitTestResultBehavior.Continue;
        }

        #endregion 
    }
}
