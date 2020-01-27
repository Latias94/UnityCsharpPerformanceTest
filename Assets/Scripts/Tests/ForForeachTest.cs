using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class ForForeachTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 100;
        private const int ListSize = 1_000_00;

        private List<int> list = null;

        public ForForeachTest() : base("For/Foreach", "A:foreach, B:for", DefaultRepetitions)
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
    }
}