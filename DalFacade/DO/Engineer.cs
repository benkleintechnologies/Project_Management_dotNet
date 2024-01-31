namespace DO;

/// <summary>
/// Engineer Entity (PDS)
/// </summary>
/// <param name="ID"></param>
/// <param name="Name"></param>
/// <param name="Email"></param>
/// <param name="Cost"></param>
/// <param name="Level"></param>
public record Engineer
(
	int ID,
    String Name,
    String Email,
	double Cost,
	EngineerExperience Level = EngineerExperience.Beginner,
	bool Active = true
)
{
    public Engineer() : this(0, "", "", 0) { } // empty ctor for stage 3
};
