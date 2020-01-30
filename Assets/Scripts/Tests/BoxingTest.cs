using System;
using System.Collections;
using System.Collections.Generic;

namespace Tests
{
    public class BoxingTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 100_000;
        private int[] numberArr = null;

        public BoxingTest() : base("Boxing", "A:object array, B:general type array", DefaultRepetitions)
        {
            Random random = new Random();
            numberArr = new int[Iterations];
            for (int i = 0; i < Iterations; i++)
            {
                int number = random.Next(256);
                numberArr[i] = number;
            }
        }

        protected override bool MeasureTestA()
        {
            Stack stack = new Stack(Iterations); // 设定大小以避免自动扩容带来的性能消耗
            for (int i = 0; i < Iterations; i++)
            {
                stack.Push(numberArr[i]); // int -> object 装箱
            }

            return true;
        }

        protected override bool MeasureTestB()
        {
            Stack<int> genericStack = new Stack<int>(Iterations);
            for (int i = 0; i < Iterations; i++)
            {
                genericStack.Push(numberArr[i]);
            }

            return true;
        }
    }
}