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
        if (DataSource.Engineers.Contains(item))
        {
            throw new Exception($"An object of type Engineer with id {item.id} already exists");
        }
        DataSource.Engineers.Add(item);
        return item.id;
    }

    public void Delete(int id)
    {
        Engineer engineer = Read(id);
        if (engineer == null) 
        { 
            throw new Exception($"Object of type Engineer with identifier {id} does not exist"); 
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
        return DataSource.Engineers.Find(item => item.id == id && item.active);
    }

    public List<Engineer> ReadAll()
    {
        return new List<Engineer>(DataSource.Engineers.FindAll(item => item.active));
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
            throw new Exception($"Object of type Engineer with identifier {item.id} does not exist");
        }
    }

    public void Reset()
    {
        DataSource.Engineers.Clear();
    }
}
