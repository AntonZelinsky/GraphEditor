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

        bool AddEdge(Edge e);

        bool AddIncomingEdge(Vertex from);

        bool AddOutgoinEdge(Vertex to);
    }
}
