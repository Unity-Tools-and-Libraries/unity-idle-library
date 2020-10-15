using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Watchable
{
    void Watch(Action<object> listener);
}
