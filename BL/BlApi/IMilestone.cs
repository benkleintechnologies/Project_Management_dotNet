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
    /// <exception cref="BO.BlDoesNotExistException">Thrown when there is no Milestone with this ID in the DAL</exception>
    public BO.Milestone GetMilestone(int id);

    /// <summary>
    /// Updates certain fields of a Milestone object
    /// </summary>
    /// <param name="id">of the Milestone to update</param>
    /// <param name="nickname">(Optional) new nickname</param>
    /// <param name="description">(Optional) new description</param>
    /// <param name="notes">(Optional) new notes</param>
    /// <returns>The updated Milestone Object</returns>
    public BO.Milestone UpdateMilestone(int id, string? nickname, string? description, string? notes);
}
