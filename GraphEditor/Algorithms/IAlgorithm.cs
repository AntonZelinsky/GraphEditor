using System.Collections.Generic;

namespace GraphEditor.Algorithms
{
    public interface IAlgorithm
    {     
        void Compute();

        LinkedList<HistoryItemAlgorithm> History { get; set; }

        HashSet<int> Visited { get; set; }

        int? SourceId { get; set; }

        int? TargetId { get; set; }

        void Clear(bool full);
    }
}