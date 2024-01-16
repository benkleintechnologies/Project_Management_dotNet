namespace Dal;

internal class Config
{
    static string s_data_config_xml = "data-config";
    internal static DateTime? _startDate { get; set; } = null;
    internal static DateTime? _endDate { get; set; } = null;
    internal static int NextDependencyId { get => XMLTools.GetAndIncreaseNextId(s_data_config_xml, "NextDependencyId"); }
    internal static int NextTaskId { get => XMLTools.GetAndIncreaseNextId(s_data_config_xml, "NextTaskId"); }
}
