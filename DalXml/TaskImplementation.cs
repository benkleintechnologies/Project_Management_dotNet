namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using Task = DO.Task;

/// <summary>
/// Implementation of Task Interface to access Tasks from XML
/// </summary>
internal class TaskImplementation : ITask
{
    readonly string s_tasks_xml = "tasks"; //Name of XML file to access

    /// <summary>
    /// Add Task to XML File
    /// </summary>
    /// <param name="item">The Task to add</param>
    /// <returns>The ID of the Task</returns>
    public int Create(Task item)
    {
        int _id = Config.NextTaskId;
        Task _task = item with { id = _id };
        List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        _tasks.Add(_task);
        XMLTools.SaveListToXMLSerializer<Task>(_tasks, s_tasks_xml);
        return _id;
    }

    /// <summary>
    /// Delete a Task from the XML File
    /// </summary>
    /// <param name="id">ID of the Task to delete</param>
    public void Delete(int id)
    {
        Task _task = Read(id);
        if (_task is not null)
        {
            List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
            _tasks.Remove(_task);
            Task _newTask = _task with { active = false };
            _tasks.Add(_newTask);
            XMLTools.SaveListToXMLSerializer<Task>(_tasks, s_tasks_xml);
        }
    }

    /// <summary>
    /// Retrieve a Task from the XML File by ID
    /// </summary>
    /// <param name="id">ID of the Task</param>
    /// <returns>The Task object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Task with the same ID exists</exception>
    public Task Read(int id)
    {
        List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        Task? _task = _tasks.FirstOrDefault(item => item.id == id && item.active);
        if (_task == null)
        {
            throw new DalDoesNotExistException($"Object of type Task with identifier {id} does not exist");
        }
        return _task;
    }

    /// <summary>
    /// Retrieve a Task from the XML File based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Task</param>
    /// <returns>The Task object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Task with the same ID and filter exists</exception>
    public Task Read(Func<Task, bool> filter)
    {
        List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        Task? _task = _tasks.Where(item => item.active).FirstOrDefault(filter);
        if (_task == null)
        {
            throw new DalDoesNotExistException($"Object of type Task with given filter does not exist");
        }
        return _task;
    }

    /// <summary>
    /// Retrieve all Tasks from the XML File
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Tasks</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Tasks with this filter exist</exception>
    public IEnumerable<Task?> ReadAll(Func<Task, bool>? filter = null)
    {
        IEnumerable<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        IEnumerable<Task> _activeTasks = _tasks.Where(item => item.active);
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
    /// Removes all Tasks from the XML File
    /// </summary>
    public void Reset()
    {
        XMLTools.SaveListToXMLSerializer<Task>(new List<Task>(), s_tasks_xml);
    }

    /// <summary>
    /// Updates a Task in the XML File
    /// </summary>
    /// <param name="item">New Task information</param>
    public void Update(Task item)
    {
        Task _old = Read(item.id);
        List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        _tasks.Remove(_old);
        _tasks.Add(item);
        XMLTools.SaveListToXMLSerializer<Task>(_tasks, s_tasks_xml);
    }
}
