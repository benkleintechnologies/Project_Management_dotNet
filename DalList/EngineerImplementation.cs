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
        if (Read(item.id) is not null)
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
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Engineer with this ID in the database</exception>
    public void Delete(int id)
    {
        Engineer? engineer = Read(id);
        if (engineer == null) 
        { 
            throw new DalDoesNotExistException($"Object of type Engineer with identifier {id} does not exist, so it cannot be deleted."); 
        } 
        else
        {
            DataSource.Engineers.Remove(engineer);
            Engineer _newEngineer = engineer with { active = false };
            DataSource.Engineers.Add(_newEngineer);
        }
    }

    /// <summary>
    /// Retreive an Engineer from the database by ID
    /// </summary>
    /// <param name="id">ID of the Engineer</param>
    /// <returns>The Engineer object requested</returns>
    public Engineer? Read(int id)
    {
        return DataSource.Engineers.FirstOrDefault(item => item.id == id && item.active);
    }

    /// <summary>
    /// Retreive an Engineer from the databse based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    public Engineer? Read(Func<Engineer, bool> filter)
    {
        return DataSource.Engineers.Where(item => item.active).FirstOrDefault(filter);
    }

    /// <summary>
    /// Retreive all Engineers from the database
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Engineers</returns>
    public IEnumerable<Engineer> ReadAll(Func<Engineer, bool>? filter = null)
    {
        if (filter == null)
        {
            return DataSource.Engineers.Where(item => item.active);
        }
        return DataSource.Engineers.Where(filter).Where(item => item.active);
    }

    /// <summary>
    /// Updates an Engineer in the database
    /// </summary>
    /// <param name="item">New Engineer information</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no Engineer with the same ID exists</exception>
    public void Update(Engineer item)
    {
        Engineer? old = Read(item.id);
        if (old != null)
        {
            DataSource.Engineers.Remove(old);
            DataSource.Engineers.Add(item);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Engineer with identifier {item.id} does not exist");
        }
    }

    /// <summary>
    /// Reset Enumerable of Engineers in the database
    /// </summary>
    public void Reset()
    {
        DataSource.Engineers.Clear();
    }
}
