namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

/// <summary>
/// Implementation of Engineer Interface, which implements CRUD methods and Reset
/// </summary>
internal class EngineerImplementation: IEngineer
{
    public int Create(Engineer item)
    {
        if (Read(item.id) is not null)
        {
            throw new DalAlreadyExistsException($"An object of type Engineer with id {item.id} already exists");
        }
        DataSource.Engineers.Add(item);
        return item.id;
    }

    public void Delete(int id)
    {
        Engineer engineer = Read(id);
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

    public Engineer? Read(int id)
    {
        return DataSource.Engineers.FirstOrDefault(item => item.id == id && item.active);
    }

    public Engineer? Read(Func<Engineer, bool> filter)
    {
        return DataSource.Engineers.Where(item => item.active).FirstOrDefault(filter);
    }

    public IEnumerable<Engineer> ReadAll(Func<Engineer, bool>? filter = null)
    {
        if (filter == null)
        {
            return DataSource.Engineers.Where(item => item.active);
        }
        return DataSource.Engineers.Where(filter).Where(item => item.active);
    }

    public void Update(Engineer item)
    {
        Engineer old = Read(item.id);
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

    public void Reset()
    {
        DataSource.Engineers.Clear();
    }
}
