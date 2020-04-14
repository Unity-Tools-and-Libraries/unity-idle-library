using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFramework.UI.Events.Payloads
{
    public class OpenPopupPayload
    {

        /*
         * The title to display.
         */
        private readonly string title;
        /*
         * The body text to display
         */
        private readonly string body;

        public OpenPopupPayload(string title, string body)
        {
            this.title = title;
            this.body = body;
        }

        public string Title => title;

        public string Body => body;
    }
}