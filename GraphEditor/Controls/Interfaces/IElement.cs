using System.Windows;
using GraphEditor.View;

namespace GraphEditor.Controls.Interfaces
{
    public interface IElement
    {
        bool IsSelected { get; set; } 

        GraphArea RootGraph { get; }
    }
}