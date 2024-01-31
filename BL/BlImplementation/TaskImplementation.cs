namespace BlImplementation;
using System;
using System.Collections.Generic;
using BlApi;

internal class TaskImplementation : ITask
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    ///  -- uses Linq and select new
    /// </summary>
    /// <param name="task"></param>
    /// <exception cref="BO.BlInvalidInputException"></exception>
    /// <exception cref="BO.BlAlreadyExistsException"></exception>
    public void AddTask(BO.Task task)
    {
        try
        {
            //Check for invalid data
            if (task.Id <= 0 || task.Name == "")
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Task with id {task.Id} was invalid");
            }

            // Getting all previous tasks to add into Dependencies
            IEnumerable<DO.Task> previousTasks = _dal.Task.ReadAll(t => t.projectedStartDate < task.ProjectedStartDate); 

            // Make the new task dependent on all previous tasks
            IEnumerable<DO.Dependency> dependencies = from t in previousTasks 
                                                  select new DO.Dependency(0, task.Id, t.id);
            dependencies.ToList().ForEach(d => _dal.Dependency.Create(d));


            //Try to add the Task to the data layer
            DO.Task newTask = new(task.Id, false,(DO.EngineerExperience)task.Complexity, task.Engineer?.Id, task.Name, task.Description, task.Deliverables, task.Notes, task.CreatedAtDate, task.ProjectedStartDate, task.ActualStartDate, task.RequiredEffortTime, task.Deadline, task.ActualEndDate);

            _dal.Task.Create(newTask);
        }
        catch (DO.DalAlreadyExistsException exc)
        {
            throw new BO.BlAlreadyExistsException(exc.Message);
        }
    }

    /// <summary>
    /// -- Uses nothing
    /// </summary>
    /// <param name="id"></param>
    /// <exception cref="BO.BlCannotBeDeletedException"></exception>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public void DeleteTask(int id)
    {
        try
        {
            //Check that this task can be deleted - i.e. Not depended on by another Task. Otherwise throw error
            if (_dal.Dependency.ReadAll(d => d.dependsOnTask == id) is not null)
            {
                throw new BO.BlCannotBeDeletedException($"Task with id {id} cannot be deleted because another task depends on it.");
            }

            //Delete from the Data Layer
            _dal.Dependency.Delete(id);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    /// <summary>
    /// -- Uses extension
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public IEnumerable<BO.Task> GetListOfTasks(Func<BO.Task, bool>? filter = null)
    {
        try
        {
            //Get all Tasks from the DL
            IEnumerable<DO.Task> tasks = _dal.Task.ReadAll();
            //Filter the DL objects based on the filter
            IEnumerable<DO.Task> filteredDlTasks = filter != null ? tasks.Where(e => filter(toBlTask(e))) : tasks;
            //Return the list of BL type Tasks
            return filteredDlTasks.Select(toBlTask);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    /// <summary>
    /// -- Uses nothing
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlInvalidInputException"></exception>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public BO.Task GetTask(int id)
    {
        try
        {
            //Check for invalid data
            if (id <= 0)
            {
                throw new BO.BlInvalidInputException($"The id {id} was invalid");
            }
            // Get the Task from the Data Layer, convert to BL Task and return it
            return toBlTask(_dal.Task.Read(id));
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    /// <summary>
    /// -- Uses nothing
    /// </summary>
    /// <param name="task"></param>
    /// <exception cref="BO.BlInvalidInputException"></exception>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public void UpdateTask(BO.Task task)
    {
        try
        {
            DO.Task dlTask = _dal.Task.Read(task.Id);
            
            // Check if task exists in the DL and that name is nonempty
            if (dlTask is not null || task.Name == "")
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Task with id {task.Id} was invalid");
            }
            
            // Create the updated task to update the existing task in the Data Layer with
            DO.Task newTask = new(task.Id, false, (DO.EngineerExperience)task.Complexity, task.Engineer?.Id, task.Name, task.Description, task.Deliverables, task.Notes, task.CreatedAtDate, task.ProjectedStartDate, task.ActualStartDate, task.RequiredEffortTime, task.Deadline, task.ActualEndDate);

            // After creating a schedule, it is possible to modify the
            // textual fields and the assigned engineer for the task
            // Not sure what this wants... what is the textual field, and do we just randomly change the engineer?

            _dal.Task.Update(newTask);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    /// <summary>
    /// -- Uses extension
    /// </summary>
    /// <param name="id"></param>
    /// <param name="startDate"></param>
    /// <exception cref="BO.BlNullPropertyException"></exception>
    /// <exception cref="BO.BlInvalidInputException"></exception>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public void UpdateTaskStartDate(int id, DateTime startDate)
    {
        try
        {
            DO.Task task = _dal.Task.Read(id);

            // Getting all previous tasks on which _task depends directly or indirectly
            IEnumerable<DO.Task> dependentTasks = getDependentTasks(task.id);

            // Check if all the projected start dates of the previous tasks are already updated (exist)
            if (dependentTasks.Any(t => t.projectedStartDate == null))
            {
                throw new BO.BlNullPropertyException("One or more of the previous tasks projected start date is null");
            }

            // Ensure that startDate isn't earlier than any of the projected end dates of previous tasks
            if (dependentTasks.Any(t => startDate < t.deadline))
            {
                throw new BO.BlInvalidInputException("Cannot make a new task start date before the previous ones finish");
            }

            // Add the updated task -- with the startDate as the projectedStartDate
            DO.Task updatedTask = new(task.id, false, task.degreeOfDifficulty, task.assignedEngineerId, task.nickname, task.description, task.deliverables, task.notes, task.dateCreated, startDate, task.actualStartDate, task.duration, task.deadline, task.actualEndDate);
            _dal.Task.Update(updatedTask);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    //--uses extension
    private BO.Task toBlTask(DO.Task task)
    {
        //Calculate status - may need to update in the future
        BO.Status getStatusForTask(DO.Task t) => t.projectedStartDate is not null ? BO.Status.Scheduled : BO.Status.Unscheduled;

        //Get Dependencies
        IEnumerable<DO.Dependency>? dependencies = _dal.Dependency.ReadAll(d => d.dependentTask == task.id);
        IEnumerable<BO.TaskInList>? dependentTasks = dependencies.Select(dep =>
        {
            DO.Task dependsOnTask = _dal.Task.Read(dep.dependsOnTask);

            return new BO.TaskInList(
                id: dep.dependsOnTask,
                name: dependsOnTask.nickname,
                description: dependsOnTask.description,
                status: getStatusForTask(dependsOnTask)
            );
        });


        //Find the connected Milestone
        BO.MilestoneInTask? milestone = null;
        //TODO

        //Calculate Projected End Date based on max of projectedStartDate and actualStartDate plus the duration
        DateTime? projectedEndDate = null;
        if (task.projectedStartDate is not null && task.duration is not null)
        {
            if (task.actualStartDate is not null)
            {
                projectedEndDate = (task.actualStartDate > task.projectedStartDate) ? task.actualStartDate.Value.Add(task.duration.Value) : task.projectedStartDate.Value.Add(task.duration.Value);
            }
            else
            {
                projectedEndDate = task.projectedStartDate.Value.Add(task.duration.Value);
            }
        }

        //Create engineerInTask
        BO.EngineerInTask? assignedEngineer = task.assignedEngineerId is not null ? new BO.EngineerInTask(task.assignedEngineerId.Value, _dal.Engineer.Read(task.assignedEngineerId.Value).name) : null;

        //Make a BL type Task
        BO.Task blTask = new(task.id, task.nickname, task.description, getStatusForTask(task), dependentTasks, milestone, task.dateCreated, task.projectedStartDate, task.actualStartDate, projectedEndDate, task.deadline, task.actualEndDate, task.duration, task.deliverables, task.notes, assignedEngineer, (BO.EngineerExperience)task.degreeOfDifficulty);

        return blTask;
    }

    /// <summary>
    /// Recursive method to get all directly or indirectly dependent tasks
    /// </summary>
    /// <param name="taskId">The ID of the task to find its dependencies</param>
    /// <returns>A collection of the tasks it depends on</returns>
    private IEnumerable<DO.Task> getDependentTasks(int taskId)
    {
        // Get direct dependencies of the current task
        IEnumerable<int> directDependencyIds = _dal.Dependency
            .ReadAll(d => d.dependentTask == taskId)
            .Select(dependency => dependency.dependsOnTask);

        // Recursively get dependent tasks of direct dependencies
        IEnumerable<DO.Task> indirectDependentTasks = directDependencyIds
            .SelectMany(getDependentTasks);

        // Combine direct and indirect dependent tasks
        return _dal.Task.ReadAll(t => directDependencyIds.Contains(t.id)).Concat(indirectDependentTasks);
    }
}
