namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

/// <summary>
/// Implementation of Engineer Interface, which implements CRUD methods and Reset
/// </summary>
internal class EngineerImplementation: IEngineer
{
    /// <summary>
    /// Add Engineer item to database
    /// </summary>
    /// <param name="item">The Engineer to add</param>
    /// <returns>The ID of the engineer</returns>
    /// <exception cref="DalAlreadyExistsException">Thrown when an Engineer with this ID is already in the database</exception>
    public int Create(Engineer item)
    {
        if (InternalRead(item.id) is not null)
        {
            throw new DalAlreadyExistsException($"An object of type Engineer with id {item.id} already exists");
        }
        DataSource.Engineers.Add(item);
        return item.id;
    }

    /// <summary>
    /// Delete an Engineer from the database
    /// </summary>
    /// <param name="id">ID of the Engineer to delete</param>
    public void Delete(int id)
    {
        Engineer _engineer = Read(id); //Check that this item exists
        DataSource.Engineers.Remove(_engineer);
        Engineer _newEngineer = _engineer with { active = false };
        DataSource.Engineers.Add(_newEngineer);
    }

    /// <summary>
    /// Retrieve an Engineer from the database by ID
    /// </summary>
    /// <param name="id">ID of the Engineer<</param>
    /// <returns>The requested Engineer</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Engineer with this ID</exception>
    public Engineer Read(int id)
    {
        Engineer? _engineer = InternalRead(id);
        if (_engineer == null)
        {
            throw new DalDoesNotExistException($"Object of type Engineer with identifier {id} does not exist");
        }
        return _engineer;
    }

    /// <summary>
    /// Retrieve an Engineer from the database based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Engineer which matches this filter</exception>
    public Engineer Read(Func<Engineer, bool> filter)
    {
        Engineer? _engineer = DataSource.Engineers.Where(item => item.active).FirstOrDefault(filter);
        if (_engineer == null)
        {
            throw new DalDoesNotExistException($"Object of type Engineer with this filter does not exist");
        }
        return _engineer;
    }

    /// <summary>
    /// Retrieve all Engineers from the database
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Engineers</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Engineer which matches this filter (or none at all if the filter is null)</exception>
    public IEnumerable<Engineer> ReadAll(Func<Engineer, bool>? filter = null)
    {
        IEnumerable<Engineer> _activeEngineers = DataSource.Engineers.Where(item => item.active);

        if (_activeEngineers.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Engineer exists");
        }
        if (filter == null)
        {
            return _activeEngineers;
        }

        IEnumerable<Engineer> _filteredEngineers = _activeEngineers.Where(filter);
        if (_filteredEngineers.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Engineer exists");
        }
        return _filteredEngineers;
    }

    /// <summary>
    /// Updates an Engineer in the database
    /// </summary>
    /// <param name="item">New Engineer information</param>
    public void Update(Engineer item)
    {
        Engineer? _old = Read(item.id);
        DataSource.Engineers.Remove(_old);
        DataSource.Engineers.Add(item);
    }

    /// <summary>
    /// Reset Enumerable of Engineers in the database
    /// </summary>
    public void Reset()
    {
        DataSource.Engineers.Clear();
    }

    private Engineer? InternalRead(int id)
    {
        return DataSource.Engineers.FirstOrDefault(item => item.id == id && item.active);
    }
}
