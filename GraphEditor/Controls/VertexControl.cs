using System;     
using System.Linq;
using System.Windows;
using GraphEditor.View;
using GraphEditor.Models;  
using System.Windows.Input;
using System.Windows.Media;    
using System.Windows.Controls;
using System.Collections.Generic;
using GraphEditor.Controls.Interfaces;
using GraphEditor.Helper;

namespace GraphEditor.Controls
{
    public class VertexControl : Control, IVertexElement
    {
        private readonly Dictionary<int, IEdgeUiElement> _edges;          

        public Dictionary<int, IEdgeUiElement> Edges => _edges;                                                

        public GraphArea RootGraph { get; }

        public int Id { get; }

        public VertexControl(GraphArea rootGraph, int id, Point p) 
        {
            Id = id;
            RootGraph = rootGraph;
            SetPosition(p);
            RootGraph.Children.Add(this);
            GraphArea.SetZIndex(this, 100);
                                                      
            _edges = new Dictionary<int, IEdgeUiElement>();
            Color = Colors.Green;
        }  

        #region Graph operation

        public void AddEdge(IEdgeUiElement e)
        {
           if(!_edges.ContainsKey(e.Id))
                _edges.Add(e.Id, e);

            InvalidateVisual();   
        }

        public void Remove(IEdgeUiElement e)
        {
            if (_edges.ContainsKey(e.Id))
                _edges.Remove(e.Id);

            InvalidateVisual(); 
        }

        public IEdgeUiElement FindEdge(IVertexElement v)
        {
            var idEdge = HashCode.GetHashCode(Id, v.Id);
            if (_edges.ContainsKey(idEdge))
                return _edges[idEdge];
            else
                return null;
        }

        #endregion

        public void Destruction()
        {
            DetachLabel();

            RootGraph.Children.Remove(this);
        }

        #region Label

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
                RootGraph.Children.Remove((UIElement) _labelElement);   
                _labelElement = null;
            }
        }

        #endregion

        #region Property

        private Brush BrushColor = Brushes.Green;
        private Brush BrushColorSelected = Brushes.Orange;
                                          
        #region DependencyProperty Content

        /// <summary>
        /// Registers a dependency property as backing store for the IsSelected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                "IsSelected", typeof (bool), typeof (VertexControl),
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

        #endregion         

        public Color Color { get; set; }

        private Brush _colorAlgorithm => new RadialGradientBrush(Color, Colors.White);

        public void ChangeColor(Color color)
        {
            Color = color;
            InvalidateVisual();
        }

        public void ResetColor()
        {
            Color = Colors.Green;
            InvalidateVisual();
        }

        #region Override event

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

        private int rate;

        public int Rate => -(rate + 12);
        protected override void OnRender(DrawingContext drawingContext)
        {            
            rate = _edges.Count / 2;                   
            drawingContext.DrawEllipse(
                Brushes.White, 
                new Pen(IsMouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3),
                new Point(0, 0), 10 + rate, 10 + rate);

            rate = _edges.Count / 3;
            if (Color != Colors.Green)
            {
                drawingContext.DrawEllipse(
                 _colorAlgorithm,
                 new Pen(null, 0),
                 new Point(0, 0), 8 + rate, 8 + rate);
            }
            else  
                drawingContext.DrawEllipse(
                    IsMouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor,
                    new Pen(IsMouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3),
                    new Point(0, 0), 5 + rate, 5 + rate);
        }

        #endregion

        #region Position trace feature

        public Point GetPosition()
        {
            return new Point((int)GraphArea.GetX(this), (int)GraphArea.GetY(this));
        }

        public void SetPosition(Point pt)
        {
            GraphArea.SetX(this, pt.X);
            GraphArea.SetY(this, pt.Y);
            OnPositionChanged(new Point(), GetPosition());
        }

        public void SetPosition(double x, double y)
        {
            GraphArea.SetX(this, x);
            GraphArea.SetY(this, y);
            OnPositionChanged(new Point(), GetPosition());
        }
        
        /// <summary>
        /// Fires when Position property set and object changes its coordinates.
        /// </summary>
        public event PositionChanged PositionChanged;

        private void OnPositionChanged(Point offset, Point pos)
        {
            PositionChanged?.Invoke(this, new EventArgs());
        }

        #endregion
             
        public override string ToString()
        {
            return $"{Id} - {LabelName}";
        }
    }
}
