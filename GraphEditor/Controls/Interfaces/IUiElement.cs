using System.Windows.Media;
using GraphEditor.View;
using GraphEditor.Models;

namespace GraphEditor.Controls.Interfaces
{
    public interface IUiElement
    {
        int Id { get; }

        bool IsSelected { get; set; } 

        GraphArea RootGraph { get; }

        void Destruction();

        /// <summary>
        /// Fires when Position property set and object changes its coordinates.
        /// </summary>
        event PositionChanged PositionChanged;
                                    
        bool IsLabel { get; }
        void AttachLabel(ILabelElement element);
        void DetachLabel();    
        string LabelName { get; set; }

        Color Color { get; set; }
        void ChangeColor(Color color);
        void ResetColor();
    }
}