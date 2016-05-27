using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Controls.Interfaces;
using GraphEditor.Helper;
using GraphEditor.Models;
using GraphEditor.View;

namespace GraphEditor.Controls
{
    public class EdgeControl : Control, IEdgeUiElement
    {
        public EdgeControl(GraphArea rootGraph, VertexControl from)
        {
            From = from;
            RootGraph = rootGraph;
            RootGraph.Children.Add(this);
            Panel.SetZIndex(this, 10);
            Color = Colors.Black;
        }

        public EdgeControl(GraphArea rootGraph, VertexControl from, VertexControl to)
        {
            To = to;
            From = from;
            RootGraph = rootGraph;
            RootGraph.Children.Add(this);
            Panel.SetZIndex(this, 10);
            Color = Colors.Black;
        }

        private Brush ColorAlgorithm => new RadialGradientBrush(Color, Colors.White);
        public Color Color { get; set; }

        public void ChangeColor(Color color)
        {
            Color = color;
            InvalidateVisual();
        }

        public void ResetColor()
        {
            Color = Colors.Black;
            InvalidateVisual();
        }

        public void Destruction()
        {
            From.RemoveEdge(this);
            To.RemoveEdge(this);

            DetachLabel();
            RootGraph.Children.Remove(this);
        }

        public int Id => GetHashCode();

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            InvalidateVisual();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            InvalidateVisual();
            base.OnMouseLeave(e);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var from = From.GetPosition();
            var to = To?.GetPosition() ?? toPoint;
            if (to.X == 0 && to.Y == 0)
                return;
            if (Color != Colors.Black)
                drawingContext.DrawLine(new Pen(ColorAlgorithm, 6), from, to);
            drawingContext.DrawLine(
                new Pen(IsMouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor,
                    Color != Colors.Black ? 2 : 3), from, to);
            if (To != null)
                RenderArrow(drawingContext, from, to);
            base.OnRender(drawingContext);
        }

        private void RenderArrow(DrawingContext drawingContext, Point from, Point to)
        {
            // alghoritm arrow from http://www.codeproject.com/Articles/23116/WPF-Arrow-and-Custom-Shapes
            var radiusVertex = To.Rate;
            var HeadHeight = 18;
            var HeadWidth = 5;
            var theta = Math.Atan2(from.Y - to.Y, from.X - to.X);
            var sint = Math.Sin(theta);
            var cost = Math.Cos(theta);

            var pt3 = new Point(
                to.X + (HeadHeight*cost - HeadWidth*sint),
                to.Y + (HeadHeight*sint + HeadWidth*cost));

            var pt4 = new Point(
                to.X + (HeadHeight*cost + HeadWidth*sint),
                to.Y - (HeadWidth*cost - HeadHeight*sint));

            // offset arrow from vertex
            var R = Math.Sqrt(Math.Pow(from.X - to.X, 2) + Math.Pow(from.Y - to.Y, 2));
            var cosX = (from.X - to.X)/R;
            var cosY = (from.Y - to.Y)/R;
            var ΔX = radiusVertex*cosX;
            var ΔY = radiusVertex*cosY;
            var pt1 = new Point(to.X - ΔX, to.Y - ΔY);

            //drawingContext.DrawLine(new Pen(new SolidColorBrush(Color), 3), pt1, pt3);
            //drawingContext.DrawLine(new Pen(new SolidColorBrush(Color), 3), pt1, pt4);

            var segments = new[]
            {
                new LineSegment(pt3, true),
                new LineSegment(pt4, true)
            };

            var figure = new PathFigure(pt1, segments, true);
            var geometry = new PathGeometry(new[] {figure});
            drawingContext.DrawGeometry(new SolidColorBrush(Color), new Pen(new SolidColorBrush(Color), 3), geometry);
        }

        public override string ToString()
        {
            return $"{Id} - {LabelName}";
        }

        private new int GetHashCode()
        {
            return HashCode.GetHashCode(From.Id, To.Id);
        }

        public void SetFrom(VertexControl from)
        {
            From = from;
        }

        public void SetTo(VertexControl to)
        {
            if (To == null)
                To = to;
            toPoint = new Point();
        }

        #region Properties & Fields

        public GraphArea RootGraph { get; }

        /// <summary>
        ///     Source visual vertex object
        /// </summary>
        public IVertexElement From
        {
            get { return (VertexControl) GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From",
            typeof (VertexControl),
            typeof (EdgeControl),
            new PropertyMetadata(null, OnFromChangedInternal));

        private static void OnFromChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EdgeControl;
            if (ctrl != null) ctrl.OnFromChanged(d, e);
        }

        protected void OnFromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            From.PositionChanged += OnPositionChanged;
            InvalidateVisual();
        }

        /// <summary>
        ///     Target visual vertex object
        /// </summary>
        public IVertexElement To
        {
            get { return (VertexControl) GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To",
            typeof (VertexControl),
            typeof (EdgeControl),
            new PropertyMetadata(null, OnToChangedInternal));

        private static void OnToChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as EdgeControl;
            if (ctrl != null) ctrl.OnToChanged(d, e);
        }

        protected void OnToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            To.PositionChanged += OnPositionChanged;
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

        private readonly Brush BrushColor = Brushes.Black;
        private readonly Brush BrushColorSelected = Brushes.Orange;

        private ILabelElement _labelElement;

        public bool IsLabel => _labelElement != null;

        public string LabelName
        {
            get { return _labelElement.Name; }
            set { _labelElement.Name = value; }
        }

        /// <summary>
        ///     Internal method. Attaches label to control
        /// </summary>
        /// <param name="element">Label control</param>
        public void AttachLabel(ILabelElement element)
        {
            _labelElement = element;
        }

        /// <summary>
        ///     Internal method. Detaches label from control.
        /// </summary>
        public void DetachLabel()
        {
            if (IsLabel)
            {
                _labelElement.Detach();
                RootGraph.Children.Remove((UIElement) _labelElement);
                _labelElement = null;
            }
        }

        /// <summary>
        ///     Registers a dependency property as backing store for the IsSelected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                "IsSelected", typeof (bool), typeof (EdgeControl),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        ///     Gets or sets the Content.
        /// </summary>
        /// <value>The IsSelected.</value>
        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        #endregion

        #region Position

        /// <summary>
        ///     Fires when Position property set and object changes its coordinates.
        /// </summary>
        public event PositionChanged PositionChanged;

        private void OnPositionChanged(object sender, EventArgs e)
        {
            PositionChanged?.Invoke(this, new EventArgs());
            //update edge on any connected vertex position changes
            InvalidateVisual();
        }

        #endregion
    }
}