namespace DalApi;

/// <summary>
/// ICrud Interface to act as a generic interface for the CRUD methods of all entities
/// </summary>
/// <typeparam name="T">Entity Type</typeparam>
public interface ICrud <T> where T : class
{
    int Create(T item); // Creates new entity object in DAL
    T? Read(int id); // Reads entity object by its ID
    IEnumerable<T?> ReadAll(Func<T, bool>? filter = null); // stage 1 only, Reads all entity object
    void Update(T item); // Updates entity object
    void Delete(int id); // Deletes an object by its Id
    void Reset(); // Reset list of dependencies
}
