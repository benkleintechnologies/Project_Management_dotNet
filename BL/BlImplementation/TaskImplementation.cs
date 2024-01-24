namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;

internal class TaskImplementation : ITask
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    public void addTask(Task Task)
    {
        throw new NotImplementedException();
    }

    public void deleteTask(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Task> getListOfTasks(Func<Task, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public Task getTask(int id)
    {
        throw new NotImplementedException();
    }

    public void updateTask(Task Task)
    {
        throw new NotImplementedException();
    }
}
