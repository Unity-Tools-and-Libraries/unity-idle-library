using IdleFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Steps;

public class LabelComponent : MonoBehaviour
{
    public ValueContainer toDisplay;
    private Text text;
    private IdleEngine engine;
    private StringContainer toDisplayString;
    private NumberContainer toDisplayNumber;
    private BooleanContainer toDisplayBoolean;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        toDisplayString = toDisplay as StringContainer;
        toDisplayNumber = toDisplay as NumberContainer;
        toDisplayBoolean = toDisplay as BooleanContainer;
    }

    // Update is called once per frame
    void Update()
    {
        if(toDisplayString != null)
        {
            text.text = toDisplayString.Get(engine);
        } else if (toDisplayNumber != null)
        {
            text.text = toDisplayNumber.Get(engine).ToString();
        } else if (toDisplayBoolean != null)
        {
            text.text = toDisplayBoolean.Get(engine).ToString();
        }
    }
}
