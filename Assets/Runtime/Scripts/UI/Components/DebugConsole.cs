using System.Collections;
using UnityEngine;
using TMPro;
using io.github.thisisnozaku.idle.framework.Engine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.IO;

namespace io.github.thisisnozaku.idle.framework.ui.components
{
    public class DebugConsole : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField input;
        private IdleEngine engine;
        [SerializeField]
        private EventSystem eventSystem;
        private List<string> history = new List<string>();
        // Start is called before the first frame update
        void Start()
        {
            engine = GameObject.FindObjectOfType<EngineHolder>().engine;
            if(engine == null)
            {
                throw new InvalidDataException("Couldn't find the engine instance to wire to the console.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            input.Select();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                string command = input.text;
                input.text = "";
                history.Add(command);

                engine.Scripting.EvaluateStringAsScript(command);

            }
        }
    }
}
