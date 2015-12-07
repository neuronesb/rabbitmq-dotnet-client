using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class ThreadInterruptedException : Exception
    {
    }


    /// <summary>Supports cloning, which creates a new instance of a class with the same value as an existing instance.</summary>
    /// <filterpriority>2</filterpriority>
    public interface ICloneable
    {
        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        /// <filterpriority>2</filterpriority>
        object Clone();
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    public sealed class SerializableAttribute : Attribute
    {
    }
}
