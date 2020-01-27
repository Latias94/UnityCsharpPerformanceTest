using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Tests
{
    /// <summary>
    /// 如果一个要实例化的类型只有在 runtime 的时候才知道，怎么才能最快
    /// 例如实例化的信息存在了 xml、xaml 等外部配置中，代码要按需实例化
    /// 又例如我们有一个很复杂的数据绑定要做，要将其转成代码
    /// </summary>
    public class InstantiationTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 100_000;

        public delegate object ConstructorDelegate();

        public InstantiationTest() : base("Instantiation", "A:reflection, B:dynamic CIL, C:compile-time",
            DefaultRepetitions)
        {
        }

        protected ConstructorDelegate GetConstructor(string typeName)
        {
            // 获得 type 的默认构造函数的引用
            Type t = Type.GetType(typeName);
            ConstructorInfo ctor = t.GetConstructor(new Type[0]);

            // 创建 dynamic method
            string methodName = t.Name + "Ctor";
            // api compatibility level 要到 .net 4.x
            DynamicMethod dm = new DynamicMethod(methodName, t, new Type[0], typeof(Activator));
            ILGenerator ilGenerator = dm.GetILGenerator();
            ilGenerator.Emit(OpCodes.Newobj, ctor);
            ilGenerator.Emit(OpCodes.Ret);

            ConstructorDelegate creator = (ConstructorDelegate) dm.CreateDelegate(typeof(ConstructorDelegate));
            return creator;
        }

        /// <summary>
        /// 通过反射实例化 StringBuilder
        /// </summary>
        protected override bool MeasureTestA()
        {
            var type = Type.GetType("System.Text.StringBuilder");
            for (int i = 0; i < Iterations; i++)
            {
                var obj = Activator.CreateInstance(type);
                if (obj.GetType() != typeof(StringBuilder))
                {
                    throw new InvalidOperationException("Constructed object is not a StringBuilder");
                }
            }

            return true;
        }

        /// <summary>
        /// 通过 dynamic CIL 实例化 StringBuilder
        /// </summary>
        protected override bool MeasureTestB()
        {
            var constructor = GetConstructor("System.Text.StringBuilder");
            for (int i = 0; i < Iterations; i++)
            {
                var obj = constructor();
                if (obj.GetType() != typeof(StringBuilder))
                {
                    throw new InvalidOperationException("Constructed object is not a StringBuilder");
                }
            }

            return true;
        }

        /// <summary>
        /// 直接实例化 StringBuilder
        /// </summary>
        protected override bool MeasureTestC()
        {
            for (int i = 0; i < Iterations; i++)
            {
                var obj = new StringBuilder(); // 这里其实类型已经在 compile time 知道了，但是将其作为基准测试例子
                if (obj.GetType() != typeof(StringBuilder))
                {
                    throw new InvalidOperationException("Constructed object is not a StringBuilder");
                }
            }

            return true;
        }
    }
}