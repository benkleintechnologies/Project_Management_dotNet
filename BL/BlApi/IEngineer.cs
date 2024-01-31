namespace BlApi;

/// <summary>
/// Interface for Business Layer methods of Engineer Entity
/// </summary>
public interface IEngineer
{
    /// <summary>
    /// An engineer list request (administrator’s view) from the DL
    /// </summary>
    /// <param name="filter">e.g. by the level of experience</param>
    /// <returns>the list object of the engineers found</returns>
    public IEnumerable<BO.Engineer> GetListOfEngineers(Func<BO.Engineer, bool>? filter = null);

    /// <summary>
    /// Get an engineer's details - creates a logic entity of type Engineer and dynamically backfills additional information (engineer details and current task)
    /// </summary>
    /// <param name="id">of the Engineer</param>
    /// <returns>The engineer detail instance</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when there is no Engineer with this ID in the DAL</exception>
    public BO.Engineer GetEngineer(int id);

    /// <summary>
    /// Add Engineer - checks for: (a) identifier’s validity (is it positive?), (b) engineer’s name (a non-empty string), (c) cost(is it positive?), (d) email address(pattern)
    /// if the information is valid, runs a request for engineer creation to the DL
    /// </summary>
    /// <param name="engineer"></param>
    /// <exception cref="BO.BlAlreadyExistsException"> thrown when the identifier already exists in the DL</exception>
    /// <exception cref="BO.BlInvalidInputException"> thrown when engineer information is invalid or missing</exception>
    public void AddEngineer(BO.Engineer engineer);

    /// <summary>
    /// Delete Engineer - checks that the engineer is neither currently assigned to any task, nor was he assigned a task in the past.
    /// </summary>
    /// <param name="id">of the Engineer to delete</param>
    /// <exception cref="BO.BlDoesNotExistException"> thrown when when the identifier is not found in the DL</exception>
    /// <exception cref="BO.BlCannotBeDeletedException"> thrown when the engineer appears as assigned or completed one of the tasks at the very least</exception>
    public void DeleteEngineer(int id);

    /// <summary>
    /// Update Engineer - checks for: (a) identifier’s validity (is it positive?), (b) engineer name (a non-empty string), (c) cost(is it positive?), (d) email address(pattern)
    /// </summary>
    /// <param name="engineer">- the engineer instance that contains the update</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when there is no Engineer with this ID in the DAL</exception>
    /// <exception cref="BO.BlInvalidInputException"> thrown when engineer information is invalid or missing</exception>
    public void UpdateEngineer(BO.Engineer engineer);


}
