namespace Dal;
using DalApi;

/// <summary>
/// DalList Class which implements the IDal Interface with functions which return the implementation of the appropriate object type.
/// </summary>
sealed public class DalList : IDal
{
    public IEngineer Engineer => new EngineerImplementation();

    public ITask Task => new TaskImplementation();

    public IDependency Dependency => new DependencyImplementation();
}
