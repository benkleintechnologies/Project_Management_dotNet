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
        if (InternalRead(item.ID) is not null)
        {
            throw new DalAlreadyExistsException($"An object of type Engineer with id {item.ID} already exists");
        }
        DataSource.Engineers.Add(item);
        return item.ID;
    }

    /// <summary>
    /// Delete an Engineer from the database
    /// </summary>
    /// <param name="id">ID of the Engineer to delete</param>
    public void Delete(int id)
    {
        Engineer engineer = Read(id); //Check that this item exists
        DataSource.Engineers.Remove(engineer);
        Engineer newEngineer = engineer with { Active = false };
        DataSource.Engineers.Add(newEngineer);
    }

    /// <summary>
    /// Retrieve an Engineer from the database by ID
    /// </summary>
    /// <param name="id">ID of the Engineer<</param>
    /// <returns>The requested Engineer</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Engineer with this ID</exception>
    public Engineer Read(int id)
    {
        Engineer? engineer = InternalRead(id);
        if (engineer == null)
        {
            throw new DalDoesNotExistException($"Object of type Engineer with identifier {id} does not exist");
        }
        return engineer;
    }

    /// <summary>
    /// Retrieve an Engineer from the database based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Engineer which matches this filter</exception>
    public Engineer Read(Func<Engineer, bool> filter)
    {
        Engineer? engineer = DataSource.Engineers.Where(item => item.Active).FirstOrDefault(filter);
        if (engineer == null)
        {
            throw new DalDoesNotExistException($"Object of type Engineer with this filter does not exist");
        }
        return engineer;
    }

    /// <summary>
    /// Retrieve all Engineers from the database
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Engineers</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Engineer which matches this filter (or none at all if the filter is null)</exception>
    public IEnumerable<Engineer> ReadAll(Func<Engineer, bool>? filter = null)
    {
        IEnumerable<Engineer> activeEngineers = DataSource.Engineers.Where(item => item.Active);

        if (activeEngineers.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Engineer exists");
        }
        if (filter == null)
        {
            return activeEngineers;
        }

        IEnumerable<Engineer> filteredEngineers = activeEngineers.Where(filter);
        if (filteredEngineers.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Engineer exists");
        }
        return filteredEngineers;
    }

    /// <summary>
    /// Updates an Engineer in the database
    /// </summary>
    /// <param name="item">New Engineer information</param>
    public void Update(Engineer item)
    {
        Engineer? old = Read(item.ID);
        DataSource.Engineers.Remove(old);
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
        return DataSource.Engineers.FirstOrDefault(item => item.ID == id && item.Active);
    }
}
