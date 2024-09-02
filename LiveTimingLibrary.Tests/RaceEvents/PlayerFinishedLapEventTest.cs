public class PlayerFinishedLapEventTest
{
    [Fact]
    public void TestMatches()
    {
        // TODO add other events
        Assert.True(PlayerFinishedLapEvent.Matches("testgame_testtrack_race;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000"));
        Assert.False(PlayerFinishedLapEvent.Matches("testgame_testtrack_race;PIT_IN;107;14;00:52:29.0490000"));
        Assert.False(PlayerFinishedLapEvent.Matches("testgame_testtrack_race;PIT_OUT;107;15;00:54:12.3710000"));
        Assert.False(PlayerFinishedLapEvent.Matches("testgame_testtrack_race;SESSION_RELOAD;12;00:37:18.1090000"));
    }

    [Fact]
    public void TestCreateValidEventFromRecoveryFileFormat()
    {
        PlayerFinishedLapEvent expected = new("testgame_testtrack_race", 2, TimeSpan.Parse("00:01:41.484"), TimeSpan.Parse("00:03:21.492"));
        Assert.Equal(expected, new("testgame_testtrack_race;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000"));
    }

    [Fact]
    public void TestThrowExceptionWhenTypeNotMatches()
    {
        string testLine = "testgame_testtrack_race;SESSION_RELOAD;2;00:02:35.1240000";
        Assert.Throws<Exception>(() => new PlayerFinishedLapEvent(testLine));
    }

    [Fact]
    public void TestThrowExceptionWhenLineHasInvalidFormat()
    {
        // SessionId is missing
        string testLine = "testgame_testtrack_race;PLAYER_FINISHED_LAP;00:01:41.4840000;00:03:21.4920000";
        Assert.Throws<Exception>(() => new PlayerFinishedLapEvent(testLine));

        // LapNumber is missing
        testLine = "PLAYER_FINISHED_LAP;13;00:01:41.4840000;00:03:21.4920000";
        Assert.Throws<Exception>(() => new PlayerFinishedLapEvent(testLine));
    }

    [Fact]
    public void TestToRecoveryFileFormat()
    {
        var e = new PlayerFinishedLapEvent("testgame_testtrack_race", 2, TimeSpan.Parse("00:01:41.4840000"), TimeSpan.Parse("00:03:21.4920000"));
        var expected = "testgame_testtrack_race;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000";
        Assert.Equal(expected, e.ToRecoveryFileFormat());
    }

    [Fact]
    public void TestToString()
    {
        string testLine = "testgame_testtrack_race;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000";
        string expected = "[ sessionId: testgame_testtrack_race, type: PLAYER_FINISHED_LAP, LapNumber: 2, LapTime: 00:01:41.4840000, ElapsedTime: 00:03:21.4920000 ]";
        Assert.Equal(expected, new PlayerFinishedLapEvent(testLine).ToString());
    }
}
