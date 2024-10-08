using Moq;

public class RaceEventHandlerTest
{
    [Fact]
    public void TestAddPitEvent()
    {
        var mockRecoveryFile = new Mock<IRaceEventRecoveryFile>();
        var mockPitEventStore = new Mock<IPitEventStore>();

        var mockPlayerFinishedLapEventStore = new Mock<IPlayerFinishedLapEventStore>();
        mockPlayerFinishedLapEventStore.Setup(m => m.CalcTotalElapsedTimeWithCurrentLapTime()).Returns(TimeSpan.Parse("01:39:10.4910000"));

        var handler = new RaceEventHandler(mockRecoveryFile.Object, mockPitEventStore.Object, mockPlayerFinishedLapEventStore.Object);
        handler.AddEvent(new PitEvent("testgame_testtrack_race", RaceEventType.PitIn, "107", 41));
        var expectedFinalEvent = new PitEvent("testgame_testtrack_race", RaceEventType.PitIn, "107", 41, TimeSpan.Parse("01:39:10.4910000"));
        mockRecoveryFile.Verify(m => m.AddEvent(expectedFinalEvent), Times.Once());
        mockPitEventStore.Verify(m => m.Add(expectedFinalEvent), Times.Once());
    }

    [Fact]
    public void TestAddPlayerFinishedLapEvent()
    {
        var mockRecoveryFile = new Mock<IRaceEventRecoveryFile>();

        var mockPlayerFinishedLapEventStore = new Mock<IPlayerFinishedLapEventStore>();
        mockPlayerFinishedLapEventStore.Setup(m => m.CalcTotalElapsedTimeAfterLastCompletedLap()).Returns(TimeSpan.Parse("01:39:10.4910000"));

        var mockPitEventStore = new Mock<IPitEventStore>();

        var handler = new RaceEventHandler(mockRecoveryFile.Object, mockPitEventStore.Object, mockPlayerFinishedLapEventStore.Object);
        handler.AddEvent(new PlayerFinishedLapEvent("testgame_testtrack_race", 41, TimeSpan.Parse("00:01:35.3810000")));
        var expectedFinalEvent = new PlayerFinishedLapEvent("testgame_testtrack_race", 41, TimeSpan.Parse("00:01:35.3810000"), TimeSpan.Parse("01:40:45.8720000"));
        mockRecoveryFile.Verify(m => m.AddEvent(expectedFinalEvent), Times.Once());
        mockPlayerFinishedLapEventStore.Verify(m => m.Add(expectedFinalEvent), Times.Once());
    }

    [Fact]
    public void TestSessionReloadEvent()
    {
        var mockRecoveryFile = new Mock<IRaceEventRecoveryFile>();

        mockRecoveryFile.Setup(m => m.ReadPitEvents("testgame_testtrack_race")).Returns(
            [PitEventStoreTest.GetFirstPitInEvent(), PitEventStoreTest.GetFirstPitOutEvent()]
        );

        mockRecoveryFile.Setup(m => m.ReadPlayerFinishedLapEvents("testgame_testtrack_race")).Returns(
            [PlayerFinishedLapEventStoreTest.GetFirstLap(), PlayerFinishedLapEventStoreTest.GetSecondLap()]
        );

        var mockPitEventStore = new Mock<IPitEventStore>();
        mockPitEventStore.Setup(m => m.Reset());
        mockPitEventStore.Setup(m => m.Add(It.IsAny<PitEvent>()));

        var mockPlayerFinishedLapEventStore = new Mock<IPlayerFinishedLapEventStore>();
        mockPlayerFinishedLapEventStore.Setup(m => m.CalcReloadTime(It.IsAny<int>())).Returns(TimeSpan.Parse("01:39:10.4910000"));
        mockPlayerFinishedLapEventStore.Setup(m => m.Reset());
        mockPlayerFinishedLapEventStore.Setup(m => m.Add(It.IsAny<PlayerFinishedLapEvent>()));

        var handler = new RaceEventHandler(mockRecoveryFile.Object, mockPitEventStore.Object, mockPlayerFinishedLapEventStore.Object);
        handler.AddEvent(new SessionReloadEvent("testgame_testtrack_race", 41));
        var expectedFinalEvent = new SessionReloadEvent("testgame_testtrack_race", 41, TimeSpan.Parse("01:39:10.4910000"));
        mockRecoveryFile.Verify(m => m.AddEvent(expectedFinalEvent), Times.Once());

        mockPitEventStore.Verify(m => m.Reset(), Times.Once());
        mockRecoveryFile.Verify(m => m.ReadPitEvents("testgame_testtrack_race"), Times.Once());
        mockPitEventStore.Verify(m => m.Add(It.IsAny<PitEvent>()), Times.Exactly(2));
        mockPitEventStore.Verify(m => m.Add(PitEventStoreTest.GetFirstPitInEvent()), Times.Once());
        mockPitEventStore.Verify(m => m.Add(PitEventStoreTest.GetFirstPitOutEvent()), Times.Once());

        mockPlayerFinishedLapEventStore.Verify(m => m.Reset(), Times.Once());
        mockRecoveryFile.Verify(m => m.ReadPlayerFinishedLapEvents("testgame_testtrack_race"), Times.Once());
        mockPlayerFinishedLapEventStore.Verify(m => m.Add(It.IsAny<PlayerFinishedLapEvent>()), Times.Exactly(2));
        mockPlayerFinishedLapEventStore.Verify(m => m.Add(PlayerFinishedLapEventStoreTest.GetFirstLap()), Times.Once());
        mockPlayerFinishedLapEventStore.Verify(m => m.Add(PlayerFinishedLapEventStoreTest.GetSecondLap()), Times.Once());
    }

    [Fact]
    public void TestGetElapsedSessionTime()
    {
        var mockRecoveryFile = new Mock<IRaceEventRecoveryFile>();
        var mockPitEventStore = new Mock<IPitEventStore>();

        var mockPlayerFinishedLapEventStore = new Mock<IPlayerFinishedLapEventStore>();
        mockPlayerFinishedLapEventStore.Setup(m => m.CalcTotalElapsedTimeWithCurrentLapTime()).Returns(TimeSpan.Parse("01:39:10.4910000"));

        var handler = new RaceEventHandler(mockRecoveryFile.Object, mockPitEventStore.Object, mockPlayerFinishedLapEventStore.Object);
        Assert.Equal(TimeSpan.Parse("01:39:10.4910000"), handler.GetElapsedSessionTime());
    }
}
