namespace Dal;
using DalApi;
using System.Diagnostics;

// stage 3
/// <summary>
/// Returns new implementation object for each of the entities
/// </summary>
sealed internal class DalXml : IDal
{
    // Lazy<DalXml> ensures lazy initialization and thread safety
    private static readonly Lazy<DalXml> lazyInstance = new Lazy<DalXml>(() => new DalXml());
    // Public property to access the singleton instance
    public static IDal Instance => lazyInstance.Value;
    private DalXml() { }
    public IConfig Config => new XMLConfigImplementation();

    public IEngineer Engineer => new EngineerImplementation();

    public ITask Task => new TaskImplementation();

    public IDependency Dependency => new DependencyImplementation();
}
