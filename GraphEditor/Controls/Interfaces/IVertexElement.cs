using System.Collections.Generic;
using System.Windows;

namespace GraphEditor.Controls.Interfaces
{
    public interface IVertexElement : IUiElement
    {
        Dictionary<int, IEdgeUiElement> Edges { get; }
        int Rate { get; }
        void AddEdge(IEdgeUiElement e);
        void RemoveEdge(IEdgeUiElement e);
        IEdgeUiElement FindEdge(IVertexElement v);
        Point GetPosition();
        void SetPosition(Point pt);
        void SetPosition(double x, double y);
    }
}