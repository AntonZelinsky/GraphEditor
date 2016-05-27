using System.Windows.Media;

namespace GraphEditor.Algorithms
{
    public struct HistoryItemAlgorithm
    {
        public Color? OldColor;

        public HistoryItemAlgorithm(int id, Color color)
        {
            Id = id;
            Color = color;
            OldColor = null;
        }

        public int Id { get; set; }
        public Color Color { get; set; }
    }
}