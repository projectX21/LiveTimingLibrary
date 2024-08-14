public class TestableOpponentTest
{
    [Fact]
    public void TestGetId()
    {
        // the id should be the car number, if filled...
        var entry = new TestableOpponent
        {
            CarNumber = "107"
        };
        Assert.Equal("107", entry.Id);

        // ... otherwise it is a combined string of Name, TeamName and CarName
        entry.CarNumber = null;
        entry.Name = "Name";
        entry.TeamName = "TeamName";
        entry.CarName = "CarName";
        Assert.Equal("Name-TeamName-CarName", entry.Id);
    }

    [Fact]
    public void TestGetCurrentLapFragmentTime()
    {
        var entry = new TestableOpponent
        {
            CurrentTimes = null
        };

        Assert.Null(entry.GetCurrentLapFragmentTime(LapFragmentType.SECTOR_1));
        Assert.Null(entry.GetCurrentLapFragmentTime(LapFragmentType.SECTOR_2));
        Assert.Null(entry.GetCurrentLapFragmentTime(LapFragmentType.SECTOR_3));
        Assert.Null(entry.GetCurrentLapFragmentTime(LapFragmentType.FULL_LAP));

        entry.CurrentTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.1920000"),
            Sector2 = TimeSpan.Parse("00:01:02.6650000")
        };

        // the current Lap won't ever have a sector 3 nor a full lap time
        Assert.Equal(TimeSpan.Parse("00:00:23.1920000"), entry.GetCurrentLapFragmentTime(LapFragmentType.SECTOR_1));
        Assert.Equal(TimeSpan.Parse("00:01:02.6650000"), entry.GetCurrentLapFragmentTime(LapFragmentType.SECTOR_2));
    }

    [Fact]
    public void TestGetLastLapFragmentTime()
    {
        var entry = new TestableOpponent
        {
            LastTimes = null
        };

        Assert.Null(entry.GetLastLapFragmentTime(LapFragmentType.SECTOR_1));
        Assert.Null(entry.GetLastLapFragmentTime(LapFragmentType.SECTOR_2));
        Assert.Null(entry.GetLastLapFragmentTime(LapFragmentType.SECTOR_3));
        Assert.Null(entry.GetLastLapFragmentTime(LapFragmentType.FULL_LAP));

        entry.LastTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000"),
            Sector2 = TimeSpan.Parse("00:01:03.1080000"),
            Sector3 = TimeSpan.Parse("00:00:16.9190000"),
            FullLap = TimeSpan.Parse("00:01:43.4390000")
        };

        Assert.Equal(TimeSpan.Parse("00:00:23.4120000"), entry.GetLastLapFragmentTime(LapFragmentType.SECTOR_1));
        Assert.Equal(TimeSpan.Parse("00:01:03.1080000"), entry.GetLastLapFragmentTime(LapFragmentType.SECTOR_2));
        Assert.Equal(TimeSpan.Parse("00:00:16.9190000"), entry.GetLastLapFragmentTime(LapFragmentType.SECTOR_3));
        Assert.Equal(TimeSpan.Parse("00:01:43.4390000"), entry.GetLastLapFragmentTime(LapFragmentType.FULL_LAP));
    }

    [Fact]
    public void TestGetFastestFragmentTime()
    {
        // one of the fragment times for current, last or best lap has to be filled
        var entry = new TestableOpponent();
        Assert.Null(entry.GetFastestFragmentTime(LapFragmentType.SECTOR_1));
        Assert.Null(entry.GetFastestFragmentTime(LapFragmentType.SECTOR_2));
        Assert.Null(entry.GetFastestFragmentTime(LapFragmentType.SECTOR_3));
        Assert.Null(entry.GetFastestFragmentTime(LapFragmentType.FULL_LAP));

        // at first only the current times are filled
        entry.CurrentTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000"),
            Sector2 = TimeSpan.Parse("00:01:03.1080000"),
        };
        Assert.Equal(TimeSpan.Parse("00:00:23.4120000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_1));
        Assert.Equal(TimeSpan.Parse("00:01:03.1080000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_2));
        Assert.Null(entry.GetFastestFragmentTime(LapFragmentType.SECTOR_3));
        Assert.Null(entry.GetFastestFragmentTime(LapFragmentType.FULL_LAP));

        // only the last times are filled
        entry.CurrentTimes = null;
        entry.LastTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000"),
            Sector2 = TimeSpan.Parse("00:01:03.1080000"),
            Sector3 = TimeSpan.Parse("00:00:16.9190000"),
            FullLap = TimeSpan.Parse("00:01:43.4390000")
        };
        Assert.Equal(TimeSpan.Parse("00:00:23.4120000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_1));
        Assert.Equal(TimeSpan.Parse("00:01:03.1080000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_2));
        Assert.Equal(TimeSpan.Parse("00:00:16.9190000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_3));
        Assert.Equal(TimeSpan.Parse("00:01:43.4390000"), entry.GetFastestFragmentTime(LapFragmentType.FULL_LAP));

        // only the best times are filled
        entry.LastTimes = null;
        entry.BestTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000"),
            Sector2 = TimeSpan.Parse("00:01:03.1080000"),
            Sector3 = TimeSpan.Parse("00:00:16.9190000"),
            FullLap = TimeSpan.Parse("00:01:43.4390000")
        };
        Assert.Equal(TimeSpan.Parse("00:00:23.4120000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_1));
        Assert.Equal(TimeSpan.Parse("00:01:03.1080000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_2));
        Assert.Equal(TimeSpan.Parse("00:00:16.9190000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_3));
        Assert.Equal(TimeSpan.Parse("00:01:43.4390000"), entry.GetFastestFragmentTime(LapFragmentType.FULL_LAP));

        // Should always return the fastest one
        // This test is easy, all three times are the same...
        entry.CurrentTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000")
        };

        entry.LastTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000")
        };

        entry.BestTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000")
        };

        Assert.Equal(TimeSpan.Parse("00:00:23.4120000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_1));

        // Current is the fastest one
        entry.CurrentTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000")
        };

        entry.LastTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4130000")
        };

        entry.BestTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4140000")
        };

        Assert.Equal(TimeSpan.Parse("00:00:23.4120000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_1));

        // Last is the fastest one
        entry.CurrentTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4140000")
        };

        entry.LastTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000")
        };

        entry.BestTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4130000")
        };

        Assert.Equal(TimeSpan.Parse("00:00:23.4120000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_1));

        // Best is the fastest one
        entry.CurrentTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4130000")
        };

        entry.LastTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4140000")
        };

        entry.BestTimes = new TestableSectorTimes
        {
            Sector1 = TimeSpan.Parse("00:00:23.4120000")
        };

        Assert.Equal(TimeSpan.Parse("00:00:23.4120000"), entry.GetFastestFragmentTime(LapFragmentType.SECTOR_1));
    }
}