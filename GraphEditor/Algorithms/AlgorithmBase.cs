using System.Windows.Media;    
using System.Collections.Generic;       

namespace GraphEditor.Algorithms
{
    public abstract class AlgorithmBase : IAlgorithm
    {
        public virtual int? SourceId { get; set; }

        public virtual int? TargetId { get; set; }
        
        public abstract void Compute();

        public LinkedList<HistoryItemAlgorithm> History { get; set; }

        protected virtual void ChangeColor(int idElement, Color color)
        {
            History.AddLast(new HistoryItemAlgorithm(idElement, color));
        }

        public HashSet<int> Visited { get; set; }

        public void Clear(bool full)
        {
            Visited.Clear();
            History.Clear();
            if (full)
            {
                SourceId = null;
                TargetId = null;
            }
        }

        protected AlgorithmBase()
        {
            Visited = new HashSet<int>(); 
            History = new LinkedList<HistoryItemAlgorithm>();
        }

    }
}