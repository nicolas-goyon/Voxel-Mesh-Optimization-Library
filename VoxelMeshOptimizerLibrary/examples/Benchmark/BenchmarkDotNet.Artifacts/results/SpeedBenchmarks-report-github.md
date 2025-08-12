```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.2 LTS (Noble Numbat) (container)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical cores and 1 physical core
.NET SDK 8.0.412
  [Host]     : .NET 8.0.18 (8.0.1825.31117), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Job-CNUJVU : .NET 8.0.18 (8.0.1825.31117), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

InvocationCount=1  UnrollFactor=1  

```
| Method    | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0      | Gen1      | Gen2      | Allocated | Alloc Ratio |
|---------- |---------:|---------:|---------:|---------:|------:|--------:|----------:|----------:|----------:|----------:|------------:|
| Default   | 14.09 ms | 1.131 ms | 3.282 ms | 12.94 ms |  1.05 |    0.32 | 1000.0000 | 1000.0000 | 1000.0000 |  31.78 MB |        1.00 |
| Occluded  | 14.36 ms | 0.964 ms | 2.734 ms | 13.39 ms |  1.07 |    0.29 |         - |         - |         - |   5.93 MB |        0.19 |
| Optimized | 17.56 ms | 1.514 ms | 4.271 ms | 15.75 ms |  1.30 |    0.41 |         - |         - |         - |    9.1 MB |        0.29 |
