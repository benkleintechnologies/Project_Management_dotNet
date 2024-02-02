namespace BO;

/// <summary>
/// The task that the engineer needs to complete
/// </summary>
public class TaskInEngineer
{
    public int ID { get; init; }
    public string? Alias { get; set; }

    /// <summary>
    /// Constructor which accepts all fields
    /// </summary>
    /// <param name="id">Of the Task</param>
    /// <param name="alias">of the Task</param>
    public TaskInEngineer(int id, string? alias)
    {
        ID = id;
        Alias = alias;
    }
    public override string ToString() => this.ToStringProperty();
}
