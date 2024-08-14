using Moq;

public class TestableOpponentConverterTest
{
    [Fact]
    public void TestFromOpponent()
    {

        var result = TestableOpponentConverter.FromOpponent(CreateOpponentMock().Object);
        Assert.Equal(CreateExpectedResult(), result);
    }

    [Fact]
    public void TestNormalizeTimeSpan()
    {
        var mockCurrentLap = CreateCurrentLapMock();
        mockCurrentLap.Setup(m => m.GetSectorSplit(1)).Returns(TimeSpan.Parse("00:00:00.0000000"));
        mockCurrentLap.Setup(m => m.GetSectorSplit(2)).Returns(TimeSpan.Parse("00:00:00.0000000"));

        var mockLastLap = CreateLastLapMock();
        mockLastLap.Setup(m => m.GetSectorSplit(1)).Returns(TimeSpan.Parse("00:00:00.0000000"));
        mockLastLap.Setup(m => m.GetSectorSplit(2)).Returns(TimeSpan.Parse("00:00:00.0000000"));
        mockLastLap.Setup(m => m.GetSectorSplit(3)).Returns(TimeSpan.Parse("00:00:00.0000000"));
        mockLastLap.Setup(m => m.GetLapTime()).Returns(TimeSpan.Parse("00:00:00.0000000"));

        var mockBestLap = CreateBestLapMock();
        mockBestLap.Setup(m => m.GetSectorSplit(1)).Returns(TimeSpan.Parse("00:00:00.0000000"));
        mockBestLap.Setup(m => m.GetSectorSplit(2)).Returns(TimeSpan.Parse("00:00:00.0000000"));
        mockBestLap.Setup(m => m.GetSectorSplit(3)).Returns(TimeSpan.Parse("00:00:00.0000000"));

        var mock = CreateOpponentMock();
        mock.SetupGet(m => m.CurrentLapTime).Returns(TimeSpan.Parse("00:00:00.0000000"));
        mock.SetupGet(m => m.CurrentLapSectorTimes).Returns(mockCurrentLap.Object);
        mock.SetupGet(m => m.LastLapSectorTimes).Returns(mockLastLap.Object);
        mock.SetupGet(m => m.BestSectorSplits).Returns(mockBestLap.Object);
        mock.SetupGet(m => m.BestLapTime).Returns(TimeSpan.Parse("00:00:00.0000000"));

        var result = TestableOpponentConverter.FromOpponent(mock.Object);

        var expectedSectorTimes = new TestableSectorTimes
        {
            Sector1 = null,
            Sector2 = null,
            Sector3 = null
        };

        Assert.Equal(expectedSectorTimes, result.CurrentTimes);
        Assert.Equal(expectedSectorTimes, result.LastTimes);
        Assert.Equal(expectedSectorTimes, result.BestTimes);
        Assert.Null(result.CurrentLapTime);
    }

    [Fact]
    public void TestInPit()
    {
        var mock = CreateOpponentMock();

        // one of the three must be true in order to make IsInPit true
        mock.SetupGet(m => m.IsCarInPit).Returns(false);
        mock.SetupGet(m => m.IsCarInPitLane).Returns(false);
        mock.SetupGet(m => m.StandingStillInPitLane).Returns(false);
        Assert.False(TestableOpponentConverter.FromOpponent(mock.Object).IsInPit);

        mock.SetupGet(m => m.IsCarInPit).Returns(true);
        mock.SetupGet(m => m.IsCarInPitLane).Returns(false);
        mock.SetupGet(m => m.StandingStillInPitLane).Returns(false);
        Assert.True(TestableOpponentConverter.FromOpponent(mock.Object).IsInPit);

        mock.SetupGet(m => m.IsCarInPit).Returns(false);
        mock.SetupGet(m => m.IsCarInPitLane).Returns(true);
        mock.SetupGet(m => m.StandingStillInPitLane).Returns(false);
        Assert.True(TestableOpponentConverter.FromOpponent(mock.Object).IsInPit);

        mock.SetupGet(m => m.IsCarInPit).Returns(false);
        mock.SetupGet(m => m.IsCarInPitLane).Returns(false);
        mock.SetupGet(m => m.StandingStillInPitLane).Returns(true);
        Assert.True(TestableOpponentConverter.FromOpponent(mock.Object).IsInPit);
    }

