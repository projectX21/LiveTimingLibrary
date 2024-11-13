public class PitEventStoreTest
{
    [Fact]
    public void TestIsAddable()
    {
        var store = new PitEventStore();
        store.Add(GetFirstPitInEvent());
        Assert.Equal(1, store.GetPitDataByEntryId("107").TotalPitStops);
        Assert.Equal(0, store.GetPitDataByEntryId("107").TotalPitDuration);

        // Should not add a PitIn event directly after another one
        store.Add(GetFirstPitInEvent());
        Assert.Equal(1, store.GetPitDataByEntryId("107").TotalPitStops);
        Assert.Equal(0, store.GetPitDataByEntryId("107").TotalPitDuration);

        store.Add(GetFirstPitOutEvent());
        Assert.Equal(1, store.GetPitDataByEntryId("107").TotalPitStops);
        Assert.Equal(37, store.GetPitDataByEntryId("107").TotalPitDuration);

        // Should not add a PitOut event directly after another one
        store.Add(GetFirstPitOutEvent());
        Assert.Equal(1, store.GetPitDataByEntryId("107").TotalPitStops);
        Assert.Equal(37, store.GetPitDataByEntryId("107").TotalPitDuration);
    }

    [Fact]
    public void TestGetPitDataByEntryId()
    {
        var store = new PitEventStore();
        var pitData = store.GetPitDataByEntryId("107");
        EntryPitData expected = new("107", 0, 0, null, null);
        Assert.Equal(expected, pitData);

        store.Add(GetFirstPitInEvent());
        pitData = store.GetPitDataByEntryId("107");
        expected = new("107", 1, 0, null, null);
        Assert.Equal(expected, pitData);

        store.Add(GetFirstPitOutEvent());
        pitData = store.GetPitDataByEntryId("107");
        expected = new("107", 1, 37, 37, 11);
        Assert.Equal(expected, pitData);

        store.Add(GetSecondPitInEvent());
        pitData = store.GetPitDataByEntryId("107");
        expected = new("107", 2, 37, 37, 11);
        Assert.Equal(expected, pitData);

        store.Add(GetSecondPitOutEvent());
        pitData = store.GetPitDataByEntryId("107");
        expected = new("107", 2, 139, 102, 28);
        Assert.Equal(expected, pitData);
    }

    [Fact]
    public void TestReset()
    {
        var store = new PitEventStore();
        store.Add(GetFirstPitInEvent());
        store.Add(GetFirstPitOutEvent());
        store.Add(GetSecondPitInEvent());
        store.Add(GetSecondPitOutEvent());

        var pitData = store.GetPitDataByEntryId("107");
        EntryPitData expected = new("107", 2, 139, 102, 28);
        Assert.Equal(expected, pitData);

        store.Reset();
        pitData = store.GetPitDataByEntryId("107");
        expected = new("107", 0, 0, null, null);
        Assert.Equal(pitData, expected);
    }

    public static PitEvent GetFirstPitInEvent()
    {
        return new PitEvent("testgame_testtrack_race", RaceEventType.PitIn, "107", 10, TimeSpan.Parse("00:01:40.1090000"));
    }

    public static PitEvent GetFirstPitOutEvent()
    {
        return new PitEvent("testgame_testtrack_race", RaceEventType.PitOut, "107", 11, TimeSpan.Parse("00:02:17.6010000"));
    }

    public static PitEvent GetSecondPitInEvent()
    {
        return new PitEvent("testgame_testtrack_race", RaceEventType.PitIn, "107", 27, TimeSpan.Parse("00:47:19.1570000"));
    }

    public static PitEvent GetSecondPitOutEvent()
    {
        return new PitEvent("testgame_testtrack_race", RaceEventType.PitOut, "107", 28, TimeSpan.Parse("00:49:01.8580000"));
    }
}
