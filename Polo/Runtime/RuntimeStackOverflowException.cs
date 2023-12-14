namespace Polo.Runtime;

internal class RuntimeStackOverflowException : Exception
{
    public RuntimeStackOverflowException(string message)  : base(message)
    {
    }
}
