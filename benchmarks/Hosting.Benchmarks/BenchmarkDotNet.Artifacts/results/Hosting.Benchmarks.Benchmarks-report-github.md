``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18956
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview8-013656
  [Host] : .NET Core 3.0.0-preview8-28405-07 (CoreCLR 4.700.19.37902, CoreFX 4.700.19.40503), 64bit RyuJIT

Job=ShortRun  Toolchain=InProcessEmitToolchain  IterationCount=3  
LaunchCount=1  WarmupCount=3  

```
|                              Method |        Mean |    Error |    StdDev | Ratio | RatioSD |
|------------------------------------ |------------:|---------:|----------:|------:|--------:|
|                     Default_Hosting |    291.8 us | 145.1 us |  7.952 us |  1.00 |    0.00 |
|              Rocket_Surgery_Hosting |  1,046.9 us | 103.4 us |  5.667 us |  3.59 |    0.08 |
|        Default_Hosting_With_Service | 15,540.8 us | 350.6 us | 19.219 us | 53.28 |    1.42 |
| Rocket_Surgery_Hosting_With_Service | 15,542.1 us | 295.6 us | 16.202 us | 53.28 |    1.40 |
