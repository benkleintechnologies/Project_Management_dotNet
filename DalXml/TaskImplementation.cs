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
            List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
            _tasks.Remove(_task);
            Task _newTask = _task with { active = false };
            _tasks.Add(_task);
            XMLTools.SaveListToXMLSerializer<Task>(_tasks, s_tasks_xml);
        }
    }

    /// <summary>
    /// Retreive a Task from the XML File by ID
    /// </summary>
    /// <param name="id">ID of the Task</param>
    /// <returns>The Task object requested</returns>
    public Task? Read(int id)
    {
        List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        return _tasks.FirstOrDefault(item => item.id == id && item.active);
    }

    /// <summary>
    /// Retreive a Task from the XML File based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Task</param>
    /// <returns>The Task object requested</returns>
    public Task? Read(Func<Task, bool> filter)
    {
        List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        return _tasks.Where(item => item.active).FirstOrDefault(filter);
    }

    /// <summary>
    /// Retreive all Tasks from the XML File
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Tasks</returns>
    public IEnumerable<Task?> ReadAll(Func<Task, bool>? filter = null)
    {
        List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        if (filter == null)
        {
            return _tasks.Where(item => item.active);
        }
        return _tasks.Where(filter).Where(item => item.active);
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
    /// <exception cref="DalDoesNotExistException">Thrown if no Task with the same ID exists</exception>
    public void Update(Task item)
    {
        Task? _old = Read(item.id);
        if (_old != null)
        {
            List<Task> _tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
            _tasks.Remove(_old);
            _tasks.Add(item);
            XMLTools.SaveListToXMLSerializer<Task>(_tasks, s_tasks_xml);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Task with identifier {item.id} does not exist");
        }
    }
}
