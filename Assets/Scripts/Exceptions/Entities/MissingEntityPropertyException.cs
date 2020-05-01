using System;

namespace IdleFramework.Exceptions
{
    public class MissingEntityPropertyException : Exception
    {
        public MissingEntityPropertyException(string entityKey, string propertyName) : base(string.Format("Entity {0} is missing property {1}.", entityKey, propertyName)) { }
    }
}