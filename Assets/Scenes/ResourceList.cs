using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleFramework;
using UnityEngine.UI;

public class ResourceList : MonoBehaviour
{
    private Dictionary<string, GameObject> listItems = new Dictionary<string, GameObject>();
    private IdleEngine engine;
    // Start is called before the first frame update
    void Start()
    {
        engine = GameObject.Find("Engine").GetComponent<Engine>().framework;
        foreach(GameEntity resource in engine.AllEntities.Values)
        {
            if(!resource.Types.Contains("resource"))
            {
                continue;
            }
            var display = Resources.Load<GameObject>("ResourceDisplay");
            var instantiated = GameObject.Instantiate(display);
            instantiated.transform.SetParent(this.transform);
            instantiated.GetComponent<ResourceDisplay>().displayedResource = resource;
            listItems.Add(resource.EntityKey, instantiated);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameEntity resource in engine.AllEntities.Values)
        {
            if (!resource.Types.Contains("resource"))
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
