using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GraphEditor.Models
{
    public class Vertex : IElement
    {
        public int Id => GetHashCode();
                                 
        public List<int> EdgesId;
         
        public string LabelName { get; set; }

        public double PositionX { get; set; }

        public double PositionY { get; set; }    
        
        public Point Position => new Point(PositionX, PositionY);

        public Vertex(Point p) : this((int)p.X, (int)p.Y) { }

        public Vertex( int x, int y)
        {           
            PositionX = x;
            PositionY = y;
                                     
            EdgesId = new List<int>();
        }
    }
}
