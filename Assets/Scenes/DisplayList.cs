using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleFramework;
using UnityEngine.UI;

public class DisplayList : MonoBehaviour
{
    public string textTemplate;
    private Dictionary<string, GameObject> listItems = new Dictionary<string, GameObject>();
    public string categoryToDisplay;
    private IdleEngine engine;
    // Start is called before the first frame update
    void Start()
    {
        engine = GameObject.Find("Canvas").GetComponent<ShadowSpiritsDemoEngine>().framework;
        foreach(GameEntity resource in engine.AllEntities.Values)
        {
            if(!resource.Types.Contains(categoryToDisplay))
            {
                continue;
            }
            var display = Resources.Load<GameObject>("Item");
            var instantiated = GameObject.Instantiate(display);
            instantiated.transform.SetParent(this.transform);
            instantiated.GetComponent<ListItem>().displayedResource = resource;
            listItems.Add(resource.EntityKey, instantiated);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameEntity resource in engine.AllEntities.Values)
        {
            if (!resource.Types.Contains(categoryToDisplay))
            {
                continue;
            }
            var item = listItems[resource.EntityKey];
            if(resource.ShouldBeHidden(engine))
            {
                item.transform.SetParent(null);
                item.transform.localScale = Vector3.zero;
            } else
            {
                item.transform.SetParent(transform);
                item.transform.localScale = Vector3.one;
                if(resource.ShouldBeDisabled(engine))
                {
                    item.GetComponent<Button>().interactable = false;
                } else
                {
                    item.GetComponent<Button>().interactable = true;

                }
            }
        }
    }
}
