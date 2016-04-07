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

namespace GraphEditor.Controls
{
    public class VertexControl : Control, IVertexElement
    {
        private readonly List<EdgeControl> _outcomingEdges;

        private readonly List<EdgeControl> _incomingEdges;
                                   
        public List<EdgeControl> IncommingEdges => _incomingEdges;
                                   
        public List<EdgeControl> OutcommingEdges => _outcomingEdges;

        public IList<EdgeControl> UndirectedEdges => _incomingEdges.Intersect(_outcomingEdges).ToList().AsReadOnly();

        public IList<EdgeControl> AllEdges => _incomingEdges.Union(_outcomingEdges).ToList().AsReadOnly();

        public GraphArea RootGraph { get; }

        public int Id { get; }

        public VertexControl(GraphArea rootGraph, int id, Point p) 
        {
            Id = id;
            RootGraph = rootGraph;
            SetPosition(p);
            RootGraph.Children.Add(this);
            GraphArea.SetZIndex(this, 100);

            _incomingEdges = new List<EdgeControl>();
            _outcomingEdges = new List<EdgeControl>();   
        }  

        #region Graph operation

        public bool AddEdge(IEdgeUiElement e)
        {
            if (Equals(e.From, this))
                _outcomingEdges.Add((EdgeControl)e);
            else if (e.To == this)
                _incomingEdges.Add((EdgeControl)e);
            else
                return false;

            InvalidateVisual();

            return true;
        }

        public bool Remove(IEdgeUiElement e)
        {
            if (e.From == this)
                _outcomingEdges.Remove((EdgeControl)e);
            else if (e.To == this)
                _incomingEdges.Remove((EdgeControl)e);
            else
                return false;

            InvalidateVisual();

            return true;
        }

        public IEdgeUiElement FindEdge(IVertexElement v)
        {
            return AllEdges.FirstOrDefault(e => e.To == v);
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
                "IsSelectedVertex", typeof (bool), typeof (VertexControl),
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
        
        protected override void OnRender(DrawingContext drawingContext)
        {            
            var rate = AllEdges.Count / 2;
            drawingContext.DrawEllipse(
                Brushes.AliceBlue, 
                new Pen(IsMouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3),
                new Point(0, 0), 10 + rate, 10 + rate);

            rate = AllEdges.Count / 3;    
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

        protected void OnPositionChanged(Point offset, Point pos)
        {     
            if (PositionChanged != null)
                PositionChanged.Invoke(this, new EventArgs());
        }

        #endregion
             
        public override string ToString()
        {
            return $"{Id} - {LabelName}";
        }
    }
}
