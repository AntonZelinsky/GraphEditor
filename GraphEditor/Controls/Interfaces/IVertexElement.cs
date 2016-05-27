using System.Collections.Generic;
using System.Windows;
using GraphEditor.Models;

namespace GraphEditor.Controls.Interfaces
{
    public interface IVertexElement : IUiElement
    {                                       
        Dictionary<int, IEdgeUiElement> Edges { get; }      

        void AddEdge(IEdgeUiElement e);

        void Remove(IEdgeUiElement e);

        IEdgeUiElement FindEdge(IVertexElement v);

        Point GetPosition();
        void SetPosition(Point pt);
        void SetPosition(double x, double y);

        int Rate { get; }
    }
}