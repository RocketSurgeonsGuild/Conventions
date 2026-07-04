```

BenchmarkDotNet v0.15.8, macOS Tahoe 26.5.1 (25F80) [Darwin 25.5.0]
Apple M4 Max, 1 CPU, 16 logical and 16 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.9, 10.0.926.27113), Arm64 RyuJIT armv8.0-a

Job=ShortRun  Toolchain=InProcessEmitToolchain  IterationCount=3
LaunchCount=1  WarmupCount=3

```

| Method                                          |     Mean |     Error |   StdDev | Ratio | RatioSD |
| ----------------------------------------------- | -------: | --------: | -------: | ----: | ------: |
| Default_Hosting                                 | 41.47 ms | 25.779 ms | 1.413 ms |  1.02 |    0.03 |
| Default_Hosting_Application                     | 40.64 ms |  4.016 ms | 0.220 ms |  1.00 |    0.01 |
| Rocket_Surgery_Hosting_Application              | 42.25 ms | 40.645 ms | 2.228 ms |  1.04 |    0.05 |
| Default_Hosting_With_Service                    | 41.60 ms |  6.121 ms | 0.336 ms |  1.02 |    0.01 |
| Rocket_Surgery_Hosting_Application_With_Service | 41.59 ms | 18.412 ms | 1.009 ms |  1.02 |    0.02 |
