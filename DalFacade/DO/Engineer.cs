namespace DO;

/// <summary>
/// Engineer entity
/// </summary>

public record Engineer
{
	int _id { get; set; }
	string _email { get; set; }	
	double _cost { get; set; }
	string _name { get; set; }	
	EngineerExperience _level { get; set; }

	public Engineer()
	{
	}
}
