using System;
using System.Linq;   
using System.Windows;
using GraphEditor.View;
using GraphEditor.Controls;    
using System.Collections.Generic;
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

        public List<IElement> AllElements => verticies.Cast<IElement>().Union((edges)).ToList();
      
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
               
        #region IElement Control 

        #region Creating IElement

        public VertexControl CreateVertexControl(Point p)
        {
            var v = new VertexControl(rootArea, p);
            AddVertex(v);           
            return v;
        }

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
            if (createdEdge.From == to)
            {
                UnreleasedEdgeControl();
                return null;
            }
            createdEdge.SetTo(to);
            createdEdge.From.AddEdge(createdEdge);
            createdEdge.To.AddEdge(createdEdge);   
            edges.Add(createdEdge);  
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

        #region Remove IElement

        public void RemoveElements(List<IElement> elements)
        {
            elements.ForEach(RemoveElement);  
        }

        public void RemoveElement(IElement element)
        {
            if (element is VertexControl) 
                Verticies.Remove(element as IVertexElement);         
            else
                Edges.Remove(element as IEdgeElement);
            element.Destruction();
        }

        #endregion

        #endregion

        #region Label Control

        public void CreateElementLabel(IElement v, string name)
        {
            var label = new LabelElement(rootArea, name);
            label.Attach(v);                       
            label.UpdatePosition();
        }

        public void UpdeteElementLabel(IElement v, string name)
        {
            v.LabelName = name;
        }

        public void RemoveElementLabel(IElement v)
        {
            v.DetachLabel();
        }           

        #endregion

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