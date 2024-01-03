namespace DO;

/// <summary>
/// Engineer Entity (PDS)
/// </summary>
/// <param name="id"></param>
/// <param name="name"></param>
/// <param name="email"></param>
/// <param name="cost"></param>
/// <param name="level"></param>
public record Engineer
(
	int id,
    String? name = null,
    String? email = null,
	double? cost = null,
	EngineerExperience level = EngineerExperience.Beginner
);
