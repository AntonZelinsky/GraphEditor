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
           if(e.From == this)
               outcomingEdges.Add(e);
           else if (e.To == this)
               incomingEdges.Add(e);
           else
               return false;
           return true;
        }

        public bool AddIncomingEdge(Vertex from)
        {
            throw new NotImplementedException();
        }

        public bool AddOutgoinEdge(Vertex to)
        {
            throw new NotImplementedException();
        }

        public bool HasEdge(Edge e)
        {
            if (e.From == this)
                return incomingEdges.Contains(e);
            else if (e.To == this)
                return outcomingEdges.Contains(e);
            else
                return false;
        }

        public bool Remove(Edge e)
        {
            if (e.From == this)
                outcomingEdges.Remove(e);
            else if (e.To == this)
                incomingEdges.Remove(e);
            else
                return false;
            return true;
        }

        public int GetIncommingEdgeCount()
        {
            return incomingEdges.Count;
        }

        public int GetOutcommingCount()
        {
            return outcomingEdges.Count;
        }
    }
}
