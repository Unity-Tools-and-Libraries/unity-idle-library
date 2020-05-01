using System;

namespace IdleFramework.Exceptions
{
    public class MissingGlobalPropertyException : Exception
    {
        public MissingGlobalPropertyException(string propertyName) : base(string.Format("Global property {0} is not defined", propertyName)) { }
    }
}