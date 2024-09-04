public class EntryProgressTest
{
    [Fact]
    public void TestInit()
    {
        var progress = new EntryProgress("Test", 10, 3, TimeSpan.Parse("00:00:10.3090000"), 5);
        Assert.Equal("Test", progress.GetEntryId());
        Assert.Equal(10, progress.GetLapNumber());
        Assert.Equal(3, progress.GetMiniSector());
        Assert.Equal(TimeSpan.Parse("00:00:10.3090000"), progress.GetElapsedTime());
        Assert.Equal(5, progress.GetSimHubPosition());
    }

    [Fact]
    public void TestIdenticalLapNumberAndMiniSector()
    {
        // different lap number and mini sector
        var a = new EntryProgress("Test", 10, 3, TimeSpan.Parse("00:00:10.3090000"), 1);
        var b = new EntryProgress("Test1", 11, 1, TimeSpan.Parse("00:00:10.3420000"), 2);
        Assert.False(a.IdenticalLapNumberAndMiniSector(b));
        Assert.False(a.IdenticalLapNumberAndMiniSector(b.GetLapNumber(), b.GetMiniSector()));
        Assert.False(b.IdenticalLapNumberAndMiniSector(a));
        Assert.False(b.IdenticalLapNumberAndMiniSector(a.GetLapNumber(), a.GetMiniSector()));

        // different mini sector
        a = new EntryProgress("Test", 10, 3, TimeSpan.Parse("00:00:10.3090000"), 1);
        b = new EntryProgress("Test1", 10, 1, TimeSpan.Parse("00:00:10.3420000"), 2);
        Assert.False(a.IdenticalLapNumberAndMiniSector(b));
        Assert.False(a.IdenticalLapNumberAndMiniSector(b.GetLapNumber(), b.GetMiniSector()));
        Assert.False(b.IdenticalLapNumberAndMiniSector(a));
        Assert.False(b.IdenticalLapNumberAndMiniSector(a.GetLapNumber(), a.GetMiniSector()));

        // different lap
        b = new EntryProgress("Test1", 9, 3, TimeSpan.Parse("00:00:10.3420000"), 2);
        Assert.False(a.IdenticalLapNumberAndMiniSector(b));
        Assert.False(a.IdenticalLapNumberAndMiniSector(b.GetLapNumber(), b.GetMiniSector()));
        Assert.False(b.IdenticalLapNumberAndMiniSector(a));
        Assert.False(b.IdenticalLapNumberAndMiniSector(a.GetLapNumber(), a.GetMiniSector()));

        // both are the same
        b = new EntryProgress("Test1", 10, 3, TimeSpan.Parse("00:00:10.3420000"), 2);
        Assert.True(a.IdenticalLapNumberAndMiniSector(b));
        Assert.True(a.IdenticalLapNumberAndMiniSector(b.GetLapNumber(), b.GetMiniSector()));
        Assert.True(b.IdenticalLapNumberAndMiniSector(a));
        Assert.True(b.IdenticalLapNumberAndMiniSector(a.GetLapNumber(), a.GetMiniSector()));

        // different mini sector
        a = new EntryProgress("Test", 10, 3, TimeSpan.Parse("00:00:10.3090000"), 1);
        b = new EntryProgress("Test1", 10, 1, TimeSpan.Parse("00:00:10.3420000"), 2);
        Assert.False(a.IdenticalLapNumberAndMiniSector(b));
        Assert.False(a.IdenticalLapNumberAndMiniSector(b.GetLapNumber(), b.GetMiniSector()));
        Assert.False(b.IdenticalLapNumberAndMiniSector(a));
        Assert.False(b.IdenticalLapNumberAndMiniSector(a.GetLapNumber(), a.GetMiniSector()));

        // different lap
        b = new EntryProgress("Test1", 9, 3, TimeSpan.Parse("00:00:10.3420000"), 2);
        Assert.False(a.IdenticalLapNumberAndMiniSector(b));
        Assert.False(b.IdenticalLapNumberAndMiniSector(a));

        // both are the same
        b = new EntryProgress("Test1", 10, 3, TimeSpan.Parse("00:00:10.3420000"), 2);
        Assert.True(a.IdenticalLapNumberAndMiniSector(b));
        Assert.True(b.IdenticalLapNumberAndMiniSector(a));
    }

    [Fact]
    public void TestCompareTo()
    {
        // both are the same
        var a = new EntryProgress("Test", 11, 1, TimeSpan.Parse("00:00:10.3090000"), 1);
        var b = new EntryProgress("Test1", 11, 1, TimeSpan.Parse("00:00:10.3090000"), 1);
        Assert.Equal(0, a.CompareTo(b));
        Assert.Equal(0, b.CompareTo(a));

        // different lap number and mini sector
        b = new EntryProgress("Test1", 10, 9, TimeSpan.Parse("00:00:10.3090000"), 1);
        Assert.Equal(1, a.CompareTo(b));
        Assert.Equal(-1, b.CompareTo(a));

        // different lap number
        b = new EntryProgress("Test1", 10, 1, TimeSpan.Parse("00:00:10.3090000"), 1);
        Assert.Equal(1, a.CompareTo(b));
        Assert.Equal(-1, b.CompareTo(a));

        // different mini sector
        b = new EntryProgress("Test1", 11, 0, TimeSpan.Parse("00:00:10.3090000"), 1);
        Assert.Equal(1, a.CompareTo(b));
        Assert.Equal(-1, b.CompareTo(a));

        // different elapsed time
        b = new EntryProgress("Test1", 11, 1, TimeSpan.Parse("00:00:10.3100000"), 1);
        Assert.Equal(1, a.CompareTo(b));
        Assert.Equal(-1, b.CompareTo(a));

        // different simhub position
        b = new EntryProgress("Test1", 11, 1, TimeSpan.Parse("00:00:10.3090000"), 2);
        Assert.Equal(1, a.CompareTo(b));
        Assert.Equal(-1, b.CompareTo(a));
    }
}