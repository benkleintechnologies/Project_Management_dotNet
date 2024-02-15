using System.Collections;

namespace PL;

internal class EngineerExperienceCollection : IEnumerable
{
    static readonly IEnumerable<BO.EngineerExperience> s_enums = (Enum.GetValues(typeof(BO.EngineerExperience)) as IEnumerable<BO.EngineerExperience>)!;

    IEnumerator IEnumerable.GetEnumerator() => s_enums.GetEnumerator();
}

internal class StatusCollection : IEnumerable
{
    static readonly IEnumerable<BO.Status> s_enums = (Enum.GetValues(typeof(BO.Status)) as IEnumerable<BO.Status>)!;

    IEnumerator IEnumerable.GetEnumerator() => s_enums.GetEnumerator();
}

internal class DateTypeCollection : IEnumerable
{
    //Implement to give an enum with values Hours, Days, Weeks, Months, Years. THis enum doesn't exist yet
    static readonly IEnumerable<DateType> s_enums = (Enum.GetValues(typeof(DateType)) as IEnumerable<DateType>)!;
    IEnumerator IEnumerable.GetEnumerator() => s_enums.GetEnumerator();
}

enum DateType
{
    Hours,
    Days,
    Weeks,
    Months,
    Years
}