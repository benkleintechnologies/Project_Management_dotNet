namespace DO;

/// <summary>
/// Task Entity (PDS)
/// </summary>
/// <param name="ID"></param>
/// <param name="AssignedEngineerId"></param>
/// <param name="IsMilestone"></param>
/// <param name="DegreeOfDifficulty"></param>
/// <param name="Nickname"></param>
/// <param name="Description"></param>
/// <param name="Deliverables"></param>
/// <param name="Notes"></param>
/// <param name="DateCreated"></param>
/// <param name="ProjectedStartDate"></param>
/// <param name="ActualStartDate"></param>
/// <param name="Duration"></param>
/// <param name="Deadline"></param>
/// <param name="ActualEndDate"></param>
public record Task
(
    int ID,
    bool IsMilestone = false,
    EngineerExperience DegreeOfDifficulty = EngineerExperience.Beginner,
    int? AssignedEngineerId = null,
    String? Nickname = null,
    String? Description = null,
    String? Deliverables = null,
    String? Notes = null,
    DateTime? DateCreated = null,
    DateTime? ProjectedStartDate = null,
    DateTime? ActualStartDate = null,
    TimeSpan? Duration = null,
    DateTime? Deadline = null,
    DateTime? ActualEndDate = null,
    bool Active = true
)
{
    public Task() : this(0) { } // empty ctor for stage 3
};
