using System.Collections.Generic;
using System.Windows;
using GraphEditor.Models;

namespace GraphEditor.Controls.Interfaces
{
    public interface IVertexElement : IUiElement
    {
        List<EdgeControl> IncommingEdges { get; }

        List<EdgeControl> OutcommingEdges { get; }
                          
        IList<EdgeControl> UndirectedEdges { get; }

        IList<EdgeControl> AllEdges { get; }

        bool AddEdge(IEdgeUiElement e);

        bool Remove(IEdgeUiElement e);

        IEdgeUiElement FindEdge(IVertexElement v);

        Point GetPosition();
        void SetPosition(Point pt);
        void SetPosition(double x, double y);
    }
}