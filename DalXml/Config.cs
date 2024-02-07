using System.Xml.Linq;

namespace Dal;
internal class Config
{
    static string s_data_config_xml = "data-config";
    internal static int NextDependencyId
    {
        get => XMLTools.GetAndIncreaseNextId(s_data_config_xml, "NextDependencyId");
        set
        {
            XElement root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.Element("NextDependencyId")?.SetValue(value.ToString() ?? "");
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }
    internal static int NextTaskId
    {
        get => XMLTools.GetAndIncreaseNextId(s_data_config_xml, "NextTaskId");
        set
        {
            XElement root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.Element("NextTaskId")?.SetValue(value.ToString() ?? "");
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static DateTime? StartDate
    {
        get => XMLTools.LoadListFromXMLElement(s_data_config_xml)
            .ToDateTimeNullable("StartDate");
        set
        {
            XElement root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.Element("StartDate")?.SetValue(value.HasValue ? value.Value.ToString("MM/dd/yyyy") : "");
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }

    internal static DateTime? EndDate
    {
        get => XMLTools.LoadListFromXMLElement(s_data_config_xml)
            .ToDateTimeNullable("EndDate");
        set
        {
            XElement root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            root.Element("EndDate")?.SetValue(value.HasValue ? value.Value.ToString("MM/dd/yyyy") : "");
            XMLTools.SaveListToXMLElement(root, s_data_config_xml);
        }
    }
    internal static void Reset()
    {
        StartDate = null;
        EndDate = null;
        NextTaskId = 0;
        NextDependencyId = 1000;
    }
}