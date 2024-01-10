namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

/// <summary>
/// Implementation of Task Interface, which implements CRUD methods and Reset
/// </summary>
public class TaskImplementation : ITask
{
    public int Create(Task item)
    {
        int id = DataSource.Config.NextTaskId;
        Task task = item with { id = id };
        DataSource.Tasks.Add(task);
        return id;
    }

    public void Delete(int id)
    {
        Task task = Read(id);
        if (task == null)
        {
            throw new Exception($"Object of type Task with identifier {id} does not exist");
        }
        else
        {
            DataSource.Tasks.Remove(task);
            Task _newTask = task with { active = false };
            DataSource.Tasks.Add(_newTask);
        }
    }

    public Task? Read(int id)
    {
        return DataSource.Tasks.Find(item => item.id == id && item.active);
    }

    public List<Task> ReadAll()
    {
        return new List<Task>(DataSource.Tasks.FindAll(item => item.active));
    }

    public void Update(Task item)
    {
        Task old = Read(item.id);
        if (old != null)
        {
            DataSource.Tasks.Remove(old);
            DataSource.Tasks.Add(item);
        }
        else
        {
            throw new Exception($"Object of type Task with identifier {item.id} does not exist");
        }
    }

    public void Reset()
    {
        DataSource.Tasks.Clear();
    }
}
