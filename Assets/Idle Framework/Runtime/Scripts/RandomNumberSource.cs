using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface RandomNumberSource
{
    int RandomInt(int maxExclusive);
}
