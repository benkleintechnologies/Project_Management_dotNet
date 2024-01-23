﻿namespace BO;

[Serializable]
public class BlDoesNotexistException : Exception
{
    public BlDoesNotexistException(string? message) : base(message) { }
}

public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
}

public class BlInvalidInputException : Exception
{
    public BlInvalidInputException(string? message) : base(message) { }
}

public class BlCannotBeDeletedException : Exception
{
    public BlCannotBeDeletedException(string? message) : base(message) { }
}

public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}