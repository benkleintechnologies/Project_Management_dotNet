namespace BlImplementation;
using BlApi;
using System.Text.RegularExpressions;

internal class MilestoneImplementation : IMilestone
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    internal const int StartMilestoneId = 1;
    private static int _nextMilestoneId = StartMilestoneId;
    internal static int NextMilestoneId { get => _nextMilestoneId++; }

    /// <summary>
    /// Creates a Tas which represents a Milestone with an auto-incrementing ID
    /// </summary>
    /// <returns>The ID of the Task created</returns>
    private int createMilestoneTaskAndGetId()
    {
        // Create a task for the milestone
        DO.Task milestoneTask = new DO.Task(0, true, nickname: "M" + NextMilestoneId);
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
        IEnumerable<IGrouping<DO.Task, DO.Task>> groupedDependencies = oldDependencies.GroupBy(d => _dal.Task.Read(d.dependentTask), d => _dal.Task.Read(d.dependsOnTask));
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
                            task.id,
                            task.nickname,
                            task.description,
                            task.projectedStartDate is not null ? BO.Status.Scheduled : BO.Status.Unscheduled)
                    ).ToList()
                )
            );

        //Reset ID Counter
        _nextMilestoneId = StartMilestoneId;

        //Create Tasks representing each milestone
        IEnumerable<int> milestoneTaskIds = filteredDependencies.Select(_ => createMilestoneTaskAndGetId()).ToList();
        //Get those Task Milestones
        IEnumerable<DO.Task> milestoneTasks = _dal.Task.ReadAll(t => t.isMilestone);
        // Add dependencies for each milestone to the tasks that it depends on
        milestoneTasks.SelectMany(milestone =>
        {
            // Find the corresponding milestone in _milestones
            BO.Milestone? correspondingMilestone = milestones.FirstOrDefault(m => "M"+m.Id == milestone.nickname);
            if (correspondingMilestone != null)
            {
                // Create dependencies for each dependency in the Milestone
                return correspondingMilestone.Dependencies!.Select(dependency =>
                    // Create dependency with milestone as dependent task and dependency as depends on task
                    new DO.Dependency(0, milestone.id, dependency.Id)
                );
            }
            return Enumerable.Empty<DO.Dependency>();
        }).ToList().ForEach(dependency => _dal.Dependency.Create(dependency));

        // Add dependencies for each dependentTask to the proper Milestone
        groupedDependencies.Select(dependency =>
        {
            //IDs of tasks which the milestone depends on
            IEnumerable<int> idOfTasks = dependency.Select(t => t.id);
            //The corresponding milestone which has in its Dependencies list, a TaskInList with id corresponding to each ID in _idOfTasks 
            BO.Milestone? correspondingMilestone = milestones.FirstOrDefault(m => idOfTasks.All(taskId => m.Dependencies!.Any(task => task.Id == taskId)));
            if (correspondingMilestone != null)
            {
                //Get the Task id which represents this milestone
                int taskId = _dal.Task.Read(t => t.nickname == "M" + correspondingMilestone.Id).id;
                return new DO.Dependency(0, dependency.Key.id, taskId);
            }
            return null;
        }).Where(d => d != null).ToList().ForEach(d => _dal.Dependency.Create(d!));

        //Create starting Milestone in database (as Task)
        int startMilestoneTaskId = _dal.Task.Create(new(0, true, nickname: "Start"));
        //Get all Tasks with no dependencies
        IEnumerable<DO.Task> tasksWithoutDependencies = _dal.Task.ReadAll(t => oldDependencies.All(d => d.dependentTask != t.id));
        //Create a dependency for every Task with no dependencies on the Start Milestone
        tasksWithoutDependencies.Select(task =>
        {
            return new DO.Dependency(0, task.id, startMilestoneTaskId);
        }).Where(d => d != null).ToList().ForEach(d => _dal.Dependency.Create(d!));

        //Create end Milestone in database (as Task)
        int endMilestoneTaskId = _dal.Task.Create(new(0, true, nickname: "End"));
        //Get all Tasks which are not depended on
        IEnumerable<DO.Task> tasksNotDependedOn = _dal.Task.ReadAll(t => oldDependencies.All(d => d.dependsOnTask != t.id));
        //Create a dependency for the End Milestone on all last tasks which are not depended on by other tasks
        tasksNotDependedOn.Select(task =>
        {
            return new DO.Dependency(0, task.id, endMilestoneTaskId);
        }).Where(d => d != null).ToList().ForEach(d => _dal.Dependency.Create(d!));

        //Clear all old dependencies
        oldDependencies.ToList().ForEach(d => _dal.Dependency.Delete(d.id));
    }

    public void CreateProjectSchedule()
    {
        try
        {
            //First create Milestones
            calculateMilestones();

            //Get initial data
            DateTime? projectStartDate = _dal.Config.getStartDate();
            DateTime? projectEndDate = _dal.Config.getEndDate();
            IEnumerable<DO.Task> tasks = _dal.Task.ReadAll();
            IEnumerable<DO.Task> milestones = _dal.Task.ReadAll(t => t.isMilestone);
            IEnumerable<DO.Dependency> dependencies = _dal.Dependency.ReadAll();

            //Check for nulls - error
            //TODO: Make sure to catch errors and throw necessary exceptions

            //Use a breadth-first style algorithm to go through the the tasks based on their dependencies, backwards, starting from the "End" milestone
            Queue<DO.Task> taskQueue = new Queue<DO.Task>();
            // Add the "End" milestone as the starting point
            taskQueue.Enqueue(milestones.Single(m => m.nickname == "End"));
            // Perform breadth-first traversal
            breadthFirstTraversal(taskQueue);

            //Check that we've succeeded so far
            //TODO

            //Reverse Process (i.e. forward breadth first search) to set start dates

            //Check that we've succeeded

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
        throw new NotImplementedException();
    }

    public BO.Milestone UpdateMilestone(int id, string? nickname, string? description, string? notes)
    {
        throw new NotImplementedException();
    }

    private void breadthFirstTraversal(Queue<DO.Task> queue)
    {
        if (queue.Count == 0)
            return;

        // Dequeue the next task from the queue
        DO.Task currentTask = queue.Dequeue();

        // Get the tasks which the current task depends on
        IEnumerable<DO.Task> dependentTasks = _dal.Dependency
            .ReadAll(d => d.dependentTask == currentTask.id)
            .Select(d => _dal.Task.Read(d.dependsOnTask));

        // Calculate the latest possible completion date (LPCD) based on the instructions
        TimeSpan? duration = currentTask.duration;
        DateTime? lpcd = duration is not null ? currentTask.deadline - duration.Value : currentTask.deadline is not null ? currentTask.deadline : _dal.Config.GetEndDate();

        // Check if deadline already exists which is later
        if (lpcd is not null)
        {
            dependentTasks.ToList().ForEach(dependentTask =>
            {
                if (dependentTask.deadline is not null)
                    lpcd = lpcd < dependentTask.deadline ? lpcd : dependentTask.deadline;
            });
        }

        // Update the task's deadline in the database
        _dal.Task.Update(currentTask with { deadline = lpcd });

        // Add dependent tasks to the queue for further processing
        dependentTasks.ToList().ForEach(task => queue.Enqueue(task));

        // Recursive call for the next iteration
        breadthFirstTraversal(queue);
    }
}
