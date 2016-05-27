using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GraphEditor.Helper;
using GraphEditor.Models;

namespace GraphEditor.Algorithms.Search
{
    public class BreadthFirstSearchAlgorithm : AlgorithmBase
    {
        private readonly GraphModel _model;
        private readonly Dictionary<int, int> _path;

        public BreadthFirstSearchAlgorithm(GraphModel model)
        {
            _model = model;
            _path = new Dictionary<int, int>();
        }

        public override int? SourceId { get; set; }
        public override int? TargetId { get; set; }

        public override void Compute()
        {
            Search(SourceId.Value);
        }

        private void Search(int root)
        {
            var queue = new Queue<int>();
            ChangeColor(root, Colors.Red);
            queue.Enqueue(root);
            Visited.Add(root);
            while (queue.Count > 0)
            {
                if (queue.ToList().Exists(d => d == TargetId))
                {
                    ChangeColor(TargetId.Value, Colors.Red);
                    BuildPath();
                    return;
                }
                var v1 = queue.Dequeue();
                foreach (var v2 in _model.GetAdjacentVerticies(v1))
                {
                    if (!Visited.Contains(v2))
                    {
                        ChangeColor(HashCode.GetHashCode(v1, v2), Colors.CornflowerBlue);
                        ChangeColor(v2, Colors.CornflowerBlue);
                        queue.Enqueue(v2);
                        Visited.Add(v2);
                        _path[v2] = v1;
                        if (v2 == TargetId)
                            break;
                    }
                }
            }
        }

        private void BuildPath()
        {
            for (var v = TargetId.Value; _path.ContainsKey(v); v = _path[v])
            {
                if (v != TargetId.Value)
                    ChangeColor(v, Colors.Gold);
                ChangeColor(HashCode.GetHashCode(v, _path[v]), Colors.Gold);
            }
        }
    }
}