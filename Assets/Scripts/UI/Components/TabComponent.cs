using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabComponent : MonoBehaviour
{
    public string tabText;
    private Text text;
    public int tabIndex;
    private int lastIndex;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(text != null)
        {
            text.text = tabText;
        }
        if(tabIndex != lastIndex)
        {
            throw new InvalidOperationException("The index of the component changed! This is not supported.");
        }
    }
}
