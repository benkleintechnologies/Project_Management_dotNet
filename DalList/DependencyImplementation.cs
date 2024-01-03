namespace Dal;
using DO;
using DalApi;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

internal class DependencyImplementation : IDependency
{
    public int Create(Dependency item)
    {
        int id = DataSource.Config.NextDependencyId;
        Dependency dependency = item with { id = id };
        DataSource.Dependencies.Add(dependency);
        return id;
    }

    public void Delete(int id)
    {
        Dependency dependency = Read(id);
        if (dependency == null)
        {
            throw new Exception($"Object of type Dependency with identifier {id} does not exist");
        }
        else
        {
            //May need to check that Dependency is not read only (or something liek that)
            DataSource.Dependencies.Remove(dependency);
        }
    }

    public Dependency? Read(int id)
    {
        foreach (Dependency item in DataSource.Dependencies)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;
    }

    public List<Dependency> ReadAll()
    {
        return new List<Dependency>(DataSource.Dependencies);
    }

    public void Update(Dependency item)
    {
        Dependency old = Read(item.id);
        if (old != null)
        {
            DataSource.Dependencies.Remove(old);
            DataSource.Dependencies.Add(item);
        }
        else
        {
            throw new Exception($"Object of type Dependency with identifier {item.id} does not exist");
        }
    }
}
