using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGraph.Interfaces;

namespace NGraph.Models
{
    public class Edge : IEdge
    {
        private readonly IVertex from;
        private readonly IVertex to;

        public IVertex From => from;

        public IVertex To => to;
        
        public Edge(IVertex from, IVertex to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{from.Name} <=> {to.Name}";
        }
    }
}