    public static Mock<IOpponent> CreateOpponentMock(string name = "Name", string carNumber = "107")
    {
        var mockCurrentLap = CreateCurrentLapMock();
        var mockLastLap = CreateLastLapMock();
        var mockBestLap = CreateBestLapMock();

        var mock = new Mock<IOpponent>();
        mock.SetupGet(m => m.Manufacturer).Returns("Manufacturer");
        mock.SetupGet(m => m.Name).Returns(name);
        mock.SetupGet(m => m.TeamName).Returns("TeamName");
        mock.SetupGet(m => m.IsCarInPit).Returns(true);
        mock.SetupGet(m => m.IsCarInPitLane).Returns(false);
        mock.SetupGet(m => m.StandingStillInPitLane).Returns(true);
        mock.SetupGet(m => m.Position).Returns(5);
        mock.SetupGet(m => m.CarName).Returns("CarName");
        mock.SetupGet(m => m.CarClass).Returns("CarClass");
        mock.SetupGet(m => m.IsPlayer).Returns(false);
        mock.SetupGet(m => m.TrackPositionPercent).Returns(0.3829);
        mock.SetupGet(m => m.CurrentLap).Returns(9);
        mock.SetupGet(m => m.CurrentLapTime).Returns(TimeSpan.Parse("00:01:01.1920000"));
        mock.SetupGet(m => m.CurrentSector).Returns(2);
        mock.SetupGet(m => m.CarNumber).Returns(carNumber);
        mock.SetupGet(m => m.GaptoLeader).Returns(65.192);
        mock.SetupGet(m => m.CurrentLapSectorTimes).Returns(mockCurrentLap.Object);
        mock.SetupGet(m => m.LastLapSectorTimes).Returns(mockLastLap.Object);
        mock.SetupGet(m => m.BestSectorSplits).Returns(mockBestLap.Object);
        mock.SetupGet(m => m.BestLapTime).Returns(TimeSpan.Parse("00:01:41.7660000"));
        mock.SetupGet(m => m.FrontTyreCompound).Returns("Soft");
        mock.SetupGet(m => m.RearTyreCompound).Returns("Hard");
        mock.SetupGet(m => m.StartPosition).Returns(18);
        return mock;
    }

    public static TestableOpponent CreateExpectedResult(string name = "Name", string carNumber = "107")
    {
        return new TestableOpponent
        {
            Manufacturer = "Manufacturer",
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

    private static Mock<ISectorTimes> CreateCurrentLapMock()
    {
        var mock = new Mock<ISectorTimes>();
        mock.Setup(m => m.GetSectorSplit(1)).Returns(TimeSpan.Parse("00:00:23.1920000"));
        mock.Setup(m => m.GetSectorSplit(2)).Returns(TimeSpan.Parse("00:01:02.6650000"));
        return mock;
    }

    private static Mock<ISectorTimes> CreateLastLapMock()
    {
        var mock = new Mock<ISectorTimes>();
        mock.Setup(m => m.GetSectorSplit(1)).Returns(TimeSpan.Parse("00:00:23.4120000"));
        mock.Setup(m => m.GetSectorSplit(2)).Returns(TimeSpan.Parse("00:01:03.1080000"));
        mock.Setup(m => m.GetSectorSplit(3)).Returns(TimeSpan.Parse("00:00:16.9190000"));
        mock.Setup(m => m.GetLapTime()).Returns(TimeSpan.Parse("00:01:43.4390000"));
        return mock;
    }

    private static Mock<ISectorTimes> CreateBestLapMock()
    {
        var mock = new Mock<ISectorTimes>();
        mock.Setup(m => m.GetSectorSplit(1)).Returns(TimeSpan.Parse("00:00:23.1290000"));
        mock.Setup(m => m.GetSectorSplit(2)).Returns(TimeSpan.Parse("00:01:02.6800000"));
        mock.Setup(m => m.GetSectorSplit(3)).Returns(TimeSpan.Parse("00:00:15.9570000"));
        return mock;
    }
}