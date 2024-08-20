public class ConcreteRaceEvent : RaceEvent
{
    public override string ToRecoveryFileFormat()
    {
        return "test";
    }

    public static string[] TestSplitLine(string line)
    {
        return SplitLine(line);
    }

    public static TimeSpan TestToTimeSpan(string value)
    {
        return ToTimeSpan(value);
    }

    public static int TestToInt(string value)
    {
        return ToInt(value);
    }
}

public class RaceEventTest
{
    [Fact]
    public void TestGetRaceEventTypeFromLine()
    {
        Assert.Equal(RaceEventType.PlayerFinishedLap, RaceEvent.GetRaceEventTypeFromLine("46452ab12lef;PLAYER_FINISHED_LAP;2;00:01:41.4840000;00:03:21.4920000;00:56:38.508"));
        Assert.Equal(RaceEventType.PitIn, RaceEvent.GetRaceEventTypeFromLine("46452ab12lef;PIT_IN;107;14;00:52:29.0490000;01:07:30.9510000"));
        Assert.Equal(RaceEventType.PitOut, RaceEvent.GetRaceEventTypeFromLine("46452ab12lef;PIT_OUT;107;15;00:54:12.3710000;01:05:47.6290000"));
        Assert.Equal(RaceEventType.SessionReload, RaceEvent.GetRaceEventTypeFromLine("46452ab12lef;SESSION_RELOAD;12;00:37:18.1090000;00:21:41.8910000"));
    }

    [Fact]
    public void TestSplitLine()
    {
        // should split by default delimiter ;
        Assert.Equal(["1", "2", "3", "4", "5"], ConcreteRaceEvent.TestSplitLine("1;2;3;4;5"));

        // wrong delimiter
        Assert.Equal(["1,2,3,4,5"], ConcreteRaceEvent.TestSplitLine("1,2,3,4,5"));
    }

    [Fact]
    public void TestToTimeSpan()
    {
        Assert.Equal(TimeSpan.Parse("00:10:20.3450000"), ConcreteRaceEvent.TestToTimeSpan("00:10:20.3450000"));
        Assert.Throws<FormatException>(() => ConcreteRaceEvent.TestToTimeSpan("invalid"));
    }

    [Fact]
    public void TestToInt()
    {
        Assert.Equal(-5, ConcreteRaceEvent.TestToInt("-5"));
        Assert.Equal(0, ConcreteRaceEvent.TestToInt("0"));
        Assert.Equal(10, ConcreteRaceEvent.TestToInt("10"));
        Assert.Throws<Exception>(() => ConcreteRaceEvent.TestToInt("a"));
    }
}