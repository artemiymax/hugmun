using System;
using System.Reflection;
using System.Reflection.Emit;

namespace HugMun.Data
{
    internal delegate void Setter<TTarget, TValue>(TTarget target, TValue value);

    internal static class DynamicSetter
    {

        public static Delegate Generate<TTarget>(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            return DynamicCodeUtils.DelegateInvoke(Generate<int, int>, typeof(TTarget), property.PropertyType, property);
        }

        public static Delegate Generate<TTarget, TValue>(PropertyInfo property)
        {
            var setInfo = property.GetSetMethod();
            var setOpCode = (setInfo.IsVirtual || setInfo.IsAbstract) ? OpCodes.Callvirt : OpCodes.Call;

            Type[] args = { typeof(TTarget), typeof(TValue) };
            var method = new DynamicMethod("DynamicSetter", null, args, typeof(DynamicSetter), true);
            var ilGen = method.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.EmitCall(setOpCode, setInfo, null);
            ilGen.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Setter<TTarget, TValue>));
        }
    }
}
