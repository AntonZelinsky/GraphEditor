using System.Collections.Generic;
using System.Windows.Media;

namespace GraphEditor.Algorithms
{
    public abstract class AlgorithmBase : IAlgorithm
    {
        protected AlgorithmBase()
        {
            Visited = new HashSet<int>();
            History = new LinkedList<HistoryItemAlgorithm>();
        }

        public virtual int? SourceId { get; set; }
        public virtual int? TargetId { get; set; }
        public abstract void Compute();
        public LinkedList<HistoryItemAlgorithm> History { get; set; }
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

        protected virtual void ChangeColor(int idElement, Color color)
        {
            History.AddLast(new HistoryItemAlgorithm(idElement, color));
        }
    }
}