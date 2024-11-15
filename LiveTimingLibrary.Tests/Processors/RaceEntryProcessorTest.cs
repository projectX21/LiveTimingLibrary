using Moq;

public class RaceEntryProcessorTest
{

    [Fact]
    public void TestShouldNotProcessAnyPitEventWhenSessionTypeIsNotARace()
    {
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        mockRaceEventHandler.Setup(m => m.GetPitDataByEntryId(It.IsAny<string>())).Returns(new EntryPitData("", 0, 0, null, null));

        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockFastestFragmentTimesStore = new Mock<IFastestFragmentTimesStore>();

        RaceEntryProcessor processor = new(mockPropertyManager.Object, mockRaceEventHandler.Object);

        var oldEntryData = new TestableOpponent
        {
            CarNumber = "107",
            IsInPit = false,
            CurrentLap = 2,
            CurrentTimes = new TestableSectorTimes(),
            LastTimes = new TestableSectorTimes(),
            BestTimes = new TestableSectorTimes()
        };

        var newEntryData = new TestableOpponent
        {
            CarNumber = "107",
            Position = 6,
            IsInPit = false,
            CurrentLap = 2,
            CurrentTimes = new TestableSectorTimes(),
            LastTimes = new TestableSectorTimes(),
            BestTimes = new TestableSectorTimes()
        };

        // IsInPit is false for both
        processor.Process("testgame_testtrack", SessionType.Qualifying, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<PitEvent>()), Times.Never());

