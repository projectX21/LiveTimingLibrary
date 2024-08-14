public class SessionReloadEventTest
{
    [Fact]
    public void TestMatches()
    {
        Assert.True(SessionReloadEvent.Matches("SESSION_RELOAD;12;00:37:18.1090000"));
        Assert.False(SessionReloadEvent.Matches("PIT_IN;107;14;00:52:29.0490000"));
        Assert.False(SessionReloadEvent.Matches("PIT_OUT;107;15;00:54:12.3710000"));
        Assert.False(SessionReloadEvent.Matches("PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000"));
    }

    [Fact]
    public void TestCreateValidEventFromRecoveryFileFormat()
    {
        SessionReloadEvent expected = new(12, TimeSpan.Parse("00:37:18.1090000"));
        Assert.Equal(expected, new("SESSION_RELOAD;12;00:37:18.1090000"));
    }


    [Fact]
    public void TestThrowExceptionWhenTypeNotMatches()
    {
        string testLine = "PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000";
        Assert.Throws<Exception>(() => new SessionReloadEvent(testLine));
    }

    [Fact]
    public void TestThrowExceptionWhenLineHasInvalidFormat()
    {
        // LapNumber is missing
        var testLine = "SESSION_RELOAD;00:37:18.1090000";
        Assert.Throws<Exception>(() => new SessionReloadEvent(testLine));
    }

    [Fact]
    public void TestToRecoveryFileFormat()
    {
        var e = new SessionReloadEvent(12, TimeSpan.Parse("00:37:18.1090000"));
        var expected = "SESSION_RELOAD;12;00:37:18.1090000";
        Assert.Equal(expected, e.ToRecoveryFileFormat());
    }

    [Fact]
    public void TestToString()
    {
        string testLine = "SESSION_RELOAD;12;00:37:18.1090000";
        string expected = "[ type: SESSION_RELOAD, LapNumber: 12, ElapsedTime: 00:37:18.1090000 ]";
        Assert.Equal(expected, new SessionReloadEvent(testLine).ToString());
    }
}