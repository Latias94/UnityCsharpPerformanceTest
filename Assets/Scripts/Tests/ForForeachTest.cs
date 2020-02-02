using System;
using System.Collections.Generic;

namespace Tests
{
    public class ForForeachTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 100;
        private const int ListSize = 100_000;

        private List<int> list = null;

        public ForForeachTest() : base("For/Foreach", "A:foreach, B:for, C store list length", DefaultRepetitions)
        {
            Random random = new Random();
            list = new List<int>(ListSize);
            for (int i = 0; i < ListSize; i++)
            {
                int number = random.Next(256);
                list.Add(number);
            }
        }

        /// <summary>
        /// 注意：Foreach 对数组进行迭代的话，编译器会优化成 for 循环而不调用 getEnumerator
        /// 另外 foreach 产生的垃圾可以参考：https://jacksondunstan.com/articles/3805
        /// IL2CPP 中的表现可以参考：https://jacksondunstan.com/articles/4573
        /// </summary>
        /// <returns></returns>
        protected override bool MeasureTestA()
        {
            for (int i = 0; i < Iterations; i++)
            {
                foreach (int number in list)
                {
                }
            }

            return true;
        }

        protected override bool MeasureTestB()
        {
            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    var number = list[j];
                }
            }

            return true;
        }

        /// <summary>
        /// 储存列表长度再迭代 这样能减少 list.Count 的调用
        /// list.Count 背后是函数调用
        /// callvirt     instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
        /// 此外对自动生成的属性（自动生成隐藏的 get set 方法）进行赋值或者获取都会有函数调用，在大循环中也要注意
        /// </summary>
        protected override bool MeasureTestC()
        {
            int length = list.Count;
            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    var number = list[j];
                }
            }

            return true;
        }
    }
}