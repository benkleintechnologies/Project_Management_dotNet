namespace Dal;

/// <summary>
/// Class which acts as our databse, storing the lists on each entity
/// </summary>
internal static class DataSource
{
	/// <summary>
	/// Internal Config class which stores project start and end dates, as well as auto-generating values for IDs
	/// </summary>
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
	internal static List<DO.Task> Tasks { get; } = new();
}
