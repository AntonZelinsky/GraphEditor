using System.Collections.Generic;
using System.Windows;
using GraphEditor.Models;

namespace GraphEditor.Controls.Interfaces
{
    public interface IVertexElement : IElement
    {
        List<EdgeControl> IncommingEdges { get; }

        List<EdgeControl> OutcommingEdges { get; }

        List<EdgeControl> UndirectedEdges { get; }

        List<EdgeControl> AllEdges { get; }

        /// <summary>
        /// Fires when Position property set and object changes its coordinates.
        /// </summary>
        event VertexPositionChanged PositionChanged;

        Point GetPosition();
        void SetPosition(Point pt);
        void SetPosition(double x, double y);
    }
}