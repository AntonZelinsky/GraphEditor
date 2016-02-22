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
        private readonly IVertex source;
        private readonly IVertex target;
        private bool oriented = false;
        public IVertex Source
        {
            get { return source; }
        }

        public IVertex Target
        {
            get { return target; }
        }

        public bool Oriented
        {
            get { return oriented; }  
            set { oriented = value; }
        }

        public Edge(IVertex source, IVertex target, bool oriented = false)
        {
            Contract.Requires(this.source != null);
            Contract.Requires(this.target != null);
            Contract.Ensures(this.Source.Equals(this.source));
            Contract.Ensures(this.Target.Equals(this.target));

            this.source = source;
            this.target = target;
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
            return source + "->" + target;
        }
    }
}
