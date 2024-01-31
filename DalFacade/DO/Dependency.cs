namespace DO;

/// <summary>
/// Dependency Entity (PDS)
/// </summary>
/// <param name="ID"></param>
/// <param name="DependentTask"></param>
/// <param name="DependsOnTask"></param>
public record Dependency
(
	int ID,
	int DependentTask,
	int DependsOnTask,
    bool Active = true
)
{
    public Dependency() : this(0,0,0) { } // empty ctor for stage 3
}