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
            switch (metaname)
            {
                case "__add":
                    return DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
                        BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), lhv + rhv);
                    });
                case "__sub":
                    return DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
                        BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), lhv - rhv);
                    });
                case "__mul":
                    return DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
                        BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), lhv * rhv);
                    });
                case "__div":
                    return DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
                        BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), lhv / rhv);
                    });
                case "__mod":
                    return null;
                case "__unm":
                    return DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
                        //BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), -lhv);
                    });
                case "__eq":
                    return DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
                        BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), lhv == rhv);
                    });
                case "__lt":
                    return DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
                        BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), lhv < rhv);
                    });
                case "__le":
                    return DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
                        BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), lhv <= rhv);
                    });
                case "__len":
                    return null;
                case "__pow":
                    return DynValue.NewCallback((ctx, args) =>
                    {
                        BigDouble lhv = args[0].ToObject<BigDouble>();
                        BigDouble rhv = args[1].ToObject<BigDouble>();
                        return DynValue.FromObject(ctx.GetScript(), lhv.Pow(rhv));
                    });
                default:
                    return null;
            }
        }

        public bool SetIndex(Script script, DynValue index, DynValue value, bool isDirectIndexing)
        {
            throw new NotImplementedException();
        }
    }
}