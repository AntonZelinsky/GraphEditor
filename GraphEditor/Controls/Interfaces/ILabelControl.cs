namespace GraphEditor.Controls.Interfaces
{
    /// <summary>
    /// Represents attachable control so it can be automaticaly attached to parent entity
    /// </summary>
    public interface ILabelControl
    {
        /// <summary>
        /// Attach control to parent entity
        /// </summary>
        /// <param name="control">Parent entity</param>
        void Attach(IElement control);
        /// <summary>
        /// Detach label from control
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