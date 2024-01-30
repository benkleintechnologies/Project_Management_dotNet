namespace BO;

/// <summary>
/// Milestone Entity
/// </summary>
public class Milestone
{
    public int Id { get; init; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAtDate { get; init; }
    public Status Status { get; set; }
    public DateTime? ProjectedEndDate { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public double CompletionPercentage { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<TaskInList>? Dependencies { get; set; }
    /// <summary>
    /// Constructor for the Milestone class.
    /// </summary>
    /// <param name="id">The unique identifier for the milestone.</param>
    /// <param name="name">The name of the milestone (can be null).</param>
    /// <param name="description">The description of the milestone (can be null).</param>
    /// <param name="createdAtDate">The date when the milestone was created.</param>
    /// <param name="status">The status of the milestone.</param>
    /// <param name="projectedEndDate">The projected end date for the milestone (can be null).</param>
    /// <param name="deadline">The deadline for the milestone (can be null).</param>
    /// <param name="actualEndDate">The actual end date for the milestone (can be null).</param>
    /// <param name="completionPercentage">The completion percentage of the milestone.</param>
    /// <param name="notes">Additional notes for the milestone (can be null).</param>
    /// <param name="dependencies">List of dependencies for the milestone (can be null).</param>
    public Milestone(
        int id,
        string? name,
        string? description,
        DateTime? createdAtDate,
        Status status,
        DateTime? projectedEndDate,
        DateTime? deadline,
        DateTime? actualEndDate,
        double completionPercentage,
        string? notes,
        IEnumerable<TaskInList>? dependencies)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAtDate = createdAtDate;
        Status = status;
        ProjectedEndDate = projectedEndDate;
        Deadline = deadline;
        ActualEndDate = actualEndDate;
        CompletionPercentage = completionPercentage;
        Notes = notes;
        Dependencies = dependencies;
    }
    public override string ToString() => this.ToStringProperty();
}
