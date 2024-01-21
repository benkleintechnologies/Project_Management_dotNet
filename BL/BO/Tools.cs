namespace BO; 
using System;
using System.Collections;
using System.Reflection;
using System.Text;

/// <summary>
/// Provides utility methods for working with BL
/// </summary>
public static class Tools
{
    /// <summary>
    /// Returns stringified version of the object
    /// </summary>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <param name="obj">The object to turn into a String</param>
    /// <returns></returns>
    public static string ToStringProperty<T>(this T obj)
    {
        if (obj == null)
            return string.Empty;

        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();

        StringBuilder result = new StringBuilder();
        result.Append(type.Name + " { ");

        foreach (PropertyInfo property in properties)
        {
            result.Append($"{property.Name}: ");

            // Check if the property is a collection type (implementing IEnumerable)
            if (property.PropertyType.GetInterface(nameof(IEnumerable)) != null)
            {
                IEnumerable? collection = (IEnumerable?)property.GetValue(obj);

                if (collection != null)
                {
                    result.Append("[");
                    foreach (var item in collection)
                    {
                        result.Append($"{item}, ");
                    }
                    result.Append("], ");
                }
                else
                {
                    result.Append("null, ");
                }
            }
            else
            {
                // Handle non-collection properties
                result.Append($"{property.GetValue(obj)}, ");
            }
        }

        result.Remove(result.Length - 2, 2); // Remove the trailing comma and space
        result.Append(" }");

        return result.ToString();
    }
}
