using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public interface ITraversableType
{
    IEnumerable GetTraversableFields();
}
