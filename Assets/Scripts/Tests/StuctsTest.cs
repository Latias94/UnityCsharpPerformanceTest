using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    /// <summary>
    /// 对象的开销可以参考：https://www.red-gate.com/simple-talk/dotnet/net-framework/object-overhead-the-hidden-net-memory-allocation-cost/
    /// 32 位系统中，对象会存 4 字节的对象头字节和 4 字节的方法表指针，引用这个对象的地址也要占 4 个字节、也就是说空对象最少都要占 12 个字节
    /// 64 位系统中，对象会存 8 字节的对象头字节和 8 字节的方法表指针，引用这个对象的地址也要占 8 个字节、也就是说空对象最少都要占 24 个字节
    /// 下面这个例子中，32 位系统为例，除了对象头字节和方法表指针，两个 int 类型各占 4 字节，因此一个对象占 24 个字节
    /// </summary>
    public class PointClass
    {
        public int X { get; set; }
        public int Y { get; set; }

        public PointClass(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class PointClassFinalized : PointClass
    {
        public PointClassFinalized(int x, int y) : base(x, y)
        {
        }

        // 当定义了析构函数时，GC 会调用析构函数以此来释放（Dispose）对象
        ~PointClassFinalized()
        {
        }
    }

    public struct PointStruct
    {
        public int X { get; set; }
        public int Y { get; set; }

        public PointStruct(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// 例子参考：Structs versus Classes in C# https://mdfarragher.com/2017/12/01/structs-versus-classes-in-csharp/
    /// MS-Choosing Between Class and Struct
    /// https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
    /// </summary>
    public class StructTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 1_000_000;


        public StructTest() : base("Structs", "A:finalized class, B:class, C:struct", DefaultRepetitions)
        {
        }

        /// <summary>
        /// 用带有析构函数的子类测试
        /// </summary>
        protected override bool MeasureTestA()
        {
            var list = new PointClassFinalized[Iterations];
            for (int i = 0; i < Iterations; i++)
            {
                list[i] = new PointClassFinalized(i, i);
            }

            return true;
        }

        protected override bool MeasureTestB()
        {
            var list = new PointClass[Iterations];
            for (int i = 0; i < Iterations; i++)
            {
                list[i] = new PointClass(i, i);
            }

            return true;
        }

        protected override bool MeasureTestC()
        {
            var list = new PointStruct[Iterations];
            for (int i = 0; i < Iterations; i++)
            {
                list[i] = new PointStruct(i, i);
            }

            return true;
        }
    }
}