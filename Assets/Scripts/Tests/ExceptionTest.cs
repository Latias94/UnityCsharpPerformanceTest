using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class ExceptionTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 10;
        public const int ListSize = 1000;
        public const int NumberSize = 5;

        // X 会让我们将要创建的五位数在 parse 的时候导致异常发生
        private char[] digitArray = {'1', '2', '3', '4', '5', '6', '7', '8', '9', 'X'};
        private List<string> numbers = new List<string>();

        /// <summary>
        /// 异常出现的概率 1-0.9*0.9*0.9*0.9*0.9 = 41%
        /// </summary>
        public ExceptionTest() : base("Exceptions", "A:Parse, B:TryParse", DefaultRepetitions)
        {
            Random random = new Random();
            for (int i = 0; i < ListSize; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < NumberSize; j++)
                {
                    int index = random.Next(digitArray.Length);
                    sb.Append(digitArray[index]);
                }

                numbers.Add(sb.ToString());
            }
        }
        
        protected override bool MeasureTestA()
        {
            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < ListSize; j++)
                {
                    try
                    {
                        int.Parse(numbers[j]);
                    }
                    catch (FormatException)
                    {
                    }
                }
            }

            return true;
        }

        protected override bool MeasureTestB()
        {
            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < ListSize; j++)
                {
                    var success = int.TryParse(numbers[j], out int number);
                }
            }

            return true;
        }
    }
}