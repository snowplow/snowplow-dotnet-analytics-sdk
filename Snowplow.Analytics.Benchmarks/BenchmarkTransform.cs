using BenchmarkDotNet.Attributes;
using Snowplow.Analytics.Json;

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
     *| EventTransformer_Original | 143.4 us | 2.86 us | 5.78 us | 237.92 KB |
     */
    [Benchmark]
    public void EventTransformer_Original()
    {
       string result = EventTransformer.Transform(_recordData);
    }
}
