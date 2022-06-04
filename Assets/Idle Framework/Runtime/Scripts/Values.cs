using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BreakInfinity;

namespace io.github.thisisnozaku.idle.framework
{
    public static class Values
    {
        public static bool IsDictionary(object value)
        {
            if(value == null)
            {
                return false;
            }
            var type = value.GetType();
            return new Type[] {
                typeof(IDictionary<,>),
                typeof(System.Collections.IDictionary),
                typeof(IReadOnlyDictionary<,>)
            }.Any(dictInterface => {
                bool exactMatch = dictInterface == type;
                bool genericMatch = type.IsGenericType && dictInterface == type.GetGenericTypeDefinition();
                bool interfaceMatch = type.GetInterfaces().Any(typeInterface => typeInterface == dictInterface ||
                    (typeInterface.IsGenericType && dictInterface == typeInterface.GetGenericTypeDefinition()));
                return exactMatch || genericMatch || interfaceMatch;
            });
        }

        public static bool IsNumber(object value)
        {
            return value is float || value is double || value is int || value is long || value is BigDouble || value is decimal;
        }
    }
}