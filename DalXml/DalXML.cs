namespace Dal;
using DalApi;

// stage 3
/// <summary>
/// Returns new implementation object for each of the entities
/// </summary>
sealed public class DalXML : IDal
{
    public IConfig Config => new ConfigImplementation();

    public IEngineer Engineer => new EngineerImplementation();

    public ITask Task => new TaskImplementation();

    public IDependency Dependency => new DependencyImplementation();
}
