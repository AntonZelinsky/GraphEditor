using System;
using NGraph.Models;  
using System.Windows; 
using GraphEditor.View;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using GraphEditor.Controls.Interfaces;

namespace GraphEditor.Controls
{
    public class EdgeControl : Control, IEdgeElement, IDisposable
    {
        #region Properties & Fields
          
        public GraphArea RootGraph { get; }

        /// <summary>
        /// Source visual vertex object
        /// </summary>
        public IVertexElement From
        {
            get { return (VertexControl)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From",
            typeof(VertexControl),
            typeof(EdgeControl),
            new PropertyMetadata(null, OnFromChangedInternal));

        private static void OnFromChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EdgeControl;
            if (ctrl != null) ctrl.OnFromChanged(d, e);
        }

        protected void OnFromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            From.PositionChanged += source_PositionChanged;
            InvalidateVisual();
        }

        /// <summary>
        /// Target visual vertex object
        /// </summary>
        public IVertexElement To
        {
            get { return (VertexControl)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To",
            typeof(VertexControl),
            typeof(EdgeControl),
            new PropertyMetadata(null, OnToChangedInternal));

        private static void OnToChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EdgeControl;
            if (ctrl != null) ctrl.OnToChanged(d, e);
        }

        protected void OnToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            To.PositionChanged += source_PositionChanged;
            InvalidateVisual();
        }

        private Point toPoint;
        public void SetToPoint(Point to)
        {
            if (To == null)
            {
                toPoint = to;
                InvalidateVisual();
            }
        }
        
        /// <summary>
        /// Data edge object
        /// </summary>
        public object Edge
        {
            get { return GetValue(EdgeProperty); }
            set { SetValue(EdgeProperty, value); }
        }

        public static readonly DependencyProperty EdgeProperty = DependencyProperty.Register("Edge", typeof(object),
            typeof(EdgeControl),
            new PropertyMetadata(null));

        private Brush BrushColor = Brushes.Black;
        private Brush BrushColorSelected = Brushes.Orange;

        public bool IsSelected { get; set; }

        private bool isMouseOver;

        #endregion

        #region Position

        private void source_PositionChanged(object sender, EventArgs e)
        {
            //update edge on any connected vertex position changes
            InvalidateVisual();
        }

        #endregion

        #region Construction Edge

        public EdgeControl(GraphArea rootGraph, VertexControl from, object edge = null)
        {
            Edge = edge;
            From = from;
            DataContext = edge;
            RootGraph = rootGraph;
            RootGraph.Children.Add(this);
            GraphArea.SetZIndex(this, 10);
        }          

        public EdgeControl(GraphArea rootGraph, VertexControl from, VertexControl to, object edge)
        {                               
            GraphArea.SetZIndex(this, 10);
            RootGraph.Children.Add(this);
            RootGraph = rootGraph;
            DataContext = edge;
            Edge = edge;
            From = from;
            To = to;
        }

        public void SetFrom(VertexControl from)
        {
            From = from;
        }

        public void SetTo(VertexControl to)
        {
            To = to;
            toPoint = new Point(); 
        }

        #endregion

        public void Destruction()
        {
            Dispose();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            isMouseOver = true;
            InvalidateVisual();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            isMouseOver = false;
            InvalidateVisual();
            base.OnMouseLeave(e);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Point from = From.GetPosition();
            Point to;
            to = To?.GetPosition() ?? toPoint;
            if (to.X == 0 && to.Y == 0)
                return;
            drawingContext.DrawLine(new Pen(isMouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3), from, to);
            base.OnRender(drawingContext);
        }

        public void Dispose()
        {
            From.Remove(this);
            To.Remove(this);

            RootGraph.Children.Remove(this);   
        }
    }
}