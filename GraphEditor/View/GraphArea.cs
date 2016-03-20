using System;             
using System.Windows;   
using GraphEditor.Models; 
using GraphEditor.Controls; 
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;  
using System.Collections.Generic;
using GraphEditor.Controls.Interfaces;

namespace GraphEditor.View
{
    public class GraphArea : Canvas
    {
        private bool multiSelection;
        private readonly List<IElement> selectedElements = new List<IElement>();
        public int GetSelectedElements => selectedElements.Count;      
        private Point startPointClick;
        private Rectangle selectionRectangle;
        private bool creating;
        private IElement targetElement;

        private Graph graph;

        public GraphArea()
        {
            graph = new Graph(this);
            Background = Brushes.White;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            Focusable = true;               
        }

        #region Override Events  
               
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPointClick = e.GetPosition(this);
            var element = GetElement(startPointClick);
            if (element == null)
            {
                if (e.ClickCount == 2)
                {
                    graph.CreateVertexControl(startPointClick);
                    creating = true;
                }
                // Мультивыделение   
                else if (e.ClickCount == 1)
                {
                    if (!Keyboard.IsKeyDown(Key.RightCtrl) && !Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        selectedElements.ForEach(s => s.IsSelected = false);
                        selectedElements.Clear();
                    }
                    CreateRectangleSelection(startPointClick);
                }
            }

            // Гарантированое получение MouseLeftButtonUp даже если вышли за пределы области
            CaptureMouse();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            startPointClick = e.GetPosition(this);
            var element = GetElement(startPointClick);

