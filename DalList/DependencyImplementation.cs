namespace Dal;
using DO;
using DalApi;
using System.Collections.Generic;

/// <summary>
/// Implementation of Dependency Interface, which implements CRUD methods and Reset
/// </summary>
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
            DataSource.Dependencies.Remove(dependency);
            Dependency _newDependency = dependency with { active = false };
            DataSource.Dependencies.Add(_newDependency);
        }
    }

    public Dependency? Read(int id)
    {
        return DataSource.Dependencies.Find(item => item.id == id && item.active);
    }

    public List<Dependency> ReadAll()
    {
        return new List<Dependency>(DataSource.Dependencies.FindAll(item => item.active));
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

    public void Reset()
    {
        DataSource.Dependencies.Clear();
    }
}
