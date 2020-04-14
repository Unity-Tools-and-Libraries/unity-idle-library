using System;

public class MissingEntityInstanceException : Exception
{
    public MissingEntityInstanceException(string entityType, string instanceKey) : base(string.Format("Entity instance \"{0}\" of type \"{1}\" is not defined", entityType, instanceKey)) { }
}
