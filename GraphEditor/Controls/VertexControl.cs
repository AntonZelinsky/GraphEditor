using System;     
using System.Linq;
using System.Windows;
using GraphEditor.View;
using GraphEditor.Models;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media; 
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Collections.Generic;
using GraphEditor.Controls.Interfaces;

namespace GraphEditor.Controls
{
    public class VertexControl : Control, IVertexElement, IDisposable
    {
        private readonly List<EdgeControl> outcomingEdges;

        private readonly List<EdgeControl> incomingEdges;
                                   
        public List<EdgeControl> IncommingEdges => incomingEdges;
                                   
        public List<EdgeControl> OutcommingEdges => outcomingEdges;

        public IList<EdgeControl> UndirectedEdges => (IList<EdgeControl>)incomingEdges.Intersect(outcomingEdges).ToList().AsReadOnly();

        public IList<EdgeControl> AllEdges => (IList<EdgeControl>)incomingEdges.Union(outcomingEdges).ToList().AsReadOnly();

        public GraphArea RootGraph { get; }

        public VertexControl(object vertexData)
        {
            DataContext = vertexData;
            Vertex = vertexData;

            incomingEdges = new List<EdgeControl>();
            outcomingEdges = new List<EdgeControl>();
        }

        public VertexControl(GraphArea rootGraph, Point coordinate)
        {                          
            RootGraph = rootGraph;
            SetPosition(coordinate);
            RootGraph.Children.Add(this);
            GraphArea.SetZIndex(this, 100);

            incomingEdges = new List<EdgeControl>();
            outcomingEdges = new List<EdgeControl>();    
        }
            
        public bool AddEdge(IEdgeElement e)
        {
            if (e.From == this)
                outcomingEdges.Add((EdgeControl)e);
            else if (e.To == this)
                incomingEdges.Add((EdgeControl)e);
            else
                return false;
            return true;
        }

        public bool Remove(IEdgeElement e)
        {
            if (e.From == this)
                outcomingEdges.Remove((EdgeControl)e);
            else if (e.To == this)
                incomingEdges.Remove((EdgeControl)e);
            else
                return false;
            return true;
        }

        public IEdgeElement FindEdge(IVertexElement v)
        {
            return AllEdges.FirstOrDefault(e => e.To == v);
        }
          
        public void Destruction()
        {
            Dispose();
        }

        #region Property

        private Brush BrushColor = Brushes.Green;
        private Brush BrushColorSelected = Brushes.Orange;

        private bool MouseOver = false;
        #region DependencyProperty Content

        /// <summary>
        /// Registers a dependency property as backing store for the Content property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                "IsSelected", typeof (bool), typeof (VertexControl),
                new FrameworkPropertyMetadata(false,
                  FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        /// <value>The Content.</value>
        public bool IsSelected
        {
            get { return (bool)GetValue(VertexControl.IsSelectedProperty); }
            set { SetValue(VertexControl.IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets vertex data object
        /// </summary>
        public object Vertex
        {
            get { return GetValue(VertexProperty); }
            set { SetValue(VertexProperty, value); }
        }

        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof(object), typeof(VertexControl), new PropertyMetadata(null));
        #endregion

        #endregion

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            MouseOver = true;
            InvalidateVisual();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            MouseOver = false;
            InvalidateVisual();
            base.OnMouseLeave(e);
        }
        
        protected override void OnRender(DrawingContext drawingContext)
        {            
            var rate = AllEdges.Count / 2;
            drawingContext.DrawEllipse(
                Brushes.AliceBlue, 
                new Pen(MouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3),
                new Point(0, 0), 10 + rate, 10 + rate);

            drawingContext.DrawEllipse(
                MouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 
                new Pen(MouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3),
                new Point(0, 0), 5 + rate, 5 + rate);
        }

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
        public event VertexPositionChanged PositionChanged;

        protected void OnPositionChanged(Point offset, Point pos)
        {
            if (PositionChanged != null)
                PositionChanged.Invoke(this, new VertexPositionEventArgs(offset, pos, this));
        }

        #endregion

        public void Dispose()
        {
            foreach (var edge in AllEdges)
            {                   
                edge.Destruction();
            }

            RootGraph.Children.Remove(this);
        }
    }
}
