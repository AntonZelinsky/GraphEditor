using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media; 
using System.Windows.Controls;
using GraphEditor.Controls.Interfaces;

namespace GraphEditor.Controls
{
    public class VertexControl : Control, IElement
    {
        static VertexControl()
        {
            
        }                                        

        #region Property

        private Brush BrushColor = Brushes.Green;
        private Brush BrushColorSelected = Brushes.Orange;

        private bool IsEnter = false;
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

        #endregion

        #endregion

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            IsEnter = true;
            InvalidateVisual();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            IsEnter = false;
            InvalidateVisual();
            base.OnMouseLeave(e);
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawEllipse(
                Brushes.AliceBlue, new Pen(IsEnter ? BrushColorSelected : IsSelected ? BrushColorSelected : BrushColor, 3),
                new Point(0, 0), 10, 10);
        }
    }
}
