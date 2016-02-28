using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGraph.Interfaces;

namespace NGraph.Models
{
    public class Vertex : IVertex
    {
        public string Name { get; set; }

        private List<IEdge> outcomingEdges;

        private List<IEdge> incomingEdges;

        public List<IEdge> IncommingEdges => incomingEdges;

        public List<IEdge> OutcommingEdges => outcomingEdges;

        public Vertex(string name = "")
        {
            Name = name;
            incomingEdges = new List<IEdge>();
            outcomingEdges = new List<IEdge>();
        }
             
        public bool AddEdge(IEdge e)
        {
           if(e.From == this)
               outcomingEdges.Add(e);
           else if (e.To == this)
               incomingEdges.Add(e);
           else
               return false;
           return true;
        }

        public bool AddIncomingEdge(IVertex from)
        {
            throw new NotImplementedException();
        }

        public bool AddOutgoinEdge(IVertex to)
        {
            throw new NotImplementedException();
        }

        public bool HasEdge(IEdge e)
        {
            if (e.From == this)
                return outcomingEdges.Contains(e);
            else if (e.To == this)
                return incomingEdges.Contains(e);
            else
                return false;
        }

        public bool Remove(IEdge e)
        {
            if (e.From == this)
                outcomingEdges.Remove(e);
            else if (e.To == this)
                incomingEdges.Remove(e);
            else
                return false;
            return true;
        }        

        public IEdge FindEdge(IVertex v)
        {
             return outcomingEdges.FirstOrDefault(e => e.To == v);
        }                    

        public int GetIncommingEdgeCount()
        {
            return incomingEdges.Count;
        }

        public int GetOutcommingCount()
        {
            return outcomingEdges.Count;
        }

        public override string ToString()
        {
            return $"{Name}, In {GetIncommingEdgeCount()}, Out {GetOutcommingCount()}";
        }
    }
}
