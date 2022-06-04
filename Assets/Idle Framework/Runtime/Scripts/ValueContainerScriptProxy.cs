using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System;

namespace io.github.thisisnozaku.idle.framework
{
    public partial class IdleEngine
    {
        public class ValueContainerScriptProxy : IUserDataType
        {
            private ValueContainer c;

            public ValueContainerScriptProxy(ValueContainer c)
            {
                this.c = c;
            }

            public ValueContainer this[string id]
            {
                get
                {
                    return c[id];
                }
                set
                {
                    c[id] = value;
                }
            }

            public DynValue Index(Script script, DynValue index, bool isDirectIndexing)
            {
                return DynValue.FromObject(script, this[index.CastToString()]);
            }

            public DynValue MetaIndex(Script script, string metaname)
            {
                throw new NotImplementedException();
            }

            public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
            {
                throw new NotImplementedException();
            }
        }
    }
}