using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg;
using io.github.thisisnozaku.idle.framework.Engine.Persistence;
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace io.github.thisisnozaku.idle.framework.Engine.Scripting
{
    public class ScriptingService
    {
        private IdleEngine engine;
        private Table defaultMetatable;
        private Dictionary<string, object> BuiltIns = new Dictionary<string, object>()
        {
            { "math", new Dictionary<string, object>() {
                { "max", new CallbackFunction((ctx, args) => {
                    return DynValue.FromObject(ctx.GetScript(), BigDouble.Max(ScriptingService.DynValueToBigDouble(args[0]), ScriptingService.DynValueToBigDouble(args[1])));
                }) },
                { "min", new CallbackFunction((ctx, args) => {
                    return DynValue.FromObject(ctx.GetScript(), BigDouble.Min(ScriptingService.DynValueToBigDouble(args[0]), ScriptingService.DynValueToBigDouble(args[1])));
                }) },
                { "clamp", new CallbackFunction((ctx, args) => {
                    BigDouble value = ScriptingService.DynValueToBigDouble(args[0]);
                    BigDouble floor = ScriptingService.DynValueToBigDouble(args[1]);
                    BigDouble ceiling = ScriptingService.DynValueToBigDouble(args[2]);
                    return DynValue.FromObject(ctx.GetScript(), BigDouble.Max(floor, BigDouble.Min(ceiling, value)));
                }) },
                { "pow", new CallbackFunction((ctx, args) => {
                    BigDouble lhv = ScriptingService.DynValueToBigDouble(args[0]);
                    BigDouble rhv = ScriptingService.DynValueToBigDouble(args[1]);
                    return DynValue.FromObject(ctx.GetScript(), BigDouble.Pow(lhv, rhv));
                }) }
            }},
            { "table", new Dictionary<string, object>() {
                { "insert", (Action<DynValue, DynValue>)((tbl, value)=> {
                    if(tbl.Type != DataType.Table)
                    {
                        throw new InvalidOperationException("Tried to insert into a non-table");
                    }
                    tbl.Table.Append(value);
                }) }
            }},
            { "error", (Action<string>)((message) => throw new ScriptRuntimeException(message)) }
        };
        internal Script script;
        public Table Globals => script.Globals;
        public ScriptingService(IdleEngine engine)
        {
            this.engine = engine;
            script = new Script(CoreModules.Preset_HardSandbox ^ CoreModules.Math);

            UserData.RegisterType<IdleEngine>();
            UserData.RegisterType<BigDouble>();
            UserData.RegisterType<WrappedDictionary>();
            UserData.RegisterType<Type>();

            SetScriptToClrCustomConversion(DataType.Number, typeof(BigDouble), (arg) =>
            {
                return BigDouble.Parse(arg.CastToString());
            });
            SetScriptToClrCustomConversion(DataType.UserData, typeof(BigDouble), (arg) =>
            {
                var obj = arg.ToObject();
                if (obj is BigDouble)
                {
                    return (BigDouble)obj;
                }
                return null;
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
            SetClrToScriptCustomConversion(typeof(Dictionary<string, object>), (script, arg) =>
            {
                return DynValue.FromObject(script, new WrappedDictionary(arg as Dictionary<string, object>));
            });
            SetClrToScriptCustomConversion(typeof(IList), (script, arg) =>
            {
                return DynValue.FromObject(script, new WrappedDictionary(arg as IList));
            });

            ConfigureBuiltIns(engine);
            EngineAwareIndexMethod = DynValue.NewCallback((Func<ScriptExecutionContext, CallbackArguments, DynValue>)((ctx, args) =>
            {
                string locationArg = args[1].CastToString();
                object value;
                if (!BuiltIns.TryGetValue(locationArg, out value))
                {
                    engine.GlobalProperties.TryGetValue(locationArg, out value);
                }
                return DynValue.FromObject(null, value);
            }));
            EngineAwareIndexSetMethod = DynValue.NewCallback((Func<ScriptExecutionContext, CallbackArguments, DynValue>)((ctx, args) =>
            {
                string locationArg = args[1].CastToString();
                if (args[2].Type == DataType.Number)
                {
                    engine.GlobalProperties[locationArg] = new BigDouble(args[2].Number);
                }
                else
                {
                    engine.GlobalProperties[locationArg] = args[2].ToObject();
                }

                return DynValue.Nil;
            }));

            defaultMetatable = new Table(script);
            defaultMetatable.Set("__index", EngineAwareIndexMethod);
            defaultMetatable.Set("__newindex", EngineAwareIndexSetMethod);
        }

        private void ConfigureBuiltIns(IdleEngine engine)
        {
            BuiltIns["engine"] = engine;
        }

        public static BigDouble DynValueToBigDouble(DynValue dynValue)
        {
            switch(dynValue.Type)
            {
                case DataType.Nil:
                    return BigDouble.Zero;
                case DataType.UserData:
                    var userData = dynValue.UserData.Object;
                    if (userData is BigDouble)
                    {
                        return (BigDouble)userData;
                    }
                    break;
                case DataType.Number:
                    return new BigDouble(dynValue.Number);
            }
            throw new InvalidOperationException();
        }

        private DynValue EngineAwareIndexMethod;
        private DynValue EngineAwareIndexSetMethod;

        public Table SetupContext(IDictionary<string, object> contextVariables = null)
        {
            var contextTable = new Table(script);
            foreach (var global in engine.GlobalProperties)
            {
                contextTable[global.Key] = global.Value;
            }
            if (contextVariables != null)
            {
                foreach (var contextVariable in contextVariables)
                {
                    contextTable[contextVariable.Key] = contextVariable.Value;
                }
            }
            contextTable.MetaTable = defaultMetatable;
            contextTable["engine"] = engine;
            return contextTable;
        }

        public void SetClrToScriptCustomConversion(Type clrType, Func<Script, object, DynValue> converter)
        {
            Script.GlobalOptions.CustomConverters.SetClrToScriptCustomConversion(clrType, converter);
        }

        public void SetScriptToClrCustomConversion(DataType scriptDataType, Type clrDataType, Func<DynValue, object> converter)
        {
            Script.GlobalOptions.CustomConverters.SetScriptToClrCustomConversion(scriptDataType, clrDataType, converter);
        }

        public DynValue EvaluateStringAsScript(string script, IDictionary<string, object> localContext = null)
        {
            if (script == null)
            {
                throw new ArgumentNullException("script");
            }
            return Evaluate(DynValue.NewString(script), localContext);
        }
        public DynValue EvaluateStringAsScript(string script, KeyValuePair<string, object> localContext)
        {
            return EvaluateStringAsScript(script, new Dictionary<string, object>() {
                { localContext.Key, localContext.Value }});
        }
        public DynValue EvaluateStringAsScript(string script, Tuple<string, object> localContext)
        {
            return EvaluateStringAsScript(script, new Dictionary<string, object>() {
                { localContext.Item1, localContext.Item2 }});
        }

        public DynValue Evaluate(DynValue toEvaluate, ScriptingContext context)
        {
            return Evaluate(toEvaluate, context.GetScriptingProperties());
        }

        public DynValue Evaluate(DynValue toEvaluate, IDictionary<string, object> localContext = null)
        {
            if (toEvaluate == null)
            {
                throw new ArgumentNullException("valueExpression");
            }
            DynValue result;
            switch (toEvaluate.Type)
            {
                case DataType.String:
                    result = script.DoString(toEvaluate.String, SetupContext(localContext));
                    break;
                case DataType.ClrFunction:
                    result = script.Call(toEvaluate.Callback, SetupContext(localContext));
                    break;
                default:
                    throw new InvalidOperationException(String.Format("The DynValue must contains a string to interpret or a function to call, but was {0}", toEvaluate.Type));
            }
            if (result.Type == DataType.Number)
            {
                return DynValue.FromObject(null, new BigDouble(result.Number));
            }
            return result;
        }

        private class WrappedDictionary : IUserDataType, ITraversableType
        {
            private Dictionary<string, object> underlying;
            public WrappedDictionary(Dictionary<string, object> underlying)
            {
                this.underlying = underlying;
            }

            public DynValue Index(Script script, DynValue index, bool isDirectIndexing)
            {
                object value;
                underlying.TryGetValue(index.CastToString(), out value);
                return DynValue.FromObject(script, value);
            }

            public DynValue MetaIndex(Script script, string metaname)
            {
                throw new NotImplementedException();
            }

            public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
            {
                underlying[index.CastToString()] = value.ToObject();
                return true;
            }

            public IEnumerable GetTraversableFields()
            {
                return underlying.Values;
            }
        }
    }
}