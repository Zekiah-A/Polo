using System;

namespace Polo.Exceptions;

internal class RuntimeErrorException : Exception
{
    public RuntimeErrorException(string message) : base(message)
    {
    }
}