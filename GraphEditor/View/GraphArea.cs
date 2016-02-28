using System;              
using System.Windows;   
using System.Diagnostics; 
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
        private List<IElement> selectedElements = new List<IElement>();          
        private Point startPointClick;
        private Rectangle selectionRectangle;

        public GraphArea()
        {
            Background = Brushes.AliceBlue;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            Focusable = true;       
        }
                 
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPointClick = e.GetPosition(this);
            if (e.ClickCount == 2)
            {
                var vc = CreateVertexControl(startPointClick);
                GraphArea.SetLeft(vc, startPointClick.X);
                GraphArea.SetTop(vc, startPointClick.Y);
                this.Children.Add(vc);
            }
            else if(e.ClickCount == 1)
            {
                var element = GetElement(startPointClick);
                if (element != null)
                {
                    if (!element.IsSelected)
                    {
                        // Выделение элемента, при нажатой CTRL добавить к уже выделеным
                        if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
                        {
                            element.IsSelected = true;
                            selectedElements.Add(element);
                        }
                        else
                        {      
                            selectedElements.RemoveAll(s => s.IsSelected = false);
                            element.IsSelected = true;
                            selectedElements.Add(element);
                        }                      
                    }
                    else
                    {
                        //selectedElements.RemoveAll(s => s.IsSelected = false);
                        element.IsSelected = false;
                        selectedElements.Remove(element);
                    }      
                }
                else
                {
                    selectedElements.RemoveAll(s => s.IsSelected = false);

                    // Мультивыделение                
                    selectionRectangle = new Rectangle
                        {
                            Width = 0, Height = 0,
                            Fill = Brushes.LightCyan,
                            Stroke = Brushes.Orange,
                            StrokeThickness = 2,
                            Opacity = 0.3
                        };

                    this.Children.Add(selectionRectangle);
                    Canvas.SetLeft(selectionRectangle, e.GetPosition(this).X);
                    Canvas.SetTop(selectionRectangle, e.GetPosition(this).Y);
                    // Гарантированое получение MouseLeftButtonUp даже если вышли за пределы области
                    CaptureMouse();
                }
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(null);
            if (e.LeftButton == MouseButtonState.Pressed && selectionRectangle != null)
            {
                var pos = e.GetPosition(this);

                var x = Math.Min(pos.X, startPointClick.X);
                var y = Math.Min(pos.Y, startPointClick.Y);

                var w = Math.Max(pos.X, startPointClick.X) - x;
                var h = Math.Max(pos.Y, startPointClick.Y) - y;

                selectionRectangle.Width = w;
                selectionRectangle.Height = h;

                Canvas.SetLeft(selectionRectangle, x);
                Canvas.SetTop(selectionRectangle, y);

                Debug.WriteLine($"SelectionSquare from {startPointClick.ToString()},to {mousePosition.ToString()}");        
            } 

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var geometry = new RectangleGeometry(new Rect(startPointClick, e.GetPosition(this)));
            var elementsInRegion = GetElements(geometry);
            this.Children.Remove(selectionRectangle);
            selectionRectangle = null; 

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

        private VertexElement CreateVertexControl(Point p)
        {                           
            var vc = new VertexElement();
            return vc;
        }  

        #region HitTest
        public IElement GetElement(Point p)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(this, p);
            return hitResult.VisualHit as IElement;
        }

        public List<IElement> GetElements(Geometry region)
        {                  
            // подготовка параметров для операции проверки попадания
            var parametrs = new GeometryHitTestParameters(region);
            HitTestResultCallback callback = new HitTestResultCallback(HitTestCallback);

            VisualTreeHelper.HitTest(this, null, callback, parametrs);

            return selectedElements;
        }

        private HitTestResultBehavior HitTestCallback(HitTestResult result)
        {
            GeometryHitTestResult geometryResult = (GeometryHitTestResult)result; //проверить почему as файлился  
            var element = result.VisualHit as IElement;
            if (element != null &&
                geometryResult.IntersectionDetail == IntersectionDetail.FullyInside)
            {
                element.IsSelected = true;
                selectedElements.Add(element);
            }
            return HitTestResultBehavior.Continue;
        }     
        #endregion  
    }
}
