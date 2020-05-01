using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelComponent : MonoBehaviour
{
    void Start()
    {
        var rt = transform as RectTransform;
        rt.anchoredPosition = Vector3.zero;
        rt.offsetMax = Vector3.zero;
        rt.offsetMin = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
