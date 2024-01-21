namespace BO;

public class Engineer
{
    public int Id { get; init; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public EngineerExperience Experience { get; set; }
    public double? Cost { get; set; }
    public TaskInEngineer? Task { get; set; }
    public bool active { get; set; }
}
