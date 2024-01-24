namespace BO;

/// <summary>
/// Contains the engineer who is completing the task
/// </summary>
public class EngineerInTask
{
    public int Id { get; init; }
    public string? Name { get; set; }

    /// <summary>
    /// Constructor for the EngineerInTask class.
    /// </summary>
    /// <param name="id">The unique identifier for the engineer in the task.</param>
    /// <param name="name">The name of the engineer in the task (can be null).</param>
    public EngineerInTask(int id, string? name)
    {
        Id = id;
        Name = name;
    }
    public override string ToString() => this.ToStringProperty();
}
