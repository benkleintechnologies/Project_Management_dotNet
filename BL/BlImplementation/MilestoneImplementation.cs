namespace BlImplementation;
using BlApi;
using System.Text.RegularExpressions;

internal class MilestoneImplementation : IMilestone
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Calculate Milestones from the list of Dependencies, and create new dependencies between the milestones and tasks
    /// </summary>
    private void calculateMilestones()
    {
        //Create a list of tasks and their dependencies
        IEnumerable<IGrouping<DO.Task, DO.Task>> _groupedDependencies = _dal.Dependency.ReadAll().GroupBy(d => _dal.Task.Read(d.dependentTask), d => _dal.Task.Read(d.dependsOnTask));
        //Sort the list by Dependent Task
        _groupedDependencies = _groupedDependencies.OrderBy(group => group.Key);
        //Filter the list using distinct (to remove items which have the same dependencies)
        IEnumerable<IGrouping<DO.Task, DO.Task>> _filteredDependencies = _groupedDependencies.Distinct();
        //Create Milestones for every item left in the filtered list
        /*IEnumerable<BO.Milestone> _milestones = _filteredDependencies
            .Select(dependency =>
                new BO.Milestone(0, null, null, null, BO.Status.Unscheduled, null, null, null, 0, null,
                    dependency.Select(task =>
                        new BO.TaskInList(
                            task.id,
                            task.nickname,
                            task.description,
                            task.projectedStartDate is not null ? BO.Status.Scheduled : BO.Status.Unscheduled)
                    ).ToList()
                )
            );
        */
        _dal.Task.Create(_filteredDependencies.Select(dependency => new DO.Task(0, true)));

        IEnumerable<DO.Task> _milestoneTasks = _dal.Task.ReadAll(t => t.isMilestone);
        //Add dependencies for each milestone to the tasks that it depends on
        _milestoneTasks.Select(milestone => _dal.Dependency.Create(new DO.Dependency(0, milestone.id, milestone.)))

    }

    public void createProjectSchedule()
    {
        throw new NotImplementedException();
    }

    public Milestone getMilestone(int id)
    {
        throw new NotImplementedException();
    }

    public Milestone updateMilestone(int id, string? nickname, string? description, string? notes)
    {
        throw new NotImplementedException();
    }
}
