public class PlayerFinishedLapEventTest
{
    [Fact]
    public void TestMatches()
    {
        // TODO add other events
        Assert.True(PlayerFinishedLapEvent.Matches("d8248d7cce41618d2caea0ac66ae8870;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000"));
        Assert.False(PlayerFinishedLapEvent.Matches("d8248d7cce41618d2caea0ac66ae8870;PIT_IN;107;14;00:52:29.0490000"));
        Assert.False(PlayerFinishedLapEvent.Matches("d8248d7cce41618d2caea0ac66ae8870;PIT_OUT;107;15;00:54:12.3710000"));
        Assert.False(PlayerFinishedLapEvent.Matches("d8248d7cce41618d2caea0ac66ae8870;SESSION_RELOAD;12;00:37:18.1090000"));
    }

    [Fact]
    public void TestCreateValidEventFromRecoveryFileFormat()
    {
        PlayerFinishedLapEvent expected = new("d8248d7cce41618d2caea0ac66ae8870", 2, TimeSpan.Parse("00:01:41.484"), TimeSpan.Parse("00:03:21.492"));
        Assert.Equal(expected, new("d8248d7cce41618d2caea0ac66ae8870;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000"));
    }

    [Fact]
    public void TestThrowExceptionWhenTypeNotMatches()
    {
        string testLine = "d8248d7cce41618d2caea0ac66ae8870;SESSION_RELOAD;2;00:02:35.1240000";
        Assert.Throws<Exception>(() => new PlayerFinishedLapEvent(testLine));
    }

    [Fact]
    public void TestThrowExceptionWhenLineHasInvalidFormat()
    {
        // SessionId is missing
        string testLine = "d8248d7cce41618d2caea0ac66ae8870;PLAYER_FINISHED_LAP;00:01:41.4840000;00:03:21.4920000";
        Assert.Throws<Exception>(() => new PlayerFinishedLapEvent(testLine));

        // LapNumber is missing
        testLine = "PLAYER_FINISHED_LAP;13;00:01:41.4840000;00:03:21.4920000";
        Assert.Throws<Exception>(() => new PlayerFinishedLapEvent(testLine));
    }

    [Fact]
    public void TestToRecoveryFileFormat()
    {
        var e = new PlayerFinishedLapEvent("d8248d7cce41618d2caea0ac66ae8870", 2, TimeSpan.Parse("00:01:41.4840000"), TimeSpan.Parse("00:03:21.4920000"));
        var expected = "d8248d7cce41618d2caea0ac66ae8870;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000";
        Assert.Equal(expected, e.ToRecoveryFileFormat());
    }

    [Fact]
    public void TestToString()
    {
        string testLine = "d8248d7cce41618d2caea0ac66ae8870;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000";
        string expected = "[ sessionId: d8248d7cce41618d2caea0ac66ae8870, type: PLAYER_FINISHED_LAP, LapNumber: 2, LapTime: 00:01:41.4840000, ElapsedTime: 00:03:21.4920000 ]";
        Assert.Equal(expected, new PlayerFinishedLapEvent(testLine).ToString());
    }
}
