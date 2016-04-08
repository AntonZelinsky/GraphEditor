using System;
using System.Windows;  
using System.Globalization;     
using System.Windows.Media;  
using System.ComponentModel;
using System.Windows.Controls;     
using System.Runtime.CompilerServices;
using GraphEditor.View;   
using GraphEditor.Controls.Interfaces;

namespace GraphEditor.Controls
{
    public sealed class LabelElement : ContentControl, ILabelElement, INotifyPropertyChanged
    {
        private GraphArea RootGraph { get; }

        /// <summary>
        /// Gets label name
        /// </summary>    
        public new string Name
        {
            get { return name; }
            set {
                InvalidateVisual();
                name = value;
            }
        }

        private string name;

        /// <summary>
        /// Gets label attach element
        /// </summary>
        public IUiElement AttachUiElement
        {
            get { return (IUiElement)GetValue(AttachUiElementProperty); }
            private set { SetValue(AttachUiElementProperty, value); OnPropertyChanged("AttachUiElement"); }
        }

        public static readonly DependencyProperty AttachUiElementProperty = 
            DependencyProperty.Register("AttachUiElement", typeof(IUiElement), typeof(LabelElement),
            new PropertyMetadata(null));

        private Point PositionLabel;
                                     
        public LabelElement(GraphArea rootGraph, string name)
        {                                    
            Name = name;
            RootGraph = rootGraph;  
            RootGraph.Children.Add(this);
            GraphArea.SetZIndex(this, 5);
        }  

        public void Attach(IUiElement uiElement)
        {
            AttachUiElement = uiElement;
            uiElement.AttachLabel(this);   
            AttachUiElement.PositionChanged += OnPositionChanged;
        }
          
        public void Detach()
        {
            if (AttachUiElement != null)
            {
                AttachUiElement.PositionChanged -= OnPositionChanged;
                AttachUiElement = null;
            }
        }

        private void OnPositionChanged(object sender, EventArgs args)
        {
            UpdatePosition();
        }

        public void UpdatePosition()
        {                                            
            if (AttachUiElement is IVertexElement)
            {                   
                var vcPos = ((IVertexElement) AttachUiElement).GetPosition();
                PositionLabel = new Point(vcPos.X - 10, vcPos.Y + 10);
            }
            if (AttachUiElement is IEdgeUiElement)
            {
                var from = (AttachUiElement as IEdgeUiElement).From.GetPosition();
                var to = (AttachUiElement as IEdgeUiElement).To.GetPosition();      
                PositionLabel = new Point(( from.X + to.X ) / 2, ( from.Y + to.Y ) / 2);
            }    

            InvalidateVisual();
        }
           
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawText(
                new FormattedText(Name, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                16, Brushes.Black), PositionLabel);
        }  
    }
}
