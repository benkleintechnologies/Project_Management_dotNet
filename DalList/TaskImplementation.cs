namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = DO.Task;

/// <summary>
/// Implementation of Task Interface, which implements CRUD methods and Reset
/// </summary>
internal class TaskImplementation : ITask
{
    /// <summary>
    /// Add Task to database
    /// </summary>
    /// <param name="item">The Task to add</param>
    /// <returns>The ID of the Task</returns>
    public int Create(Task item)
    {
        int _id = DataSource.Config.NextTaskId;
        Task _task = item with { id = _id };
        DataSource.Tasks.Add(_task);
        return _id;
    }

    /// <summary>
    /// Delete a Task from the database
    /// </summary>
    /// <param name="id">ID of the Task to delete</param>
    public void Delete(int id)
    {
        Task _task = Read(id);
        if (_task is not null)
        {
            DataSource.Tasks.Remove(_task);
            Task _newTask = _task with { active = false };
            DataSource.Tasks.Add(_newTask);
        }
    }

    /// <summary>
    /// Retrieve a Task from the database by ID
    /// </summary>
    /// <param name="id">ID of the Task</param>
    /// <returns>The Task object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Task with this ID in the database</exception>
    public Task Read(int id)
    {
        Task? _task = DataSource.Tasks.FirstOrDefault(item => item.id == id && item.active);
        if (_task == null)
        {
            throw new DalDoesNotExistException($"Object of type Task with identifier {id} does not exist");
        }
        return _task;
    }

    /// <summary>
    /// Retrieve an Engineer from the database based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Task with this information and filter in the database</exception>
    public Task Read(Func<Task, bool> filter)
    {
        Task? _task = DataSource.Tasks.Where(item => item.active).FirstOrDefault(filter);
        if (_task == null)
        {
            throw new DalDoesNotExistException($"Object of type Task with given filter does not exist");
        }
        return _task;
    }

    /// <summary>
    /// Retrieve all Tasks from the database
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Tasks</returns>
    /// /// <exception cref="DalDoesNotExistException">Thrown if there is no Task with this information and filter in the database</exception>
    public IEnumerable<Task> ReadAll(Func<Task, bool>? filter = null)
    {
        IEnumerable<Task> _activeTasks = DataSource.Tasks.Where(filter).Where(item => item.active);
        if (_activeTasks.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Task exists");
        }
        if (filter == null)
        {
            return _activeTasks;
        }
        IEnumerable<Task> _filteredTasks = _activeTasks.Where(filter);
        if (_filteredTasks.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Task exists");
        }
        return _filteredTasks;
    }

    /// <summary>
    /// Updates a Task in the database
    /// </summary>
    /// <param name="item">New Task information</param>
    public void Update(Task item)
    {
        Task _old = Read(item.id);
        DataSource.Tasks.Remove(_old);
        DataSource.Tasks.Add(item);
    }

    /// <summary>
    /// Reset Enumerable of Tasks in the database
    /// </summary>
    public void Reset()
    {
        DataSource.Tasks.Clear();
    }
}
