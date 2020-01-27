using System.Text;

namespace Tests
{
    /// <summary>
    /// buffer2 的数据复制到 buffer1
    /// </summary>
    public class MemoryTest : PerformanceTest
    {
        public new const int DefaultRepetitions = 500;
        private const int BufferSize = 100_000;

        private byte[] buffer1 = null;
        private byte[] buffer2 = null;

        public MemoryTest() : base("Byte array copy", "A:direct, B:with pointers, C:CopyTo", DefaultRepetitions)
        {
            buffer1 = new byte[BufferSize];
            buffer2 = new byte[BufferSize];
        }

        /// <summary>
        /// 一维数组的复制会被编译器优化，因此速度和用指针复制差不多
        /// </summary>
        protected override bool MeasureTestA()
        {
            for (int i = 0; i < Iterations; i++)
            {
                for (int j = 0; j < BufferSize; j++)
                {
                    buffer2[j] = buffer1[i];
                }
            }

            return true;
        }

        /// <summary>
        /// 使用指针来复制 buffer
        /// </summary>
        protected override unsafe bool MeasureTestB()
        {
            fixed (byte* fixed1 = &buffer1[0])
            fixed (byte* fixed2 = &buffer1[0])
            {
                for (int i = 0; i < Iterations; i++)
                {
                    var source = fixed1;
                    var dest = fixed2;
                    for (int j = 0; j < BufferSize; j++)
                    {
                        *dest++ = *source++;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 直接用 Array.CopyTo 复制，速度最快
        /// 系统底层直接复制一块内存，但注意这是 shadow copy，如果是引用类型的数组，并不会深复制。
        /// </summary>
        protected override bool MeasureTestC()
        {
            for (int i = 0; i < Iterations; i++)
            {
                buffer1.CopyTo(buffer2, 0);
            }

            return true;
        }
    }
}