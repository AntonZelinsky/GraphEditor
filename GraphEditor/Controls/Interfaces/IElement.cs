using GraphEditor.View;
using GraphEditor.Models;

namespace GraphEditor.Controls.Interfaces
{
    public interface IElement
    {
        bool IsSelected { get; set; } 

        GraphArea RootGraph { get; }

        void Destruction();

        /// <summary>
        /// Fires when Position property set and object changes its coordinates.
        /// </summary>
        event PositionChanged PositionChanged;

        void AttachLabel(ILabelElement element);
        void DetachLabel();
    }
}