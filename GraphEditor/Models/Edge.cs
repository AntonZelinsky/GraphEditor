using GraphEditor.Helper;

namespace GraphEditor.Models
{
    public class Edge : IElement
    {
        public int Id => GetHashCode();

        public int FromId { get; }

        public int ToId { get; }

        public string LabelName { get; set; }

        public Edge(int fromId, int toId)
        {           
            FromId = fromId;
            ToId = toId;                    
        }

        public override int GetHashCode()
        {
            return HashCode.GetHashCode(FromId, ToId);
        }
    }
}
