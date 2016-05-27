using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;

namespace GraphEditor.Models
{
    [DataContract]
    public class Vertex : IElement
    {
        private int? _id;

        [DataMember] public List<int> EdgesId;

        public Vertex() : this(new Point())
        {
        }

        public Vertex(Point p) : this((int) p.X, (int) p.Y)
        {
        }

        public Vertex(int x, int y)
        {
            PositionX = x;
            PositionY = y;

            EdgesId = new List<int>();
        }

        [DataMember]
        public double PositionX { get; set; }

        [DataMember]
        public double PositionY { get; set; }

        public Point Position => new Point(PositionX, PositionY);

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

        [DataMember]
        public string LabelName { get; set; }
    }
}