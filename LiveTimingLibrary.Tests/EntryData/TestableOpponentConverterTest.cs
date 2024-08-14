using GameReaderCommon;
using Moq;

public class TestableOpponentConverterTest
{
    [Fact]
    public void TestFromOpponent()
    {
        var result = TestableOpponentConverter.FromOpponent(CreateOpponent());
        Assert.Equal(CreateExpectedResult(), result);
    }

    [Fact]
    public void TestInPit()
    {
        var o = CreateOpponent();

        // one of the three must be true in order to make IsInPit true
        o.IsCarInPit = false;
        o.IsCarInPitLane = false;
        o.IsCarInGarage = false;
        Assert.False(TestableOpponentConverter.FromOpponent(o).IsInPit);

        o.IsCarInPit = true;
        o.IsCarInPitLane = false;
        o.IsCarInGarage = false;
        Assert.True(TestableOpponentConverter.FromOpponent(o).IsInPit);

        o.IsCarInPit = false;
        o.IsCarInPitLane = true;
        o.IsCarInGarage = false;
        Assert.True(TestableOpponentConverter.FromOpponent(o).IsInPit);

        o.IsCarInPit = false;
        o.IsCarInPitLane = false;
        o.IsCarInGarage = true;
        Assert.True(TestableOpponentConverter.FromOpponent(o).IsInPit);
    }

    public static Opponent CreateOpponent(string name = "Name", string carNumber = "107")
    {
        SectorSplits bestTimes = new();
        bestTimes.SetSplit(1, TimeSpan.Parse("00:00:23.1290000"));
        bestTimes.SetSplit(2, TimeSpan.Parse("00:01:02.6800000"));
        bestTimes.SetSplit(3, TimeSpan.Parse("00:00:15.9570000"));
        bestTimes.SetSplit(4, TimeSpan.Parse("00:01:41.7660000"));

        return new Opponent
        {
            Name = name,
            TeamName = "TeamName",
            IsCarInPit = true,
            IsCarInPitLane = false,
            Position = 5,
            CarName = "CarName",
            CarClass = "CarClass",
            IsPlayer = false,
            TrackPositionPercent = 0.3829,
            CurrentLap = 9,
            CurrentLapTime = TimeSpan.Parse("00:01:01.1920000"),
            CurrentSector = 2,
            CarNumber = carNumber,
            GaptoLeader = 65.192,
            // the sector times containing the total time 
            CurrentLapSectorTimes = SectorTimes.FromTimes([
                TimeSpan.Parse("00:00:23.1920000").TotalMilliseconds,
                TimeSpan.Parse("00:01:25.8570000").TotalMilliseconds
            ]),
            LastLapSectorTimes = SectorTimes.FromTimes([
                TimeSpan.Parse("00:00:23.4120000").TotalMilliseconds,
                TimeSpan.Parse("00:01:26.5200000").TotalMilliseconds,
                TimeSpan.Parse("00:01:43.4390000").TotalMilliseconds,
                TimeSpan.Parse("00:01:43.4390000").TotalMilliseconds
            ]),
            BestSectorSplits = bestTimes,
            BestLapTime = TimeSpan.Parse("00:01:41.7660000"),
            FrontTyreCompound = "Soft",
            RearTyreCompound = "Hard",
            StartPosition = 18
        };
    }

    public static TestableOpponent CreateExpectedResult(string name = "Name", string carNumber = "107")
    {
        return new TestableOpponent
        {
            Name = name,
            TeamName = "TeamName",
            IsInPit = true,
            Position = 5,
            CarName = "CarName",
            CarClass = "CarClass",
            IsPlayer = false,
            TrackPositionPercent = 0.3829,
            CurrentLap = 9,
            CurrentLapTime = TimeSpan.Parse("00:01:01.1920000"),
            CurrentSector = 2,
            CarNumber = carNumber,
            GapToLeader = 65.192,
            CurrentTimes = new TestableSectorTimes
            {
                Sector1 = TimeSpan.Parse("00:00:23.1920000"),
                Sector2 = TimeSpan.Parse("00:01:02.6650000")
            },
            LastTimes = new TestableSectorTimes
            {
                Sector1 = TimeSpan.Parse("00:00:23.4120000"),
                Sector2 = TimeSpan.Parse("00:01:03.1080000"),
                Sector3 = TimeSpan.Parse("00:00:16.9190000"),
                FullLap = TimeSpan.Parse("00:01:43.4390000")
            },
            BestTimes = new TestableSectorTimes
            {
                Sector1 = TimeSpan.Parse("00:00:23.1290000"),
                Sector2 = TimeSpan.Parse("00:01:02.6800000"),
                Sector3 = TimeSpan.Parse("00:00:15.9570000"),
                FullLap = TimeSpan.Parse("00:01:41.7660000")
            },
            FrontTyreCompound = "Soft",
            RearTyreCompound = "Hard",
            StartPosition = 18
        };
    }
}