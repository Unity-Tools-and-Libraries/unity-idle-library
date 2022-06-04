using io.github.thisisnozaku.idle.framework;
using io.github.thisisnozaku.idle.framework.Definitions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDefinitionManager
{
    T GetDefinition<T>(string typeName, string id) where T: IDefinition;
    ICollection<T> GetDefinitions<T>(string typeName) where T : IDefinition;
    void SetDefinitions(string typeName, IDictionary<string, IDefinition> definitions);
}
