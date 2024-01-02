using System;
using System.Collections.Generic;

namespace Dal;

internal static class DataSource
{
	internal static class Config {
		internal static DateTime? _startDate { get; set; } = null;
		internal static DateTime? _endDate { get; set; } = null;

		internal const int _startTaskId = 0;
		private static int _nextTaskId = _startTaskId;
		internal static int NextTaskId { get => _nextTaskId++;  }

        internal const int _startDependencyId = 1000;
        private static int _nextDependencyId = _startDependencyId;
        internal static int NextDependencyId { get => _nextDependencyId++; }
    }

	internal static List<DO.Engineer> Engineers { get; } = new();
	internal static List<DO.Dependency> Dependencies { get; } = new();
	internal.static List<DO.Task> Tasks { get; } = new();

	public DataSource()
	{
	}
}
