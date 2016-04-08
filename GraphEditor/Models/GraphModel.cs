using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;    

namespace GraphEditor.Models
{
    [DataContract]
    public class GraphModel
    {
        [DataMember]
        public Dictionary<int, Edge> Edges { get; set; }

        [DataMember]
        public Dictionary<int, Vertex> Verticies { get; set; }  

        public Dictionary<int, List<int>> VertexOfEdgesById { get; set; }

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
        }
              
        public Vertex GetVertex(int id)
        {
            return Verticies[id];
        }

        public Edge GetEdge(int id)
        {
            return Edges[id];
        }

        public IElement GetElement(int id)
        {
            if (ContainsEdges(id))
                return Edges[id];
            return Verticies[id];
        }

        public List<IElement> GetAllElements()
        {
            return Verticies.ToList().Cast<IElement>().Union(Edges.Values).ToList();
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
            if (VertexOfEdgesById.ContainsKey(id)) 
                VertexOfEdgesById.Remove(id);     
            if (Verticies.ContainsKey(id))
                Verticies.Remove(id);
            if (Edges.ContainsKey(id))
                Edges.Remove(id);
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
