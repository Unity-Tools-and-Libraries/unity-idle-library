using BreakInfinity;
using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace io.github.thisisnozaku.idle.framework.Engine.Scripting
{
    public class ScriptingService
    {
        internal Script script;
        private IdleEngine engine;
        public ScriptingService(IdleEngine engine)
        {
            script = new Script(CoreModules.Preset_HardSandbox);

            UserData.RegisterType<IdleEngine>();
            UserData.RegisterProxyType<ValueContainerScriptProxy, ValueContainer>(c => new ValueContainerScriptProxy(c));
            UserData.RegisterType<BigDouble>();
            UserData.RegisterType<ParentNotifyingList>();
            UserData.RegisterType<ParentNotifyingDictionary>();
            SetScriptToClrCustomConversion(DataType.Number, typeof(BigDouble), (arg) =>
            {
                return BigDouble.Parse(arg.CastToString());
            });
            SetScriptToClrCustomConversion(DataType.UserData, typeof(BigDouble), (arg) =>
            {
                var obj = arg.ToObject();
                if (obj is ValueContainer)
                {
                    return (obj as ValueContainer).ValueAsNumber();
                }
                else if (obj is BigDouble)
                {
                    return (BigDouble)obj;
                }
                else
                {
                    return null;
                }
            });
            SetScriptToClrCustomConversion(DataType.UserData, typeof(string), (arg) =>
            {
                var obj = arg.ToObject();
                if (obj is ValueContainer)
                {
                    return (obj as ValueContainer).ValueAsString();
                }
                else
                {
                    return obj.ToString();
                }
            });
            SetScriptToClrCustomConversion(DataType.String, typeof(BigDouble), (arg) =>
            {
                var converted = arg.CastToNumber();
                if (converted != null)
                {
                    return new BigDouble(converted.Value);
                }
                return BigDouble.NaN;
            });
            SetScriptToClrCustomConversion(DataType.Void, typeof(BigDouble), (arg) =>
            {
                return BigDouble.Zero;
            });
            SetScriptToClrCustomConversion(DataType.Nil, typeof(BigDouble), (arg) =>
            {
                return BigDouble.Zero;
            });
            SetClrToScriptCustomConversion(typeof(BigDouble), (script, arg) =>
            {
                return UserData.Create(arg);
            });
            SetClrToScriptCustomConversion(typeof(ValueContainer), (script, arg) =>
            {
                return UserData.Create(arg);
            });
            this.engine = engine;
        }

        public static BigDouble DynValueToBigDouble(DynValue dynValue)
        {
            if (dynValue.CastToNumber().HasValue)
            {
                return new BigDouble(dynValue.Number);
            }
            else if (dynValue.UserData != null)
            {
                var userData = dynValue.UserData.Object;
                if (userData is ValueContainer)
                {
                    return (userData as ValueContainer).AsNumber;
                }
                else if (userData is BigDouble)
                {
                    return (BigDouble)userData;
                }
            }
            throw new InvalidOperationException();
        }

        private DynValue GlobalIndexMethod;

        public Table GenerateContextTable(IDictionary<string, object> contextVariables = null)
        {
            var contextTable = new Table(script);
            foreach (var global in engine.GetScriptingContext())
            {
                contextTable[global.Key] = global.Value;
            }
            if (contextVariables != null)
            {
                foreach (var contextVariable in contextVariables)
                {
                    contextTable.Set(contextVariable.Key, DynValue.FromObject(script, contextVariable.Value));
                }
            }
            contextTable["engine"] = engine;
            return contextTable;
        }

        public void SetClrToScriptCustomConversion(Type clrType, Func<object, object, DynValue> converter)
        {
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion(clrType, converter);
        }

        public void SetScriptToClrCustomConversion(DataType scriptDataType, Type clrDataType, Func<DynValue, object> converter)
        {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(scriptDataType, clrDataType, converter);
        }

        public DynValue DoString(string expression, IDictionary<string, object> context = null)
        {
            return DoString(expression, GenerateContextTable(context));
        }

        public DynValue DoString(string expression, Table context = null)
        {
            return script.DoString(expression, context);
        }
    }
}