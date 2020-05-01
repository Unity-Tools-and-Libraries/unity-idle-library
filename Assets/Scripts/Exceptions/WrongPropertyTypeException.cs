using System;

namespace IdleFramework.Exceptions
{
    public class WrongPropertyTypeException : Exception
    {
        public WrongPropertyTypeException(string propertyName, string requestedType, string actualType): base(string.Format("Tried to get property {0} as a {1} but it's a {2}", propertyName, requestedType, actualType)) { }

    }
}