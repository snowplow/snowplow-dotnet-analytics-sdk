using System.IO;
using Newtonsoft.Json.Linq;
using Snowplow.Analytics.Json;
using Xunit;

namespace Snowplow.Analytics.Tests.Data;

public sealed class DataConsistencyTest
{
    [Fact]
    public void CanParseSampleRecord()
    {
        var record = File.ReadAllText("Data/SampleRecord.txt");
        var output = EventTransformer.Transform(record);
        var json = JObject.Parse(output);
        Assert.Equal("xx_ios", json["app_id"].Value<string>());
    }
}
