using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.UI.Components {
    public class ButtonConfiguration : AbstractComponentConfiguration
    {
        private readonly StringContainer buttonText;
        private readonly List<Action<IdleEngine>> onClickActions = new List<Action<IdleEngine>>();
        public ButtonConfiguration(string componentid, StringContainer textContainer): this(componentid, textContainer, Always.Instance)
        {
        }

        public ButtonConfiguration(string componentid, StringContainer textContainer, StateMatcher enabledWhen) : base(componentid, enabledWhen)
        {
            this.buttonText = textContainer;
        }

        public ButtonConfiguration(string componentid, string text) : this(componentid, Literal.Of(text))
        {
            
        }
     
        public StringContainer ButtonText => buttonText;

        public List<Action<IdleEngine>> OnClickActions => onClickActions;

        public ButtonConfiguration OnClick(Action<IdleEngine> action)
        {
            onClickActions.Add(action);
            return this;
        }
    }
}