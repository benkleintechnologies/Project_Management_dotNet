﻿namespace DalApi;
using DO;

/// <summary>
/// Interface to access Dependencies using CRUD
/// </summary>
public interface IDependency
{
    int Create(Dependency item); // Creates new entity object in DAL
    Dependency? Read(int id); // Reads entity object by its ID
    List<Dependency> ReadAll(); // stage 1 only, Reads all entity objects // Updates entity object
    void Update(Dependency item); // Updates entity object
    void Delete(int id); // Deletes an object by its Id
    void Reset(); // Reset list of dependencies
}
