public class TestableSectorTimesTest
{
    [Fact]
    public void TestGetByLapFragmentTime()
    {
        var times = new TestableSectorTimes
        {
            Sector1 = null,
            Sector2 = null,
            Sector3 = null,
            FullLap = null
        };

        Assert.Null(times.GetByLapFragmentType(LapFragmentType.SECTOR_1));
        Assert.Null(times.GetByLapFragmentType(LapFragmentType.SECTOR_2));
        Assert.Null(times.GetByLapFragmentType(LapFragmentType.SECTOR_3));
        Assert.Null(times.GetByLapFragmentType(LapFragmentType.FULL_LAP));

        times = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:15.1250000"),
            Sector2 = TimeSpan.Parse("00:01:01.6880000"),
            Sector3 = TimeSpan.Parse("00:00:32.9810000"),
            FullLap = TimeSpan.Parse("00:01:49.7940000")
        };

        Assert.Equal(TimeSpan.Parse("00:00:15.1250000"), times.GetByLapFragmentType(LapFragmentType.SECTOR_1));
        Assert.Equal(TimeSpan.Parse("00:01:01.6880000"), times.GetByLapFragmentType(LapFragmentType.SECTOR_2));
        Assert.Equal(TimeSpan.Parse("00:00:32.9810000"), times.GetByLapFragmentType(LapFragmentType.SECTOR_3));
        Assert.Equal(TimeSpan.Parse("00:01:49.7940000"), times.GetByLapFragmentType(LapFragmentType.FULL_LAP));
    }
}