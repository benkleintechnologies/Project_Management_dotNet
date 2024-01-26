namespace BlImplementation;
using BlApi;
using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

internal class TaskImplementation : ITask
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    public void addTask(BO.Task task)
    {
        try
        {
            //Check for invalid data
            if (task.Id <= 0 || task.Name == "")
            {
                throw new BlInvalidInputException($"One of the fields of the Task with id {task.Id} was invalid");
            }

            // Getting all previous tasks to add into Dependencies
            IEnumerable<DO.Task> _previousTasks = _dal.Task.ReadAll(t => t.projectedStartDate < task.ProjectedStartDate); 

            // Make the new task dependent on all previous tasks
            IEnumerable<Dependency> _dependencies = from t in _previousTasks 
                                                  select new Dependency(0, task.Id, t.id);
            _dependencies.ToList().ForEach(d => _dal.Dependency.Create(d));


            //Try to add the Task to the data layer
            DO.Task _newTask = new(task.Id, false,(DO.EngineerExperience)task.Complexity, task.Engineer?.Id, task.Name, task.Description, task.Deliverables, task.Notes, task.CreatedAtDate, task.ProjectedStartDate, task.ActualStartDate, task.RequiredEffortTime, task.Deadline, task.ActualEndDate);

            _dal.Task.Create(_newTask);
        }
        catch (DalAlreadyExistsException exc)
        {
            throw new BlAlreadyExistsException(exc.Message);
        }
    }

    public void deleteTask(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.Task> getListOfTasks(Func<BO.Task, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public BO.Task getTask(int id)
    {
        throw new NotImplementedException();
    }

    public void updateTask(BO.Task Task)
    {
        throw new NotImplementedException();
    }
}
