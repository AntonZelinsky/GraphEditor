using GraphEditor.Helper;
using System.Runtime.Serialization;  

namespace GraphEditor.Models
{
    [DataContract]
    public class Edge : IElement
    {
        [DataMember]
        public int Id
        {
            get {
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
        public int FromId { get; set; }

        [DataMember]
        public int ToId { get; set; }

        [DataMember]
        public string LabelName { get; set; }
                      
        public Edge() { }

        public Edge(int fromId, int toId)
        {
            FromId = fromId;
            ToId = toId;                    
        }

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
