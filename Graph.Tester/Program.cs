using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
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
            var e2 = new Edge(v1, v2);
            g.AddVertex(v1);
            g.AddVertex(v2);    
            g.AddVertex(v3); 
            g.AddEdge(v1, v3); // TODO: Нет проверки при добавлении дуги на наличие вершины      

            g.AddEdge(e2);
            v1.HasEdge(e2);
            g.RemoveVertex(v2);

            g.RemoveEdge(e2);
            g.RemoveEdge(v1,v3);


            Console.WriteLine(g);
            foreach (var v in g.GetVerticies())
            {
                Console.WriteLine(v);         
            }

            foreach (var e in g.GetEdges())
            {
                Console.WriteLine(e);        
            }                    
        }
    }
}
