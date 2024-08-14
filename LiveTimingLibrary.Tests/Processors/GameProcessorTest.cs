using Moq;

public class TestGameProcessor()
{
    [Fact]
    public void TestDoCalculationCycle()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            NewData = new TestableStatusDataBase
            {
                SessionName = "Race",
                Opponents = []
            }
        };

        // the easiest way is to check if the SESSION_TYPE is set in the propertyManager,
        // because it will be set again in every cycle.

        // OldData is null, therefore the update cycle won't be executed
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.Add("SessionType", SessionType.Race.ToString()), Times.Never());

        // now OldData is set
        gameData.OldData = new TestableStatusDataBase();
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.Add("SessionType", SessionType.Race.ToString()), Times.Once());

        // now GameRunning is false which shouldn't execute the update cycle
        gameData.GameRunning = false;
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.Add("SessionType", SessionType.Race.ToString()), Times.Once());

        // And finally GameRunning is true again, but NewData is null
        gameData.GameRunning = true;
        gameData.NewData = null;
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.Add("SessionType", SessionType.Race.ToString()), Times.Once());
    }

    [Fact]
    public void TestHandleSessionIdChange()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            OldData = new TestableStatusDataBase
            {
                SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
                SessionName = "Race",
                Opponents = []
            },
            NewData = new TestableStatusDataBase
            {
                SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
                SessionName = "Race",
                Opponents = []
            }
        };

        // SessionId hasn't changed
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.ResetAll(), Times.Never());
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore(), Times.Never());
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore(), Times.Never());

        // now a other SessionId is set -> should handle session change again
        gameData.NewData.SessionId = Guid.Parse("00000000-1234-3456-6789-000000000001");
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.ResetAll(), Times.Once());
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore(), Times.Once());
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore(), Times.Once());
    }

    [Fact]
    public void TestHandleSessionReload()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            NewData = new TestableStatusDataBase
            {
                SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
                SessionName = "Race",
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                Opponents = []
            }
        };

        // Old data is null -> no session reload
        processor.Run(gameData);
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<SessionReloadEvent>()), Times.Never());

        // Old data is set, but the CurrentLap (1) is less than the CurrentLap in NewData (2)
        gameData.OldData = new TestableStatusDataBase
        {
            SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
            CurrentLap = 1,
            CurrentLapTime = TimeSpan.Parse("00:01:31.3950000"),
        };
        processor.Run(gameData);
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<SessionReloadEvent>()), Times.Never());

        // CurrentLap is the same, but the CurrentLapTime in NewData is greater than in OldData
        gameData.OldData.CurrentLap = 2;
        gameData.OldData.CurrentLapTime = TimeSpan.Parse("00:00:00.9850000");
        processor.Run(gameData);
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<SessionReloadEvent>()), Times.Never());

        // Now the CurrentLapTime in OldData is greater than in NewData -> reload
        gameData.OldData.CurrentLapTime = TimeSpan.Parse("00:00:04.9850000");
        processor.Run(gameData);
        SessionReloadEvent expected = new(2);
        mockRaceEventHandler.Verify(m => m.AddEvent(expected), Times.Once());

        // Now the CurrentLap in OldData is greater than in NewData -> reload
        gameData.NewData.CurrentLap = 5;
        gameData.OldData.CurrentLap = 9;
        processor.Run(gameData);
        expected = new(5);
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<SessionReloadEvent>()), Times.Exactly(2));
        mockRaceEventHandler.Verify(m => m.AddEvent(expected), Times.Once());
    }

    [Fact]
    public void TestUpdateCurrentLapTimeInLapEventStore()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            NewData = new TestableStatusDataBase
            {
                SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
                SessionName = "Race",
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                Opponents = []
            },
            OldData = new TestableStatusDataBase
            {
                SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
                SessionName = "Race",
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                Opponents = []
            }
        };
        processor.Run(gameData);
        mockRaceEventHandler.Verify(m => m.SetCurrentLapTime(TimeSpan.Parse("00:00:01.4100000")), Times.Once());

        gameData.NewData.CurrentLapTime = TimeSpan.Parse("00:01:20.1930000");
        processor.Run(gameData);
        mockRaceEventHandler.Verify(m => m.SetCurrentLapTime(It.IsAny<TimeSpan>()), Times.Exactly(2));
        mockRaceEventHandler.Verify(m => m.SetCurrentLapTime(TimeSpan.Parse("00:01:20.1930000")), Times.Once());
    }

    [Fact]
    public void TestProcessEntries()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            NewData = new TestableStatusDataBase
            {
                SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
                SessionName = "Race",
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                Opponents = [
                    new TestableOpponent
                    {
                        CarNumber = "107",
                        Position = 1,
                        CurrentLap = 2
                    },
                    new TestableOpponent
                    {
                        CarNumber = "108",
                        Position = 2,
                        CurrentLap = 2
                    },
                    new TestableOpponent
                    {
                        CarNumber = "109",
                        Position = 3,
                        CurrentLap = 4
                    }
                ]
            },
            OldData = new TestableStatusDataBase
            {
                SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
                SessionName = "Race",
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:00.7980000"),
                Opponents = [
                    new TestableOpponent
                    {
                        CarNumber = "109",
                        Position = 1,
                        CurrentLap = 3,
                    },
                    new TestableOpponent
                    {
                        CarNumber = "110",
                        Position = 2,
                        CurrentLap = 2
                    },
                    new TestableOpponent
                    {
                        CarNumber = "107",
                        Position = 3,
                        CurrentLap = 1
                    },
                ]
            }
        };
        processor.Run(gameData);
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                SessionType.Race,
                It.IsAny<TestableOpponent>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<FastestFragmentTimesStore>()
            ),
            Times.Exactly(3)
        );

        // CarNumber 107
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                SessionType.Race,
                gameData.NewData.Opponents[0],
                gameData.OldData.Opponents[2],
                null,
                null,
                It.IsAny<FastestFragmentTimesStore>()
            ),
            Times.Once()
        );

        // CarNumber 108
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                SessionType.Race,
                gameData.NewData.Opponents[1],
                null,
                gameData.NewData.Opponents[0],
                gameData.NewData.Opponents[0],
                It.IsAny<FastestFragmentTimesStore>()
            ),
            Times.Once()
        );

        // CarNumber 109
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                SessionType.Race,
                gameData.NewData.Opponents[2],
                gameData.OldData.Opponents[0],
                gameData.NewData.Opponents[0],
                gameData.NewData.Opponents[1],
                It.IsAny<FastestFragmentTimesStore>()
            ),
            Times.Once()
        );
    }

    [Fact]
    public void TestNormalizeEntryData()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            NewData = new TestableStatusDataBase
            {
                SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
                SessionName = "Race",
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                Opponents = [
                    new TestableOpponent
                    {
                        CarNumber = "107",
                        Position = 1,
                        CurrentLap = 2,
                        CurrentSector = 1
                    }
                ]
            },
            OldData = new TestableStatusDataBase
            {
                SessionId = Guid.Parse("00000000-1234-3456-6789-000000000000"),
                SessionName = "Race",
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:00.7980000"),
                Opponents = [
                    new TestableOpponent
                    {
                        CarNumber = "107",
                        Position = 3,
                        CurrentLap = 2,
                        CurrentSector = 3
                    }
                ]
            }
        };
        processor.Run(gameData);

        // Should use OldData as NewData, when the CurrentLap/CurrentSector in OldData is higher than in NewData
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                SessionType.Race,
                gameData.OldData.Opponents[0],
                gameData.OldData.Opponents[0],
                null,
                null,
                It.IsAny<FastestFragmentTimesStore>()
            ),
            Times.Once()
        );
    }
}