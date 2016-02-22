using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGraph.Collections;
using NGraph.Interfaces;

namespace NGraph.Models
{
    public class Vertex : IVertex
    {
        public string Name { get; set; }
        /// <summary>
        /// Unique vertex ID
        /// </summary>
        public long ID { get; set; }

        private EdgeList outcomingEdges;
        private EdgeList incomingEdges;

        public Vertex():this(String.Empty)
        {
        }

        public Vertex(string name = "")
        {
            Name = name;
            incomingEdges = new EdgeList();
            outcomingEdges = new EdgeList();
        }

        public override string ToString()
        {
            return Name;
        }

        public bool AddEdge(Edge e)
        {
            throw new NotImplementedException();
        }

        public bool AddIncomingEdge(Vertex from)
        {
            throw new NotImplementedException();
        }

        public bool AddOutgoinEdge(Vertex to)
        {
            throw new NotImplementedException();
        }
    }
}
