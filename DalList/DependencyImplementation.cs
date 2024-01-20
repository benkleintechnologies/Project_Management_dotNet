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
        int _id = DataSource.Config.NextDependencyId;
        Dependency _dependency = item with { id = _id };
        DataSource.Dependencies.Add(_dependency);
        return _id;
    }

    /// <summary>
    /// Deletes a Dependency item from the database
    /// </summary>
    /// <param name="id">the id of the Dependency to delete</param>
    public void Delete(int id)
    {
        Dependency _dependency = Read(id); //Check that this item exists
        DataSource.Dependencies.Remove(_dependency);
        Dependency _newDependency = _dependency with { active = false };
        DataSource.Dependencies.Add(_newDependency);
    }

    /// <summary>
    /// Retrieving a Dependency in the database
    /// </summary>
    /// <param name="id">id of the Dependency</param>
    /// <returns>The requested Dependency</returns>
    /// <exception cref="DalDoesNotExistException">Dependency doesn't exist</exception>
    public Dependency Read(int id)
    {
        Dependency? _dependency = DataSource.Dependencies.FirstOrDefault(item => item.id == id && item.active);
        if (_dependency == null)
        {
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {id} does not exist");
        }
        return _dependency;
    }

    /// <summary>
    /// Retrieving a Dependency based on a filter
    /// </summary>
    /// <param name="filter">filter to find a specific type of Dependency</param>
    /// <returns>The requested Dependency</returns>
    /// <exception cref="DalDoesNotExistException">Dependency doesn't exist with this filter</exception>
    public Dependency Read(Func<Dependency, bool> filter) 
    {
        Dependency? _dependency = DataSource.Dependencies.Where(item => item.active).FirstOrDefault(filter);
        if (_dependency == null)
        {
            throw new DalDoesNotExistException($"Object of type Dependency with this filter does not exist");
        }
        return _dependency;

    }

    /// <summary>
    /// Retrieves a IEnumerable of Dependencies in the database
    /// </summary>
    /// <param name="filter">optional filter to find a specific type of Dependency</param>
    /// <returns>All Dependencies which match the filter</returns>
    /// <exception cref="DalDoesNotExistException">No Dependencies match the filter (or none exist if filter is null)</exception>
    public IEnumerable<Dependency> ReadAll(Func<Dependency, bool>? filter = null)
    {
        IEnumerable<Dependency> _activeDependencies = DataSource.Dependencies.Where(item => item.active);
        if (_activeDependencies.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Dependency exists");
        }
        if (filter == null)
        {
            return _activeDependencies;
        }

        IEnumerable<Dependency> _filteredDependencies = _activeDependencies.Where(filter);
        if (_filteredDependencies.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Dependency exists");
        }
        return _filteredDependencies;
    }

    /// <summary>
    /// Updating an existing Dependency in the database
    /// </summary>
    /// <param name="item">Dependency to update</param>
    public void Update(Dependency item)
    {
        Dependency _old = Read(item.id);
        DataSource.Dependencies.Remove(_old);
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
