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
            XElement _root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            _root.Element("NextDependencyId")?.SetValue(value.ToString() ?? "");
            XMLTools.SaveListToXMLElement(_root, s_data_config_xml);
        }
    }
    internal static int NextTaskId
    {
        get => XMLTools.GetAndIncreaseNextId(s_data_config_xml, "NextTaskId");
        set
        {
            XElement _root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            _root.Element("NextTaskId")?.SetValue(value.ToString() ?? "");
            XMLTools.SaveListToXMLElement(_root, s_data_config_xml);
        }
    }

    internal static DateTime? StartDate
    {
        get => XMLTools.LoadListFromXMLElement(s_data_config_xml)
            .ToDateTimeNullable("StartDate");
        set
        {
            XElement _root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            _root.Element("StartDate")?.SetValue(value.HasValue ? value.Value.ToString("dd/MM/yyyy") : "");
            XMLTools.SaveListToXMLElement(_root, s_data_config_xml);
        }
    }

    internal static DateTime? EndDate
    {
        get => XMLTools.LoadListFromXMLElement(s_data_config_xml)
            .ToDateTimeNullable("EndDate");
        set
        {
            XElement _root = XMLTools.LoadListFromXMLElement(s_data_config_xml);
            _root.Element("EndDate")?.SetValue(value.HasValue ? value.Value.ToString("dd/MM/yyyy") : "");
            XMLTools.SaveListToXMLElement(_root, s_data_config_xml);
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