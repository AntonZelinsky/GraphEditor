﻿using System.Collections.Generic;
using System.Windows;
using GraphEditor.Models;

namespace GraphEditor.Controls.Interfaces
{
    public interface IVertexElement : IElement
    {
        List<EdgeControl> IncommingEdges { get; }

        List<EdgeControl> OutcommingEdges { get; }
                          
        IList<EdgeControl> UndirectedEdges { get; }

        IList<EdgeControl> AllEdges { get; }

        bool AddEdge(IEdgeElement e);

        bool Remove(IEdgeElement e);

        IEdgeElement FindEdge(IVertexElement v);

        Point GetPosition();
        void SetPosition(Point pt);
        void SetPosition(double x, double y);
    }
}