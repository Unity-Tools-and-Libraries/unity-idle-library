using System;

namespace IdleFramework
{
    public class ReservedEntityPropertyNameException : Exception
    {
        public ReservedEntityPropertyNameException(string propertyName) : base(string.Format("The property name {0} is reserved.", propertyName)) { }
    }
}