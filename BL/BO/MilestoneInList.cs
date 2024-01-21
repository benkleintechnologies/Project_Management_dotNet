namespace BO;

public class MilestoneInList
{
    public string Name { get; init; }
    public string? Description { get; set; }
    public DateTime? CreatedAtDate { get; init; }
    public Status Status { get; set; }
    public double CompletionPercentage { get; set; }
}
