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
    /// <exception cref="DalDoesNotExistException">the Dependency can't be deleted because doesn't exist in the database</exception>
    public void Delete(int id)
    {
        Dependency? _dependency = Read(id);
        if (_dependency == null)
        {
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {id} does not exist");
        }
        else
        {
            DataSource.Dependencies.Remove(_dependency);
            Dependency _newDependency = _dependency with { active = false };
            DataSource.Dependencies.Add(_newDependency);
        }
    }

    /// <summary>
    /// Retrieving a Dependency in the database
    /// </summary>
    /// <param name="id">id of the Dependency</param>
    /// <returns></returns>
    public Dependency? Read(int id)
    {
        return DataSource.Dependencies.FirstOrDefault(item => item.id == id && item.active);
    }

    /// <summary>
    /// Retrieving a Dependency based on a filter
    /// </summary>
    /// <param name="filter">filter to find a specific type of Dependency</param>
    /// <returns></returns>
    public Dependency? Read(Func<Dependency, bool> filter) 
    {
        return DataSource.Dependencies.Where(item => item.active).FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieves a IEnumerable of Dependencies in the database
    /// </summary>
    /// <param name="filter">optional filter to find a specific type of Dependency</param>
    /// <returns></returns>
    public IEnumerable<Dependency> ReadAll(Func<Dependency, bool>? filter = null)
    {
        if (filter == null)
        {
            return DataSource.Dependencies.Where(item => item.active);
        }
        return DataSource.Dependencies.Where(filter).Where(item => item.active);
    }

    /// <summary>
    /// Updating an exisiting Dependency in the database
    /// </summary>
    /// <param name="item">Dependency to update</param>
    /// <exception cref="DalDoesNotExistException">Dependency doesn't exist so can't update</exception>
    public void Update(Dependency item)
    {
        Dependency? _old = Read(item.id);
        if (_old != null)
        {
            DataSource.Dependencies.Remove(_old);
            DataSource.Dependencies.Add(item);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {item.id} does not exist");
        }
    }

    /// <summary>
    /// Reset all Dependencies in the database
    /// </summary>
    public void Reset()
    {
        DataSource.Dependencies.Clear();
    }
}
