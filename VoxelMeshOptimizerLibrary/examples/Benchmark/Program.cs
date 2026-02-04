using Benchmark;

namespace ConsoleAppExample;
using BenchmarkDotNet.Running;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<SpeedBenchmarks>();
        BenchmarkRunner.Run<TasksBenchmark>();
    }
}

