using System.Windows.Media;
using GraphEditor.Helper;
using GraphEditor.Models;

namespace GraphEditor.Algorithms.Search
{
    public class DepthFirstSearchUndirectedAlgorithm : AlgorithmBase
    {
        private readonly GraphModel _model;

        public DepthFirstSearchUndirectedAlgorithm(GraphModel model)
        {
            _model = model;
        }

        public override int? SourceId { get; set; }
        public override int? TargetId { get; set; }

        public override void Compute()
        {
            ChangeColor(SourceId.Value, Colors.Red);
            Search(SourceId.Value);
        }

        private bool Search(int root)
        {
            if (root == TargetId)
            {
                ChangeColor(root, Colors.Red);
                return true;
            }
            Visited.Add(root);
            if (root != SourceId)
                ChangeColor(root, Colors.Gold);
            foreach (var v in _model.GetAdjacentVerticies(root))
            {
                if (!Visited.Contains(v))
                {
                    ChangeColor(HashCode.GetHashCode(root, v), Colors.Gold);
                    if (Search(v))
                        return true;
                    ChangeColor(HashCode.GetHashCode(root, v), Colors.DarkSlateGray);
                }
            }
            ChangeColor(root, Colors.DarkSlateGray);
            return false;
        }
    }
}