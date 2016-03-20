using System;
using NGraph.Models;  
using System.Windows; 
using GraphEditor.View;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using GraphEditor.Controls.Interfaces;
using GraphEditor.Models;

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

        protected internal ILabelElement LabelElement;  

        public bool IsLabel => LabelElement != null;

        public string LabelName
        {
            get { return LabelElement.Name; }
            set { LabelElement.Name = value; }
        }

        /// <summary>
        /// Internal method. Attaches label to control
        /// </summary>
        /// <param name="element">Label control</param>
        public void AttachLabel(ILabelElement element)
        {
            LabelElement = element;
        }

        /// <summary>
        /// Internal method. Detaches label from control.
        /// </summary>
        public void DetachLabel()
        {
            if (IsLabel)
            {
                LabelElement.Detach();
                RootGraph.Children.Remove((UIElement)LabelElement);
                LabelElement = null;
            }
        }

        /// <summary>
        /// Registers a dependency property as backing store for the IsSelected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                "IsSelectedEdge", typeof(bool), typeof(EdgeControl),
                new FrameworkPropertyMetadata(false,
                  FrameworkPropertyMetadataOptions.AffectsRender));
        
        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The IsSelected.</value>
        public bool IsSelected
        {
            get { return (bool)GetValue(EdgeControl.IsSelectedProperty); }
            set { SetValue(EdgeControl.IsSelectedProperty, value); }
        }
                                
        #endregion

        #region Position

        /// <summary>
        /// Fires when Position property set and object changes its coordinates.
        /// </summary>
        public event PositionChanged PositionChanged;

        private void OnPositionChanged(object sender, EventArgs e)
        {
            if (PositionChanged != null)
                PositionChanged.Invoke(this, new EventArgs());
            //update edge on any connected vertex position changes
            InvalidateVisual();
        }

        #endregion

        #region Construction Edge

        public int Id { get; }

        public EdgeControl(GraphArea rootGraph, int id,  VertexControl from, object edge = null)
        {
            Id = id;
            Edge = edge;
            From = from;
            DataContext = edge;
            RootGraph = rootGraph;
            RootGraph.Children.Add(this);
            GraphArea.SetZIndex(this, 10);
        }          

        public EdgeControl(GraphArea rootGraph, int id,  VertexControl from, VertexControl to, object edge)
        {                               
            GraphArea.SetZIndex(this, 10);
            RootGraph.Children.Add(this);
            RootGraph = rootGraph;
            DataContext = edge;
            Edge = edge;
            From = from;
            To = to;
            Id = id;
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
            Point to;
            to = To?.GetPosition() ?? toPoint;
            if (to.X == 0 && to.Y == 0)
                return;
            drawingContext.DrawLine(new Pen(IsMouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3), from, to);
            base.OnRender(drawingContext);
        }

        public override string ToString()
        {
            return $"{Id} - {LabelName}";
        }

        public void Dispose()
        {                        
            From.Remove(this);
            To.Remove(this);
            DetachLabel();
            RootGraph.Children.Remove(this);   
        }
    }
}