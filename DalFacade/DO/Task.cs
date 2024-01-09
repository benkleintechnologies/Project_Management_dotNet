namespace DO;

/// <summary>
/// Task Entity (PDS)
/// </summary>
/// <param name="id"></param>
/// <param name="assignedEngineerId"></param>
/// <param name="isMilestone"></param>
/// <param name="degreeOfDifficulty"></param>
/// <param name="nickname"></param>
/// <param name="description"></param>
/// <param name="deliverables"></param>
/// <param name="notes"></param>
/// <param name="dateCreated"></param>
/// <param name="projectedStartDate"></param>
/// <param name="actualStartDate"></param>
/// <param name="duration"></param>
/// <param name="deadline"></param>
/// <param name="actualEndDate"></param>
public record Task
	(
		int id,
        bool isMilestone = false,
        EngineerExperience degreeOfDifficulty = EngineerExperience.Beginner,
        int? assignedEngineerId = null,
        String? nickname = null,
        String? description = null,
        String? deliverables = null,
        String? notes = null,
        DateTime? dateCreated = null,
        DateTime? projectedStartDate = null,
        DateTime? actualStartDate = null,
        TimeSpan? duration = null,
        DateTime? deadline = null,
        DateTime? actualEndDate = null
    );
