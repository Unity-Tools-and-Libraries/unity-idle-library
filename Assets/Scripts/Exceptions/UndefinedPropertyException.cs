using System;
namespace io.github.thisisnozaku.idle.framework
{
    public class UndefinedPropertyException : Exception
    {
        public UndefinedPropertyException(string propertyName, string contextName): 
            base(string.Format("The property '{0}' was not defined on '{1}'", propertyName, contextName)) { }
    }
}