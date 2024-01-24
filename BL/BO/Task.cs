namespace BO;

/// <summary>
/// Task Entity
/// </summary>
public class Task
{
    public int Id { get; init; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }
    public List<TaskInList>? Dependencies { get; set; }
    public MilestoneInTask? Milestone { get; set; }
    public DateTime? CreatedAtDate { get; init; }
    public DateTime? ProjectedStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ProjectedEndDate { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ActualEndDate { get; set;}
    public TimeSpan? RequiredEffortTime { get; set; }
    public string? Deliverables { get; set; }
    public string? Notes { get; set; }
    public EngineerInTask? Engineer { get; set; }
    public EngineerExperience Complexity { get; set; }

    /// <summary>
    /// Constructor for the Task class.
    /// </summary>
    /// <param name="id">The unique identifier for the task.</param>
    /// <param name="name">The name of the task (can be null).</param>
    /// <param name="description">The description of the task (can be null).</param>
    /// <param name="status">The status of the task.</param>
    /// <param name="dependencies">List of dependencies for the task (can be null).</param>
    /// <param name="milestone">The milestone associated with the task (can be null).</param>
    /// <param name="createdAtDate">The date when the task was created.</param>
    /// <param name="projectedStartDate">The projected start date for the task (can be null).</param>
    /// <param name="actualStartDate">The actual start date for the task (can be null).</param>
    /// <param name="projectedEndDate">The projected end date for the task (can be null).</param>
    /// <param name="deadline">The deadline for the task (can be null).</param>
    /// <param name="actualEndDate">The actual end date for the task (can be null).</param>
    /// <param name="requiredEffortTime">The required effort time for the task (can be null).</param>
    /// <param name="deliverables">The deliverables associated with the task (can be null).</param>
    /// <param name="notes">Additional notes for the task (can be null).</param>
    /// <param name="engineer">The engineer associated with the task (can be null).</param>
    /// <param name="complexity">The complexity of the task.</param>
    public Task(
        int id,
        string? name,
        string? description,
        Status status,
        List<TaskInList>? dependencies,
        MilestoneInTask? milestone,
        DateTime? createdAtDate,
        DateTime? projectedStartDate,
        DateTime? actualStartDate,
        DateTime? projectedEndDate,
        DateTime? deadline,
        DateTime? actualEndDate,
        TimeSpan? requiredEffortTime,
        string? deliverables,
        string? notes,
        EngineerInTask? engineer,
        EngineerExperience complexity)
    {
        Id = id;
        Name = name;
        Description = description;
        Status = status;
        Dependencies = dependencies;
        Milestone = milestone;
        CreatedAtDate = createdAtDate;
        ProjectedStartDate = projectedStartDate;
        ActualStartDate = actualStartDate;
        ProjectedEndDate = projectedEndDate;
        Deadline = deadline;
        ActualEndDate = actualEndDate;
        RequiredEffortTime = requiredEffortTime;
        Deliverables = deliverables;
        Notes = notes;
        Engineer = engineer;
        Complexity = complexity;
    }
    public override string ToString() => this.ToStringProperty();

}
