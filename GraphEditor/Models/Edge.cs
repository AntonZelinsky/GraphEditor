using System.Runtime.Serialization;
using GraphEditor.Helper;

namespace GraphEditor.Models
{
    [DataContract]
    public class Edge : IElement
    {
        private int? _id;

        public Edge()
        {
        }

        public Edge(int fromId, int toId)
        {
            FromId = fromId;
            ToId = toId;
        }

        [DataMember]
        public int FromId { get; set; }

        [DataMember]
        public int ToId { get; set; }

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

        public int OtherId(int id)
        {
            return FromId == id ? ToId : ToId == id ? FromId : 0;
        }

        public override int GetHashCode()
        {
            return HashCode.GetHashCode(FromId, ToId);
        }
    }
}