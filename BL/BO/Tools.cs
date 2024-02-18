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
        //result.Append(type.Name + " { ");

        foreach (PropertyInfo property in properties)
        {
            result.Append($"{property.Name}: ");

            // Check if the property is a collection type (implementing IEnumerable)
            if (property.PropertyType != typeof(string) && property.PropertyType.GetInterface(nameof(IEnumerable)) != null)
            {
                IEnumerable? collection = (IEnumerable?)property.GetValue(obj);

                if (collection != null)
                {
                    result.Append("[");
                    foreach (var item in collection)
                    {
                        //If the item is a complex type, print its properties on one line
                        if (item != null && item.GetType().GetProperties().Length > 1)
                        {
                            result.Append("{ ");
                            foreach (PropertyInfo itemProperty in item.GetType().GetProperties())
                            {
                                result.Append($"{itemProperty.Name}: {itemProperty.GetValue(item)}, ");
                            }
                            result.Remove(result.Length - 2, 2);// Remove the trailing comma and space
                            result.Append("}, ");
                        }
                        else
                        {
                            result.Append($"{item}, ");
                        }
                    }
                    result.Remove(result.Length - 2, 2);// Remove the trailing comma and space
                    result.AppendLine("]");
                }
                else
                {
                    result.AppendLine("null");
                }
            }
            else if (property.PropertyType != typeof(string) &&
                     property.PropertyType != typeof(DateTime) &&
                     property.PropertyType != typeof(DateTime?) &&
                     property.PropertyType != typeof(TimeSpan) &&
                     property.PropertyType != typeof(TimeSpan?) &&
                     property.PropertyType.GetProperties().Length > 1)
            {
                //If the property is a complex type, print its properties on one line
                result.Append("{ ");
                foreach (PropertyInfo itemProperty in property.PropertyType.GetProperties())
                {
                    //Print each property of the item, all on one line
                    if (itemProperty is not null)
                    {
                        object? propertyValue = property.GetValue(obj);
                        if (propertyValue != null)
                        {
                            result.Append($"{itemProperty.Name}: {itemProperty.GetValue(propertyValue)}, ");
                        }
                    }
                }
                result.Remove(result.Length - 2, 2);// Remove the trailing comma and space
                result.AppendLine(" }");
            }
            else
            {
                // Handle non-collection properties
                result.AppendLine($"{property.GetValue(obj)}");
            }
        }

        //result.Remove(result.Length - 2, 2); // Remove the trailing comma and space
        //result.Append(" }");
        result.Append(new string('-', 50)); // Add a line of dashes as a separator

        return result.ToString();
    }
}
