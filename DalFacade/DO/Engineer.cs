namespace DO;

/// <summary>
/// Engineer entity
/// </summary>

public record Engineer
(
	int id,
    string name,
    string? email = null,
	double? cost = null,
	EngineerExperience level = EngineerExperience.Beginner
);
