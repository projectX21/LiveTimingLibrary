public class RaceEventRecoveryFileEventSelectorTest
{
    [Fact]
    public void BasicSelectOfPitEvents()
    {
        var testFile = @"C:\Users\chris\Documents\simhub\test.json";
        File.Delete(testFile);

        var content =
            "PIT_IN;107;5;00:10:18.1930000\n" +
            "PIT_OUT;107;6;00:10:46.0920000\n";

        File.AppendAllText(testFile, content);

        var filteredEvents = new RaceEventRecoveryFileEventSelector<PitEvent>().SelectSpecificEvents(testFile);
        Assert.Equal(2, filteredEvents.Count);
    }

    [Fact]
    public void SelectOfPitEventsContainingSessionReloads()
    {
        var testFile = @"C:\Users\chris\Documents\simhub\test.json";
        File.Delete(testFile);

        // first reload event doesn't change anything, because the previous pit event is older.
        // second reload however removes the previous pit event, because the reload is older.
        // should not consider any other events than pit and session reload events
        var content =
            "PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000\n" +
            "PIT_IN;107;5;00:10:18.1930000\n" +
            "PIT_OUT;107;6;00:10:46.0920000\n" +
            "PIT_IN;107;17;00:43:49.6580000\n" +
            "SESSION_RELOAD;17;00:43:58.2510000\n" +
            "PIT_OUT;107;18;00:45:02.3570000\n" +
            "PIT_IN;107;21;00:52:15.7780000\n" +
            "SESSION_RELOAD;20;00:52:00.8120000\n" +
            "PIT_IN;107;21;00:52:14.1580000\n" +
            "PIT_OUT;107;22;00:53:10.3590000\n";

        File.AppendAllText(testFile, content);

        var filteredEvents = new RaceEventRecoveryFileEventSelector<PitEvent>().SelectSpecificEvents(testFile);
        Assert.Equal(6, filteredEvents.Count);

        var expectedPitIn1 = new PitEvent(RaceEventType.PitIn, "107", 5, TimeSpan.Parse("00:10:18.1930000"));
        Assert.Equal(expectedPitIn1, filteredEvents.ElementAt(0));

        var expectedPitOut1 = new PitEvent(RaceEventType.PitOut, "107", 6, TimeSpan.Parse("00:10:46.0920000"));
        Assert.Equal(expectedPitOut1, filteredEvents.ElementAt(1));

        var expectedPitIn2 = new PitEvent(RaceEventType.PitIn, "107", 17, TimeSpan.Parse("00:43:49.6580000"));
        Assert.Equal(expectedPitIn2, filteredEvents.ElementAt(2));

        var expectedPitOut2 = new PitEvent(RaceEventType.PitOut, "107", 18, TimeSpan.Parse("00:45:02.3570000"));
        Assert.Equal(expectedPitOut2, filteredEvents.ElementAt(3));

        var expectedPitIn3 = new PitEvent(RaceEventType.PitIn, "107", 21, TimeSpan.Parse("00:52:14.1580000"));
        Assert.Equal(expectedPitIn3, filteredEvents.ElementAt(4));

        var expectedPitOut3 = new PitEvent(RaceEventType.PitOut, "107", 22, TimeSpan.Parse("00:53:10.3590000"));
        Assert.Equal(expectedPitOut3, filteredEvents.ElementAt(5));
    }

    [Fact]
    public void BasicSelectOfPlayerFinishedLapEvents()
    {
        var testFile = @"C:\Users\chris\Documents\simhub\test.json";
        File.Delete(testFile);

        var content =
            "PLAYER_FINISHED_LAP;1;00:01:46.1850000;00:01:46.1850000\n" +
            "PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:27.6690000\n";

        File.AppendAllText(testFile, content);

        var filteredEvents = new RaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent>().SelectSpecificEvents(testFile);
        Assert.Equal(2, filteredEvents.Count);
    }

    [Fact]
    public void SelectOfPlayerFinishedLapEventsContainingSessionReloads()
    {
        var testFile = @"C:\Users\chris\Documents\simhub\test.json";
        File.Delete(testFile);

        // first reload event doesn't change anything, because the previous finished lap event is older.
        // second reload however removes the previous finished lap event, because the reload is older.
        // should not consider any other events than finished lap and session reload events
        var content =
            "PLAYER_FINISHED_LAP;1;00:01:46.1850000;00:01:46.1850000\n" +
            "PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:27.6690000\n" +
            "SESSION_RELOAD;2;00:03:40.1850000\n" +
            "PLAYER_FINISHED_LAP;3;00:01:41.1210000;00:05:08.7900000\n" +
            "SESSION_RELOAD;2;00:05:02.5810000\n" +
            "PLAYER_FINISHED_LAP;3;00:01:41.1690000;00:05:08.8380000\n" +
            "PIT_IN;107;5;00:10:18.1930000\n" +
            "PIT_OUT;107;6;00:10:46.0920000\n";

        File.AppendAllText(testFile, content);

        var filteredEvents = new RaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent>().SelectSpecificEvents(testFile);
        Assert.Equal(3, filteredEvents.Count);

        var expectedEvent1 = new PlayerFinishedLapEvent(1, TimeSpan.Parse("00:01:46.1850000"), TimeSpan.Parse("00:01:46.1850000"));
        Assert.Equal(expectedEvent1, filteredEvents.ElementAt(0));

        var expectedEvent2 = new PlayerFinishedLapEvent(2, TimeSpan.Parse("00:01:41.4840000"), TimeSpan.Parse("00:03:27.6690000"));
        Assert.Equal(expectedEvent2, filteredEvents.ElementAt(1));

        var expectedEvent3 = new PlayerFinishedLapEvent(3, TimeSpan.Parse("00:01:41.1690000"), TimeSpan.Parse("00:05:08.8380000"));
        Assert.Equal(expectedEvent3, filteredEvents.ElementAt(2));
    }

    [Fact]
    public void TestThrowExceptionWhenLineHasInvalidFormat()
    {
        var testFile = @"C:\Users\chris\Documents\simhub\test.json";
        File.Delete(testFile);

        // first reload event doesn't change anything, because the following pit event is older.
        // second reload however removes the previous pit event, because the reload is older.
        // should not consider any other events than pit and session reload events
        var content =
            "PIT_IN;107;5;00:10:18.1930000;00:49:41.8070000\n" +
            "INVALID_EVENT;2;00:01:41.4840000;00:03:21.4920000;00:56:38.508\n" +
            "PIT_OUT;107;6;00:10:46.0920000;00:49:13.9080000\n";

        File.AppendAllText(testFile, content);
        Assert.Throws<Exception>(() => new RaceEventRecoveryFileEventSelector<PitEvent>().SelectSpecificEvents(testFile));
    }
}