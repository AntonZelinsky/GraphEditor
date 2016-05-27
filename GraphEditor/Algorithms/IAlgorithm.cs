using System.Collections.Generic;

namespace GraphEditor.Algorithms
{
    public interface IAlgorithm
    {
        LinkedList<HistoryItemAlgorithm> History { get; set; }
        HashSet<int> Visited { get; set; }
        int? SourceId { get; set; }
        int? TargetId { get; set; }
        void Compute();
        void Clear(bool full);
    }
}