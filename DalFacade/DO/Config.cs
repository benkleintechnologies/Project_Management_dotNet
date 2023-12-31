using System;
using System.Diagnostics.Eventing.Reader;

namespace DO;

/// <summary>
/// Config entity
/// </summary>
public record Config
{
	Status _status { get; set; }
	DateTime _startDate { get; set; }
	DateTime _endDate { get; set; }

	public Config()
	{
	}
}
