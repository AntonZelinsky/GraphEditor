using System.Windows;
using GraphEditor.View;

namespace GraphEditor.Controls.Interfaces
{
    public interface IElement
    {
        bool IsSelected { get; set; } 

        GraphArea RootGraph { get; }

        void Destruction();

        void AttachLabel(ILabelControl vertexLabelControl);
        void DetachLabel();
    }
}