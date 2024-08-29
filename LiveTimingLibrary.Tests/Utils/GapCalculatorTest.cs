public class GapCalculatorTest
{
    [Fact]
    public void TestLapGap()
    {
        Assert.Equal("", GapCalculator.ToLapGap(null));
        Assert.Equal("+2L", GapCalculator.ToLapGap(2));
    }

    [Fact]
    public void TestTimeGap()
    {
        // at first for double
        Assert.Equal("", GapCalculator.ToTimeGap(null));
        Assert.Equal("+0.885", GapCalculator.ToTimeGap(0.88578));
        Assert.Equal("+10.531", GapCalculator.ToTimeGap(10.53192));
        Assert.Equal("+1:04.303", GapCalculator.ToTimeGap(64.30397));

        // now for TimeSpan
        Assert.Equal("+0.885", GapCalculator.ToTimeGap(TimeSpan.FromSeconds(0.88578)));
        Assert.Equal("+10.531", GapCalculator.ToTimeGap(TimeSpan.FromSeconds(10.53192)));
        Assert.Equal("+1:04.303", GapCalculator.ToTimeGap(TimeSpan.FromSeconds(64.30397)));
    }

    [Fact]
    public void TestGapInRace()
    {
        var e1 = GetEntry1();
        var e2 = GetEntry2();

        // Should not calculate any gap when one of both entries is null
        Assert.Null(GapCalculator.Calc(SessionType.Race, null, null));
        Assert.Null(GapCalculator.Calc(SessionType.Race, e1, null));
        Assert.Null(GapCalculator.Calc(SessionType.Race, null, e2));

        // Should not calculate any gap when one of both entries is in the pits
        e1.IsInPit = false;
        e2.IsInPit = true;
        Assert.Null(GapCalculator.Calc(SessionType.Race, e1, e2));

        e1.IsInPit = true;
        e2.IsInPit = false;
        Assert.Null(GapCalculator.Calc(SessionType.Race, e1, e2));

        e1.IsInPit = true;
        e2.IsInPit = true;
        Assert.Null(GapCalculator.Calc(SessionType.Race, e1, e2));

        e1.IsInPit = false;
        e2.IsInPit = false;

        // both entries are in the same lap
        Assert.Equal("+1:49.421", GapCalculator.Calc(SessionType.Race, e1, e2));

        // Entry2 has one lap more and the higher TrackPositionPercent -> one lap in front
        e1.CurrentLap = 9;
        e1.TrackPositionPercent = 0.010;
        e2.CurrentLap = 10;
        e2.TrackPositionPercent = 0.060;
        Assert.Equal("+1L", GapCalculator.Calc(SessionType.Race, e1, e2));

        // Entry2 has still one lap more, but entry1 has the higher TrackPositionPercent now -> not lapped and the gapToLeader should be displayed
        e1.TrackPositionPercent = 0.061;
        Assert.Equal("+1:49.421", GapCalculator.Calc(SessionType.Race, e1, e2));

        // Entry2 has two laps more now, but entry1 has still the higher TrackPositionPercent -> one lap in front
        e1.CurrentLap = 8;
        Assert.Equal("+1L", GapCalculator.Calc(SessionType.Race, e1, e2));

        // Entry2 has still two laps more, but entry2 has the higher TrackPositionPercent now -> two laps in front
        e1.TrackPositionPercent = 0.059;
        Assert.Equal("+2L", GapCalculator.Calc(SessionType.Race, e1, e2));

        // TrackPositionPercent should be reseted to 0.0 when it's null or greater or equals than 1.0
        e1.TrackPositionPercent = null;
        e2.TrackPositionPercent = 1.01;

        // both should be 0.0 and therefore it should still be 2 laps in front
        Assert.Equal("+2L", GapCalculator.Calc(SessionType.Race, e1, e2));

        // now test to another entry which is not the leader (e.g. both have a GapToLeader set)
        e1 = GetEntry1();
        e2 = GetEntry2();
        e2.GapToLeader = 10.491;

        // Entry1 has a gap of 109.421 to the leader, entry2 of 10.491
        Assert.Equal("+1:38.930", GapCalculator.Calc(SessionType.Race, e1, e2));

        // in some games the gaps are negative, but the program wants them positive
        e1.GapToLeader = -109.421;

        // the result should still be the same
        Assert.Equal("+1:38.930", GapCalculator.Calc(SessionType.Race, e1, e2));

        // the gap should be empty, when the first entry parameter is in front of the other
        e1.GapToLeader = -10.491;
        e2.GapToLeader = -109.421;
        Assert.Equal("", GapCalculator.Calc(SessionType.Race, e1, e2));
    }

    [Fact]
    public void TestGapForPractice()
    {
        TestGapForPracticeOrQualifyingSession(SessionType.Practice);
    }

    [Fact]
    public void TestGapForQualifying()
    {
        TestGapForPracticeOrQualifyingSession(SessionType.Qualifying);
    }

    [Fact]
    public void TestIsFasterThan()
    {
        Assert.False(GapCalculator.IsFasterThan(null, null));
        Assert.False(GapCalculator.IsFasterThan(null, TimeSpan.Parse("00:01:30.1030000")));
        Assert.False(GapCalculator.IsFasterThan(TimeSpan.Parse("00:01:30.1030000"), TimeSpan.Parse("00:01:30.1030000")));
        Assert.False(GapCalculator.IsFasterThan(TimeSpan.Parse("00:01:30.1040000"), TimeSpan.Parse("00:01:30.1030000")));

        Assert.True(GapCalculator.IsFasterThan(TimeSpan.Parse("00:01:30.1030000"), null));
        Assert.True(GapCalculator.IsFasterThan(TimeSpan.Parse("00:01:30.1020000"), TimeSpan.Parse("00:01:30.1030000")));
    }

    private static void TestGapForPracticeOrQualifyingSession(SessionType sessionType)
    {
        if (sessionType != SessionType.Practice && sessionType != SessionType.Qualifying)
        {
            throw new Exception("TestGapForPracticeOrQualifyingSession requires SessionType Practice or Qualifying!");
        }

        var e1 = GetEntry1();
        var e2 = GetEntry2();

        // Should not calculate any gap when one of both entries is null
        Assert.Null(GapCalculator.Calc(sessionType, null, null));
        Assert.Null(GapCalculator.Calc(sessionType, e1, null));
        Assert.Null(GapCalculator.Calc(sessionType, null, e2));

        // Should not calculate any gap when both entries have no lap time yet
        e1.BestTimes = new TestableSectorTimes
        {
            FullLap = null
        };

        e2.BestTimes = new TestableSectorTimes
        {
            FullLap = null
        };

        Assert.Null(GapCalculator.Calc(sessionType, e1, e2));

        // Should not calculate any gap when one of entries has no lap time
        e1.BestTimes = new TestableSectorTimes
        {
            FullLap = TimeSpan.Parse("00:01:41.4910000")
        };

        e2.BestTimes = new TestableSectorTimes
        {
            FullLap = null
        };

        Assert.Null(GapCalculator.Calc(sessionType, e1, e2));


        // entry1 has no lap time, but entry2 has -> no gap as well
        e1.BestTimes = new TestableSectorTimes
        {
            FullLap = null
        };

        e2.BestTimes = new TestableSectorTimes
        {
            FullLap = TimeSpan.Parse("00:01:41.4910000")
        };

        Assert.Null(GapCalculator.Calc(sessionType, e1, e2));

        // Now both entries have a lap time
        e1.BestTimes = new TestableSectorTimes
        {
            FullLap = TimeSpan.Parse("00:01:41.6980000")
        };

        e2.BestTimes = new TestableSectorTimes
        {
            FullLap = TimeSpan.Parse("00:01:41.4910000")
        };

        Assert.Equal("+0.207", GapCalculator.Calc(sessionType, e1, e2));

        // the gap should be empty, when the first entry parameter is in front of the other

        e1.BestTimes = new TestableSectorTimes
        {
            FullLap = TimeSpan.Parse("00:01:41.1000000")
        };

        e2.BestTimes = new TestableSectorTimes
        {
            FullLap = TimeSpan.Parse("00:01:41.4910000")
        };

        Assert.Equal("", GapCalculator.Calc(sessionType, e1, e2));
    }

    private static TestableOpponent GetEntry1()
    {
        return new TestableOpponent
        {
            TrackPositionPercent = 0.010,
            CurrentLap = 10,
            GapToLeader = 109.421,
        };
    }

    private static TestableOpponent GetEntry2()
    {
        return new TestableOpponent
        {
            TrackPositionPercent = 0.060,
            CurrentLap = 10,
            GapToLeader = null,
        };
    }
}