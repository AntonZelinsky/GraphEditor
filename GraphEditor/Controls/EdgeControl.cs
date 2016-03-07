using System;
using NGraph.Models;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using GraphEditor.Controls.Interfaces;

namespace GraphEditor.Controls
{
    public class EdgeControl : Control, IElement
    {
        #region Properties & Fields

        /// <summary>
        /// Source visual vertex object
        /// </summary>
        public VertexControl From
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
            From.SizeChanged += Source_SizeChanged;
        }

        /// <summary>
        /// Target visual vertex object
        /// </summary>
        public VertexControl To
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
            To.SizeChanged += Source_SizeChanged;
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

        private bool mouseOver;

        #endregion

        #region Position

        private void source_PositionChanged(object sender, EventArgs e)
        {
            //update edge on any connected vertex position changes
            InvalidateVisual();
        }


        void Source_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }

        #endregion

        #region Construction Edge

        public EdgeControl(VertexControl from)
        {
            From = from;
        }

        public EdgeControl(VertexControl from, object edge)
        {
            DataContext = edge;
            Edge = edge;
            From = from;
        }

        public EdgeControl(VertexControl from, VertexControl to, object edge)
        {
            DataContext = edge;
            Edge = edge;
            From = from;
            To = to;
        }

        public void SetFrom(VertexControl from)
        {
            From = from;
            InvalidateVisual();
        }

        public void SetTo(VertexControl to)
        {
            To = to;
            toPoint = new Point();
            InvalidateVisual();
        }
        
        #endregion

        public Point GetPosition()
        {
            throw new System.NotImplementedException();
        }

        public void SetPosition(Point pt)
        {
            throw new System.NotImplementedException();
        }

        public void SetPosition(double x, double y)
        {
            throw new System.NotImplementedException();
        }
        
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            mouseOver = true;
            InvalidateVisual();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            mouseOver = false;
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
            drawingContext.DrawLine(new Pen(mouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3), from, to);
            base.OnRender(drawingContext);
        }
    }
}