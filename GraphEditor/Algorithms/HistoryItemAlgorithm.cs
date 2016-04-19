using System.Windows.Media;

namespace GraphEditor.Algorithms
{
    public struct HistoryItemAlgorithm
    {
        public int Id { get; set; }

        public Color Color { get; set; }

        public Color? OldColor;

        public HistoryItemAlgorithm(int id, Color color)
        {
            Id = id;
            Color = color;
            OldColor = null;
        }
    }
}