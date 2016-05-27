using System;
using System.Collections.Generic;  
using System.Linq;  

namespace GraphEditor.Models
{                
    public class GraphModel
    {              
        public Dictionary<int, Edge> Edges { get; set; }
                     
        public Dictionary<int, Vertex> Verticies { get; set; }  

        /// <summary>
        /// Коллекция дуг принадлежищих вершине
        /// </summary>
        public Dictionary<int, List<int>> VertexOfEdgesById { get; set; }

        public bool Changed { get; set; } 

        public string FileName { get; set; }

        public GraphModel() : this(
            new Dictionary<int, Vertex>(),
            new Dictionary<int, Edge>(),
            new Dictionary<int, List<int>>()) { }
         
        public GraphModel(
            Dictionary<int, Vertex> verticies, 
            Dictionary<int, Edge> edges,
            Dictionary<int, List<int>> veIds)
        {                
            Verticies = verticies;
            Edges = edges;
            VertexOfEdgesById = veIds;
            Changed = true;
        }
                              
        public Vertex GetVertex(int id)
        {
            return Verticies[id];
        }

        public int GetVertexIdByEdge(int edgeId, int vertexId)
        {
            return Edges[edgeId].OtherId(vertexId);
        }

        public List<int> GetAdjacentVerticies(int id)
        {
            var v = new List<int>();
            VertexOfEdgesById[id].ForEach(edge => v.Add(GetVertexIdByEdge(edge, id)));
            return v;
        }
          
        public List<int> GetAdjacentOrientedVerticies(int id)
        {
            var verticies = new List<int>();
            VertexOfEdgesById[id].ForEach(edge => { if(Edges[edge].ToId != id)
                    verticies.Add(Edges[edge].ToId); });
            return verticies;
        }

        public Edge GetEdge(int id)
        {
            return Edges[id];
        }

        public List<int> GetEdgesForVertex(int vertexId)
        {
            return VertexOfEdgesById[vertexId];
        }
        public IElement GetElement(int id)
        {
            if (ContainsEdges(id))
                return Edges[id];
            return Verticies[id];
        }

        public List<IElement> GetAllElements()
        {
            return Verticies.Values.ToList().Cast<IElement>().Union(Edges.Values).ToList();
        }

        public List<int> GetAllElementsById()
        {
            return Verticies.Keys.Union(Edges.Keys).ToList();
        }

        public void AddVertex(Vertex v)
        {
            Verticies.Add(v.Id, v);    
        }

        public void AddEdge(Edge e)
        {
            Edges.Add(e.Id, e);
            AddVertexOfEdgesById(e.FromId, e.Id);
            AddVertexOfEdgesById(e.ToId, e.Id);    

        }

        private void AddVertexOfEdgesById(int vertexId, int edgeId)
        {
            if (VertexOfEdgesById.ContainsKey(vertexId))
                VertexOfEdgesById[vertexId].Add(edgeId);
            else    
                VertexOfEdgesById.Add(vertexId, new List<int> {edgeId});   
        }

        public void RemoveById(int id)
        {             
            if (Verticies.ContainsKey(id))
            {
                Verticies.Remove(id);
                if (VertexOfEdgesById.ContainsKey(id))
                    VertexOfEdgesById.Remove(id);
            }
            if (Edges.ContainsKey(id))
            {
                var e = Edges[id];
                VertexOfEdgesById.Remove(e.FromId);
                VertexOfEdgesById.Remove(e.ToId);
                Edges.Remove(id);
            }     
        }
        
        public bool Contains(int id)
        {
            return GetAllElementsById().Contains(id);
        }

        public bool ContainsVerticies(int id)
        {
            return Verticies.ContainsKey(id);
        }

        public bool ContainsEdges(int id)
        {
            return Edges.ContainsKey(id);
        } 
    }
}
