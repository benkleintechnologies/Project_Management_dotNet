namespace BO;

/// <summary>
/// Engineer Entity
/// </summary>
public class Engineer
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Email { get; set; }
    public EngineerExperience Experience { get; set; }
    public double Cost { get; set; }
    public TaskInEngineer? Task { get; set; }
    
    /// <summary>
    /// Constructor for the Engineer class.
    /// </summary>
    /// <param name="id">The unique identifier for the engineer.</param>
    /// <param name="name">The name of the engineer.</param>
    /// <param name="email">The email address of the engineer.</param>
    /// <param name="experience">The experience level of the engineer.</param>
    /// <param name="cost">The cost associated with the engineer.</param>
    /// <param name="task">The task assigned to the engineer (can be null).</param>
    public Engineer(int id, string name, string email, EngineerExperience experience, double cost, TaskInEngineer? task)
    {
        Id = id;
        Name = name;
        Email = email;
        Experience = experience;
        Cost = cost;
        Task = task;
    }

    public override string ToString() => this.ToStringProperty();
}
