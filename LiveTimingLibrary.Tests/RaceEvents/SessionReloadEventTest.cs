public class SessionReloadEventTest
{
    [Fact]
    public void TestMatches()
    {
        Assert.True(SessionReloadEvent.Matches("46452ab12lef;SESSION_RELOAD;12;00:37:18.1090000"));
        Assert.False(SessionReloadEvent.Matches("46452ab12lef;PIT_IN;107;14;00:52:29.0490000"));
        Assert.False(SessionReloadEvent.Matches("46452ab12lef;PIT_OUT;107;15;00:54:12.3710000"));
        Assert.False(SessionReloadEvent.Matches("46452ab12lef;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000"));
    }

    [Fact]
    public void TestCreateValidEventFromRecoveryFileFormat()
    {
        SessionReloadEvent expected = new("46452ab12lef", 12, TimeSpan.Parse("00:37:18.1090000"));
        Assert.Equal(expected, new("46452ab12lef;SESSION_RELOAD;12;00:37:18.1090000"));
    }


    [Fact]
    public void TestThrowExceptionWhenTypeNotMatches()
    {
        string testLine = "testgame_testtrack_race;LAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000";
        Assert.Throws<Exception>(() => new SessionReloadEvent(testLine));
    }

    [Fact]
    public void TestThrowExceptionWhenLineHasInvalidFormat()
    {
        // SessionId is missing
        var testLine = "SESSION_RELOAD;15;00:37:18.1090000";
        Assert.Throws<Exception>(() => new SessionReloadEvent(testLine));

        // LapNumber is missing
        testLine = "testgame_testtrack_race;SESSION_RELOAD;00:37:18.1090000";
        Assert.Throws<Exception>(() => new SessionReloadEvent(testLine));
    }

    [Fact]
    public void TestToRecoveryFileFormat()
    {
        var e = new SessionReloadEvent("testgame_testtrack_race", 12, TimeSpan.Parse("00:37:18.1090000"));
        var expected = "testgame_testtrack_race;SESSION_RELOAD;12;00:37:18.1090000";
        Assert.Equal(expected, e.ToRecoveryFileFormat());
    }

    [Fact]
    public void TestToString()
    {
        string testLine = "testgame_testtrack_race;SESSION_RELOAD;12;00:37:18.1090000";
        string expected = "[ sessionId: testgame_testtrack_race, type: SESSION_RELOAD, LapNumber: 12, ElapsedTime: 00:37:18.1090000 ]";
        Assert.Equal(expected, new SessionReloadEvent(testLine).ToString());
    }
}
