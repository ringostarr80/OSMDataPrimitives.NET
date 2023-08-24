using System;

namespace OSMDataPrimitives;

public class DataException : Exception
{
    public DataException() { }
    public DataException(string message) : base(message) { }
    public DataException(string message, System.Exception inner) : base(message, inner) { }
}