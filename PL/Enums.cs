using System.Collections;

namespace PL;

internal class EngineerExperienceCollection : IEnumerable
{
    static readonly IEnumerable<BO.EngineerExperience> s_enums = (Enum.GetValues(typeof(BO.EngineerExperience)) as IEnumerable<BO.EngineerExperience>)!;

    IEnumerator IEnumerable.GetEnumerator() => s_enums.GetEnumerator();
}
