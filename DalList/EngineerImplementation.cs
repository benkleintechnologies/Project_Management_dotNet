namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class EngineerImplementation: IEngineer
{
	public EngineerImplementation()
	{
	}

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
            //May need to check that Engineer is not read only (or something liek that)
            DataSource.Engineers.Remove(engineer);
        }
    }

    public Engineer? Read(int id)
    {
        foreach (Engineer item in DataSource.Engineers)
        {
            if (item.id == id) {
                return item;
            }
        }
        return null;
    }

    public List<Engineer> ReadAll()
    {
        return new List<Engineer>(DataSource.Engineers);
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
