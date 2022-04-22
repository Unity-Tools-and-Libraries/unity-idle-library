using System;

namespace io.github.thisisnozaku.idle.framework.Exceptions
{
    public class UndefinedEntityException : Exception
    {
        public UndefinedEntityException(string entityType) : base(string.Format("The entity type %s is not defined", entityType))
        {

        }
    }
}