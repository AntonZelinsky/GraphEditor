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

        public Point GetPosition()
        {
            return new Point((int)GraphArea.GetX(this), (int)GraphArea.GetY(this));
        }

        public void SetPosition(Point pt)
        {
            GraphArea.SetX(this, pt.X);
            GraphArea.SetY(this, pt.Y);
        }

        public void SetPosition(double x, double y)
        {
            GraphArea.SetX(this, x);
            GraphArea.SetY(this, y);
        }

        #region Event tracing

        internal void UpdateEventhandling(EventType typ)
        {
            switch (typ)
            {
                case EventType.PositionChangeNotify:
                    UpdatePositionTraceState();
                    break;
            }
        }

        #endregion

        #region Position trace feature
        /// <summary>
        /// Fires when IsPositionTraceEnabled property set and object changes its coordinates.
        /// </summary>
        public event VertexPositionChanged PositionChanged;

        protected void OnPositionChanged(Point offset, Point pos)
        {
            if (PositionChanged != null)
                PositionChanged.Invoke(this, new VertexPositionEventArgs(offset, pos, this));
        }

        private ChangeMonitor _xChangeMonitor;
        private ChangeMonitor _yChangeMonitor;
        internal void UpdatePositionTraceState()
        {
            //if (EventOptions.PositionChangeNotification)
            //{
            //    if (_xChangeMonitor == null)
            //    {
            //        _xChangeMonitor = new ChangeMonitor();
            //        _xChangeMonitor.Bind(this, GraphArea.XProperty);
            //        _xChangeMonitor.ChangeDetected += changeMonitor_ChangeDetected;
            //    }
            //    if (_yChangeMonitor == null)
            //    {
            //        _yChangeMonitor = new ChangeMonitor();
            //        _yChangeMonitor.Bind(this, GraphArea.YProperty);
            //        _yChangeMonitor.ChangeDetected += changeMonitor_ChangeDetected;
            //    }
            //}
            //else
            //{
            //    if (_xChangeMonitor != null)
            //    {
            //        _xChangeMonitor.ChangeDetected -= changeMonitor_ChangeDetected;
            //        _xChangeMonitor.Unbind();
            //        _xChangeMonitor = null;
            //    }
            //    if (_yChangeMonitor != null)
            //    {
            //        _yChangeMonitor.ChangeDetected -= changeMonitor_ChangeDetected;
            //        _yChangeMonitor.Unbind();
            //        _yChangeMonitor = null;
            //    }
            //}
        }

        private void changeMonitor_ChangeDetected(object source, EventArgs args)
        {
            //if (ShowLabel && VertexLabelControl != null)
            //    VertexLabelControl.UpdatePosition();
            OnPositionChanged(new Point(), GetPosition());
        }

        #endregion


        #region ChangeMonitor class

        /// <summary>
        /// This class is used to monitor for changes on the specified property of the specified control.
        /// </summary>
        private class ChangeMonitor : DependencyObject
        {
            public void Bind(UIElement el, DependencyProperty property)
            {
                var b = new Binding
                {
                    Path = new PropertyPath(property),
                    Source = el
                };
                BindingOperations.SetBinding(this, MonitorForChangeProperty, b);
            }

            public void Unbind()
            {
                BindingOperations.ClearBinding(this, MonitorForChangeProperty);
            }

            public delegate void Changed(object source, EventArgs args);

            public event Changed ChangeDetected;

            public static readonly DependencyProperty MonitorForChangeProperty =
                DependencyProperty.Register("MonitorForChange", typeof(object), typeof(ChangeMonitor), new PropertyMetadata(null, MonitoredPropertyChanged));

            public object MonitorForChange
            {
                get { return GetValue(MonitorForChangeProperty); }
                set { SetValue(MonitorForChangeProperty, value); }
            }

            private static void MonitoredPropertyChanged(object source, DependencyPropertyChangedEventArgs args)
            {
                var cm = source as ChangeMonitor;
                if (cm == null)
                {
                    return;
                }
                var changeDetected = cm.ChangeDetected;
                if (changeDetected != null)
                {
                    changeDetected(cm, new EventArgs());
                }
            }
        }

        #endregion
    }
}
