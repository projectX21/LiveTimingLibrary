public class PitEventTest
{
    [Fact]
    public void TestMatches()
    {
        Assert.True(PitEvent.Matches("PIT_IN;107;14;00:52:29.0490000"));
        Assert.True(PitEvent.Matches("PIT_OUT;107;15;00:54:12.3710000"));
        Assert.False(PitEvent.Matches("PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000"));
        Assert.False(PitEvent.Matches("SESSION_RELOAD;12;00:37:18.1090000"));
    }

    [Fact]
    public void TestCreateValidEventFromRecoveryFileFormat()
    {
        PitEvent expected = new(RaceEventType.PitIn, "107", 14, TimeSpan.Parse("00:52:29.0490000"));
        Assert.Equal(expected, new("PIT_IN;107;14;00:52:29.0490000"));

        expected = new(RaceEventType.PitOut, "107", 15, TimeSpan.Parse("00:54:12.3710000"));
        Assert.Equal(expected, new("PIT_OUT;107;15;00:54:12.3710000"));
    }


    [Fact]
    public void TestThrowExceptionWhenTypeNotMatches()
    {
        string testLine = "SESSION_RELOAD;2;00:02:35.1240000";
        Assert.Throws<Exception>(() => new PitEvent(testLine));
    }

    [Fact]
    public void TestThrowExceptionWhenLineHasInvalidFormat()
    {
        // EntryId is missing for PitIn event
        string testLine = "PIT_IN;14;00:52:29.0490000;01:07:30.9510000";
        Assert.Throws<Exception>(() => new PitEvent(testLine));

        // LapNumber is missing for PitIn event
        testLine = "PIT_IN;107;00:52:29.0490000;01:07:30.9510000";
        Assert.Throws<Exception>(() => new PitEvent(testLine));

        // EntryId is missing for PitOut event
        testLine = "PIT_OUT;14;00:52:29.0490000;01:07:30.9510000";
        Assert.Throws<Exception>(() => new PitEvent(testLine));

        // LapNumber is missing for PitOut event
        testLine = "PIT_OUT;107;00:52:29.0490000;01:07:30.9510000";
        Assert.Throws<Exception>(() => new PitEvent(testLine));
    }

    [Fact]
    public void TestToRecoveryFileFormat()
    {
        var pitInEvent = new PitEvent(RaceEventType.PitIn, "107", 14, TimeSpan.Parse("00:52:29.0490000"));
        var expected = "PIT_IN;107;14;00:52:29.0490000";
        Assert.Equal(expected, pitInEvent.ToRecoveryFileFormat());

        var pitOutEvent = new PitEvent(RaceEventType.PitOut, "107", 15, TimeSpan.Parse("00:54:12.3710000"));
        expected = "PIT_OUT;107;15;00:54:12.3710000";
        Assert.Equal(expected, pitOutEvent.ToRecoveryFileFormat());
    }

    [Fact]
    public void TestToString()
    {
        string testLine = "PIT_IN;107;14;00:52:29.0490000";
        string expected = "[ type: PIT_IN, EntryId: 107, LapNumber: 14, ElapsedTime: 00:52:29.0490000 ]";
        Assert.Equal(expected, new PitEvent(testLine).ToString());

        testLine = "PIT_OUT;107;15;00:54:12.3710000";
        expected = "[ type: PIT_OUT, EntryId: 107, LapNumber: 15, ElapsedTime: 00:54:12.3710000 ]";
        Assert.Equal(expected, new PitEvent(testLine).ToString());
    }
}