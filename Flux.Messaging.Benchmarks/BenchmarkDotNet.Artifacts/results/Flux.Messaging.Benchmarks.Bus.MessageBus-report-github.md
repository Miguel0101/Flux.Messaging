```

BenchmarkDotNet v0.15.8, Linux Debian GNU/Linux 13 (trixie)
Intel Xeon CPU E5-2680 v4 2.40GHz, 1 CPU, 28 logical and 14 physical cores
.NET SDK 10.0.103
  [Host]     : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.3 (10.0.3, 10.0.326.7603), X64 RyuJIT x86-64-v3


```
| Method  | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|-------- |---------:|----------:|----------:|-------:|-------:|----------:|
| Publish | 1.081 μs | 0.0130 μs | 0.0122 μs | 0.0134 | 0.0114 |     285 B |
| Send    | 1.294 μs | 0.0041 μs | 0.0036 μs | 0.0172 |      - |     329 B |
