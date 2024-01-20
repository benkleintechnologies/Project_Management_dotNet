namespace DO;

/// <summary>
/// Dependency Entity (PDS)
/// </summary>
/// <param name="id"></param>
/// <param name="dependentTask"></param>
/// <param name="dependsOnTask"></param>
public record Dependency
(
	int id,
	int dependentTask,
	int dependsOnTask,
    bool active = true
)
{
    public Dependency() : this(0,0,0) { } // empty ctor for stage 3
}