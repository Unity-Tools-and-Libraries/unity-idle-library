using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using io.github.thisisnozaku.idle.framework.Engine.Logging;
using io.github.thisisnozaku.scripting;
using io.github.thisisnozaku.scripting.context;

namespace io.github.thisisnozaku.idle.framework.Engine.Scripting
{
    public class ScriptingService
    {
        private ScriptingModule module;
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
                }) },
                { "ceil", new CallbackFunction((ctx, args) =>
                {
                    return DynValue.FromObject(null, BigDouble.Ceiling(DynValueToBigDouble(args[0])));
                }) },
                { "floor", new CallbackFunction((ctx, args) =>
                {
                    return DynValue.FromObject(null, BigDouble.Floor(DynValueToBigDouble(args[0])));
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
        public ScriptingService(IdleEngine engine)
        {
            this.engine = engine;
            module = new ScriptingModule(ScriptingModuleConfigurationFlag.DICTIONARY_WRAPPING);

            module.AddTypeAdapter(new scripting.types.TypeAdapter<IdleEngine>.AdapterBuilder<IdleEngine>().Build());
            module.AddTypeAdapter(new scripting.types.TypeAdapter<IdleEngine>.AdapterBuilder<IdleEngine>().Build());

            module.ContextCustomizer = ctx => {

                return ctx;
            };

            UserData.RegisterType(new BigDoubleTypeDescriptor(typeof(BigDouble), InteropAccessMode.Default));
            UserData.RegisterType<WrappedDictionary>();
            UserData.RegisterType<Type>();
            UserData.RegisterType<LoggingService>();
            UserData.RegisterType<KeyValuePair<object, object>>();

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
            SetClrToScriptCustomConversion(typeof(IDictionary), (script, arg) =>
            {
                return DynValue.FromObject(script, new WrappedDictionary(arg as IDictionary));
            });
            SetClrToScriptCustomConversion(typeof(Dictionary<string, object>), (script, arg) =>
            {
                return DynValue.FromObject(script, new WrappedDictionary(arg as Dictionary<string, object>));
            });

            ConfigureBuiltIns(engine);

            defaultMetatable = new Table(null);
            defaultMetatable.Set("__index", DynValue.NewCallback((Func<ScriptExecutionContext, CallbackArguments, DynValue>)((ctx, args) =>
            {
                string locationArg = args[1].CastToString();
                object value;
                if (!BuiltIns.TryGetValue(locationArg, out value))
                {
                    engine.GlobalProperties.TryGetValue(locationArg, out value);
                }
                return DynValue.FromObject(null, value);
            })));
            defaultMetatable.Set("__newindex", DynValue.NewCallback((Func<ScriptExecutionContext, CallbackArguments, DynValue>)((ctx, args) =>
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
            })));
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

        public DynValue Evaluate(DynValue toEvaluate, IScriptingContext context)
        {
            return module.Evaluate(toEvaluate, context);
        }

        public DynValue Evaluate(DynValue toEvaluate, IDictionary<string, object> localContext = null)
        {
            return module.Evaluate(toEvaluate, localContext);
        }

        /*
        private class WrappedDictionary : IUserDataType, ITraversableType
        {
            private IDictionary underlying;
            public WrappedDictionary(IDictionary underlying)
            {
                this.underlying = underlying;
            }

            public DynValue Index(Script script, DynValue index, bool isDirectIndexing)
            {
                if(underlying.Contains(index.ToObject()))
                {
                    return DynValue.FromObject(script, underlying[index.ToObject()]);
                }
                return DynValue.Nil;
                
            }

            public DynValue MetaIndex(Script script, string metaname)
            {
                throw new NotImplementedException();
            }

            public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
            {
                underlying[index.ToObject()] = value.ToObject();
                return true;
            }

            public IEnumerable GetTraversableFields()
            {
                return underlying.Values;
            }
        }
    */
    }
}