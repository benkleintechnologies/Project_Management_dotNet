namespace BlApi;

/// <summary>
/// Interface for Milestone entity methods and creating project schedule
/// </summary>
public interface IMilestone
{
    /// <summary>
    /// Create Project Schedule
    /// </summary>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when trying to access an item which does not exist in the DAL</exception>
    /// <exception cref="BO.BlCannotBeDeletedException">Thrown when trying to delete an item which cannot be deleted in the DAL</exception>
    public void CreateProjectSchedule();

    /// <summary>
    /// Get a Milestone object by ID
    /// </summary>
    /// <param name="id">of the Milestone</param>
    /// <returns>The Milestone object</returns>
    /// <exception cref="BO.BlInvalidInputException">Thrown when the ID is invalid</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when there is no Milestone with this ID in the DAL</exception>
    public BO.Milestone GetMilestone(int id);

    /// <summary>
    /// An Milestone list request (administrator’s view)
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>the list object of the milestones found</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when there is no Milestone which matches this filter</exception>
    public IEnumerable<BO.MilestoneInList> GetListOfMilestones(Func<BO.Milestone, bool>? filter = null);

    /// <summary>
    /// Updates certain fields of a Milestone object
    /// </summary>
    /// <param name="id">of the Milestone to update</param>
    /// <param name="nickname">(Optional) new nickname</param>
    /// <param name="description">(Optional) new description</param>
    /// <param name="notes">(Optional) new notes</param>
    /// <returns>The updated Milestone Object</returns>
    /// <exception cref="BO.BlInvalidInputException">Thrown when the ID is invalid</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when there is no Milestone with this ID in the DAL</exception>
    public BO.Milestone UpdateMilestone(int id, string? nickname, string? description, string? notes);

    /// <summary>
    /// Reset Milestones - i.e. Reset Dependencies in Database, and Reset Milestone counter
    /// </summary>
    public void Reset();
}
