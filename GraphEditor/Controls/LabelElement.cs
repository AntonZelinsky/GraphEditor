using System;
using System.Windows; 
using GraphEditor.View;   
using GraphEditor.Models;
using System.Globalization;     
using System.Windows.Media;  
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;     
using System.Runtime.CompilerServices;
using GraphEditor.Controls.Interfaces;

namespace GraphEditor.Controls
{
    public class LabelElement : ContentControl, ILabelElement, INotifyPropertyChanged
    {      
        public GraphArea RootGraph { get; }

        /// <summary>
        /// Gets label attach element
        /// </summary>
        public IElement AttachElement
        {
            get { return (IElement)GetValue(AttachElementProperty); }
            private set { SetValue(AttachElementProperty, value); OnPropertyChanged("AttachElement"); }
        }

        public static readonly DependencyProperty AttachElementProperty = 
            DependencyProperty.Register("AttachElement", typeof(IElement), typeof(LabelElement),
            new PropertyMetadata(null));

        protected Point PositionLabel;

        public LabelElement(GraphArea rootGraph)
        {
            RootGraph = rootGraph;
            DataContext = this; 
            RootGraph.Children.Add(this);
            GraphArea.SetZIndex(this, 5);
        }
        ~LabelElement()
        {
            Debug.WriteLine("Delete Label");
        }

        public void Attach(IElement element)
        {
            AttachElement = element;
            element.AttachLabel(this);   
            ((IElement)AttachElement).PositionChanged += OnPositionChanged;
        }
          
        public void Detach()
        {
            if (AttachElement != null)
            {
                ((IElement)AttachElement).PositionChanged -= OnPositionChanged;
                AttachElement = null;
            }
        }

        private void OnPositionChanged(object sender, EventArgs args)
        {
            UpdatePosition();
        }

        public virtual void UpdatePosition()
        {                                            
            if (AttachElement is IVertexElement)
            {                   
                var vcPos = ((IVertexElement) AttachElement).GetPosition();
                PositionLabel = new Point(vcPos.X - 10, vcPos.Y + 10);
            }
            if (AttachElement is IEdgeElement)
            {
                var from = (AttachElement as IEdgeElement).From.GetPosition();
                var to = (AttachElement as IEdgeElement).To.GetPosition();      
                PositionLabel = new Point(( from.X + to.X ) / 2, ( from.Y + to.Y ) / 2);
            }    

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
                16, Brushes.Black), PositionLabel);
        }  
    }
}