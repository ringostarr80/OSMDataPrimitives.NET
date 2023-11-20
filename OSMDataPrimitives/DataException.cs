using System;

namespace OSMDataPrimitives;

/// <summary>
/// A Custom DataException-class.
/// </summary>
public class DataException : Exception
{
    /// <summary>
    /// DataException constructor.
    /// </summary>
    public DataException() { }
    /// <summary>
    /// DataException constructor.
    /// </summary>
    public DataException(string message) : base(message) { }
    /// <summary>
    /// DataException constructor.
    /// </summary>
    public DataException(string message, Exception inner) : base(message, inner) { }
}