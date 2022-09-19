using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Snowplow.Analytics.Json;
using Snowplow.Analytics.V2;
using Snowplow.Analytics.V3;
using Xunit;

namespace Snowplow.Analytics.Tests.Compare;

public sealed class EventTransformerComparison
{
    [Fact]
    public void CanParseSampleRecord_V2()
    {
        var record = File.ReadAllText("Data/SampleRecord.txt");

        var output1 = EventTransformer.Transform(record);
        var output2 = EventTransformer2.Transform(record);

        output1.Length.Should().Be(output2.Length);
        output1.Should().Be(output2);
    }

    [Fact]
    public void CanParseSampleRecord_V3()
    {
        var record = File.ReadAllText("Data/SampleRecord.txt");

        var output1 = EventTransformer.Transform(record);
        var output3 = EventTransformer3.Transform(record);

        File.WriteAllText(@"C:\Users\jon.rea\Desktop\o1r.txt", output1);
        File.WriteAllText(@"C:\Users\jon.rea\Desktop\o2r.txt", output3);
        File.WriteAllText(@"C:\Users\jon.rea\Desktop\o1.txt", JsonConvert.SerializeObject(JObject.Parse(output1), Formatting.Indented));
        File.WriteAllText(@"C:\Users\jon.rea\Desktop\o2.txt", JsonConvert.SerializeObject(JObject.Parse(output3), Formatting.Indented));


        output1.Length.Should().Be(output3.Length);
        output1.Should().Be(output3);
    }
}
