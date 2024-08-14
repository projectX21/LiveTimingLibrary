public class RaceEventTypeTest
{
    [Fact]
    public void TestToEnum()
    {
        Assert.Equal(RaceEventType.PitIn, RaceEventTypeConverter.ToEnum("PIT_IN"));
        Assert.Equal(RaceEventType.PitOut, RaceEventTypeConverter.ToEnum("PIT_OUT"));
        Assert.Equal(RaceEventType.SessionReload, RaceEventTypeConverter.ToEnum("SESSION_RELOAD"));
        Assert.Equal(RaceEventType.PlayerFinishedLap, RaceEventTypeConverter.ToEnum("PLAYER_FINISHED_LAP"));

        Assert.Throws<Exception>(() => RaceEventTypeConverter.ToEnum(""));
        Assert.Throws<Exception>(() => RaceEventTypeConverter.ToEnum("PITIN"));
        Assert.Throws<Exception>(() => RaceEventTypeConverter.ToEnum("PIT-IN"));
    }

    [Fact]
    public void TestFromEnum()
    {
        Assert.Equal("PIT_IN", RaceEventTypeConverter.FromEnum(RaceEventType.PitIn));
        Assert.Equal("PIT_OUT", RaceEventTypeConverter.FromEnum(RaceEventType.PitOut));
        Assert.Equal("SESSION_RELOAD", RaceEventTypeConverter.FromEnum(RaceEventType.SessionReload));
        Assert.Equal("PLAYER_FINISHED_LAP", RaceEventTypeConverter.FromEnum(RaceEventType.PlayerFinishedLap));
    }
}