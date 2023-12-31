using System;
using System.Collections.Generic;

namespace Dal;

internal static class DataSource
{
	internal static class Config {
        internal static Status _status { get; set; }
        internal static DateTime _startDate { get; set; }
        internal static DateTime _endDate { get; set; }
    }

	internal static List<DO.Engineer> Engineers { get; } = new();
	internal static List<DO.Dependency> Dependencies { get; } = new();
	internal.static List<DO.Task> Tasks { get; } = new();

	public DataSource()
	{
	}
}
