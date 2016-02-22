using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGraph.Collections;
using NGraph.Interfaces;

namespace NGraph.Models
{
    [DebuggerDisplay("VertexCount = {VertexCount}, EdgeCount = {EdgeCount}")]
    public class Graph : IGraph
    {
        private readonly VertexList vertexs;
        private readonly EdgeList edges;


        /// <summary>
        /// Inbox edges
        /// </summary>
        private readonly VertexEdgeDictionary vertexInEdges;
        /// <summary>
        /// Outbox edges
        /// </summary>
        private readonly VertexEdgeDictionary vertexOutEdges;    

        //private readonly Vertex
        private int edgeCount = 0;

        public int EdgeCount { get { return edgeCount; } }
          public int VertexCount { get { return vertexInEdges.Count; } }

        public Graph()
        {
            vertexs = new VertexList();
            edges = new EdgeList();

            vertexInEdges = new VertexEdgeDictionary();
            vertexOutEdges = new VertexEdgeDictionary();
        }

    #region vertex
        public bool AddVertex(IVertex v)
        {
            if (ContainsVertex(v))
                return false;

            vertexs.Add(v);
            //vertexInEdges.Add(v, new EdgeList());
            //vertexOutEdges.Add(v, new EdgeList());
            //event OnVertexAdded
            return true;
        }

        public bool RemoveVertex(IVertex v)
        {
            if (!ContainsVertex(v))
                return false;

            var edgeToRemove = new EdgeList();


            /**/
            return false;
        }
    #endregion

    #region Edge

        public bool AddEdge(IEdge e, bool oriented = false)
        {

            //if(ContainsEdge(e))
            //vertexInEdges[e.Target].Add(e);
            //vertexOutEdges[e.Source].Add(e);
            //edgeCount++;
            //OnEdgeAdded
            return true;
        }

        public bool ContainsEdge(IEdge e)
        {
            return ContainsEdge(e.Source, e.Target);
        }

        public bool ContainsEdge(IVertex source, IVertex Target)
        {
            return true;
        }
    #endregion


        #region sup
        //public IEnumerable<IEdge> OutEdges(IVertex v)
        //{
        //    return this.vertexs[v];
        //}

        public bool ContainsVertex(IVertex v)
        {
            return vertexOutEdges.ContainsKey(v);
            //return vertexs.Contains(v);
        }

#endregion
    }
}
