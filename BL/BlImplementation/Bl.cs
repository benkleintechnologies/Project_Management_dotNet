namespace BlImplementation;
using BlApi;

internal class Bl : IBl
{
    public IEngineer Engineer => new EngineerImplementation();

    public ITask task => new TaskImplementation();

    public IMilestone milestone => new MilestoneImplementation();
}
