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
        private bool oriented = false;

        public IVertex From
        {
            get { return from; }
        }

        public IVertex To => to;

        public bool Oriented
        {
            get { return oriented; }  
            set { oriented = value; }
        }

        public Edge(IVertex from, IVertex to, bool oriented = false)
        {
            //Contract.Requires(this.source != null);
            //Contract.Requires(this.target != null);
            //Contract.Ensures(this.Source.Equals(this.source));
            //Contract.Ensures(this.Target.Equals(this.target));

            this.from = from;
            this.to = to;
            this.oriented = oriented;
        }         

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{from.Name} -> {to.Name}";
        }
    }
}
