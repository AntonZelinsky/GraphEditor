using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;  
using NGraph.Interfaces;

namespace NGraph.Models
{
   // [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class Graph : IGraph
    {
        private readonly List<IVertex> verticies;
        private readonly List<IEdge> edges;      
                                     
        public int EdgeCount { get { return edges.Count; } }
        public int VertexCount => verticies.Count;

        public Graph()
        {
            verticies = new List<IVertex>();
            edges = new List<IEdge>();                    
        }

        public bool IsEmpty()
        {
            return verticies.Count == 0;
        }

    #region Add

        public bool AddVertex(IVertex v)
        {
            if (ContainsVertex(v))
                return false;

            verticies.Add(v);                      
            //event OnVertexAdded
            return true;
        }

        /// <summary>
        /// Insert a directed, Edge into the graph
        /// </summary>
        /// <param name="e">the Edge </param>
        /// <returns></returns>
        public bool AddEdge(IVertex from, IVertex to)
        {
            return AddEdge(new Edge(from, to));
        }
        
        /// <summary>
        /// Insert a directed, Edge into the graph
        /// </summary>
        /// <param name="e">the Edge </param>
        /// <returns></returns>
        public bool AddEdge(IEdge e)
        {
            //if(ContainsEdge(e))

            if (e.From.FindEdge(e.To) != null)
                return false;

            e.From.AddEdge(e);
            e.To.AddEdge(e);
            edges.Add(e);
            return true;
        }

    #endregion

    #region Remove

        public bool RemoveVertex(IVertex v)
        {
            if (!ContainsVertex(v))
                return false;
            verticies.Remove(v);
            
            // Remove the edges associated with v
            foreach (var e in v.IncommingEdges.ToArray())
            {
                v.Remove(e);
                e.From.Remove(e);  //возможно зацикливание
                edges.Remove(e);
            }
            foreach (var e in v.OutcommingEdges)
            {
                v.Remove(e);
                e.To.Remove(e);   
                edges.Remove(e);    // out error
            }
            return true;
        }

        public bool RemoveEdge(IVertex from, IVertex to)
        {
            var e = from.FindEdge(to);
            if (e == null)
                return false;
            return RemoveEdge(e);//new Edge(from, to)
        }

        public bool RemoveEdge(IEdge e)
        {
            e.From.Remove(e);
            e.To.Remove(e);
            edges.Remove(e);
            return true;
        }

        public bool ContainsEdge(IEdge e)
        {
            return ContainsEdge(e.From, e.To);
        }

        public bool ContainsEdge(IVertex source, IVertex target)
        {
            return true;
        }

        public List<IEdge> GetEdges()
        {
            return edges;
        }

        public List<IVertex> GetVerticies()
        {
            return verticies;
        }
    #endregion


    #region sup   

        public bool ContainsVertex(IVertex v)
        {
            return verticies.Contains(v); 
        }

    #endregion

        public override string ToString()
        {
            return $"VertexCount = {VertexCount}, EdgeCount = {EdgeCount}";
        }
    }
}
