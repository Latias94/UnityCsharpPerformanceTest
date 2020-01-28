using System.Diagnostics;
using UnityEngine.Profiling;

public class PerformanceResult
{
    public int MillisecondsA { get; set; }
    public int MillisecondsB { get; set; }
    public int MillisecondsC { get; set; }
    public int TickA { get; set; }
    public int TickB { get; set; }
    public int TickC { get; set; }
}

public class PerformanceTest
{
    public const int DefaultRepetitions = 10;
    public string Name { get; }
    public string Description { get; }
    public int Iterations { get; set; }
    public bool RunBaseline { get; set; }

    protected virtual bool MeasureTestA()
    {
        return false;
    }

    protected virtual bool MeasureTestB()
    {
        return false;
    }

    protected virtual bool MeasureTestC()
    {
        return false;
    }

    public PerformanceTest(string name, string description, int interactions)
    {
        Name = name;
        Description = description;
        Iterations = interactions;
    }

    public PerformanceResult Measure()
    {
        long msA = 0, msB = 0, msC = 0;
        long tickA = 0, tickB = 0, tickC = 0;

        var stopwatch = new Stopwatch();
        if (RunBaseline)
        {
            for (int i = 0; i < DefaultRepetitions; i++)
            {
                Profiler.BeginSample($"TestA run {DefaultRepetitions} Times");
                stopwatch.Restart();
                var implemented = MeasureTestA();
                stopwatch.Stop();
                Profiler.EndSample();

                if (implemented)
                {
                    msA += stopwatch.ElapsedMilliseconds;
                    tickA += stopwatch.ElapsedTicks;
                }
            }
        }

        for (int i = 0; i < DefaultRepetitions; i++)
        {
            Profiler.BeginSample($"TestB run {DefaultRepetitions} Times");
            stopwatch.Restart();
            var implemented = MeasureTestB();
            stopwatch.Stop();
            Profiler.EndSample();

            if (implemented)
            {
                msB += stopwatch.ElapsedMilliseconds;
                tickB += stopwatch.ElapsedTicks;
            }
        }

        for (int i = 0; i < DefaultRepetitions; i++)
        {
            Profiler.BeginSample($"TestC run {DefaultRepetitions} Times");
            stopwatch.Restart();
            var implemented = MeasureTestC();
            stopwatch.Stop();
            Profiler.EndSample();

            if (implemented)
            {
                msC += stopwatch.ElapsedMilliseconds;
                tickC += stopwatch.ElapsedTicks;
            }
        }

        PerformanceResult averageResult = new PerformanceResult
        {
            MillisecondsA = (int) (msA / DefaultRepetitions),
            MillisecondsB = (int) (msB / DefaultRepetitions),
            MillisecondsC = (int) (msC / DefaultRepetitions),
            TickA = (int) (tickA / DefaultRepetitions),
            TickB = (int) (tickB / DefaultRepetitions),
            TickC = (int) (tickC / DefaultRepetitions)
        };
        return averageResult;
    }
}