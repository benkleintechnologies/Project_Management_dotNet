namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

internal class TaskImplementation : ITask
{
    public int Create(Task item)
    {
        int id = DataSource.Config.NextTaskId;
        Task task = new Task(id, item._alias, item._description, item._createdAtDate, item._requiredEffortTime, item._isMilestone, item._complexity, item._startDate, item._scheduledDate, item._deadlineDate, item._completeDate, item._deliverables, item._remarks, item._engineerID);
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
            //May need to check that Task is not read only (or something liek that)
            DataSource.Tasks.Remove(task);
        }
    }

    public Task? Read(int id)
    {
        foreach (Task item in DataSource.Tasks)
        {
            if (item._id.== id)
            {
                return item;
            }
        }
        return null;
    }

    public List<Task> ReadAll()
    {
        return new List<Task>(DataSource.Tasks);
    }

    public void Update(Task item)
    {
        Task old = Read(item._id);
        if (old != null)
        {
            DataSource.Tasks.Remove(old);
            DataSource.Tasks.Add(item);
        }
        else
        {
            throw new Exception($"Object of type Task with identifier {item._id} does not exist");
        }
    }
}
