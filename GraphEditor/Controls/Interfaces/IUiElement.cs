using System.Windows.Media;
using GraphEditor.Models;
using GraphEditor.View;

namespace GraphEditor.Controls.Interfaces
{
    public interface IUiElement
    {
        int Id { get; }
        bool IsSelected { get; set; }
        GraphArea RootGraph { get; }
        bool IsLabel { get; }
        string LabelName { get; set; }
        Color Color { get; set; }
        void Destruction();

        /// <summary>
        ///     Fires when Position property set and object changes its coordinates.
        /// </summary>
        event PositionChanged PositionChanged;

        void AttachLabel(ILabelElement element);
        void DetachLabel();
        void ChangeColor(Color color);
        void ResetColor();
    }
}