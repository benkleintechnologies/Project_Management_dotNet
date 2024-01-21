namespace BO;

/// <summary>
/// The task that the engineer needs to complete
/// </summary>
public class TaskInEngineer
{
    public int Id { get; init; }
    public string? Alias { get; set; }
    public override string ToString() => this.ToStringProperty();
}