        // IsInPit is true for both
        oldEntryData.IsInPit = true;
        newEntryData.IsInPit = true;
        processor.Process("testgame_testtrack", SessionType.Qualifying, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<PitEvent>()), Times.Never());

        // IsInPit is only true on NewEntryData -> for SessionType.Race a PitEvent with Type PitIn should be created, but not for Qualifying
        oldEntryData.IsInPit = false;
        newEntryData.IsInPit = true;
        processor.Process("testgame_testtrack", SessionType.Qualifying, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<PitEvent>()), Times.Never());

        // IsInPit is only true on OldEntryData -> for SessionType.Race a PitEvent with Type PitOut should be created, but not for Qualifying
        oldEntryData.IsInPit = true;
        newEntryData.IsInPit = false;
        processor.Process("testgame_testtrack", SessionType.Qualifying, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        PitEvent expectedPitOutEvent = new("testgame_testtrack", RaceEventType.PitOut, "107", 2, TimeSpan.Parse("01:14:20.2010000"));
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<PitEvent>()), Times.Never());

        // No new event because OldEntryData is null
        oldEntryData = null;
        processor.Process("testgame_testtrack", SessionType.Qualifying, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockRaceEventHandler.Verify(m => m.AddEvent(It.IsAny<PitEvent>()), Times.Never());
    }

    [Fact]
    public void TestGeneralProperties()
    {
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        mockRaceEventHandler.Setup(m => m.GetPitDataByEntryId(It.IsAny<string>())).Returns(new EntryPitData("", 0, 0, null, null));

        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockFastestFragmentTimesStore = new Mock<IFastestFragmentTimesStore>();

        RaceEntryProcessor processor = new(mockPropertyManager.Object, mockRaceEventHandler.Object);

        var oldEntryData = new TestableOpponent
        {
            CarNumber = "107",
            IsInPit = false,
            CurrentLap = 2,
            CurrentTimes = new TestableSectorTimes(),
            LastTimes = new TestableSectorTimes(),
            BestTimes = new TestableSectorTimes(),
            CurrentSector = 1
        };

        var newEntryData = new TestableOpponent
        {
            CarNumber = "107",
            Position = 6,
            Name = "Test Name",
            TeamName = "Test Team Name",
            CarName = "Test Car Name",
            CarClass = "Test Car Class",
            IsInPit = false,
            CurrentLap = 2,
            CurrentTimes = new TestableSectorTimes(),
            LastTimes = new TestableSectorTimes(),
            BestTimes = new TestableSectorTimes(),
            CurrentSector = 3,
            TrackPositionPercent = 0.901,
            GapToLeader = 17.395,
            FrontTyreCompound = "Soft",
            FuelCapacity = 50.209,
            FuelLoad = 30.131
        };

        var leaderData = new TestableOpponent
        {
            IsInPit = false,
            CurrentLap = 2,
            CurrentSector = 3,
            TrackPositionPercent = 0.940,
            GapToLeader = null
        };

        var inFrontData = new TestableOpponent
        {
            IsInPit = false,
            CurrentLap = 2,
            CurrentSector = 3,
            TrackPositionPercent = 0.927,
            GapToLeader = 10.985
        };

        processor.Process("testgame_testtrack", SessionType.Race, 6, newEntryData, oldEntryData, leaderData, inFrontData, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockPropertyManager.Verify(m => m.Add(6, "CarNumber", "107"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Name", "Test Name"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "TeamName", "Test Team Name"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "CarName", "Test Car Name"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "CarClass", "Test Car Class"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Position", 6), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "CurrentLapNumber", (int?)2), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "CurrentSector", (int?)3), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "GapToFirst", "+17.395"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "GapToInFront", "+6.410"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "TyreCompound", "Soft"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "FuelCapacity", (double?)50.209), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "FuelLoad", (double?)30.131), Times.Once());
    }

    [Fact]
    public void TestGapCalculation()
    {
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockFastestFragmentTimesStore = new Mock<IFastestFragmentTimesStore>();
        var mockEntryProgressStore = new Mock<IEntryProgressStore>();

        RaceEntryProcessor processor = new(mockPropertyManager.Object, mockRaceEventHandler.Object);

        var newEntryData = new TestableOpponent
        {
            CarNumber = "107",
            Position = 6,
            Name = "Test Name",
            TeamName = "Test Team Name",
            CarName = "Test Car Name",
            CarClass = "Test Car Class",
            IsInPit = false,
            CurrentLap = 2,
            CurrentTimes = new TestableSectorTimes(),
            LastTimes = new TestableSectorTimes(),
            BestTimes = new TestableSectorTimes
            {
                FullLap = TimeSpan.Parse("00:01:20.3150000")
            },
            CurrentSector = 3,
            TrackPositionPercent = 0.901,
            GapToLeader = 17.395,
            FrontTyreCompound = "Soft",
            FuelCapacity = 50.209,
            FuelLoad = 30.131,
        };

        var leaderData = new TestableOpponent
        {
            CarNumber = "108",
            IsInPit = false,
            CurrentLap = 2,
            CurrentSector = 3,
            TrackPositionPercent = 0.940,
            GapToLeader = null,
            BestTimes = new TestableSectorTimes
            {
                FullLap = TimeSpan.Parse("00:01:19.1520000")
            },
        };

        var inFrontData = new TestableOpponent
        {
            CarNumber = "109",
            IsInPit = false,
            CurrentLap = 2,
            CurrentSector = 3,
            TrackPositionPercent = 0.927,
            GapToLeader = 10.985,
            BestTimes = new TestableSectorTimes
            {
                FullLap = TimeSpan.Parse("00:01:20.2580000")
            },
        };

        // Should calculate gaps by GapToLeader property when EntryProgressStore.UseCustomGapCalculation is false...
        mockEntryProgressStore.Setup(m => m.UseCustomGapCalculation()).Returns(false);
        processor.Process("testgame_testtrack", SessionType.Race, 6, newEntryData, null, leaderData, inFrontData, mockFastestFragmentTimesStore.Object, mockEntryProgressStore.Object);
        mockPropertyManager.Verify(m => m.Add(6, "GapToFirst", "+17.395"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "GapToInFront", "+6.410"), Times.Once());
        mockPropertyManager.Invocations.Clear();

        // ... or session type is not 'Race'
        mockEntryProgressStore.Setup(m => m.UseCustomGapCalculation()).Returns(true);
        processor.Process("testgame_testtrack", SessionType.Qualifying, 6, newEntryData, null, leaderData, inFrontData, mockFastestFragmentTimesStore.Object, mockEntryProgressStore.Object);
        mockPropertyManager.Verify(m => m.Add(6, "GapToFirst", "+1.163"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "GapToInFront", "+0.057"), Times.Once());
        mockPropertyManager.Invocations.Clear();

        // Now calculate gaps by custom logic
        mockEntryProgressStore.Setup(m => m.UseCustomGapCalculation()).Returns(true);
        mockEntryProgressStore.Setup(m => m.CalcGap("108", "107")).Returns("+2L");
        mockEntryProgressStore.Setup(m => m.CalcGap("109", "107")).Returns("+10.391");
        processor.Process("testgame_testtrack", SessionType.Race, 6, newEntryData, null, leaderData, inFrontData, mockFastestFragmentTimesStore.Object, mockEntryProgressStore.Object);
        mockPropertyManager.Verify(m => m.Add(6, "GapToFirst", "+2L"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "GapToInFront", "+10.391"), Times.Once());
    }

    [Fact]
    public void TestUpdateLapTimeProperties()
    {
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        mockRaceEventHandler.Setup(m => m.GetPitDataByEntryId(It.IsAny<string>())).Returns(new EntryPitData("", 0, 0, null, null));

        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockFastestFragmentTimesStore = new Mock<IFastestFragmentTimesStore>();

        RaceEntryProcessor processor = new(mockPropertyManager.Object, mockRaceEventHandler.Object);

        var oldEntryData = new TestableOpponent
        {
            CarNumber = "107",
            IsInPit = false,
            CurrentLap = 2,
            CurrentTimes = new TestableSectorTimes(),
            LastTimes = new TestableSectorTimes(),
            BestTimes = new TestableSectorTimes(),
            CurrentSector = 1
        };

        var newEntryData = new TestableOpponent
        {
            CarNumber = "107",
            Position = 6,
            IsInPit = false,
            CurrentLap = 2,
            CurrentTimes = new TestableSectorTimes(),
            LastTimes = new TestableSectorTimes
            {
                Sector1 = TimeSpan.Parse("00:00:21.4020000"),
                Sector2 = TimeSpan.Parse("00:01:01.9530000"),
                Sector3 = TimeSpan.Parse("00:00:10.1580000"),
                FullLap = TimeSpan.Parse("00:01:33.5130000")
            },
            BestTimes = new TestableSectorTimes
            {
                FullLap = TimeSpan.Parse("00:01:33.5100000")
            },
            CurrentSector = 1
        };

        // no CurrentTimes yet, should show the LastTimes
        // mock the fastestFragmentTimeStore in order to not have to initialize one
        mockFastestFragmentTimesStore.Setup(m => m.GetLastLapFragmentTimeIndicator(It.IsAny<TestableOpponent>(), LapFragmentType.SECTOR_1)).Returns("ENTRY_BEST");
        mockFastestFragmentTimesStore.Setup(m => m.GetLastLapFragmentTimeIndicator(It.IsAny<TestableOpponent>(), LapFragmentType.SECTOR_2)).Returns("");
        mockFastestFragmentTimesStore.Setup(m => m.GetLastLapFragmentTimeIndicator(It.IsAny<TestableOpponent>(), LapFragmentType.SECTOR_3)).Returns("CLASS_BEST");
        mockFastestFragmentTimesStore.Setup(m => m.GetLastLapFragmentTimeIndicator(It.IsAny<TestableOpponent>(), LapFragmentType.FULL_LAP)).Returns("");
        mockFastestFragmentTimesStore.Setup(m => m.GetFragmentTimeIndicator(It.IsAny<TestableOpponent>(), It.IsAny<TimeSpan>(), LapFragmentType.FULL_LAP)).Returns("ENTRY_BEST");
        processor.Process("testgame_testtrack", SessionType.Race, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockPropertyManager.Verify(m => m.Add(6, "Sector1Time", "21.402"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector1Indicator", "ENTRY_BEST"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector2Time", "1:01.953"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector2Indicator", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector3Time", "10.158"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector3Indicator", "CLASS_BEST"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "LapTime", "1:33.513"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "LapTimeIndicator", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "BestLapTime", "1:33.510"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "BestLapTimeIndicator", ""), Times.Once()); // ENTRY_BEST should never occur, only CLASS_BEST
        mockPropertyManager.Invocations.Clear();

        // Sector1 in CurrentLap was added, Should reset S2 and S3 times
        newEntryData.CurrentTimes.Sector1 = TimeSpan.Parse("00:00:21.2100000");
        mockFastestFragmentTimesStore.Setup(m => m.GetCurrentLapFragmentTimeIndicator(It.IsAny<TestableOpponent>(), LapFragmentType.SECTOR_1)).Returns("CLASS_BEST");
        newEntryData.CurrentSector = 2;
        processor.Process("testgame_testtrack", SessionType.Race, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockPropertyManager.Verify(m => m.Add(6, "Sector1Time", "21.210"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector1Indicator", "CLASS_BEST"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector2Time", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector2Indicator", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector3Time", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector3Indicator", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "LapTime", "1:33.513"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "LapTimeIndicator", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "BestLapTime", "1:33.510"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "BestLapTimeIndicator", ""), Times.Once());
        mockPropertyManager.Invocations.Clear();

        // Sector2 in CurrentLap was added, should show S2 time from current lap
        newEntryData.CurrentTimes.Sector2 = TimeSpan.Parse("00:01:02.3580000");
        mockFastestFragmentTimesStore.Setup(m => m.GetCurrentLapFragmentTimeIndicator(It.IsAny<TestableOpponent>(), LapFragmentType.SECTOR_2)).Returns("ENTRY_BEST");
        newEntryData.CurrentSector = 3;
        processor.Process("testgame_testtrack", SessionType.Race, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockPropertyManager.Verify(m => m.Add(6, "Sector1Time", "21.210"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector1Indicator", "CLASS_BEST"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector2Time", "1:02.358"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector2Indicator", "ENTRY_BEST"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector3Time", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "Sector3Indicator", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "LapTime", "1:33.513"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "LapTimeIndicator", ""), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "BestLapTime", "1:33.510"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "BestLapTimeIndicator", ""), Times.Once());
        mockPropertyManager.Invocations.Clear();

        // should use last lap time, when best lap time isn't filled
        newEntryData.BestTimes.FullLap = null;
        newEntryData.LastTimes.FullLap = TimeSpan.Parse("00:01:24.1090000");
        mockFastestFragmentTimesStore.Setup(m => m.GetLastLapFragmentTimeIndicator(It.IsAny<TestableOpponent>(), LapFragmentType.FULL_LAP)).Returns("CLASS_BEST");
        processor.Process("testgame_testtrack", SessionType.Race, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockPropertyManager.Verify(m => m.Add(6, "BestLapTime", "1:24.109"), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "BestLapTimeIndicator", "CLASS_BEST"), Times.Once());
        mockPropertyManager.Invocations.Clear();
    }

    [Fact]
    public void TestUpdatePitProperties()
    {
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        mockRaceEventHandler.Setup(m => m.GetPitDataByEntryId(It.IsAny<string>())).Returns(new EntryPitData("", 0, 0, null, null));

        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockFastestFragmentTimesStore = new Mock<IFastestFragmentTimesStore>();

        RaceEntryProcessor processor = new(mockPropertyManager.Object, mockRaceEventHandler.Object);

        var oldEntryData = new TestableOpponent
        {
            CarNumber = "107",
            IsInPit = false,
            CurrentLap = 2,
            CurrentTimes = new TestableSectorTimes(),
            LastTimes = new TestableSectorTimes(),
            BestTimes = new TestableSectorTimes(),
            CurrentSector = 1
        };

        var newEntryData = new TestableOpponent
        {
            CarNumber = "107",
            Position = 6,
            IsInPit = true,
            CurrentLap = 29,
            CurrentTimes = new TestableSectorTimes(),
            LastTimes = new TestableSectorTimes(),
            BestTimes = new TestableSectorTimes(),
            CurrentSector = 1
        };

        mockRaceEventHandler.Setup(m => m.GetPitDataByEntryId(It.IsAny<string>())).Returns(new EntryPitData("107", 4, 194, 42, 26));
        processor.Process("testgame_testtrack", SessionType.Race, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockPropertyManager.Verify(m => m.Add(6, "InPits", true), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "PitStopsTotal", 4), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "PitStopsTotalDuration", 194), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "PitStopLastDuration", (int?)42), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "LapsInCurrentStint", (int?)4), Times.Once()); // CurrentLap 29 - LastPitLap (26 - 1) = 4; -1 because we want the current lap in the stint and not the completed ones
        mockPropertyManager.Invocations.Clear();

        // and now when no pitData exists yet
        mockRaceEventHandler.Setup(m => m.GetPitDataByEntryId(It.IsAny<string>())).Returns(new EntryPitData("107", 0, 0, null, null));
        processor.Process("testgame_testtrack", SessionType.Race, 6, newEntryData, oldEntryData, null, null, mockFastestFragmentTimesStore.Object, new EntryProgressStore("Testgame"));
        mockPropertyManager.Verify(m => m.Add(6, "PitStopsTotal", 0), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "PitStopsTotalDuration", 0), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "PitStopLastDuration", (int?)null), Times.Once());
        mockPropertyManager.Verify(m => m.Add(6, "LapsInCurrentStint", (int?)29), Times.Once()); // no pit data yet -> CurrentLap
        mockPropertyManager.Invocations.Clear();
    }
}
