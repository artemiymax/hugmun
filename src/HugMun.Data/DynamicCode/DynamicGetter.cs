using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace HugMun.Data
{
    internal delegate void Getter<in TTarget, TValue>(TTarget target, ref TValue value);

    internal static class DynamicGetter
    {

        private static readonly Dictionary<Type, OpCode> OpStoreCodes = new Dictionary<Type, OpCode>
        {
            {typeof(float), OpCodes.Stind_R4},
            {typeof(double), OpCodes.Stind_R8},
            {typeof(sbyte), OpCodes.Stind_I1},
            {typeof(byte), OpCodes.Stind_I1},
            {typeof(short), OpCodes.Stind_I2},
            {typeof(ushort), OpCodes.Stind_I2},
            {typeof(int), OpCodes.Stind_I4},
            {typeof(uint), OpCodes.Stind_I4},
            {typeof(long), OpCodes.Stind_I8},
            {typeof(ulong), OpCodes.Stind_I8},
            {typeof(bool), OpCodes.Stind_I1}
        };

        private static void EmitTypeAssignment(this ILGenerator generator, Type type)
        {
            if (!OpStoreCodes.TryGetValue(type, out var code))
            {
                generator.Emit(OpCodes.Stobj, type);
            }
            else
            {
                generator.Emit(code);
            }
        }

        public static Delegate Generate<TTarget>(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            return DynamicCodeUtils.DelegateInvoke(Generate<int, int>, typeof(TTarget), property.PropertyType, property);
        }

        public static Delegate Generate<TTarget, TValue>(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            var getInfo = property.GetGetMethod();
            var getOpCode = (getInfo.IsVirtual || getInfo.IsAbstract) ? OpCodes.Callvirt : OpCodes.Call;

            Type[] args = { typeof(TTarget), typeof(TValue).MakeByRefType() };
            var method = new DynamicMethod("DynamicGetter", null, args, typeof(DynamicGetter), true);
            var ilGen = method.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.EmitCall(getOpCode, getInfo, null);
            ilGen.EmitTypeAssignment(typeof(TValue));
            ilGen.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Getter<TTarget, TValue>));
        }
    }
}
