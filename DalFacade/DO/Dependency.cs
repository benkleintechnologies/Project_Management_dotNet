namespace DO;

/// <summary>
/// Dependency entity
/// </summary>

public record Dependency
{
	int _id { get; set; }
	int _dependentTask { get; set; }
	int _dependsOnTask { get; set; }


	public Dependency(int id, int dependentTask, int dependsOntask)
	{
		_id = id;
		_dependentTask = dependentTask;
		_dependsOnTask = dependsOntask;
	}
}
