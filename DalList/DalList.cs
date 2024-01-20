namespace Dal;
using DalApi;

/// <summary>
/// DalList Class which implements the IDal Interface with functions which return the implementation of the appropriate object type.
/// </summary>
sealed internal class DalList : IDal
{
    // Lazy<DalList> ensures lazy initialization and thread safety
    private static readonly Lazy<DalList> lazyInstance = new Lazy<DalList>(() => new DalList());
    // Public property to access the singleton instance
    public static IDal Instance => lazyInstance.Value;
    private DalList() { }
    public IConfig Config => new ConfigImplementation();

    public IEngineer Engineer => new EngineerImplementation();

    public ITask Task => new TaskImplementation();

    public IDependency Dependency => new DependencyImplementation();
}
