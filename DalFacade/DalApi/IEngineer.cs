namespace DalApi;
using DO;
using System;

/// <summary>
/// Interface to access Engineers using CRUD
/// </summary>
public interface IEngineer : ICrud<Engineer>
{
    Engineer? Read(Func<Engineer, bool> filter);
}
