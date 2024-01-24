namespace BO;

/// <summary>
/// The milestone for the given task
/// </summary>
public class MilestoneInTask
{
    public int Id { get; init; }
    public string? Alias { get; set; }

    /// <summary>
    /// Constructor for the MilestoneInTask class.
    /// </summary>
    /// <param name="id">The unique identifier for the milestone in the task.</param>
    /// <param name="alias">The alias for the milestone in the task (can be null).</param>
    public MilestoneInTask(int id, string? alias)
    {
        Id = id;
        Alias = alias;
    }
    public override string ToString() => this.ToStringProperty();
}
