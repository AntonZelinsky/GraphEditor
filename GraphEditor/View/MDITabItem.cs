using System.Windows;
using System.Windows.Controls;
using GraphEditor.ViewModels;

namespace GraphEditor.View
{
    public class MDITabItem : TabItem
    {
        public static readonly RoutedEvent CloseTabEvent =
            EventManager.RegisterRoutedEvent("CloseTab", RoutingStrategy.Bubble,
                typeof (RoutedEventHandler), typeof (MDITabItem));

        static MDITabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (MDITabItem),
                new FrameworkPropertyMetadata(typeof (MDITabItem)));
        }

        public GraphViewModel GraphViewModel { get; set; }

        public event RoutedEventHandler CloseTab
        {
            add { AddHandler(CloseTabEvent, value); }
            remove { RemoveHandler(CloseTabEvent, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeButton = GetTemplateChild("PART_Close") as Button;
            if (closeButton != null)
                closeButton.Click += closeButton_Click;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CloseTabEvent, this));
        }
    }
}