namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

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
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Task with this ID in the database</exception>
    public void Delete(int id)
    {
        Task? _task = Read(id);
        if (_task == null)
        {
            throw new DalDoesNotExistException($"Object of type Task with identifier {id} does not exist");
        }
        else
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
    public Task? Read(int id)
    {
        return DataSource.Tasks.FirstOrDefault(item => item.id == id && item.active);
    }

    /// <summary>
    /// Retrieve an Engineer from the database based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    public Task? Read(Func<Task, bool> filter)
    {
        return DataSource.Tasks.Where(item => item.active).FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieve all Tasks from the database
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Tasks</returns>
    public IEnumerable<Task> ReadAll(Func<Task, bool>? filter = null)
    {
        if (filter == null)
        {
            return DataSource.Tasks.Where(item => item.active);
        }
        return DataSource.Tasks.Where(filter).Where(item => item.active);
    }

    /// <summary>
    /// Updates a Task in the database
    /// </summary>
    /// <param name="item">New Task information</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no Task with the same ID exists</exception>
    public void Update(Task item)
    {
        Task? _old = Read(item.id);
        if (_old != null)
        {
            DataSource.Tasks.Remove(_old);
            DataSource.Tasks.Add(item);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Task with identifier {item.id} does not exist");
        }
    }

    /// <summary>
    /// Reset Enumerable of Tasks in the database
    /// </summary>
    public void Reset()
    {
        DataSource.Tasks.Clear();
    }
}
