using System;
using NGraph.Models;  
using System.Windows; 
using GraphEditor.View;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using GraphEditor.Controls.Interfaces;
using GraphEditor.Helper;
using GraphEditor.Models;

namespace GraphEditor.Controls
{
    public class EdgeControl : Control, IEdgeUiElement
    {
        #region Construction Edge

        public int Id => GetHashCode();

        public EdgeControl(GraphArea rootGraph, VertexControl from)
        {
            From = from;
            RootGraph = rootGraph;
            RootGraph.Children.Add(this);
            GraphArea.SetZIndex(this, 10);
            Color = Colors.Black;
        }

        public EdgeControl(GraphArea rootGraph, VertexControl from, VertexControl to)
        {           
            To = to;
            From = from;
            RootGraph = rootGraph;
            RootGraph.Children.Add(this);
            GraphArea.SetZIndex(this, 10);
            Color = Colors.Black;
        }

        public void SetFrom(VertexControl from)
        {
            From = from;
        }

        public void SetTo(VertexControl to)
        {
            if(To == null)
                To = to;
            toPoint = new Point();
        }

        #endregion

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
            From.PositionChanged += OnPositionChanged;
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
     
        private Brush BrushColor = Brushes.Black;
        private Brush BrushColorSelected = Brushes.Orange;

        private ILabelElement _labelElement;  

        public bool IsLabel => _labelElement != null;

        public string LabelName
        {
            get { return _labelElement.Name; }
            set { _labelElement.Name = value; }
        }

        /// <summary>
        /// Internal method. Attaches label to control
        /// </summary>
        /// <param name="element">Label control</param>
        public void AttachLabel(ILabelElement element)
        {
            _labelElement = element;
        }

        /// <summary>
        /// Internal method. Detaches label from control.
        /// </summary>
        public void DetachLabel()
        {
            if (IsLabel)
            {
                _labelElement.Detach();
                RootGraph.Children.Remove((UIElement)_labelElement);
                _labelElement = null;
            }
        }

        /// <summary>
        /// Registers a dependency property as backing store for the IsSelected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                "IsSelected", typeof(bool), typeof(EdgeControl),
                new FrameworkPropertyMetadata(false,
                  FrameworkPropertyMetadataOptions.AffectsRender));
        
        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The IsSelected.</value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
                                
        #endregion

        #region Position

        /// <summary>
        /// Fires when Position property set and object changes its coordinates.
        /// </summary>
        public event PositionChanged PositionChanged;

        private void OnPositionChanged(object sender, EventArgs e)
        {
            PositionChanged?.Invoke(this, new EventArgs());
            //update edge on any connected vertex position changes
            InvalidateVisual();
        }

        #endregion

        public Color Color { get; set; }

        private Brush ColorAlgorithm => new RadialGradientBrush(Color, Colors.White);

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
            From.Remove(this);
            To.Remove(this);

            DetachLabel();
            RootGraph.Children.Remove(this);     
        }

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
            Point from = From.GetPosition();
            var to = To?.GetPosition() ?? toPoint;
            if (to.X == 0 && to.Y == 0)
                return;
            if (Color != Colors.Black) 
                drawingContext.DrawLine(new Pen(ColorAlgorithm, 6), from, to);                                                    
            drawingContext.DrawLine(new Pen(IsMouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, Color != Colors.Black ? 2 : 3), from, to);
            base.OnRender(drawingContext);
        }

        public override string ToString()
        {
            return $"{Id} - {LabelName}";
        }

        private new int GetHashCode()
        {
            return HashCode.GetHashCode(From.Id, To.Id);  
        }   
    }
}