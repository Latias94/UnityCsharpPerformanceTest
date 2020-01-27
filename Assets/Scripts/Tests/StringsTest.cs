using System.Text;

namespace Tests
{
    public class StringsTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 5000;

        public StringsTest() : base("Strings", "A:string, B:StringBuilder, C:char pointer", DefaultRepetitions)
        {
        }

        protected override bool MeasureTestA()
        {
            var result = string.Empty;
            for (int i = 0; i < Iterations; i++)
            {
                result = result + "*";
            }

            return true;
        }

        /// <summary>
        /// StringBuilder 连接字符串
        /// </summary>
        protected override bool MeasureTestB()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Iterations; i++)
            {
                sb.Append("*");
            }

            return true;
        }

        /// <summary>
        /// 指针连接字符串
        /// </summary>
        protected override unsafe bool MeasureTestC()
        {
            var result = new char[Iterations];
            fixed (char* fixedPointer = result)
            {
                var pointer = fixedPointer;
                for (int i = 0; i < Iterations; i++)
                {
                    *pointer++ = '*';
                }
            }

            return true;
        }
    }
}