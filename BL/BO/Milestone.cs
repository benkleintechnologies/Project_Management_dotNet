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
    List<TaskInList>? Dependencies { get; set; }
}
