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
    public override string ToString() => this.ToStringProperty();
}
