using UnityEngine;
using TMPro;
using io.github.thisisnozaku.idle.framework.Engine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using MoonSharp.Interpreter;

namespace io.github.thisisnozaku.idle.framework.ui.components
{
    public class DebugConsole : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField input;
        [SerializeField]
        private TMP_Text historyDisplay;
        private IdleEngine engine;
        private List<string> messageHistory = new List<string>();
        private List<string> commandHistory = new List<string>();
        private int historyCursor = -1;
        [SerializeField]
        private CanvasGroup canvasGroup;
        // Start is called before the first frame update
        void Start()
        {
            var holder = GameObject.FindObjectOfType<EngineHolder>();
            if(!holder)
            {
                throw new InvalidDataException("Couldn't find the engine instance to wire to the console.");
            }
            engine = GameObject.FindObjectOfType<EngineHolder>().engine;
            if(engine == null)
            {
                throw new InvalidDataException("Couldn't find the engine instance to wire to the console.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                if (canvasGroup.interactable)
                {
                    canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
                    canvasGroup.alpha = 0f;
                }
                else
                {
                    canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
                    canvasGroup.alpha = 1f;
                }
                
                if (input.text.EndsWith("`"))
                {
                    input.text = input.text.Substring(0, input.text.Length - 1);
                }
                
            } else {
                if (canvasGroup.interactable)
                {
                    input.ActivateInputField();
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        try
                        {
                            string command = input.text;
                            input.text = "";
                            commandHistory.Add(command);
                            messageHistory.Add(command);

                            var result = engine.Scripting.EvaluateStringAsScript(command);
                            messageHistory.Add(result.ToString());
                        } catch (Exception ex)
                        {
                            messageHistory.Add(ex.ToString()); 
                        }
                    } else if(Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if(commandHistory.Count > 0 && historyCursor < commandHistory.Count - 1)
                        {
                            historyCursor++;
                            input.text = commandHistory[commandHistory.Count - 1 - historyCursor];
                        }
                    } else if(Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        if (commandHistory.Count > 0 && historyCursor > -1)
                        {
                            historyCursor--;
                        }
                        if(historyCursor == -1)
                        {
                            input.text = "";
                        } else
                        {
                            input.text = commandHistory[commandHistory.Count - 1 - historyCursor];
                        }
                    }
                }
            }
            historyDisplay.text = string.Join(Environment.NewLine, messageHistory);
        }
    }
}
