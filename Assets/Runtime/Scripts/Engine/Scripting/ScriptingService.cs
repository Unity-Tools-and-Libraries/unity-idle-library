using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Events;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using io.github.thisisnozaku.scripting;
using io.github.thisisnozaku.scripting.context;
using io.github.thisisnozaku.scripting.types;
using io.github.thisisnozaku.logging;
using io.github.thisisnozaku.idle.framework.Engine.State;

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
            Script.GlobalOptions.CustomConverters.Clear();
            this.engine = engine;
            module = new ScriptingModule(ScriptingModuleConfigurationFlag.DICTIONARY_WRAPPING);

            module.AddTypeAdapter(new scripting.types.TypeAdapter<IdleEngine>.AdapterBuilder<IdleEngine>().Build());
            module.AddTypeAdapter(new scripting.types.TypeAdapter<BigDouble>.AdapterBuilder<BigDouble>()
                .WithScriptConversion(DataType.Number, arg => BigDouble.Parse(arg.CastToString()))
                .WithScriptConversion(DataType.UserData, arg => {
                    var obj = arg.ToObject();
                    if (obj is BigDouble)
                    {
                        return (BigDouble)obj;
                    }
                    return null;
                })
                .WithScriptConversion(DataType.String, arg =>
                {
                    var converted = arg.CastToNumber();
                    if (converted != null)
                    {
                        return new BigDouble(converted.Value);
                    }
                    return BigDouble.NaN;
                })
                .WithScriptConversion(DataType.Nil, arg => BigDouble.Zero)
                .WithScriptConversion(DataType.Void, arg => BigDouble.Zero)
                .WithClrConversion((script, arg) => UserData.Create(arg))
                .Build());

            module.ContextCustomizer = ctx => {
                ctx.MetaTable = defaultMetatable;
                return ctx;
            };

            UserData.RegisterType(new BigDoubleTypeDescriptor(typeof(BigDouble), InteropAccessMode.Default));
            UserData.RegisterType<Type>();
            UserData.RegisterType<LoggingModule>();
            UserData.RegisterType<KeyValuePair<object, object>>();
            UserData.RegisterType<StateChangedEvent>();

            module.Globals["engine"] = engine;

            foreach (var builtIn in BuiltIns)
            {
                module.Globals[builtIn.Key] = builtIn.Value;
            }

            defaultMetatable = new Table(null);
            defaultMetatable.Set("__index", DynValue.NewCallback((Func<ScriptExecutionContext, CallbackArguments, DynValue>)((ctx, args) =>
            {
                var index = args[1];
                object output = ctx.CurrentGlobalEnv[args[1]];
                if(output == null && !engine.GlobalProperties.TryGetValue(args[1].CastToString(), out output))
                {
                    if(index.CastToString()  == "engine")
                    {
                        return DynValue.FromObject(ctx.GetScript(), engine);
                    }
                    return DynValue.Nil;
                }
                return DynValue.FromObject(ctx.GetScript(), output);
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

        public Table Globals => module.Globals;

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

        public void AddTypeAdaptor<T>(TypeAdapter<T> typeAdapter)
        {
            module.AddTypeAdapter(typeAdapter);
        }

        public DynValue EvaluateStringAsScript(string script, IDictionary<string, object> localContext = null)
        {
            return module.EvaluateStringAsScript(script, localContext);
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
    }
}