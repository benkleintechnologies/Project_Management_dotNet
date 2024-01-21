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

}
