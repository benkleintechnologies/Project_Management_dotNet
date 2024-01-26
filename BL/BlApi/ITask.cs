namespace BlApi;

/// <summary>
/// Interface for Task Entity (BL)
/// </summary>
public interface ITask
{
    /// <summary>
    /// Gets the list of Tasks with optional filter
    /// </summary>
    /// <param name="filter">optional filter</param>
    /// <returns>list of Tasks</returns>
    public IEnumerable<BO.Task> getListOfTasks(Func<BO.Task, bool>? filter = null);

    /// <summary>
    /// Gets the details of a Task
    /// </summary>
    /// <param name="id">id of the desired Task</param>
    /// <returns>the Task instance</returns>
    /// <exception cref="BO.BlDoesNotExistException">when a Task id is not found in the DAL<exception>
    public BO.Task getTask(int id);

    /// <summary>
    /// Creates a new Task
    /// </summary>
    /// <param name="task"></param>
    /// <exception cref="BO.BlAlreadyExistsException">when a Task already exists in the DAL<exception>
    /// <exception cref="BO.BlInvalidInputException">when invalid input for Task<exception>
    public void addTask(BO.Task task);

    /// <summary>
    /// Updating an existing Task
    /// </summary>
    /// <param name="task">the entity Task</param>
    /// <exception cref="BO.BlDoesNotExistException">when a Task id is not found in the DAL<exception>
    /// <exception cref="BO.BlInvalidInputException">when invalid input for Task<exception>
    public void updateTask(BO.Task task);

    /// <summary>
    /// Deleting an existing Task
    /// </summary>
    /// <param name="id">id of the Task to delete</param>
    /// <exception cref="BO.BlDoesNotExistException">when a Task id is not found in the DAL<exception>
    /// <exception cref="BO.BlCannotBeDeletedException">when a Task cannot be deleted from the DAL<exception>
    public void deleteTask(int id);

    /// <summary>
    /// Update the start date of an existing Task
    /// </summary>
    /// <param name="id">if of the Task to update</param>
    /// <param name="startDate">New start date</param>
    /// <exception cref="BO.BlDoesNotExistException">when a Task id is not found in the DAL<exception>
    /// <exception cref="BO.BlInvalidInputException">when invalid input for Task<exception>
    public void updateTaskStartDate(int id, DateTime startDate);
}
