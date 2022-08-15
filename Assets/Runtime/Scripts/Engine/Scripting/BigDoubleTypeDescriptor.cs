using System;
using System.Collections;
using System.Collections.Generic;
using BreakInfinity;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Scripting
{
    public class BigDoubleTypeDescriptor : StandardUserDataDescriptor
    {
        public BigDoubleTypeDescriptor(Type type, InteropAccessMode accessMode, string friendlyName = null) : base(type, accessMode, friendlyName)
        {
        }

        public override DynValue MetaIndex(Script script, object obj, string metaname)
        {
            switch(metaname)
            {
                case "__concat":
                    return DynValue.FromObject(script, (Func<DynValue, DynValue, string>)Concat);
                default:
                    return base.MetaIndex(script, obj, metaname);
            }
        }

        private string Concat(DynValue lhs, DynValue rhs)
        {
            return (lhs.Type == DataType.UserData ? lhs.ToObject().ToString() : lhs.String) +
                (rhs.Type == DataType.UserData ? rhs.ToObject().ToString() : rhs.String);
        }
    }
}