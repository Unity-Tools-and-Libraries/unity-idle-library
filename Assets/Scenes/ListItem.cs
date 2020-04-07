using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Numerics;
using System;
using BreakInfinity;
using System.Text.RegularExpressions;

public class ListItem : MonoBehaviour
{
    private string textTemplate;
    public IdleFramework.GameEntity displayedResource;
    private Button button;
    private IdleFramework.IdleEngine engine;
    private List<string> itemProperties = new List<string>();
    private Text displayText;
    void Start()
    {
        textTemplate = transform.parent.gameObject.GetComponent<DisplayList>().textTemplate;
        displayText = transform.Find("Panel").Find("Value").gameObject.GetComponent<Text>();
        Regex extractor = new Regex("\\{(\\D+?)\\}");
        var matches = extractor.Matches(textTemplate);
        foreach(Match match in matches)
        {
            itemProperties.Add(match.Groups[1].Value);
        }
        for(int i = 0; i < itemProperties.Count; i++)
        {
            textTemplate = extractor.Replace(textTemplate, string.Format("{{{0}}}", i), 1);
        }
        button = GetComponent<Button>();
        engine = GameObject.Find("Canvas").GetComponent<Engine>().framework;
        button.onClick.AddListener(() => {
            engine.BuyEntity(displayedResource, 1, false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(displayedResource != null)
        {
            List<String> values = new List<String>();
            foreach(var property in itemProperties)
            {
                var value = displayedResource.GetPropertyAsString(property);
                values.Add(value);
            }
            var toDisplay = String.Format(textTemplate, values.ToArray());
            displayText.text = toDisplay;
            this.GetComponent<Button>().interactable = displayedResource.IsEnabled && displayedResource.IsAvailable;
        }
    }
}
