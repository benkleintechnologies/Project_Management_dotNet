namespace Dal;

/// <summary>
/// Class which acts as our database, storing the lists on each entity
/// </summary>
internal static class DataSource
{
	/// <summary>
	/// Internal Config class which stores project start and end dates, as well as auto-generating values for IDs
	/// </summary>
	internal static class Config {
		internal static DateTime? startDate { get; set; } = null;
		internal static DateTime? endDate { get; set; } = null;
        internal static DateTime systemClock { get; set; } = DateTime.Now;
		internal const int startTaskId = 1;
		private static int nextTaskId = startTaskId;
		internal static int NextTaskId { get => nextTaskId++;  }

        internal const int startDependencyId = 1000;
        private static int nextDependencyId = startDependencyId;
        internal static int NextDependencyId { get => nextDependencyId++; }

        // Method to reset the nextTaskId
        internal static void resetTaskId()
        {
            nextTaskId = startTaskId;
        }

        // Method to reset the nextDependencyId
        internal static void resetDependencyId()
        {
            nextDependencyId = startDependencyId;
        }
    }

	internal static List<DO.Engineer> Engineers { get; } = new();
	internal static List<DO.Dependency> Dependencies { get; } = new();
	internal static List<DO.Task> Tasks { get; } = new();
}
