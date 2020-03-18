using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Numerics;
using System;
using BreakInfinity;

public class ResourceDisplay : MonoBehaviour
{
    public IdleFramework.GameEntity displayedResource;
    private Text resourceName;
    private Text resourceValue;
    private Text resourceProgress;
    private Text resourceIncome;
    private Button button;
    private IdleFramework.IdleEngine engine;
    void Start()
    {
        resourceName = transform.Find("Panel").Find("Name").GetComponent<Text>();
        resourceValue = transform.Find("Panel").Find("Value").GetComponent<Text>();
        resourceIncome = transform.Find("Panel").Find("Income").GetComponent<Text>();
        resourceProgress = transform.Find("Progress").GetComponent<Text>();
        
        button = GetComponent<Button>();
        engine = GameObject.Find("Engine").GetComponent<Engine>().framework;
        button.onClick.AddListener(() =>
        {
            displayedResource.Buy(1);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(displayedResource != null)
        {
            resourceName.text = displayedResource.Name;
            BigDouble effectiveQuantity = displayedResource.Quantity;
            BigDouble actualQuantity = displayedResource.RealQuantity;
            if(effectiveQuantity != actualQuantity)
            {
                resourceValue.text = String.Format("{0} ({1})", effectiveQuantity.ToString("F0"), actualQuantity.ToString("F0"));
            } else
            {
                resourceValue.text = displayedResource.Quantity.ToString("F0");
            }
            
            resourceProgress.text = (displayedResource.Progress).ToString("F2");
            BigDouble income = engine.GetProductionForEntity(displayedResource.EntityKey);
            resourceIncome.text = String.Format("({0}{1})", income >= 0 ? "+" : "", income.ToString("F4"));
            this.GetComponent<Button>().interactable = displayedResource.IsEnabled && displayedResource.CanBeBought;
        }
    }
}
