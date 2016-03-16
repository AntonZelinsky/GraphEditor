using System;
using System.Windows;    
using GraphEditor.Models;
using System.Globalization;     
using System.Windows.Media;  
using System.ComponentModel;
using System.Windows.Controls;     
using System.Runtime.CompilerServices;
using GraphEditor.Controls.Interfaces;

namespace GraphEditor.Controls
{
    public class VertexLabelControl : ContentControl, ILabelControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets label attach node
        /// </summary>
        public IElement AttachNode
        {
            get { return (IElement)GetValue(AttachNodeProperty); }
            private set { SetValue(AttachNodeProperty, value); OnPropertyChanged("AttachNode"); }
        }

        public static readonly DependencyProperty AttachNodeProperty = 
            DependencyProperty.Register("AttachNode", typeof(IElement), typeof(VertexLabelControl),
            new PropertyMetadata(null));

        private Point positionLabel;

        public VertexLabelControl()
        {
            DataContext = this;
        }

        public void Attach(IElement node)
        {
            AttachNode = node;
            node.AttachLabel(this);   
            ((IVertexElement)AttachNode).PositionChanged += OnPositionChanged;
        }
          
        public void Detach()
        {
            ((IVertexElement)AttachNode).PositionChanged -= OnPositionChanged;
            AttachNode = null;
        }

        private void OnPositionChanged(object sender, VertexPositionEventArgs args)
        {
            UpdatePosition();
        }

        public void UpdatePosition()
        {          
            var vc = AttachNode as IVertexElement;                                   
            if (AttachNode == null)
                return;
            var vcPos = vc.GetPosition();
            positionLabel = new Point(vcPos.X - 10, vcPos.Y + 10);   
            InvalidateVisual();  
        }

        public void Hide()
        {               
            SetCurrentValue(UIElement.VisibilityProperty, Visibility.Collapsed);   
        }

        public void Show()
        {    
            SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);     
        }

        public event PropertyChangedEventHandler PropertyChanged;
                                          
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawText(
                new FormattedText("null", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                16, Brushes.Black), positionLabel);
        }  
    }
}