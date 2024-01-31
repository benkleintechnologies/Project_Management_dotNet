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
        int id = Config.NextTaskId;
        Task task = item with { ID = id };
        List<Task> tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        tasks.Add(task);
        XMLTools.SaveListToXMLSerializer<Task>(tasks, s_tasks_xml);
        return id;
    }

    /// <summary>
    /// Delete a Task from the XML File
    /// </summary>
    /// <param name="id">ID of the Task to delete</param>
    public void Delete(int id)
    {
        Task task = Read(id);
        if (task is not null)
        {
            List<Task> tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
            tasks.Remove(task);
            Task newTask = task with { Active = false };
            tasks.Add(newTask);
            XMLTools.SaveListToXMLSerializer<Task>(tasks, s_tasks_xml);
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
        List<Task> tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        Task? task = tasks.FirstOrDefault(item => item.ID == id && item.Active);
        if (task == null)
        {
            throw new DalDoesNotExistException($"Object of type Task with identifier {id} does not exist");
        }
        return task;
    }

    /// <summary>
    /// Retrieve a Task from the XML File based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Task</param>
    /// <returns>The Task object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Task with the same ID and filter exists</exception>
    public Task Read(Func<Task, bool> filter)
    {
        List<Task> tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        Task? task = tasks.Where(item => item.Active).FirstOrDefault(filter);
        if (task == null)
        {
            throw new DalDoesNotExistException($"Object of type Task with given filter does not exist");
        }
        return task;
    }

    /// <summary>
    /// Retrieve all Tasks from the XML File
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Tasks</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Tasks with this filter exist</exception>
    public IEnumerable<Task?> ReadAll(Func<Task, bool>? filter = null)
    {
        IEnumerable<Task> tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        IEnumerable<Task> activeTasks = tasks.Where(item => item.Active);
        if (activeTasks.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Task exists");
        }
        if (filter == null)
        {
            return activeTasks;
        }
        
        IEnumerable<Task> filteredTasks = activeTasks.Where(filter);
        if (filteredTasks.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Task exists");
        }
        return filteredTasks;
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
        Task old = Read(item.ID);
        List<Task> tasks = XMLTools.LoadListFromXMLSerializer<Task>(s_tasks_xml);
        tasks.Remove(old);
        tasks.Add(item);
        XMLTools.SaveListToXMLSerializer<Task>(tasks, s_tasks_xml);
    }
}
