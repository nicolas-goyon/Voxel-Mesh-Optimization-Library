```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.2 LTS (Noble Numbat) (container)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 2 logical cores and 1 physical core
.NET SDK 8.0.410
  [Host]     : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method          | Mean     | Error    | StdDev   | Gen0        | Gen1       | Gen2      | Allocated  |
|---------------- |---------:|---------:|---------:|------------:|-----------:|----------:|-----------:|
| Naive           |  1.331 s | 0.0251 s | 0.0289 s |  23000.0000 | 22000.0000 | 3000.0000 |  625.12 MB |
| BitOps          | 15.222 s | 0.1466 s | 0.1300 s | 303000.0000 | 63000.0000 | 5000.0000 | 7274.63 MB |
| BitOpsOptimized |  3.562 s | 0.0674 s | 0.0630 s |  23000.0000 | 22000.0000 | 3000.0000 |  640.75 MB |
| Simd            |  2.908 s | 0.0502 s | 0.0470 s |  23000.0000 | 22000.0000 | 3000.0000 |  625.12 MB |