            // Draw edge
            if ( element is VertexControl)
            {
                targetElement = graph.CreateEdgeControl((VertexControl)element);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(null);

            // Derawing edge
            if (e.RightButton == MouseButtonState.Pressed && targetElement != null)
            {
                graph.CreatingEdgeControl(mousePosition);
                creating = true;
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                // мультивыделение              
                if (multiSelection)
                {
                    MoveRectangleSelection(mousePosition);
                }  
                // перетаскивание
                else
                {
                    var vectorPosition = startPointClick - mousePosition;     
                    startPointClick = mousePosition;

                    if (selectedElements.Count > 0)
                    {
                        foreach (IElement element in selectedElements)
                        {
                            // отсеиваем не вершины
                            if (!(element is IVertexElement))
                                continue;
                            var currentPosition = ((IVertexElement)element).GetPosition();
                            ((IVertexElement)element).SetPosition(currentPosition.X - vectorPosition.X, currentPosition.Y - vectorPosition.Y);
                        }                                   
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            var element = GetElement(mousePosition);
            if (e.ClickCount == 1)
            {     
                // мультивыделение
                if (selectionRectangle != null)
                {
                    CompleteRectangleSelection(mousePosition);
                }
                // еденичное выделение 
                else if (element != null && !creating)
                {   
                    if (!element.IsSelected)
                    {
                        // Выделение элемента, при нажатой CTRL добавить к уже выделеным
                        if (!Keyboard.IsKeyDown(Key.RightCtrl) && !Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            selectedElements.ForEach(s => s.IsSelected = false);
                            selectedElements.Clear();
                        }

                        element.IsSelected = true;
                        selectedElements.Add(element);
                    }
                    else
                    {
                        element.IsSelected = false;
                        selectedElements.Remove(element);
                    }              
                } 
            }

            creating = false;
            ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            Point mousePosition = e.GetPosition(this);

            var element = GetElement(mousePosition);  

            // Complete drawed edge
            if (element != null)
            {
                if (creating)
                {
                    graph.ReleasedEdgeControl((VertexControl) element);
                    creating = false;
                }
                else
                {                      
                    ContextMenu cm = new ContextMenu();
                    var rename = new MenuItem {Header = "Rename", };
                    rename.Click += (sender, args) => RenameOnClick(element, args);
                    cm.Items.Add(rename);                
                    cm.IsOpen = true;
                }
                targetElement = null;
            }
        }

        private void RenameOnClick(IElement sender, RoutedEventArgs routedEventArgs)
        {                                                      
            string old = String.Empty;
            if (sender.IsLabel)
                old = sender.LabelName;
            var rene = new RenameDialog(old);
            if (rene.ShowDialog() == true)
            {
                if (rene.Rename != "")
                {
                    if (sender.IsLabel)
                        graph.UpdeteElementLabel(sender, rene.Rename);
                    else
                        graph.CreateElementLabel(sender, rene.Rename);
                }     
                else
                    graph.RemoveElementLabel(sender);
            }                                                
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {                        
            if (e.Key == Key.Delete)
            {                       
                graph.RemoveElements(selectedElements);
                selectedElements.Clear();     
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.A)
                {
                    selectedElements.AddRange(graph.AllElements); 
                    graph.AllElements.ForEach(s => s.IsSelected = true); 
                }
            }
            base.OnKeyDown(e);
        }

        #endregion

        #region RectangleSelection

        private void CreateRectangleSelection(Point mousePosition)
        {
            multiSelection = true;
            selectionRectangle = new Rectangle
            {
                Width = 0,
                Height = 0,
                Fill = Brushes.LightCyan,
                Stroke = Brushes.Orange,
                StrokeThickness = 2,
                Opacity = 0.3
            };

            this.Children.Add(selectionRectangle);
            Canvas.SetZIndex(selectionRectangle, 200);
            Canvas.SetLeft(selectionRectangle, mousePosition.X);
            Canvas.SetTop(selectionRectangle, mousePosition.Y);
        }

        private void MoveRectangleSelection(Point mousePosition)
        {
            // защита от случайных движений мыши              
            if (Math.Max(Math.Abs(startPointClick.X - mousePosition.X), Math.Abs(startPointClick.Y - mousePosition.Y)) < 5)
                return;
            var x = Math.Min(mousePosition.X, startPointClick.X);
            var y = Math.Min(mousePosition.Y, startPointClick.Y);

            var w = Math.Max(mousePosition.X, startPointClick.X) - x;
            var h = Math.Max(mousePosition.Y, startPointClick.Y) - y;

            selectionRectangle.Width = w;
            selectionRectangle.Height = h;

            Canvas.SetLeft(selectionRectangle, x);
            Canvas.SetTop(selectionRectangle, y);
        }

        private void CompleteRectangleSelection(Point mousePosition)
        {
            var geometry = new RectangleGeometry(new Rect(startPointClick, mousePosition));
            GetElements(geometry);
            this.Children.Remove(selectionRectangle);
            selectionRectangle = null;
            multiSelection = false;
        }

        #endregion
       
        #region Attached Dependency Property registrations

        public static readonly DependencyProperty XProperty =
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

        public static readonly DependencyProperty YProperty =
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

        public IElement GetElement(Point p)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, p);
            return hitResult.VisualHit as IElement;
        }

        public IVertexElement GetVertexElement(Point p)
        {
            targetElement = null;
            HitTestResultCallback callback = new HitTestResultCallback(HitTestClickCallback);
            VisualTreeHelper.HitTest(this, null, callback, new PointHitTestParameters(p));
            return targetElement as IVertexElement;
        }

        public List<IElement> GetElements(Geometry region)
        {                  
            VisualTreeHelper.HitTest(this, null, HitTestRectangleCallback, new GeometryHitTestParameters(region));

            return selectedElements;
        }

        private HitTestResultBehavior HitTestClickCallback(HitTestResult result)
        {
            var element = result.VisualHit as VertexControl;
            if (element != null)
                targetElement = element;
            return HitTestResultBehavior.Stop;
        }

        private HitTestResultBehavior HitTestRectangleCallback(HitTestResult result)
        {
            var geometryResult = (GeometryHitTestResult) result;
            var element = result.VisualHit as IElement;
            if (element != null &&
                geometryResult.IntersectionDetail == IntersectionDetail.FullyInside)
            {
                element.IsSelected = true;
                if (!selectedElements.Contains(element))
                    selectedElements.Add(element);
            }               
            return HitTestResultBehavior.Continue;
        }

        #endregion
    }
}
