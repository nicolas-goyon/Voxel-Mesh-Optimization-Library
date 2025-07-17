```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.2 LTS (Noble Numbat) (container)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical cores and 1 physical core
.NET SDK 8.0.410
  [Host]     : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method | Mean       | Error    | StdDev   | Gen0        | Gen1       | Gen2      | Allocated  |
|------- |-----------:|---------:|---------:|------------:|-----------:|----------:|-----------:|
| Naive  |   626.8 ms | 12.47 ms | 27.89 ms |  12000.0000 | 11000.0000 | 2000.0000 |  305.18 MB |
| BitOps | 6,586.6 ms | 78.32 ms | 69.43 ms | 139000.0000 | 31000.0000 | 4000.0000 | 3300.88 MB |
| Simd   | 1,341.5 ms | 26.16 ms | 29.08 ms |  12000.0000 | 11000.0000 | 2000.0000 |  305.18 MB |
