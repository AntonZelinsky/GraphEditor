using System.Linq;
using System.Collections.Generic;            
using System.Runtime.Serialization;

namespace GraphEditor.Models
{
    [DataContract]
    public class GraphModelSerialization 
    {
        [DataMember]
        public List<Vertex> Verticies {
            get
            {
                if (_model == null)
                    return _verticies;
                return _model.Verticies.Values.ToList();
            }
            set
            {
                _verticies = value;  
            }
        }   

        private List<Vertex> _verticies;

        [DataMember]
        public List<Edge> Edges {
            get
            {
                if (_model == null)
                    return _edges;
                return _model.Edges.Values.ToList();
            }
            set
            {
                _edges = value;
                
            }
        } 
        private List<Edge> _edges;

        private readonly GraphModel _model;

        public bool Changed { get; set; }  
              
        public string FileName {
            get
            {
                if (_model == null)
                    return _fileName;
                return _model.FileName;
            }
            set
            {
                if (_model != null)
                    _model.FileName = value;
                _fileName = value;  
            }
        }

        private string _fileName;

        public GraphModelSerialization()
        {                    
        }

        public GraphModelSerialization(GraphModel model)
        {
            _model = model;
        }    
    }
}