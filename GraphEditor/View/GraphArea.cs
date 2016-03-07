﻿using System;     
using NGraph.Models;         
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
        private bool selection;
        private readonly List<IElement> selectedElements = new List<IElement>();
        public int GetSelectedElements => selectedElements.Count;      
        private Point startPointClick;
        private Rectangle selectionRectangle;
        private bool created;

        public GraphArea()
        {
            Background = Brushes.White;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            Focusable = true; 
        }
                 
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {                   
                Point mousePosition = e.GetPosition(null);

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
                // мультивыделение
                if (selectionRectangle != null)
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

        private VertexControl CreateVertexControl(Point p)
        {
            var v = new Vertex();  
            var vc = new VertexControl(v);             
            vc.SetPosition(p);
            this.Children.Add(vc);
            return vc;
        }


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
                if(!selectedElements.Contains(element))
                    selectedElements.Add(element);
            }
            return HitTestResultBehavior.Continue;
        }     
        #endregion
    }
}