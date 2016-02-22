using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphEditor.View
{
    public class WorkArea : Canvas
    {
        private readonly VisualCollection visualObjects;

        public WorkArea()                
        {
            visualObjects = new VisualCollection(this);
            visualObjects.Add(AddRec());
                                          
            MouseDown += OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Point p = mouseButtonEventArgs.GetPosition((UIElement)sender);
            var b = new Button();
            b.Content = "sdfdsfdsfdsf";
            Canvas.SetLeft(b, p.X);
            Canvas.SetRight(b, p.Y);
            //VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(ClicCallback), new PointHitTestParameters(p));
        }

        private HitTestResultBehavior ClicCallback(HitTestResult result)
        {
            //Если щелчёк был соверён на визуальном объекте
            if (result.VisualHit.GetType() == typeof(DrawingVisual))
            {
                if (( (DrawingVisual)result.VisualHit ).Transform == null)
                    ( (DrawingVisual)result.VisualHit ).Transform = new SkewTransform(7, 7);
                else
                    ( (DrawingVisual)result.VisualHit ).Transform = null;
            }
            return HitTestResultBehavior.Stop;
        }

        Visual AddRec()
        {
            var DrVi = new DrawingVisual();
            using (DrawingContext dc = DrVi.RenderOpen())
            {                                                                
                dc.DrawEllipse(Brushes.BlueViolet, null, new Point(70, 90), 40, 50);
            }
            return DrVi;
        }

        #region MyRegion    
        protected override int VisualChildrenCount => visualObjects.Count;

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= visualObjects.Count)
                throw new ArgumentOutOfRangeException();
            return visualObjects[index];
        }

        public void AddVisual(Visual visual)
        {
            visualObjects.Add(visual);
            base.AddLogicalChild(visual);
            base.AddVisualChild(visual);
        }

        public void RemoveVisual(Visual visual)
        {
            visualObjects.Remove(visual);
            base.RemoveLogicalChild(visual);
            base.RemoveVisualChild(visual);
        }
        #endregion
    }
}
