using System;

namespace Polo.Exceptions;

internal class ParsingErrorException : Exception
{
    public ParsingErrorException(string message) : base(message)
    {
    }
}