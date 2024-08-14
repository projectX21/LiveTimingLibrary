public class TimeSpanFormatterTest
{
    [Fact]
    public void TestFormatting()
    {
        Assert.Equal("", TimeSpanFormatter.Format(null));
        Assert.Equal("0.001", TimeSpanFormatter.Format(TimeSpan.Parse("0:00:00.001")));
        Assert.Equal("9.001", TimeSpanFormatter.Format(TimeSpan.Parse("0:00:09.001")));
        Assert.Equal("59.100", TimeSpanFormatter.Format(TimeSpan.Parse("0:00:59.100")));
        Assert.Equal("1:01.500", TimeSpanFormatter.Format(TimeSpan.Parse("0:01:01.500")));
        Assert.Equal("1:15.430", TimeSpanFormatter.Format(TimeSpan.Parse("0:01:15.430")));
        Assert.Equal("13:01.040", TimeSpanFormatter.Format(TimeSpan.Parse("0:13:01.040")));
        Assert.Equal("13:52.421", TimeSpanFormatter.Format(TimeSpan.Parse("0:13:52.421")));
        Assert.Equal("1:01:02.900", TimeSpanFormatter.Format(TimeSpan.Parse("1:01:02.900")));
        Assert.Equal("1:21:02.900", TimeSpanFormatter.Format(TimeSpan.Parse("1:21:02.900")));
        Assert.Equal("1:01:25.900", TimeSpanFormatter.Format(TimeSpan.Parse("1:01:25.900")));
        Assert.Equal("10:01:25.900", TimeSpanFormatter.Format(TimeSpan.Parse("10:01:25.900")));
    }
}