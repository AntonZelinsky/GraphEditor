using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGraph.Collections;
using NGraph.Models;

namespace NGraph.Interfaces
{
    public interface IVertex
    {
        string Name { get; set; }

        EdgeList IncommingEdges { get; }

        EdgeList OutcommingEdges { get; }

        bool AddEdge(IEdge e);

        bool AddIncomingEdge(IVertex from);

        bool AddOutgoinEdge(IVertex to);

        bool HasEdge(IEdge e);

        bool Remove(IEdge e);

        int GetIncommingEdgeCount();

        int GetOutcommingCount();

        IEdge FindEdge(IVertex v);
    }
}
