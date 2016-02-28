using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NGraph.Interfaces
{
    public interface IEdge
    {
        /// <summary>
        /// Gets the source vertex
        /// </summary>
        IVertex From { get; }
        /// <summary>
        /// Gets the target vertex
        /// </summary>
        IVertex To { get; }

        bool Oriented { get; set; }
    }
}
