public class PlayerFinishedLapEventTest
{
    [Fact]
    public void TestMatches()
    {
        // TODO add other events
        Assert.True(PlayerFinishedLapEvent.Matches("PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000"));
        Assert.False(PlayerFinishedLapEvent.Matches("PIT_IN;107;14;00:52:29.0490000"));
        Assert.False(PlayerFinishedLapEvent.Matches("PIT_OUT;107;15;00:54:12.3710000"));
        Assert.False(PlayerFinishedLapEvent.Matches("SESSION_RELOAD;12;00:37:18.1090000"));
    }

    [Fact]
    public void TestCreateValidEventFromRecoveryFileFormat()
    {
        PlayerFinishedLapEvent expected = new(2, TimeSpan.Parse("00:01:41.484"), TimeSpan.Parse("00:03:21.492"));
        Assert.Equal(expected, new("PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000"));
    }

    [Fact]
    public void TestThrowExceptionWhenTypeNotMatches()
    {
        string testLine = "SESSION_RELOAD;2;00:02:35.1240000";
        Assert.Throws<Exception>(() => new PlayerFinishedLapEvent(testLine));
    }

    [Fact]
    public void TestThrowExceptionWhenLineHasInvalidFormat()
    {
        // LapNumber is missing
        string testLine = "PLAYER_FINISHED_LAP;00:01:41.4840000;00:03:21.4920000";
        Assert.Throws<Exception>(() => new PlayerFinishedLapEvent(testLine));
    }

    [Fact]
    public void TestToRecoveryFileFormat()
    {
        var e = new PlayerFinishedLapEvent(2, TimeSpan.Parse("00:01:41.4840000"), TimeSpan.Parse("00:03:21.4920000"));
        var expected = "PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000";
        Assert.Equal(expected, e.ToRecoveryFileFormat());
    }

    [Fact]
    public void TestToString()
    {
        string testLine = "PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000";
        string expected = "[ type: PLAYER_FINISHED_LAP, LapNumber: 2, LapTime: 00:01:41.4840000, ElapsedTime: 00:03:21.4920000 ]";
        Assert.Equal(expected, new PlayerFinishedLapEvent(testLine).ToString());
    }
}