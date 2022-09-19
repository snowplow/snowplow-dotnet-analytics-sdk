using BenchmarkDotNet.Attributes;
using Snowplow.Analytics.Json;
using Snowplow.Analytics.V2;
using Snowplow.Analytics.V3;

namespace Snowplow.Analytics.Benchmarks;

[MemoryDiagnoser(false)]
public class BenchmarkTransform
{
    private readonly string _recordData;

    public BenchmarkTransform()
    {
        _recordData = File.ReadAllText("Data/SampleRecord.txt");
    }

    /*
     *|                    Method |     Mean |   Error |  StdDev | Allocated |
     *|-------------------------- |---------:|--------:|--------:|----------:|
     *| EventTransformer_Original | 143.4 us | 2.86 us | 5.78 us | 237.92 KB | - original
     *| EventTransformer_Original | 150.8 us | 2.98 us | 5.88 us | 243.64 KB | - latest Newtonsoft.Json
     *| EventTransformer_Original | 149.2 us | 2.97 us | 6.58 us | 243.06 KB | - Project to net6.0
     */
    [Benchmark]
    public void EventTransformer_Original()
    {
       string result = EventTransformer.Transform(_recordData);
    }

    /*
     * |                    Method |      Mean |    Error |   StdDev | Allocated |
     * |-------------------------- |----------:|---------:|---------:|----------:|
     * | EventTransformer_Original | 133.56 us | 2.580 us | 3.444 us | 243.06 KB |
     * |       EventTransformer_V2 |  60.29 us | 1.121 us | 1.101 us |  119.5 KB |
     */
    [Benchmark]
    public void EventTransformer_V2()
    {
        string result = EventTransformer2.Transform(_recordData);
    }

    /*
     *|                    Method |      Mean |    Error |   StdDev | Allocated |
     *|-------------------------- |----------:|---------:|---------:|----------:|
     *| EventTransformer_Original | 133.88 us | 2.612 us | 3.910 us | 243.06 KB |
     *|       EventTransformer_V2 |  59.89 us | 1.164 us | 1.513 us |  119.5 KB |
     *|       EventTransformer_V3 |  54.89 us | 0.494 us | 0.386 us | 104.29 KB |
     */
    [Benchmark]
    public void EventTransformer_V3()
    {
        string result = EventTransformer3.Transform(_recordData);
    }
}
