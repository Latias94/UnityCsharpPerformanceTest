using System.Text;

namespace Tests
{
    public class ArraysTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 300;

        public ArraysTest() : base("Arrays", "A:3_dimensional, B:1_dimensional, C:incremental", DefaultRepetitions)
        {
        }

        protected override bool MeasureTestA()
        {
            var array = new int[Iterations, Iterations, Iterations];
            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < Iterations; j++)
                {
                    for (int k = 0; k < Iterations; k++)
                    {
                        array[i, j, k]++;    
                    }
                }
            }

            return true;
        }

        protected override bool MeasureTestB()
        {
            var array = new int[Iterations * Iterations * Iterations];
            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < Iterations; j++)
                {
                    for (int k = 0; k < Iterations; k++)
                    {
                        var index = k + Iterations * (j + Iterations * i);
                        array[index]++;
                    }
                }
            }

            return true;
        }

        protected override bool MeasureTestC()
        {
            var array = new int[Iterations * Iterations * Iterations];
            var index = 0;
            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < Iterations; j++)
                {
                    for (int k = 0; k < Iterations; k++)
                    {
                        array[index]++;
                        index++;
                    }
                }
            }

            return true;
        }
    }
}