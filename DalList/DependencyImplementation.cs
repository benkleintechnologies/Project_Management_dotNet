namespace Dal;
using DO;
using DalApi;
using System.Collections.Generic;

/// <summary>
/// Implementation of Dependency Interface, which implements CRUD methods and Reset
/// </summary>
internal class DependencyImplementation : IDependency
{
    /// <summary>
    /// Add Dependency item to database
    /// </summary>
    /// <param name="item">The Dependency to add</param>
    /// <returns>id of the Dependency</returns>
    public int Create(Dependency item)
    {
        int id = DataSource.Config.NextDependencyId;
        Dependency dependency = item with { ID = id };
        DataSource.Dependencies.Add(dependency);
        return id;
    }

    /// <summary>
    /// Deletes a Dependency item from the database
    /// </summary>
    /// <param name="id">the id of the Dependency to delete</param>
    public void Delete(int id)
    {
        Dependency dependency = Read(id); //Check that this item exists
        DataSource.Dependencies.Remove(dependency);
        Dependency newDependency = dependency with { Active = false };
        DataSource.Dependencies.Add(newDependency);
    }

    /// <summary>
    /// Retrieving a Dependency in the database
    /// </summary>
    /// <param name="id">id of the Dependency</param>
    /// <returns>The requested Dependency</returns>
    /// <exception cref="DalDoesNotExistException">Dependency doesn't exist</exception>
    public Dependency Read(int id)
    {
        Dependency? dependency = DataSource.Dependencies.FirstOrDefault(item => item.ID == id && item.Active);
        if (dependency == null)
        {
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {id} does not exist");
        }
        return dependency;
    }

    /// <summary>
    /// Retrieving a Dependency based on a filter
    /// </summary>
    /// <param name="filter">filter to find a specific type of Dependency</param>
    /// <returns>The requested Dependency</returns>
    /// <exception cref="DalDoesNotExistException">Dependency doesn't exist with this filter</exception>
    public Dependency Read(Func<Dependency, bool> filter) 
    {
        Dependency? dependency = DataSource.Dependencies.Where(item => item.Active).FirstOrDefault(filter);
        if (dependency == null)
        {
            throw new DalDoesNotExistException($"Object of type Dependency with this filter does not exist");
        }
        return dependency;

    }

    /// <summary>
    /// Retrieves a IEnumerable of Dependencies in the database
    /// </summary>
    /// <param name="filter">optional filter to find a specific type of Dependency</param>
    /// <returns>All Dependencies which match the filter</returns>
    /// <exception cref="DalDoesNotExistException">No Dependencies match the filter (or none exist if filter is null)</exception>
    public IEnumerable<Dependency> ReadAll(Func<Dependency, bool>? filter = null)
    {
        IEnumerable<Dependency> activeDependencies = DataSource.Dependencies.Where(item => item.Active);
        if (activeDependencies.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Dependency exists");
        }
        if (filter == null)
        {
            return activeDependencies;
        }

        IEnumerable<Dependency> filteredDependencies = activeDependencies.Where(filter);
        if (filteredDependencies.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Dependency exists");
        }
        return filteredDependencies;
    }

    /// <summary>
    /// Updating an existing Dependency in the database
    /// </summary>
    /// <param name="item">Dependency to update</param>
    public void Update(Dependency item)
    {
        Dependency old = Read(item.ID);
        DataSource.Dependencies.Remove(old);
        DataSource.Dependencies.Add(item);
    }

    /// <summary>
    /// Reset all Dependencies in the database
    /// </summary>
    public void Reset()
    {
        DataSource.Dependencies.Clear();
    }
}
