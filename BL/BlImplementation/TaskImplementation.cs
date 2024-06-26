﻿namespace BlImplementation;
using System;
using System.Collections.Generic;
using DO;

internal class TaskImplementation : BlApi.ITask
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddTask(BO.Task task)
    {
        try
        {
            if (_dal.Config.GetStartDate().HasValue)
            {
                throw new BO.BlUnableToPerformActionInProductionException("Cannot add task while in production");
            }
            //Check for invalid data
            if (task.ID < 0 || task.Name == "" || task.RequiredEffortTime is null)
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Task with id {task.ID} was invalid");
            }

            //Try to add the Task to the data layer
            DO.Task newTask = new(task.ID, task.Name, false, (DO.EngineerExperience)task.Complexity, null, task.Description, task.Deliverables, task.Notes, _dal.Config.GetSystemClock(), task.ProjectedStartDate, task.ActualStartDate, task.RequiredEffortTime, task.Deadline, task.ActualEndDate);

            int newTaskId = _dal.Task.Create(newTask);

            // Make the new task dependent on all previous tasks
            IEnumerable<DO.Dependency> dependencies = task.Dependencies != null
                ? from t in task.Dependencies select new DO.Dependency(0, newTaskId, t.ID)
                : new List<DO.Dependency>();

            dependencies.ToList().ForEach(d => _dal.Dependency.Create(d));
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
            try
            {
                _dal.Dependency.ReadAll(d => d.DependsOnTask == id); //will throw an exception if there are no dependencies
                throw new BO.BlCannotBeDeletedException($"Task with id {id} cannot be deleted because another task depends on it.");
            }
            catch (DO.DalDoesNotExistException)
            {
                //Delete from the Data Layer because no other task depends on it
                _dal.Task.Delete(id);
            }
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    public IEnumerable<BO.TaskInList> GetListOfTasks(Func<BO.Task, bool>? filter = null)
    {
        try
        {
            //Get all Tasks from the DL
            IEnumerable<DO.Task> tasks = _dal.Task.ReadAll(t => !t.IsMilestone);
            //Filter the DL objects based on the filter
            IEnumerable<DO.Task> filteredDlTasks = filter != null ? tasks.Where(e => filter(toBlTask(e))) : tasks;
            //Return the list of BL type Tasks
            return filteredDlTasks.Select(toBlTask).Select(t => new BO.TaskInList(t.ID, t.Name, t.Description, t.Status));
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
            return toBlTask(_dal.Task.Read(t => t.ID == id && !t.IsMilestone));
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
            DO.Task dlTask = _dal.Task.Read(task.ID);

            // Check if task exists in the DL and that name is nonempty
            if (dlTask is null || task.Name == "")
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Task with id {task.ID} was invalid");
            }

            DO.Task? newTask = null;

            // Check which phase of the project we are in
            if (_dal.Config.GetStartDate().HasValue && dlTask.ProjectedStartDate.HasValue) // Production phase
            {
                newTask = new(task.ID, task.Name, dlTask.IsMilestone, (DO.EngineerExperience)task.Complexity, task.Engineer?.ID, task.Description, task.Deliverables, task.Notes, dlTask.DateCreated, dlTask.ProjectedStartDate, task.ActualStartDate, dlTask.Duration, dlTask.Deadline, task.ActualEndDate);

                if (task.Engineer is not null)
                {
                    //Check that new assigned engineer has proper level (equal or greater than complexity)
                    if (_dal.Engineer.Read(task.Engineer.ID).Level < (DO.EngineerExperience)task.Complexity)
                    {
                        throw new BO.BlTaskCannotBeAssignedException("The assigned engineer's experience level is not sufficient for the task's complexity");
                    }

                    //check that all dependencies of this task's milestone were already completed before this task is assigned
                    try
                    {
                        if (task.Dependencies is not null &&
                                                _dal.Task.ReadAll(t =>
                                                _dal.Dependency.ReadAll(d => d.DependentTask == task.Milestone.ID)
                                                .Select(d => d.DependsOnTask).ToList().Contains(t.ID))
                                                .Any(t => t.ActualEndDate.HasValue == false))
                        {
                            throw new BO.BlTaskCannotBeAssignedException("Cannot assign an engineer to a task that has dependencies that are not completed yet");
                        }
                    } catch (DO.DalDoesNotExistException)
                    {
                        //There are no dependencies for this task. Not an error
                    }

                }
            }
            else // Planning stage
            {
                newTask = new(task.ID, task.Name, dlTask.IsMilestone, (DO.EngineerExperience)task.Complexity, dlTask.AssignedEngineerId, task.Description, task.Deliverables, task.Notes, dlTask.DateCreated, task.ProjectedStartDate, dlTask.ActualStartDate, task.RequiredEffortTime, task.Deadline, dlTask.ActualEndDate);

                // Update the dependencies for the task (remove any old and add any new)
                IEnumerable<DO.Dependency> oldDependencies;
                try
                {
                    oldDependencies = _dal.Dependency.ReadAll(d => d.DependentTask == task.ID);
                    oldDependencies.ToList().ForEach(d => _dal.Dependency.Delete(d.ID));
                }
                catch (DO.DalDoesNotExistException)
                {
                    //There are no dependencies for this task. Not an error
                }

                IEnumerable<DO.Dependency> newDependencies = task.Dependencies != null
                    ? from t in task.Dependencies select new DO.Dependency(0, task.ID, t.ID)
                    : Enumerable.Empty<DO.Dependency>();

                //Check for circular dependencies
                foreach (DO.Dependency dependency in newDependencies)
                {
                    if (createsCircularDependency(dependency.DependentTask, dependency.DependsOnTask))
                    {
                        throw new BO.BlCircularDependencyException("The dependencies you are trying to create are circular");
                    }
                }

                newDependencies.ToList().ForEach(d => _dal.Dependency.Create(d));
            }

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
            IEnumerable<DO.Task> dependentTasks = Enumerable.Empty<DO.Task>();
            try
            {
                dependentTasks = getDependentTasks(task.ID);
            }
            catch (DO.DalDoesNotExistException)
            {
                // There are no dependent tasks, not an error
            }

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
            DO.Task updatedTask = new(task.ID, task.Nickname, false, task.DegreeOfDifficulty, task.AssignedEngineerId, task.Description, task.Deliverables, task.Notes, task.DateCreated, startDate, task.ActualStartDate, task.Duration, task.Deadline, task.ActualEndDate);
            _dal.Task.Update(updatedTask);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    public void Reset()
    {
        _dal.Task.Reset();
    }

    /// <summary>
    /// Convert Data Layer Task to a Business Layer Task and calculate all necessary fields
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    private BO.Task toBlTask(DO.Task task)
    {
        //Get Dependencies
        IEnumerable<DO.Dependency>? dependencies = null;

        try
        {
            dependencies = _dal.Dependency.ReadAll(d => d.DependentTask == task.ID);
        }
        catch (DO.DalDoesNotExistException)
        {
            //There are no dependencies for this task. Not an error
        }

        IEnumerable<BO.TaskInList>? dependentTasks = dependencies?.Select(dep =>
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
        if (!task.IsMilestone) //Only if this task isn't a milestone
        {
            try
            {
                DO.Task milestoneTask = _dal.Task.Read(t => t.ID == _dal.Dependency.Read(d => d.DependentTask == task.ID).DependsOnTask && t.IsMilestone);
                milestone = new(milestoneTask.ID, milestoneTask.Nickname);
            }
            catch (DO.DalDoesNotExistException)
            {
                //There is no connected milestone, not an error 
            }
        }

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
        else if (t.Deadline.HasValue && (_dal.Config.GetSystemClock() > t.Deadline || t.ProjectedStartDate+t.Duration > t.Deadline))
        {
            return BO.Status.InJeopardy;
        }
        else if (t.ActualStartDate.HasValue)
        {
            return BO.Status.OnTrack;
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

    /// <summary>
    /// Figure out if the dependentTask and DependsOnTask will create a circular dependency 
    /// </summary>
    /// <param name="dependentTask"></param>
    /// <param name="dependsOnTask"></param>
    /// <returns>If the dependency is circular</returns>
    private bool createsCircularDependency(int dependentTask, int dependsOnTask)
    {
        // Check if dependsOnTask is directly or indirectly dependent on dependentTask
        if (dependentTask == dependsOnTask)
        {
            return true;
        }
        return isIndirectlyDependent(dependentTask, dependsOnTask, new List<int>());
    }

    /// <summary>
    /// Recursively check if there is a direct or indirect circular dependency
    /// </summary>
    /// <param name="targetTask"></param>
    /// <param name="currentTask"></param>
    /// <param name="visitedTasks"></param>
    /// <returns>True if circular dependency found</returns>
    private bool isIndirectlyDependent(int targetTask, int currentTask, List<int> visitedTasks)
    {
        if (visitedTasks.Contains(currentTask))
        {
            // We've encountered a task we've already visited in this recursion branch, indicating a circular dependency
            return true;
        }

        visitedTasks.Add(currentTask);

        Dependency[] dependencies;
        try
        {
            dependencies = _dal.Dependency.ReadAll(d => d.DependentTask == currentTask).ToArray();
        }
        catch (DO.DalDoesNotExistException)
        {
            //No dependencies exist yet
            visitedTasks.Remove(currentTask); // Backtrack when moving to the next task
            return false;
        }


        foreach (Dependency dependency in dependencies)
        {
            if (dependency.DependsOnTask == targetTask || isIndirectlyDependent(targetTask, dependency.DependsOnTask, visitedTasks))
            {
                return true;
            }
        }

        visitedTasks.Remove(currentTask); // Backtrack when moving to the next task
        return false;
    }

    public bool AllTaskDatesSet()
    {
        //Get all tasks
        IEnumerable<DO.Task> tasks;
        try
        {
            tasks = _dal.Task.ReadAll();
        }
        catch (DO.DalDoesNotExistException)
        {
            return false;
        }
        //Check if all tasks have start and end dates
        return tasks.All(t => t.ProjectedStartDate.HasValue);
    }
}
