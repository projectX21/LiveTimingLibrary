using Moq;

public class TestGameProcessor()
{
    [Fact]
    public void TestReturnDirectlyWhenGameIsNotRunning()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var mockEntryProgressStore = new Mock<IEntryProgressStore>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, mockEntryProgressStore.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = false,
            NewData = new TestableStatusDataBase
            {
                SessionName = "Race",
                TrackName = "Testtrack",
                Opponents = [
                    new TestableOpponent
                    {
                        IsPlayer = true,
                        CurrentLap = 2,
                        CurrentSector = 3,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.4100000")
                    }
                ]
            }
        };

        // GameRunning is false, therefore it shouldn't do anything
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.Add("SessionType", SessionType.Race.ToString()), Times.Never());

        // now GameRunning is true which should execute the update cycle
        gameData.GameRunning = true;
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.Add("SessionType", SessionType.Race.ToString()), Times.Once());
    }

    [Fact]
    public void TestHandleSessionIdChange()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var mockEntryProgressStore = new Mock<IEntryProgressStore>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, mockEntryProgressStore.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Testgame",
            NewData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Qualifying",
                Opponents = [
                    new TestableOpponent
                    {
                        IsPlayer = true,
                        CurrentLap = 2,
                        CurrentSector = 3,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.4100000")
                    }
                ]
            }
        };

        // SessionId is empty initial, which shouldn't be treated as a session id change
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.ResetAll(), Times.Never());
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore(It.IsAny<string>()), Times.Never());
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore(It.IsAny<string>()), Times.Never());

        // Run Processor with the same data again.
        // SessionId shouldn't have changed (SessionId will be generated by the three parameters GameName, TrackName and SessionName)
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.ResetAll(), Times.Never());
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore(It.IsAny<string>()), Times.Never());
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore(It.IsAny<string>()), Times.Never());

        // new SessionName -> should handle session change
        gameData.NewData.SessionName = "Race";
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.ResetAll(), Times.Once());
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore(It.IsAny<string>()), Times.Once());
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore("testgame_testtrack_race"), Times.Once());
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore(It.IsAny<string>()), Times.Once());
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore("testgame_testtrack_race"), Times.Once());

        // new TrackName -> -> should handle session change again
        gameData.NewData.TrackName = "Testtrack new";
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.ResetAll(), Times.Exactly(2));
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore(It.IsAny<string>()), Times.Exactly(2));
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore("testgame_testtrack_new_race"), Times.Once());
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore(It.IsAny<string>()), Times.Exactly(2));
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore("testgame_testtrack_new_race"), Times.Once());

        // new GameName -> -> should handle session change again
        gameData.NewData.GameName = "Testgame new";
        processor.Run(gameData);
        mockPropertyManager.Verify(m => m.ResetAll(), Times.Exactly(3));
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore(It.IsAny<string>()), Times.Exactly(3));
        mockRaceEventHandler.Verify(m => m.ReinitPitEventStore("testgame_new_testtrack_new_race"), Times.Once());
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore(It.IsAny<string>()), Times.Exactly(3));
        mockRaceEventHandler.Verify(m => m.ReinitPlayerFinishedLapEventStore("testgame_new_testtrack_new_race"), Times.Once());
    }

    [Fact]
    public void TestHandleSessionReload()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var mockEntryProgressStore = new Mock<IEntryProgressStore>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, mockEntryProgressStore.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Testgame",
            NewData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Race",
                Opponents = [
                    new TestableOpponent
                    {
                        Position = 1,
                        CarNumber = "107",
                        IsPlayer = true,
                        CurrentLap = 2,
                        CurrentSector = 3,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.4100000")
                    },
                    new TestableOpponent
                    {
                        Position = 2,
                        CarNumber = "108",
                        IsPlayer = false,
                        CurrentLap = 2,
                        CurrentSector = 3,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.4120000")
                    }
                ]
            }
        };

        // Shouldn't do anything for the first Run() execution, because _lastCurrentLap and _lastCurrentLapTime are null initially
        processor.Run(gameData);
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<SessionReloadEvent>()), Times.Never());

        // _lastCurrentLap (2) is less than the CurrentLap in NewData (3)
        gameData.NewData.Opponents[0].CurrentLap = 3;
        processor.Run(gameData);
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<SessionReloadEvent>()), Times.Never());

        // CurrentLap is the same, but the CurrentLapTime in NewData is greater than in _lastCurrentLapTime
        gameData.NewData.Opponents[0].CurrentLapTime = TimeSpan.Parse("00:00:04.4910000");
        processor.Run(gameData);
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<SessionReloadEvent>()), Times.Never());

        // Now the CurrentLapTime in NewData is less than _lastCurrentLapTime -> reload
        gameData.NewData.Opponents[0].CurrentLapTime = TimeSpan.Parse("00:00:02.5820000");
        processor.Run(gameData);
        SessionReloadEvent expected = new("testgame_testtrack_race", 3);
        mockRaceEventHandler.Verify(m => m.AddEvent(expected), Times.Once());

        // And finally the CurrentLap in NewData is less than _lastCurrentLap -> reload
        gameData.NewData.Opponents[0].CurrentLap = 2;
        processor.Run(gameData);
        expected = new("testgame_testtrack_race", 2);
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<SessionReloadEvent>()), Times.Exactly(2));
        mockRaceEventHandler.Verify(m => m.AddEvent(expected), Times.Once());
    }

    [Fact]
    public void TestUpdateCurrentLapTimeInLapEventStore()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var mockEntryProgressStore = new Mock<IEntryProgressStore>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, mockEntryProgressStore.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Testgame",
            NewData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Race",
                Opponents = [
                new TestableOpponent
                    {
                        IsPlayer = true,
                        CurrentLap = 2,
                        CurrentSector = 3,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.4100000")
                    }
            ]
            },
            OldData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Race",
                Opponents = [
                    new TestableOpponent
                    {
                        IsPlayer = true,
                        CurrentLap = 2,
                        CurrentSector = 3,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.3150000")
                    }
                ]
            }
        };
        processor.Run(gameData);
        mockRaceEventHandler.Verify(m => m.SetCurrentLapTime(TimeSpan.Parse("00:00:01.4100000")), Times.Once());

        gameData.NewData.Opponents[0].CurrentLapTime = TimeSpan.Parse("00:01:20.1930000");
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
        var mockEntryProgressStore = new Mock<IEntryProgressStore>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, mockEntryProgressStore.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Testgame",
            NewData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Race",
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                Opponents = [
                    new TestableOpponent
                        {
                            CarNumber = "107",
                            Position = 1,
                            CurrentLap = 2,
                            IsPlayer = true,
                            CurrentSector = 3,
                            CurrentLapTime = TimeSpan.Parse("00:00:01.3150000")
                        },
                        new TestableOpponent
                        {
                            CarNumber = "108",
                            Position = 2,
                            CurrentLap = 2,
                            IsPlayer = false,
                        },
                        new TestableOpponent
                        {
                            CarNumber = "109",
                            Position = 3,
                            CurrentLap = 4,
                            IsPlayer = false,
                        }
                ]
            },
            OldData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Race",
                Opponents = [
                    new TestableOpponent
                        {
                            CarNumber = "109",
                            Position = 1,
                            CurrentLap = 3,
                            IsPlayer = false,
                        },
                        new TestableOpponent
                        {
                            CarNumber = "110",
                            Position = 2,
                            CurrentLap = 2,
                            IsPlayer = false,
                        },
                        new TestableOpponent
                        {
                            CarNumber = "107",
                            Position = 3,
                            CurrentLap = 1,
                            IsPlayer = true,
                            CurrentSector = 3,
                            CurrentLapTime = TimeSpan.Parse("00:00:01.3090000")
                        },
                    ]
            }
        };
        processor.Run(gameData);
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                It.IsAny<int>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<FastestFragmentTimesStore>(),
                It.IsAny<IEntryProgressStore>()
            ),
            Times.Exactly(3)
        );

        // CarNumber 107
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                It.IsAny<int>(),
                gameData.NewData.Opponents[0],
                gameData.OldData.Opponents[2],
                null,
                null,
                It.IsAny<FastestFragmentTimesStore>(),
                It.IsAny<IEntryProgressStore>()
            ),
            Times.Once()
        );

        // CarNumber 108
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                It.IsAny<int>(),
                gameData.NewData.Opponents[1],
                null,
                gameData.NewData.Opponents[0],
                gameData.NewData.Opponents[0],
                It.IsAny<FastestFragmentTimesStore>(),
                It.IsAny<IEntryProgressStore>()
            ),
            Times.Once()
        );

        // CarNumber 109
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                It.IsAny<int>(),
                gameData.NewData.Opponents[2],
                gameData.OldData.Opponents[0],
                gameData.NewData.Opponents[0],
                gameData.NewData.Opponents[1],
                It.IsAny<FastestFragmentTimesStore>(),
                It.IsAny<IEntryProgressStore>()
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
        var mockEntryProgressStore = new Mock<IEntryProgressStore>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, mockEntryProgressStore.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Testgame",
            NewData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Race",
                Opponents = [
                    new TestableOpponent
                        {
                            CarNumber = "107",
                            Position = 1,
                            CurrentLap = 2,
                            CurrentSector = 1,
                            IsPlayer = true,
                            CurrentLapTime = TimeSpan.Parse("00:00:01.3150000")
                        }
                ]
            },
            OldData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Race",
                Opponents = [
                    new TestableOpponent
                        {
                            CarNumber = "107",
                            Position = 3,
                            CurrentLap = 2,
                            CurrentSector = 3,
                            IsPlayer = true,
                            CurrentLapTime = TimeSpan.Parse("00:00:01.3170000")
                        }
                ]
            }
        };
        processor.Run(gameData);

        // Should use OldData as NewData, when the CurrentLap/CurrentSector in OldData is higher than in NewData
        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                It.IsAny<int>(),
                gameData.OldData.Opponents[0],
                gameData.OldData.Opponents[0],
                null,
                null,
                It.IsAny<FastestFragmentTimesStore>(),
                It.IsAny<IEntryProgressStore>()
            ),
            Times.Once()
        );
    }

    [Fact]
    public void TestShouldThrowWhenPlayerDataCannotBeFound()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var mockEntryProgressStore = new Mock<IEntryProgressStore>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, mockEntryProgressStore.Object, "test");

        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Testgame",
            NewData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Race",
                Opponents = [
                    new TestableOpponent
                    {
                        CarNumber = "107",
                        Position = 1,
                        CurrentLap = 2,
                        CurrentSector = 1,
                        IsPlayer = false,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.3150000")
                    },
                    new TestableOpponent
                    {
                        CarNumber = "108",
                        Position = 2,
                        CurrentLap = 2,
                        CurrentSector = 1,
                        IsPlayer = false,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.3170000")
                    }
                ]
            }
        };

        Assert.Throws<Exception>(() => processor.Run(gameData));
    }

    [Fact]
    public void TestPrepareCustomScoring()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        var mockEntryProgressStore = new Mock<IEntryProgressStore>();
        var processor = new GameProcessor(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object, mockEntryProgressStore.Object, "test");

        mockEntryProgressStore.Setup(m => m.UseCustomGapCalculation()).Returns(true);
        mockEntryProgressStore.Setup(m => m.GetEntryIdsSortedByProgress()).Returns(["107", "110", "108", "109"]);
        mockRaceEventHandler.Setup(m => m.GetElapsedSessionTime()).Returns(TimeSpan.Parse("00:04:46.1090000"));

        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Testgame",
            NewData = new TestableStatusDataBase
            {
                GameName = "Testgame",
                TrackName = "Testtrack",
                SessionName = "Race",
                Opponents = [
                    new TestableOpponent
                    {
                        CarNumber = "107",
                        Position = 2,
                        CurrentLap = 2,
                        CurrentSector = 1,
                        TrackPositionPercent = 0.831,
                        IsPlayer = true,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.3150000")
                    },
                    new TestableOpponent
                    {
                        CarNumber = "108",
                        Position = 4,
                        CurrentLap = 2,
                        CurrentSector = 1,
                        TrackPositionPercent = 0.766,
                        IsPlayer = false,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.9880000")
                    },
                    new TestableOpponent
                    {
                        CarNumber = "109",
                        Position = 1,
                        CurrentLap = 2,
                        CurrentSector = 2,
                        TrackPositionPercent = 0.899,
                        IsPlayer = false,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.3020000")
                    },
                    new TestableOpponent
                    {
                        CarNumber = "110",
                        Position = 3,
                        CurrentLap = 2,
                        CurrentSector = 1,
                        TrackPositionPercent = 0.791,
                        IsPlayer = false,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.8550000")
                    }
                ]
            }
        };

        processor.Run(gameData);
        mockEntryProgressStore.Verify(m => m.AddIfNotAlreadyExists(It.IsAny<EntryProgress>()), Times.Exactly(4));
        mockEntryProgressStore.Verify(m => m.AddIfNotAlreadyExists(new EntryProgress("107", 2, 24, TimeSpan.Parse("00:04:46.1090000"), 2)), Times.Once);
        mockEntryProgressStore.Verify(m => m.AddIfNotAlreadyExists(new EntryProgress("108", 2, 22, TimeSpan.Parse("00:04:46.1090000"), 4)), Times.Once);
        mockEntryProgressStore.Verify(m => m.AddIfNotAlreadyExists(new EntryProgress("109", 2, 26, TimeSpan.Parse("00:04:46.1090000"), 1)), Times.Once);
        mockEntryProgressStore.Verify(m => m.AddIfNotAlreadyExists(new EntryProgress("110", 2, 23, TimeSpan.Parse("00:04:46.1090000"), 3)), Times.Once);

        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                It.IsAny<int>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<TestableOpponent>(),
                It.IsAny<FastestFragmentTimesStore>(),
                mockEntryProgressStore.Object
            ),
            Times.Exactly(4)
        );

        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                1,
                gameData.NewData.Opponents[0],
                null,
                null,
                null,
                It.IsAny<FastestFragmentTimesStore>(),
                mockEntryProgressStore.Object
            ),
            Times.Once()
        );

        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                2,
                gameData.NewData.Opponents[3],
                null,
                gameData.NewData.Opponents[0],
                gameData.NewData.Opponents[0],
                It.IsAny<FastestFragmentTimesStore>(),
                mockEntryProgressStore.Object
            ),
            Times.Once()
        );

        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                3,
                gameData.NewData.Opponents[1],
                null,
                gameData.NewData.Opponents[0],
                gameData.NewData.Opponents[3],
                It.IsAny<FastestFragmentTimesStore>(),
                mockEntryProgressStore.Object
            ),
            Times.Once()
        );

        mockRaceEntryProcessor.Verify(
            m => m.Process(
                "testgame_testtrack_race",
                SessionType.Race,
                4,
                gameData.NewData.Opponents[2],
                null,
                gameData.NewData.Opponents[0],
                gameData.NewData.Opponents[1],
                It.IsAny<FastestFragmentTimesStore>(),
                mockEntryProgressStore.Object
            ),
            Times.Once()
        );
    }
}
