using System;     
using NGraph.Models;         
using System.Windows;   
using NGraph.Interfaces;
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
        private bool selection;
        private readonly List<IElement> selectedElements = new List<IElement>();
        public int GetSelectedElements => selectedElements.Count;      
        private Point startPointClick;
        private Rectangle selectionRectangle;
        private bool created;
        private EdgeControl createEdge;
        private IElement targetElement;

        public GraphArea()
        {
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
                    var vc = CreateVertexControl(startPointClick);
                    
                    created = true;
                }
                // Мультивыделение   
                else if (e.ClickCount == 1)
                {                        
                    if (!Keyboard.IsKeyDown(Key.RightCtrl) && !Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        selectedElements.ForEach(s => s.IsSelected = false);
                        selectedElements.Clear();
                    }
                    selection = true;
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
                    Canvas.SetLeft(selectionRectangle, e.GetPosition(this).X);
                    Canvas.SetTop(selectionRectangle, e.GetPosition(this).Y);
                }
            }
            // Рисование дуги
            else if (e.MouseDevice.RightButton == MouseButtonState.Pressed)
            {
                CreateEdgeControl((VertexControl)element);
            }

            // Гарантированое получение MouseLeftButtonUp даже если вышли за пределы области
            CaptureMouse();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(null);

            // Создание дуги
            if (createEdge != null && 
                e.RightButton == MouseButtonState.Pressed &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                CreatingEdgeControl(mousePosition);
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                // мультивыделение              
                if(selection)
                {                   
                    // защита от случайных движений мыши              
                   if(Math.Max(Math.Abs(startPointClick.X - mousePosition.X), Math.Abs(startPointClick.Y - mousePosition.Y)) < 5)
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
                // перетаскивание
                else
                {
                    var vectorPosition = startPointClick - mousePosition;     
                    startPointClick = mousePosition;

                    if (selectedElements.Count > 0)
                    {
                        foreach (var element in selectedElements)
                        {
                            var selectedElement = (IElement)element;    
                            if(selectedElement is EdgeControl)
                                 continue;
                            var currentPosition = selectedElement.GetPosition();
                            selectedElement.SetPosition(currentPosition.X - vectorPosition.X, currentPosition.Y - vectorPosition.Y);
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
                // Рисование дуги
                if (e.MouseDevice.RightButton == MouseButtonState.Pressed && createEdge != null)
                {
                    var v = GetVertexElement(mousePosition);
                    if (v != null && createEdge != v)
                    {
                        ReleasedEdgeControl((VertexControl) v);
                    }
                    else
                    {
                        UnreleasedEdgeControl();
                    }
                    createEdge = null;
                }
                // мультивыделение
                else if (selectionRectangle != null)
                {
                    var geometry = new RectangleGeometry(new Rect(startPointClick, e.GetPosition(this)));
                    GetElements(geometry);
                    this.Children.Remove(selectionRectangle);
                    selectionRectangle = null;
                    selection = false;
                }
                // еденичное выделение 
                else if (element != null && !created)
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

            created = false;
            ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {                        
            if (e.Key == Key.Delete)
            {
                foreach (var selectedElement in selectedElements)
                {
                    this.Children.Remove((UIElement)selectedElement);
                }
                selectedElements.Clear();
            }
            base.OnKeyDown(e);
        }

        #endregion

        #region IElement

        private VertexControl CreateVertexControl(Point p)
        {
            var v = new Vertex();  
            var vc = new VertexControl(v);             
            vc.SetPosition(p);
            GraphArea.SetZIndex(vc, 100);
            this.Children.Add(vc);
            return vc;
        }

        private EdgeControl CreateEdgeControl(VertexControl from, VertexControl to)
        {
            var edge = new Edge((IVertex)from.Vertex, (IVertex)to.Vertex);
            var edgeControl = new EdgeControl(from, to, edge);
            this.Children.Add(edgeControl);
            GraphArea.SetZIndex(edgeControl, 10);
            createEdge = edgeControl;
            return edgeControl;
        }

        private EdgeControl CreateEdgeControl(VertexControl from)
        {
            var edgeControl = new EdgeControl(from);
            this.Children.Add(edgeControl);
            GraphArea.SetZIndex(edgeControl, 10);
            createEdge = edgeControl;
            return edgeControl;
        }

        private EdgeControl CreatingEdgeControl(Point to)
        {
            if (createEdge.To != null)
                throw new Exception();
            createEdge.SetToPoint(to);
            return createEdge;
        }

        private EdgeControl ReleasedEdgeControl(VertexControl to)
        {
            createEdge.SetTo(to);
            var edge = new Edge((IVertex)createEdge.From.Vertex, (IVertex)createEdge.To.Vertex);
            createEdge.Edge = edge;
            return createEdge;
        }

        private void UnreleasedEdgeControl()
        {
            this.Children.Remove(createEdge);
            createEdge = null;
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

        public IElement GetVertexElement(Point p)
        {
            targetElement = null;
            HitTestResultCallback callback = new HitTestResultCallback(HitTestClickCallback);
            VisualTreeHelper.HitTest(this, null, callback, new PointHitTestParameters(p));
            return targetElement;
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
            GeometryHitTestResult geometryResult = (GeometryHitTestResult)result; //проверить почему as файлился  
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
