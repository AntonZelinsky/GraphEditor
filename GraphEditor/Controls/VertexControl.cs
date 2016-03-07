using System;     
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
    public class VertexControl : Control, IElement
    {
        static VertexControl()
        {

        }
        public VertexControl(object vertexData)
        {
            DataContext = vertexData;
            Vertex = vertexData;
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
            drawingContext.DrawEllipse(
                Brushes.AliceBlue, 
                new Pen(MouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3),
                new Point(0, 0), 10, 10);

            drawingContext.DrawEllipse(
                MouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 
                new Pen(MouseOver ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3),
                new Point(0, 0), 5, 5);
        }

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
        
        #region Position trace feature
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
    }
}
