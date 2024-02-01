namespace BlImplementation;
using System;
using System.Collections.Generic;
using BlApi;

internal class TaskImplementation : ITask
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

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
            IEnumerable<DO.Task> previousTasks = _dal.Task.ReadAll(t => t.ProjectedStartDate < task.ProjectedStartDate); 

            // Make the new task dependent on all previous tasks
            IEnumerable<DO.Dependency> dependencies = from t in previousTasks 
                                                  select new DO.Dependency(0, task.Id, t.ID);
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

    public void DeleteTask(int id)
    {
        try
        {
            //Check that this task can be deleted - i.e. Not depended on by another Task. Otherwise throw error
            if (_dal.Dependency.ReadAll(d => d.DependsOnTask == id) is not null)
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

    public void UpdateTaskStartDate(int id, DateTime startDate)
    {
        try
        {
            DO.Task task = _dal.Task.Read(id);

            // Getting all previous tasks on which _task depends directly or indirectly
            IEnumerable<DO.Task> dependentTasks = getDependentTasks(task.ID);

            // Check if all the projected start dates of the previous tasks are already updated (exist)
            if (dependentTasks.Any(t => t.ProjectedStartDate == null))
            {
                throw new BO.BlNullPropertyException("One or more of the previous tasks projected start date is null");
            }

            // Ensure that startDate isn't earlier than any of the projected end dates of previous tasks
            if (dependentTasks.Any(t => startDate < t.Deadline))
            {
                throw new BO.BlInvalidInputException("Cannot make a new task start date before the previous ones finish");
            }

            // Add the updated task -- with the startDate as the projectedStartDate
            DO.Task updatedTask = new(task.ID, false, task.DegreeOfDifficulty, task.AssignedEngineerId, task.Nickname, task.Description, task.Deliverables, task.Notes, task.DateCreated, startDate, task.ActualStartDate, task.Duration, task.Deadline, task.ActualEndDate);
            _dal.Task.Update(updatedTask);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    /// <summary>
    /// Convert Data Layer Task to a Business Layer Task and calculate all necessary fields
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    private BO.Task toBlTask(DO.Task task)
    {
        //Get Dependencies
        IEnumerable<DO.Dependency>? dependencies = _dal.Dependency.ReadAll(d => d.DependentTask == task.ID);
        IEnumerable<BO.TaskInList>? dependentTasks = dependencies.Select(dep =>
        {
            DO.Task dependsOnTask = _dal.Task.Read(dep.DependsOnTask);

            return new BO.TaskInList(
                id: dep.DependsOnTask,
                name: dependsOnTask.Nickname,
                description: dependsOnTask.Description,
                status: getStatusForTask(dependsOnTask)
            );
        });


        //Find the connected Milestone
        BO.MilestoneInTask? milestone = null;
        //TODO

        //Calculate Projected End Date based on max of projectedStartDate and actualStartDate plus the duration
        DateTime? projectedEndDate = null;
        if (task.ProjectedStartDate is not null && task.Duration is not null)
        {
            if (task.ActualStartDate is not null)
            {
                projectedEndDate = (task.ActualStartDate > task.ProjectedStartDate) ? task.ActualStartDate.Value.Add(task.Duration.Value) : task.ProjectedStartDate.Value.Add(task.Duration.Value);
            }
            else
            {
                projectedEndDate = task.ProjectedStartDate.Value.Add(task.Duration.Value);
            }
        }

        //Create engineerInTask
        BO.EngineerInTask? assignedEngineer = task.AssignedEngineerId is not null ? new BO.EngineerInTask(task.AssignedEngineerId.Value, _dal.Engineer.Read(task.AssignedEngineerId.Value).Name) : null;

        //Make a BL type Task
        BO.Task blTask = new(task.ID, task.Nickname, task.Description, getStatusForTask(task), dependentTasks, milestone, task.DateCreated, task.ProjectedStartDate, task.ActualStartDate, projectedEndDate, task.Deadline, task.ActualEndDate, task.Duration, task.Deliverables, task.Notes, assignedEngineer, (BO.EngineerExperience)task.DegreeOfDifficulty);

        return blTask;
    }

    /// <summary>
    /// Calculates the status of the Task based on the dates set in the DO.Task
    /// </summary>
    /// <param name="t">The DO.Task object</param>
    /// <returns>The Status of the Task</returns>
    private BO.Status getStatusForTask(DO.Task t)
    {
        if (t.ActualEndDate.HasValue)
        {
            return BO.Status.Done;
        }
        else if (t.ActualStartDate.HasValue)
        {
            if (t.Deadline.HasValue && DateTime.Now > t.Deadline)
            {
                return BO.Status.InJeopardy;
            }
            else
            {
                return BO.Status.OnTrack;
            }
        }
        else if (t.ProjectedStartDate.HasValue)
        {
            return BO.Status.Scheduled;
        }
        else
        {
            return BO.Status.Unscheduled;
        }
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
            .ReadAll(d => d.DependentTask == taskId)
            .Select(dependency => dependency.DependsOnTask);

        // Recursively get dependent tasks of direct dependencies
        IEnumerable<DO.Task> indirectDependentTasks = directDependencyIds
            .SelectMany(getDependentTasks);

        // Combine direct and indirect dependent tasks
        return _dal.Task.ReadAll(t => directDependencyIds.Contains(t.ID)).Concat(indirectDependentTasks);
    }
}
