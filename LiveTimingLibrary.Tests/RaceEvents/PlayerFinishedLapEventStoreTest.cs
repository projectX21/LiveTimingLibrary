public class PlayerFinishedLapEventStoreTest
{
    [Fact]
    public void TestShouldThrowWhenFirstLapHasNotLapNumber1()
    {
        var store = new PlayerFinishedLapEventStore();
        Assert.Throws<Exception>(() => store.Add(GetSecondLap()));
    }

    [Fact]
    public void TestShouldThrowWhenConsecutiveLapNumberDoesNotMatch()
    {
        var store = new PlayerFinishedLapEventStore();
        store.Add(GetFirstLap());
        Assert.Throws<Exception>(() => store.Add(GetThirdLap()));
    }

    [Fact]
    public void TestCalculateTotalElapsedTimeAfterLastCompletedLap()
    {
        var store = new PlayerFinishedLapEventStore
        {
            CurrentLapTime = TimeSpan.Parse("00:00:41.3940000") // should not be considered
        };

        Assert.Equal(TimeSpan.Zero, store.CalcTotalElapsedTimeAfterLastCompletedLap());

        store.Add(GetFirstLap());
        Assert.Equal(TimeSpan.Parse("00:01:45.2910000"), store.CalcTotalElapsedTimeAfterLastCompletedLap());

        store.Add(GetSecondLap());
        Assert.Equal(TimeSpan.Parse("00:03:26.9590000"), store.CalcTotalElapsedTimeAfterLastCompletedLap());

        store.Add(GetThirdLap());
        Assert.Equal(TimeSpan.Parse("00:05:08.0730000"), store.CalcTotalElapsedTimeAfterLastCompletedLap());
    }

    [Fact]
    public void TestCalculateTotalElapsedTimeWithCurrentLapTime()
    {
        var store = new PlayerFinishedLapEventStore
        {
            CurrentLapTime = TimeSpan.Parse("00:00:06.3940000") // should be considered
        };

        Assert.Equal(TimeSpan.Parse("00:00:06.3940000"), store.CalcTotalElapsedTimeWithCurrentLapTime());

        store.Add(GetFirstLap());
        Assert.Equal(TimeSpan.Parse("00:01:51.6850000"), store.CalcTotalElapsedTimeWithCurrentLapTime());

        store.Add(GetSecondLap());
        Assert.Equal(TimeSpan.Parse("00:03:33.3530000"), store.CalcTotalElapsedTimeWithCurrentLapTime());

        store.Add(GetThirdLap());
        Assert.Equal(TimeSpan.Parse("00:05:14.4670000"), store.CalcTotalElapsedTimeWithCurrentLapTime());
    }

    [Fact]
    public void TestCalculateReloadTime()
    {
        var store = new PlayerFinishedLapEventStore
        {
            CurrentLapTime = TimeSpan.Parse("00:00:06.3940000") // should be considered
        };

        // no completed lap yet, reload time should be the current lap time
        Assert.Equal(TimeSpan.Parse("00:00:06.3940000"), store.CalcReloadTime(1));

        store.Add(GetFirstLap());

        // should still be the same, because the reload lap was the first lap
        Assert.Equal(TimeSpan.Parse("00:00:06.3940000"), store.CalcReloadTime(1));

        // reload lap is the second one -> first lap time + current lap time
        Assert.Equal(TimeSpan.Parse("00:01:51.6850000"), store.CalcReloadTime(2));

        store.Add(GetSecondLap());
        Assert.Equal(TimeSpan.Parse("00:00:06.3940000"), store.CalcReloadTime(1));
        Assert.Equal(TimeSpan.Parse("00:01:51.6850000"), store.CalcReloadTime(2));
        Assert.Equal(TimeSpan.Parse("00:03:33.3530000"), store.CalcReloadTime(3));

        store.Add(GetThirdLap());
        Assert.Equal(TimeSpan.Parse("00:00:06.3940000"), store.CalcReloadTime(1));
        Assert.Equal(TimeSpan.Parse("00:01:51.6850000"), store.CalcReloadTime(2));
        Assert.Equal(TimeSpan.Parse("00:03:33.3530000"), store.CalcReloadTime(3));
        Assert.Equal(TimeSpan.Parse("00:05:14.4670000"), store.CalcReloadTime(4));
    }

    [Fact]
    public void TestReset()
    {
        var store = new PlayerFinishedLapEventStore
        {
            CurrentLapTime = TimeSpan.Parse("00:00:06.3940000") // should be considered
        };

        store.Reset();
        Assert.Equal(TimeSpan.Zero, store.CurrentLapTime);
        Assert.Equal(TimeSpan.Zero, store.CalcTotalElapsedTimeAfterLastCompletedLap());
        Assert.Equal(TimeSpan.Zero, store.CalcTotalElapsedTimeWithCurrentLapTime());
        Assert.Equal(TimeSpan.Zero, store.CalcReloadTime(1));
    }

    public static PlayerFinishedLapEvent GetFirstLap()
    {
        return new PlayerFinishedLapEvent("testgame_testtrack_race", 1, TimeSpan.Parse("00:01:45.2910000"), TimeSpan.Parse("00:01:45.2910000"));
    }

    public static PlayerFinishedLapEvent GetSecondLap()
    {
        return new PlayerFinishedLapEvent("testgame_testtrack_race", 2, TimeSpan.Parse("00:01:41.6680000"), TimeSpan.Parse("00:03:26.9590000"));
    }

    public static PlayerFinishedLapEvent GetThirdLap()
    {
        return new PlayerFinishedLapEvent("testgame_testtrack_race", 3, TimeSpan.Parse("00:01:41.1140000"), TimeSpan.Parse("00:05:08.0730000"));
    }
}
