using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System;

namespace io.github.thisisnozaku.idle.framework.Engine
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
                return c.GetProperty(id, IdleEngine.GetOperationType.GET_OR_CREATE);
            }
            set
            {
                c[id] = value;
            }
        }

        public DynValue Index(Script script, DynValue index, bool isDirectIndexing)
        {
            var argument = index.CastToString();
            switch(argument)
            {
                case "Path":
                    return DynValue.NewString(c.Path);
                case "Parent":
                    return DynValue.FromObject(script, c.Parent);
                case "AsNumber":
                    return DynValue.FromObject(script, c.AsNumber);
                case "AsString":
                    return DynValue.NewString(c.AsString);
                case "AsBool":
                    return DynValue.NewBoolean(c.AsBool);
                case "AsList":
                    return DynValue.FromObject(script, c.AsList);
                case "AsMap":
                    return DynValue.FromObject(script, c.AsMap);
                default:
                    return DynValue.FromObject(script, c.GetProperty(index.CastToString(), IdleEngine.GetOperationType.GET_OR_CREATE));
            }
        }

        private static DynValue DoAddStrings(string a, string b)
        {
            return DynValue.NewString(a + b);
        }

        public DynValue MetaIndex(Script script, string metaname)
        {
            switch (metaname)
            {
                case "__add":
                    return Operations.Add;
                case "__sub":
                    return Operations.Sub;
                case "__mul":
                    return Operations.Mul;
                case "__div":
                    return Operations.Div;
                case "__unm":
                    return Operations.Negate;
                case "__eq":
                    return Operations.Eq;
                case "__lt":
                    return Operations.Lt;
                case "__le":
                    return Operations.Le;
                case "__pow":
                    return Operations.Pow;
                default:
                    return null;
            }
        }

        public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
        {
            throw new NotImplementedException();
        }

        private static class Operations
        {
            public static DynValue Add = DynValue.NewCallback((ctx, args) =>
            {

                if (args[0].Type == DataType.UserData && args[0].UserData.Object.GetType() == typeof(ValueContainer))
                {
                    var leftContainer = args[0].ToObject<ValueContainer>();
                    switch (leftContainer.DataType)
                    {
                        case "string":
                            return DoAddStrings(args[0].ToObject<string>(), args[1].ToObject<string>());
                        case "number":
                            return DynValue.FromObject(ctx.GetScript(), leftContainer.ValueAsNumber() + args[1].ToObject<BigDouble>());
                    }
                }
                else if (args[0].Type == DataType.String)
                {
                    return DoAddStrings(args[0].String, args[1].Type == DataType.UserData ? args[1].ToObject<string>() : args[1].CastToString());
                }
                throw new InvalidOperationException();
            });
            public static DynValue Sub = DynValue.NewCallback((ctx, args) =>
            {
                BigDouble lhv = args[0].ToObject<BigDouble>();
                BigDouble rhv = args[1].ToObject<BigDouble>();
                return DynValue.FromObject(ctx.GetScript(), lhv - rhv);
            });
            public static DynValue Pow = DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
            BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), lhv.Pow(rhv));
                    });
            public static DynValue Mul = DynValue.NewCallback((ctx, args) =>
            {
                BigDouble lhv = args[0].ToObject<BigDouble>();
                BigDouble rhv = args[1].ToObject<BigDouble>();
                return DynValue.FromObject(ctx.GetScript(), lhv * rhv);
            });
            public static DynValue Div = DynValue.NewCallback((ctx, args) =>
            {
                BigDouble lhv = args[0].ToObject<BigDouble>();
                BigDouble rhv = args[1].ToObject<BigDouble>();
                return DynValue.FromObject(ctx.GetScript(), lhv / rhv);
            });
            public static DynValue Negate = DynValue.NewCallback((ctx, args) =>
            {
                BigDouble lhv = args[0].ToObject<BigDouble>();
                //BigDouble rhv = args[1].ToObject<BigDouble>();
                return DynValue.FromObject(ctx.GetScript(), -lhv);
            });
            public static DynValue Eq = DynValue.NewCallback((ctx, args) =>
            {
                BigDouble lhv = args[0].ToObject<BigDouble>();
                BigDouble rhv = args[1].ToObject<BigDouble>();
                return DynValue.FromObject(ctx.GetScript(), lhv == rhv);
            });
            public static DynValue Lt = DynValue.NewCallback((ctx, args) =>
            {
                BigDouble lhv = args[0].ToObject<BigDouble>();
                BigDouble rhv = args[1].ToObject<BigDouble>();
                return DynValue.FromObject(ctx.GetScript(), lhv < rhv);
            });
            public static DynValue Le = DynValue.NewCallback((ctx, args) =>
            {
                BigDouble lhv = args[0].ToObject<BigDouble>();
                BigDouble rhv = args[1].ToObject<BigDouble>();
                return DynValue.FromObject(ctx.GetScript(), lhv <= rhv);
            });
        }
    }
}