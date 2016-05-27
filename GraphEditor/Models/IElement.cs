namespace GraphEditor.Models
{
    public interface IElement
    {
        int Id { get; }
        string LabelName { get; set; }
    }
}