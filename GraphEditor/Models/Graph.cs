using System;
using GraphEditor.View;
using System.Collections.Generic;
using System.Windows;
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
               
        #region Creating IElement Control

        public VertexControl CreateVertexControl(Point p)
        {
            var v = new VertexControl(rootArea, p);
            AddVertex(v);
            GenerateVertexLabel(v);
            return v;
        }

        #region Creating Edge

        private EdgeControl createdEdge;

        public EdgeControl CreateEdgeControl(VertexControl from)
        {
            var edgeControl = new EdgeControl(rootArea, from);
            createdEdge = edgeControl;
            return edgeControl;
        }

        public EdgeControl CreatingEdgeControl(Point to)
        {
            if (createdEdge.To != null)
                throw new Exception();
            createdEdge.SetToPoint(to);
            return createdEdge;
        }

        public EdgeControl ReleasedEdgeControl(VertexControl to)
        {
            createdEdge.SetTo(to);
            createdEdge.From.AddEdge(createdEdge);
            createdEdge.To.AddEdge(createdEdge);
            GenerateEdgeLabel(createdEdge);
            var e = createdEdge;  
            createdEdge = null;
            return e;
        }

        public void UnreleasedEdgeControl()
        {
            rootArea.Children.Remove(createdEdge);
            createdEdge = null;
        }

        public EdgeControl CreateEdgeControl(VertexControl from, VertexControl to)
        {
            var e = new EdgeControl(rootArea, from, to);
            AddEdge(e);
            return e;
        }

        #endregion

        #endregion

        public void GenerateVertexLabel(IVertexElement v)
        {
            var label = new LabelElement(rootArea);
            label.Attach(v);                       
            label.UpdatePosition();
        }

        public void GenerateEdgeLabel(IEdgeElement e)
        {
            var label = new LabelElement(rootArea);
            label.Attach(e);                       
            label.UpdatePosition();
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