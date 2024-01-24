namespace BO;

/// <summary>
/// Milestones that exist
/// </summary>
public class MilestoneInList
{
    public string Name { get; init; }
    public string? Description { get; set; }
    public DateTime? CreatedAtDate { get; init; }
    public Status Status { get; set; }
    public double CompletionPercentage { get; set; }

    /// <summary>
    /// Constructor for the MilestoneInList class.
    /// </summary>
    /// <param name="name">The name of the milestone in the list.</param>
    /// <param name="description">The description of the milestone in the list (can be null).</param>
    /// <param name="createdAtDate">The date when the milestone in the list was created.</param>
    /// <param name="status">The status of the milestone in the list.</param>
    /// <param name="completionPercentage">The completion percentage of the milestone in the list.</param>
    public MilestoneInList(
        string name,
        string? description,
        DateTime? createdAtDate,
        Status status,
        double completionPercentage)
    {
        Name = name;
        Description = description;
        CreatedAtDate = createdAtDate;
        Status = status;
        CompletionPercentage = completionPercentage;
    }
    public override string ToString() => this.ToStringProperty();
}
