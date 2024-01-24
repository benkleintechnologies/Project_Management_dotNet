namespace BO;

/// <summary>
/// The list of tasks that the entity depends on
/// </summary>
public class TaskInList
{
    public int Id { get; init; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }

    /// <summary>
    /// Constructor for the TaskInList class.
    /// </summary>
    /// <param name="id">The unique identifier for the task in the list.</param>
    /// <param name="name">The name of the task in the list (can be null).</param>
    /// <param name="description">The description of the task in the list (can be null).</param>
    /// <param name="status">The status of the task in the list.</param>
    public TaskInList(int id, string? name, string? description, Status status)
    {
        Id = id;
        Name = name;
        Description = description;
        Status = status;
    }

    public override string ToString() => this.ToStringProperty();
}
