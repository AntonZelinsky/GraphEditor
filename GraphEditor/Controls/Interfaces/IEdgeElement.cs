namespace GraphEditor.Controls.Interfaces
{
    public interface IEdgeElement : IElement
    {
        /// <summary>
        /// Gets the source vertex
        /// </summary>
        IVertexElement From { get; set; }

        /// <summary>
        /// Gets the target vertex
        /// </summary>
        IVertexElement To { get; set; }
    }
}