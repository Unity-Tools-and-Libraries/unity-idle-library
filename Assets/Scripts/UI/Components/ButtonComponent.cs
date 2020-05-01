using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleFramework;
using UnityEngine.UI;
using System;

public class ButtonComponent : MonoBehaviour
{
    public StringContainer text;
    private IdleEngine engine;
    private Text buttonText;
    private Button button;
    public List<Action<IdleEngine>> onClickActions;
    private StateMatcher enabledWhen;
    void Start()
    {
        buttonText = GetComponentInChildren<Text>();
        button = GetComponent<Button>();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            try
            {
                onClickActions.ForEach(a => a.Invoke(engine));
            } catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        });
        
        
    }

    // Update is called once per frame
    void Update()
    {
        buttonText.text = text.Get(engine);
        if(enabledWhen.Matches(engine)) {
            button.interactable = true;
        } else
        {
            button.interactable = false;
        }
    }
}
