``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18956
Intel Core i7-7700K CPU 4.20GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview8-013656
  [Host] : .NET Core 3.0.0-preview8-28405-07 (CoreCLR 4.700.19.37902, CoreFX 4.700.19.40503), 64bit RyuJIT

Job=ShortRun  Toolchain=InProcessEmitToolchain  IterationCount=3  
LaunchCount=1  WarmupCount=3  

```
|                 Method |       Mean |      Error |    StdDev | Ratio | RatioSD |
|----------------------- |-----------:|-----------:|----------:|------:|--------:|
|        Default_Hosting |   327.6 us |   557.1 us |  30.53 us |  1.00 |    0.00 |
| Rocket_Surgery_Hosting | 1,305.8 us | 1,903.1 us | 104.32 us |  4.03 |    0.71 |
