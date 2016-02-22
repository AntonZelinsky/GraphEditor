using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;  
using NGraph.Models;

namespace Graph.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var g = new NGraph.Models.Graph();
            var v1 = new Vertex("1");
            var v2 = new Vertex("2");
            var v3 = new Vertex("3");
            g.AddVertex(v1);
            g.AddVertex(v2);
            g.AddVertex(v3);
            g.AddEdge(new Edge(v1, v2));

            g.AddEdge(new Edge(v1, v3));
            //g.AddEdge
        }
    }
}
