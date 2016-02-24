using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;  
using NGraph.Models;

namespace NGraph.Interfaces
{
    public interface IVertex
    {
        string Name { get; set; }

        List<IEdge> IncommingEdges { get; }

        List<IEdge> OutcommingEdges { get; }

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
