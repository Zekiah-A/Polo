using System;

namespace Polo.Exceptions;

internal class ScanningErrorException : Exception
{
    public ScanningErrorException(string message) : base(message)
    {
    }
}