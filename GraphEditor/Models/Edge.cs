using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int salt = 100;   
            unchecked
            {
                return FromId * ToId + salt;
            } 
        }
    }
}
