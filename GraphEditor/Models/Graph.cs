using GraphEditor.View;
using System.Collections.Generic;
using GraphEditor.Controls;
using GraphEditor.Controls.Interfaces;  

namespace GraphEditor.Models
{
    public class Graph
    {
        #region Properties

        private GraphArea rootArea;

        private List<IEdgeElement> edges;
        public List<IEdgeElement> Edges => edges;


        private List<IVertexElement> verticies;
        public List<IVertexElement> Verticies => verticies;

        #endregion

        public Graph(GraphArea area)
        {
            rootArea = area;
            edges = new List<IEdgeElement>();
            verticies = new List<IVertexElement>();
        }

        public bool AddVertex(IVertexElement v)
        {
            if (ContainsVertex(v))
                return false;

            verticies.Add(v);
            //event OnVertexAdded
            return true;
        }

        /// <summary>
        /// Insert a directed Edge into the graph
        /// </summary>
        /// <param name="e">the Edge </param>
        /// <returns>status</returns>
        public bool AddEdge(IVertexElement from, IVertexElement to)
        {
            return AddEdge(new EdgeControl(rootArea, (VertexControl)from, (VertexControl)to));
        }

        /// <summary>
        /// Insert a directed Edge into the graph
        /// </summary>
        /// <param name="e">the Edge </param>
        /// <returns>status</returns>
        public bool AddEdge(IEdgeElement e)
        {
            //if(ContainsEdge(e))

            if (e.From.FindEdge(e.To) != null)
                return false;

            e.From.AddEdge(e);
            e.To.AddEdge(e);
            edges.Add(e);
            return true;
        }


        public bool IsEmpty()
        {
            return verticies.Count == 0;
        }

        public bool ContainsVertex(IVertexElement v)
        {
            return verticies.Contains(v);
        }

    }
}