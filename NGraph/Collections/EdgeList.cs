using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGraph.Interfaces;

namespace NGraph.Collections
{
    public sealed class EdgeList : List<IEdge>, ICloneable
    {
        public EdgeList()
        { }

        public EdgeList(int capacity)
            : base(capacity)
        { }

        public EdgeList(EdgeList list)
            : base(list)
        { }
           
        public object Clone()
        {
            return new EdgeList(this);
        }
    }
}
