using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IdleFramework
{
    public class FormattedString : StringContainer
    {
        private ValueContainer[] arguments;
        private StringContainer templateString;

        public FormattedString(StringContainer templateString, params ValueContainer[] arguments)
        {
            this.arguments = arguments;
            this.templateString = templateString;
        }

        public FormattedString(string templateString, params ValueContainer[] arguments): this(Literal.Of(templateString), arguments)
        {
        }

        public string Get(IdleEngine engine)
        {
            var template = templateString.Get(engine);
            string[] arguments = this.arguments.Select(a => {
                if(a is NumberContainer)
                {
                    return a.AsNumber().Get(engine).ToString();
                } else if(a is BooleanContainer)
                {
                    return a.AsBoolean().Get(engine).ToString();
                }
                return a.AsString().Get(engine);
            }).ToArray();
            return string.Format(template, arguments);
        }
    }
}