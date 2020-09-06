using System;
using System.Reflection;
using HugMun.Core;

namespace HugMun.Data
{
    internal static class DynamicCodeUtils
    {
        public static Delegate GenerateGetter<TTarget>(AttributeDefinition attribute)
        {
            if (!(attribute.MemberInfo is PropertyInfo propertyInfo)) throw new InvalidOperationException("Expected PropertyInfo");
            return DynamicGetter.Generate<TTarget>(propertyInfo);
        }

        public static Delegate GenerateSetter<TTarget>(AttributeDefinition attribute)
        {
            if (!(attribute.MemberInfo is PropertyInfo propertyInfo)) throw new InvalidOperationException("Expected PropertyInfo");
            return DynamicSetter.Generate<TTarget>(propertyInfo);
        }

        internal static TResult DelegateInvoke<TArg, TResult>(Func<TArg, TResult> function, Type type1, Type type2, TArg arg)
        {
            return (TResult)function
                .GetMethodInfo()
                .GetGenericMethodDefinition()
                .MakeGenericMethod(type1, type2)
                .Invoke(null, new object[] { arg });
        }

        internal static TResult DelegateInvoke<TArg, TResult>(Func<TArg, TResult> function, Type type1, TArg arg, object target)
        {
            return (TResult)function
                .GetMethodInfo()
                .GetGenericMethodDefinition()
                .MakeGenericMethod(type1)
                .Invoke(target, new object[] { arg });
        }
    }
}
