using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Tests
{
    /// <summary>
    /// Dynamic method 可以注入所有的 CIL 变成新的方法
    /// 这里拿来测试是否也能在属性的 getter setter 上用
    /// </summary>
    public class PropertiesTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 100_000;

        public delegate object PropertyGetDelegate(object obj);

        public delegate void PropertySetDelegate(object obj, object value);

        public PropertiesTest() : base("Property access", "A:reflection, B:dynamic CIL, C:compile-time",
            DefaultRepetitions)
        {
        }

        protected PropertyGetDelegate GetPropertyGetter(string typeName, string propertyName)
        {
            // 获得属性的 getter
            Type t = Type.GetType(typeName);
            PropertyInfo pi = t.GetProperty(propertyName);
            MethodInfo getter = pi.GetGetMethod();

            // 创建一个 dynamic method 来调用属性 getter
            DynamicMethod dm =
                new DynamicMethod("GetValue", typeof(object), new[] {typeof(object)}, typeof(object), true);
            ILGenerator ilGenerator = dm.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, getter);

            if (getter.ReturnType.GetTypeInfo().IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, getter.ReturnType);
            }

            ilGenerator.Emit(OpCodes.Ret);
            return dm.CreateDelegate(typeof(PropertyGetDelegate)) as PropertyGetDelegate;
        }

        protected PropertySetDelegate GetPropertySetter(string typeName, string propertyName)
        {
            // 获得属性的 setter
            Type t = Type.GetType(typeName);
            PropertyInfo pi = t.GetProperty(propertyName);
            MethodInfo setter = pi.GetSetMethod();

            // 创建一个 dynamic method 来调用属性 setter
            DynamicMethod dm =
                new DynamicMethod("SetValue", typeof(object), new[] {typeof(object)}, typeof(object), true);
            ILGenerator ilGenerator = dm.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);

            Type parameterType = setter.GetParameters()[0].ParameterType;

            if (setter.ReturnType.GetTypeInfo().IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox, parameterType);
            }

            ilGenerator.Emit(OpCodes.Call, setter);
            ilGenerator.Emit(OpCodes.Ret);
            return dm.CreateDelegate(typeof(PropertySetDelegate)) as PropertySetDelegate;
        }

        string str = "Frankorz";

        protected override bool MeasureTestA()
        {
            var sb = new StringBuilder(str);
            PropertyInfo pi = sb.GetType().GetProperty("Length");
            for (int i = 0; i < Iterations; i++)
            {
                var length = pi.GetValue(sb);
                if (!str.Length.Equals(length))
                    throw new InvalidOperationException($"Invalid length {length} returned");
            }

            return true;
        }

        protected override bool MeasureTestB()
        {
            var sb = new StringBuilder(str);
            var getter = GetPropertyGetter("System.Text.StringBuilder", "Length");
            for (int i = 0; i < Iterations; i++)
            {
                var length = getter(sb);
                if (!str.Length.Equals(length))
                    throw new InvalidOperationException($"Invalid length {length} returned");
            }

            return true;
        }

        protected override bool MeasureTestC()
        {
            var sb = new StringBuilder(str);
            for (int i = 0; i < Iterations; i++)
            {
                var length = sb.Length;
                if (!str.Length.Equals(length))
                    throw new InvalidOperationException($"Invalid length {length} returned");
            }

            return true;
        }
    }
}