﻿namespace BlImplementation;
using BlApi;

internal class MilestoneImplementation : IMilestone
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    internal const int StartMilestoneId = 1;
    private static int _nextMilestoneId = StartMilestoneId;
    internal static int NextMilestoneId { get => _nextMilestoneId++; }

    /// <summary>
    /// Creates a Task which represents a Milestone with an auto-incrementing ID
    /// </summary>
    /// <returns>The ID of the Task created</returns>
    private int createMilestoneTaskAndGetId()
    {
        // Create a task for the milestone
        DO.Task milestoneTask = new DO.Task(0, true, Nickname: "M" + NextMilestoneId);
        // Add the milestone task to the database and return its ID
        return _dal.Task.Create(milestoneTask);
    }

    /// <summary>
    /// Calculate Milestones from the list of Dependencies, and create new dependencies between the milestones and tasks
    /// </summary>
    private void calculateMilestones()
    {
        IEnumerable<DO.Dependency> oldDependencies = _dal.Dependency.ReadAll();

        //Create a list of tasks and their dependencies, grouped
        IEnumerable<IGrouping<DO.Task, DO.Task>> groupedDependencies = oldDependencies.GroupBy(d => _dal.Task.Read(d.DependentTask), d => _dal.Task.Read(d.DependsOnTask));
        //Sort the list by Dependent Task
        groupedDependencies = groupedDependencies.OrderBy(group => group.Key);
        //Filter the list using distinct (to remove items which have the same dependencies)
        IEnumerable<IGrouping<DO.Task, DO.Task>> filteredDependencies = groupedDependencies.Distinct();
        //Create Milestones for every item left in the filtered list
        IEnumerable<BO.Milestone> milestones = filteredDependencies
            .Select(dependency =>
                new BO.Milestone(NextMilestoneId, null, null, null, BO.Status.Unscheduled, null, null, null, 0, null,
                    dependency.Select(task =>
                        new BO.TaskInList(
                            task.ID,
                            task.Nickname,
                            task.Description,
                            getStatusForTask(task))
                    ).ToList()
                )
            );

        //Reset ID Counter
        _nextMilestoneId = StartMilestoneId;

        //Create Tasks representing each milestone
        IEnumerable<int> milestoneTaskIds = filteredDependencies.Select(_ => createMilestoneTaskAndGetId()).ToList();
        //Get those Task Milestones
        IEnumerable<DO.Task> milestoneTasks = _dal.Task.ReadAll(t => t.IsMilestone);
        // Add dependencies for each milestone to the tasks that it depends on
        milestoneTasks.SelectMany(milestone =>
        {
            // Find the corresponding milestone in _milestones
            BO.Milestone? correspondingMilestone = milestones.FirstOrDefault(m => "M"+m.Id == milestone.Nickname);
            if (correspondingMilestone != null)
            {
                // Create dependencies for each dependency in the Milestone
                return correspondingMilestone.Dependencies!.Select(dependency =>
                    // Create dependency with milestone as dependent task and dependency as depends on task
                    new DO.Dependency(0, milestone.ID, dependency.ID)
                );
            }
            return Enumerable.Empty<DO.Dependency>();
        }).ToList().ForEach(dependency => _dal.Dependency.Create(dependency));

        // Add dependencies for each dependentTask to the proper Milestone
        groupedDependencies.Select(dependency =>
        {
            //IDs of tasks which the milestone depends on
            IEnumerable<int> idOfTasks = dependency.Select(t => t.ID);
            //The corresponding milestone which has in its Dependencies list, a TaskInList with id corresponding to each ID in _idOfTasks 
            BO.Milestone? correspondingMilestone = milestones.FirstOrDefault(m => idOfTasks.All(taskId => m.Dependencies!.Any(task => task.ID == taskId)));
            if (correspondingMilestone != null)
            {
                //Get the Task id which represents this milestone
                int taskId = _dal.Task.Read(t => t.Nickname == "M" + correspondingMilestone.Id).ID;
                return new DO.Dependency(0, dependency.Key.ID, taskId);
            }
            return null;
        }).Where(d => d != null).ToList().ForEach(d => _dal.Dependency.Create(d!));

        //Create starting Milestone in database (as Task)
        int startMilestoneTaskId = _dal.Task.Create(new(0, true, Nickname: "Start"));
        //Get all Tasks with no dependencies
        IEnumerable<DO.Task> tasksWithoutDependencies = _dal.Task.ReadAll(t => oldDependencies.All(d => d.DependentTask != t.ID));
        //Create a dependency for every Task with no dependencies on the Start Milestone
        tasksWithoutDependencies.Select(task =>
        {
            return new DO.Dependency(0, task.ID, startMilestoneTaskId);
        }).Where(d => d != null).ToList().ForEach(d => _dal.Dependency.Create(d!));

        //Create end Milestone in database (as Task)
        int endMilestoneTaskId = _dal.Task.Create(new(0, true, Nickname: "End"));
        //Get all Tasks which are not depended on
        IEnumerable<DO.Task> tasksNotDependedOn = _dal.Task.ReadAll(t => oldDependencies.All(d => d.DependsOnTask != t.ID));
        //Create a dependency for the End Milestone on all last tasks which are not depended on by other tasks
        tasksNotDependedOn.Select(task =>
        {
            return new DO.Dependency(0, task.ID, endMilestoneTaskId);
        }).Where(d => d != null).ToList().ForEach(d => _dal.Dependency.Create(d!));

        //Clear all old dependencies
        oldDependencies.ToList().ForEach(d => _dal.Dependency.Delete(d.ID));
    }

    public void CreateProjectSchedule()
    {
        try
        {
            //Get initial data
            DateTime? projectStartDate = _dal.Config.GetStartDate();
            DateTime? projectEndDate = _dal.Config.GetEndDate();

            if (projectStartDate is null || projectEndDate is null)
                throw new BO.BlNullPropertyException("Cannot create project schedule because the Project's start and end date have not been set.");

            if (_dal.Task.ReadAll(t => t.ProjectedStartDate.HasValue).Count() == _dal.Task.ReadAll().Count())
                throw new BO.BlUnableToPerformActionInProductionException("Cannot create milestones and schedule because dates have already been set.");

            //First create Milestones
            calculateMilestones();

            IEnumerable<DO.Task> tasks = _dal.Task.ReadAll();
            IEnumerable<DO.Task> milestones = _dal.Task.ReadAll(t => t.IsMilestone);
            IEnumerable<DO.Dependency> dependencies = _dal.Dependency.ReadAll();
            
            if (!tasks.Any() || !milestones.Any() || !dependencies.Any())
                throw new BO.BlNullPropertyException("Cannot create project schedule because there are no milestones, tasks, or dependencies.");

            //Use a breadth-first style algorithm to go through the the tasks based on their dependencies, backwards, starting from the "End" milestone
            Queue<DO.Task> taskQueue = new Queue<DO.Task>();
            // Add the "End" milestone as the starting point
            taskQueue.Enqueue(milestones.Single(m => m.Nickname == "End"));
            // Perform breadth-first traversal backward
            breadthFirstTraversalBackwards(taskQueue);

            //Check that we've succeeded so far - deadline of Start milestone should be at or later than start date
            if (milestones.Single(m => m.Nickname == "Start").Deadline < projectStartDate)
            {
                throw new BO.BlUnableToCreateScheduleException("The calculation of deadlines starting from the end goes past the start date of the project");
            }

            //Reverse Process (i.e. forward breadth first search) to set start dates
            taskQueue.Enqueue(milestones.Single(m => m.Nickname == "Start"));
            // Perform breadth-first traversal forward
            breadthFirstTraversalForward(taskQueue);

            //Check that we've succeeded - projected start of End milestone should be at or before end date
            if (milestones.Single(m => m.Nickname == "End").ProjectedStartDate > projectStartDate)
            {
                throw new BO.BlUnableToCreateScheduleException("The calculation of start dates starting from the beginning goes past the end date of the project");
            }
        }
        catch (DO.DalDoesNotExistException exc) 
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
        catch (DO.DalDeletionImpossible exc)
        {
            throw new BO.BlCannotBeDeletedException(exc.Message);
        }
    }

    public BO.Milestone GetMilestone(int id)
    {
        try
        {
            //Check for invalid data
            if (id < 0)
            {
                throw new BO.BlInvalidInputException($"The id {id} was invalid");
            }
            // Get the Task from the Data Layer
            DO.Task milestoneTask = _dal.Task.Read(t => t.ID == id && t.IsMilestone);
            // Get all dependencies of this milestone
            IEnumerable<DO.Task> dependentTasks = _dal.Dependency.ReadAll(d => d.DependentTask == id).Select(d => _dal.Task.Read(d.DependsOnTask));
            //Create a TaskInList for all dependencies
            IEnumerable<BO.TaskInList> dependenciesList = dependentTasks.Select(t => new BO.TaskInList(t.ID, t.Nickname, t.Description, getStatusForTask(t)));
            //Calculate completion percentage
            double completedPercentage = dependenciesList.Where(t => t.Status == BO.Status.Done).Count() / dependenciesList.Count();
            //Create Milestone object (and calculate Business layer fields)
            return new BO.Milestone(milestoneTask.ID, milestoneTask.Nickname, milestoneTask.Description, milestoneTask.DateCreated, getStatusForTask(milestoneTask), milestoneTask.ProjectedStartDate, milestoneTask.Deadline, milestoneTask.ActualEndDate, completedPercentage, milestoneTask.Notes, dependenciesList);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    public BO.Milestone UpdateMilestone(int id, string? nickname, string? description, string? notes)
    {
        try
        {
            //Check for invalid data
            if (id < 0)
            {
                throw new BO.BlInvalidInputException($"The id {id} was invalid");
            }
            // Get the Task from the Data Layer
            DO.Task milestoneTask = _dal.Task.Read(t => t.ID == id && t.IsMilestone);

            //Update the relevant fields
            if (nickname is not null)
                milestoneTask = milestoneTask with { Nickname = nickname };
            if (description is not null)
                milestoneTask = milestoneTask with { Description = description };
            if (notes is not null)
                milestoneTask = milestoneTask with { Notes = notes };

            //Update in database
            _dal.Task.Update(milestoneTask);

            // Get all dependencies of this milestone
            IEnumerable<DO.Task> dependentTasks = _dal.Dependency.ReadAll(d => d.DependentTask == id).Select(d => _dal.Task.Read(d.DependsOnTask));
            //Create a TaskInList for all dependencies
            IEnumerable<BO.TaskInList> dependenciesList = dependentTasks.Select(t => new BO.TaskInList(t.ID, t.Nickname, t.Description, getStatusForTask(t)));
            //Calculate completion percentage
            double completedPercentage = dependenciesList.Where(t => t.Status == BO.Status.Done).Count() / dependenciesList.Count();
            //Create Milestone object (and calculate Business layer fields)
            return new BO.Milestone(milestoneTask.ID, milestoneTask.Nickname, milestoneTask.Description, milestoneTask.DateCreated, getStatusForTask(milestoneTask), milestoneTask.ProjectedStartDate, milestoneTask.Deadline, milestoneTask.ActualEndDate, completedPercentage, milestoneTask.Notes, dependenciesList);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
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
    /// Recursive method to go through the queue of tasks from end to beginning and set the deadlines for each
    /// </summary>
    /// <param name="queue">of tasks</param>
    private void breadthFirstTraversalBackwards(Queue<DO.Task> queue)
    {
        if (queue.Count == 0)
            return;

        // Dequeue the next task from the queue
        DO.Task currentTask = queue.Dequeue();

        // Calculate the latest possible completion date (LPCD) based on dependencies
        DateTime? lpcd = null;

        // Handle first task separately (no dependencies)
        if (currentTask.IsMilestone && currentTask.Nickname == "End")
        {
            // Set LPCD to project end date as it has no dependencies
            lpcd = _dal.Config.GetEndDate();
        }
        else
        {
            // Find dependent task (the one which depends on the current task, with the longest duration, or earliest deadline if they're milestones)
            IEnumerable<DO.Task> dependentTasks = _dal.Dependency
                .ReadAll(d => d.DependsOnTask == currentTask.ID)
                .Select(d => _dal.Task.Read(d.DependentTask));
            DO.Task? dependentTask = null;
            if (dependentTasks.Any())
            {
                // If any dependent task has a duration, use the one with the maximum duration
                if (dependentTasks.Any(t => t.Duration is not null))
                {
                    dependentTask = dependentTasks.MaxBy(t => t.Duration);
                }
                else
                {
                    // Otherwise, use the one with the minimum deadline
                    dependentTask = dependentTasks.MinBy(t => t.Deadline);
                }
            }

            // Consider dependent task's deadline or duration
            if (dependentTask is not null)
            {
                if (dependentTask.Duration is not null) //Task
                {
                    // Use duration to calculate LPCD based on dependent task's end date
                    lpcd = dependentTask.Deadline.HasValue ? dependentTask.Deadline.Value.Subtract(dependentTask.Duration.Value) : null;
                }
                else if (dependentTask.Deadline is not null) //Milestone
                {
                    lpcd = dependentTask.Deadline;
                }
            }
            else
            {
                throw new BO.BlUnableToCreateScheduleException("Project schedule was not able to be automatically generated because there was an error setting the deadlines");
            }
        }

        // Update the task's deadline in the database
        _dal.Task.Update(currentTask with { Deadline = lpcd });

        // Add the tasks which this task depends on to the queue for further processing (backward traversal)
        IEnumerable<DO.Task> dependsOnTasks = _dal.Dependency
            .ReadAll(d => d.DependentTask == currentTask.ID)
            .Select(d => _dal.Task.Read(d.DependsOnTask));
        dependsOnTasks.ToList().ForEach(queue.Enqueue);

        // Recursive call for the next iteration
        breadthFirstTraversalBackwards(queue);
    }

    /// <summary>
    /// Recursive method to go through the queue of tasks from beginning to end and set the start date for each
    /// </summary>
    /// <param name="queue">of Tasks</param>
    private void breadthFirstTraversalForward(Queue<DO.Task> queue)
    {
        if (queue.Count == 0)
            return;

        // Dequeue the next task from the queue
        DO.Task currentTask = queue.Dequeue();

        // Calculate the projected start date (PSD) based on dependencies and project start date
        DateTime? psd = null;

        // Handle first milestone separately
        if (currentTask.IsMilestone && currentTask.Nickname == "Start")
        {
            psd = _dal.Config.GetStartDate();
        }
        else
        {
            // Find the depends on tasks (the ones this task depends on)
            IEnumerable<DO.Task> dependsOnTasks = _dal.Dependency
            .ReadAll(d => d.DependentTask == currentTask.ID)
            .Select(d => _dal.Task.Read(d.DependsOnTask));

            // Find the dependsOn task with the latest start date or longest duration
            DO.Task? dependsOnTask = null;
            if (dependsOnTasks.Any())
            {
                // If any dependsOn task has a duration, use the one with the maximum duration
                if (dependsOnTasks.Any(t => t.Duration is not null))
                {
                    dependsOnTask = dependsOnTasks.MaxBy(t => t.Duration);
                }
                else
                {
                    // Otherwise, use the one with the latest start date
                    dependsOnTask = dependsOnTasks.MaxBy(t => t.ProjectedStartDate);
                }
            }

            // Calculate PSD based on dependsOn task's projected start date or deadline
            if (dependsOnTask is not null)
            {
                if (dependsOnTask.ProjectedStartDate.HasValue && dependsOnTask.Duration.HasValue) // Task
                {
                    psd = dependsOnTask.ProjectedStartDate.Value + dependsOnTask.Duration.Value;
                }
                else if (dependsOnTask.ProjectedStartDate.HasValue) // Milestone with only a PSD
                {
                    psd = dependsOnTask.ProjectedStartDate.Value;
                }
            }
            else
            {
                throw new BO.BlUnableToCreateScheduleException("Project schedule was not able to be automatically generated because there was an error setting the start dates");
            }
        }

        // Update the task's projected start date in the database
        _dal.Task.Update(currentTask with { ProjectedStartDate = psd });

        // Add dependent tasks to the queue for further processing (forward traversal)
        IEnumerable<DO.Task> dependentTasks = _dal.Dependency
            .ReadAll(d => d.DependsOnTask == currentTask.ID)
            .Select(d => _dal.Task.Read(d.DependentTask));
        dependentTasks.ToList().ForEach(queue.Enqueue);

        // Recursive call for the next iteration
        breadthFirstTraversalForward(queue);
    }

}
