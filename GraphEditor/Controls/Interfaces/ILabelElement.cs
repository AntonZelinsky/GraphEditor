namespace GraphEditor.Controls.Interfaces
{
    /// <summary>
    /// Represents attachable element so it can be automaticaly attached to parent entity
    /// </summary>
    public interface ILabelElement
    {
        string Name { get; set; }

        /// <summary>
        /// Attach element to parent entity
        /// </summary>
        /// <param name="element">Parent entity</param>
        void Attach(IElement element);
        /// <summary>
        /// Detach label from element
        /// </summary>
        void Detach();

        /// <summary>
        /// Automaticaly update vertex label position
        /// </summary>
        void UpdatePosition();

        void Hide();
        void Show();
    }
}