namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ITask = BlApi.ITask;

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
        try
        {
            //Check that this task can be deleted - i.e. Not depended on by another Task. Otherwise throw error
            if (_dal.Dependency.ReadAll(d => d.dependsOnTask == id) is not null)
            {
                throw new BlCannotBeDeletedException($"Task with id {id} cannot be deleted because another task depends on it.");
            }

            //Delete from the Data Layer
            _dal.Dependency.Delete(id);
        }
        catch (DalDoesNotExistException exc)
        {
            throw new BlDoesNotExistException(exc.Message);
        }
    }

    public IEnumerable<BO.Task> getListOfTasks(Func<BO.Task, bool>? filter = null)
    {
        try
        {
            //Get all Tasks from the DL
            IEnumerable<DO.Task> _tasks = _dal.Task.ReadAll();
            //Filter the DL objects based on the filter
            IEnumerable<DO.Task> _filteredDlTasks = filter != null ? _tasks.Where(e => filter(toBlTask(e))) : _tasks;
            //Return the list of BL type Tasks
            return _filteredDlTasks.Select(toBlTask);
        }
        catch (DalDoesNotExistException exc)
        {
            throw new BlDoesNotExistException(exc.Message);
        }
    }

    public BO.Task getTask(int id)
    {
        try
        {
            //Check for invalid data
            if (id <= 0)
            {
                throw new BlInvalidInputException($"The id {id} was invalid");
            }
            // Get the Task from the Data Layer, convert to BL Task and return it
            return toBlTask(_dal.Task.Read(id));
        }
        catch (DalDoesNotExistException exc)
        {
            throw new BlDoesNotExistException(exc.Message);
        }
    }

    public void updateTask(BO.Task task)
    {
        try
        {
            DO.Task _dlTask = _dal.Task.Read(task.Id);
            
            // Check if task exists in the DL and that name is nonempty
            if (_dlTask is not null || task.Name == "")
            {
                throw new BlInvalidInputException($"One of the fields of the Task with id {task.Id} was invalid");
            }
            
            // Create the updated task to update the existing task in the Data Layer with
            DO.Task _newTask = new(task.Id, false, (DO.EngineerExperience)task.Complexity, task.Engineer?.Id, task.Name, task.Description, task.Deliverables, task.Notes, task.CreatedAtDate, task.ProjectedStartDate, task.ActualStartDate, task.RequiredEffortTime, task.Deadline, task.ActualEndDate);

            // After creating a schedule, it is possible to modify the
            // textual fields and the assigned engineer for the task
            // Not sure what this wants... what is the textual field, and do we just randomly change the engineer?

            _dal.Task.Update(_newTask);
        }
        catch (DalDoesNotExistException exc)
        {
            throw new BlDoesNotExistException(exc.Message);
        }
    }

    public void updateTaskStartDate(int id, DateTime startDate)
    {
        try
        {
            DO.Task _task = _dal.Task.Read(id);

            // Getting all previous tasks
            IEnumerable<DO.Task> _prevTasks = _dal.Task.ReadAll(t => t.dateCreated < _task.dateCreated);
            
            // Check if all the projected start dates of the previous tasks are already updated (exist)
            if (_prevTasks.Any(t => t.projectedStartDate == null))
            {
                throw new BlNullPropertyException("One or more of the previous tasks projected start date is null");
            }

            // Ensure that startDate isn't earlier than any of the projected end dates of previous tasks
            if (_prevTasks.All(t => startDate < t.deadline))
            {
                throw new BlInvalidInputException("Cannot make a new task start date before the previous ones finish");
            }

            // Add the updated task -- with the startDate as the projectedStartDate
            DO.Task _updatedTask = new(_task.id, false, _task.degreeOfDifficulty, _task.assignedEngineerId, _task.nickname, _task.description, _task.deliverables, _task.notes, _task.dateCreated, startDate, _task.actualStartDate, _task.duration, _task.deadline, _task.actualEndDate);
            _dal.Task.Update(_updatedTask);
        }
        catch (DalDoesNotExistException exc)
        {
            throw new BlDoesNotExistException(exc.Message);
        }
    }

    private BO.Task toBlTask(DO.Task task)
    {
        //Calculate status - may need to update in the future
        Status GetStatusForTask(DO.Task t) => t.projectedStartDate is not null ? Status.Scheduled : Status.Unscheduled;

        //Get Dependencies
        IEnumerable<Dependency>? _dependencies = _dal.Dependency.ReadAll(d => d.dependentTask == task.id);
        IEnumerable<TaskInList>? _dependentTasks = _dependencies.Select(dep =>
        {
            DO.Task _dependsOnTask = _dal.Task.Read(dep.dependsOnTask);

            return new TaskInList(
                id: dep.dependsOnTask,
                name: _dependsOnTask.nickname,
                description: _dependsOnTask.description,
                status: GetStatusForTask(_dependsOnTask)
            );
        });


        //Find the connected Milestone
        MilestoneInTask? _milestone = null;
        //TODO

        //Calculate Projected End Date based on max of projectedStartDate and actualStartDate plus the duration
        DateTime? _projectedEndDate = null;
        if (task.projectedStartDate is not null && task.duration is not null)
        {
            if (task.actualStartDate is not null)
            {
                _projectedEndDate = (task.actualStartDate > task.projectedStartDate) ? task.actualStartDate.Value.Add(task.duration.Value) : task.projectedStartDate.Value.Add(task.duration.Value);
            }
            else
            {
                _projectedEndDate = task.projectedStartDate.Value.Add(task.duration.Value);
            }
        }

        //Create engineerInTask
        EngineerInTask? _assignedEngineer = task.assignedEngineerId is not null ? new EngineerInTask(task.assignedEngineerId.Value, _dal.Engineer.Read(task.assignedEngineerId.Value).name) : null;

        //Make a BL type Task
        BO.Task _blTask = new(task.id, task.nickname, task.description, GetStatusForTask(task), _dependentTasks, _milestone, task.dateCreated, task.projectedStartDate, task.actualStartDate, _projectedEndDate, task.deadline, task.actualEndDate, task.duration, task.deliverables, task.notes, _assignedEngineer, (BO.EngineerExperience)task.degreeOfDifficulty);

        return _blTask;
    }
}
