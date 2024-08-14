using Moq;

public class RaceEventRecoveryFileTest
{
    [Fact]
    public void TestWritePitInEventToRecoveryFile()
    {
        var testFile = @"C:\Users\chris\Documents\simhub\recovery-file-test.txt";
        File.Delete(testFile);

        var mockPitEventSelector = new Mock<IRaceEventRecoveryFileEventSelector<PitEvent>>();
        var mockFinishedLapSelector = new Mock<IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent>>();
        var recoveryFile = new RaceEventRecoveryFile(mockPitEventSelector.Object, mockFinishedLapSelector.Object, testFile);


        recoveryFile.AddEvent(new PitEvent(RaceEventType.PitIn, "107", 5, TimeSpan.Parse("00:30:19.3910000")));
        var fileContent = File.ReadAllLines(testFile);
        var expectedLine = "PIT_IN;107;5;00:30:19.3910000";

        Assert.Single(fileContent);
        Assert.Equal(expectedLine, fileContent[0]);
    }

    [Fact]
    public void TestWritePitOutEventToRecoveryFile()
    {
        var testFile = @"C:\Users\chris\Documents\simhub\recovery-file-test.txt";
        File.Delete(testFile);

        var mockPitEventSelector = new Mock<IRaceEventRecoveryFileEventSelector<PitEvent>>();
        var mockFinishedLapSelector = new Mock<IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent>>();
        var recoveryFile = new RaceEventRecoveryFile(mockPitEventSelector.Object, mockFinishedLapSelector.Object, testFile);

        recoveryFile.AddEvent(new PitEvent(RaceEventType.PitOut, "107", 6, TimeSpan.Parse("00:30:57.1510000")));
        var fileContent = File.ReadAllLines(testFile);
        var expectedLine = "PIT_OUT;107;6;00:30:57.1510000";

        Assert.Single(fileContent);
        Assert.Equal(expectedLine, fileContent[0]);
    }

    [Fact]
    public void TestWritePlayerFinishedLapEventToRecoveryFile()
    {
        var testFile = @"C:\Users\chris\Documents\simhub\recovery-file-test.txt";
        File.Delete(testFile);

        var mockPitEventSelector = new Mock<IRaceEventRecoveryFileEventSelector<PitEvent>>();
        var mockFinishedLapSelector = new Mock<IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent>>();
        var recoveryFile = new RaceEventRecoveryFile(mockPitEventSelector.Object, mockFinishedLapSelector.Object, testFile);

        recoveryFile.AddEvent(new PlayerFinishedLapEvent(2, TimeSpan.Parse("00:01:41.5540000"), TimeSpan.Parse("00:03:24.1680000")));
        var fileContent = File.ReadAllLines(testFile);
        var expectedLine = "PLAYER_FINISHED_LAP;2;00:01:41.5540000;00:03:24.1680000";

        Assert.Single(fileContent);
        Assert.Equal(expectedLine, fileContent[0]);
    }

    [Fact]
    public void TestWriteSessionReloadEventToRecoveryFile()
    {
        var testFile = @"C:\Users\chris\Documents\simhub\recovery-file-test.txt";
        File.Delete(testFile);

        var mockPitEventSelector = new Mock<IRaceEventRecoveryFileEventSelector<PitEvent>>();
        var mockFinishedLapSelector = new Mock<IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent>>();
        var recoveryFile = new RaceEventRecoveryFile(mockPitEventSelector.Object, mockFinishedLapSelector.Object, testFile);

        recoveryFile.AddEvent(new SessionReloadEvent(4, TimeSpan.Parse("00:07:13.3530000")));
        var fileContent = File.ReadAllLines(testFile);
        var expectedLine = "SESSION_RELOAD;4;00:07:13.3530000";

        Assert.Single(fileContent);
        Assert.Equal(expectedLine, fileContent[0]);
    }

    [Fact]
    public void TestReadPitEvents()
    {
        var mockedPitEvents = new List<PitEvent>() {
            new(RaceEventType.PitIn, "107", 14, TimeSpan.Parse("00:52:29.0490000")),
            new(RaceEventType.PitOut, "107", 15, TimeSpan.Parse("00:54:12.3710000"))
        };

        var mockPitEventSelector = new Mock<IRaceEventRecoveryFileEventSelector<PitEvent>>();
        mockPitEventSelector.Setup(m => m.SelectSpecificEvents("does-not-matter.txt")).Returns(mockedPitEvents);

        var mockFinishedLapSelector = new Mock<IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent>>();

        var recoveryFile = new RaceEventRecoveryFile(mockPitEventSelector.Object, mockFinishedLapSelector.Object, "does-not-matter.txt");
        var result = recoveryFile.ReadPitEvents();
        mockPitEventSelector.Verify(m => m.SelectSpecificEvents("does-not-matter.txt"), Times.Exactly(1));
        Assert.Equal(2, result.Count);
        Assert.Equal(mockedPitEvents[0], result[0]);
        Assert.Equal(mockedPitEvents[1], result[1]);
    }


    [Fact]
    public void TestReadPlayerFinishedLapEvents()
    {
        var mockedFinishedLapEvents = new List<PlayerFinishedLapEvent>() {
            new(1, TimeSpan.Parse("00:01:44.1580000"), TimeSpan.Parse("00:01:44.1580000")),
            new(2, TimeSpan.Parse("00:01:41.4840000"), TimeSpan.Parse("00:03:25.6420000"))
        };

        var mockPitEventSelector = new Mock<IRaceEventRecoveryFileEventSelector<PitEvent>>();

        var mockFinishedLapSelector = new Mock<IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent>>();
        mockFinishedLapSelector.Setup(m => m.SelectSpecificEvents("does-not-matter.txt")).Returns(mockedFinishedLapEvents);

        var recoveryFile = new RaceEventRecoveryFile(mockPitEventSelector.Object, mockFinishedLapSelector.Object, "does-not-matter.txt");
        var result = recoveryFile.ReadPlayerFinishedLapEvents();
        mockFinishedLapSelector.Verify(m => m.SelectSpecificEvents("does-not-matter.txt"), Times.Exactly(1));
        Assert.Equal(2, result.Count);
        Assert.Equal(mockedFinishedLapEvents[0], result[0]);
        Assert.Equal(mockedFinishedLapEvents[1], result[1]);
    }
}