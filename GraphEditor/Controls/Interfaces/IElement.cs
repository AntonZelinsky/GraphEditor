using System.Windows;

namespace GraphEditor.Controls.Interfaces
{
    public interface IElement
    {
        Point GetPosition();
        void SetPosition(Point pt);
        void SetPosition(double x, double y);
        bool IsSelected { get; set; } 
    }
}