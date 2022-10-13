using System;

namespace DudCo.Events
{
    /// <summary>
    /// A request to send an event was made before the previous one finished. this could be due to recursion or multithreadding.
    /// </summary>
    public sealed class ConcurrentSendException : Exception
    {
         internal ConcurrentSendException() 
            : base("A request to send an event was made before the previous one finished. this could be due to recursion or multithreadding.")
        {
        }
    }
}