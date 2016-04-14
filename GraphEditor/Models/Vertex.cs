using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;

namespace GraphEditor.Models
{
    [DataContract]
    public class Vertex : IElement
    {
        [DataMember]
        public int Id
        {
            get
            {
                if (!_id.HasValue)
                    _id = GetHashCode();
                return _id.Value;
            }
            set
            {
                if (!_id.HasValue)
                    _id = value;
            }
        }
        private int? _id;

        [DataMember]
        public List<int> EdgesId;
         
        [DataMember]
        public string LabelName { get; set; }

        [DataMember]
        public double PositionX { get; set; }

        [DataMember]
        public double PositionY { get; set; }    
        
        public Point Position => new Point(PositionX, PositionY);

        public Vertex() : this(new Point()) { }
        public Vertex(Point p) : this((int)p.X, (int)p.Y) { }

        public Vertex( int x, int y)
        {           
            PositionX = x;
            PositionY = y;
                                     
            EdgesId = new List<int>();
        }
    }
}
