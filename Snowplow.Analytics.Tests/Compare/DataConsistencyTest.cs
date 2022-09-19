using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Snowplow.Analytics.Json;
using Snowplow.Analytics.V2;
using Xunit;

namespace Snowplow.Analytics.Tests.Compare;

public sealed class EventTransformerComparison
{
    [Fact]
    public void CanParseSampleRecord()
    {
        var record = File.ReadAllText("Data/SampleRecord.txt");

        var output1 = EventTransformer.Transform(record);
        var output2 = EventTransformer2.Transform(record);

        for (int i = 0; i < output1.Length; i++)
        {
            if (output1[i] != output2[i])
            {
                continue;
            }
        }

        File.WriteAllText(@"C:\Users\jon.rea\Desktop\o1r.txt", output1);
        File.WriteAllText(@"C:\Users\jon.rea\Desktop\o2r.txt", output2);
        File.WriteAllText(@"C:\Users\jon.rea\Desktop\o1.txt", JsonConvert.SerializeObject(JObject.Parse(output1), Formatting.Indented));
        File.WriteAllText(@"C:\Users\jon.rea\Desktop\o2.txt", JsonConvert.SerializeObject(JObject.Parse(output2), Formatting.Indented));

        output1.Length.Should().Be(output2.Length);

        output1.Should().Be(output2);
    }
}
